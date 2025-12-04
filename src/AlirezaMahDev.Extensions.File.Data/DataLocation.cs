using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using Microsoft.Extensions.Logging;

using AlirezaMahDev.Extensions.File.Data.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
internal partial class DataLocation : IDataLocation
{
    [LoggerMessage(LogLevel.Debug, "{message}")]
    private partial void LogDebug(string message);

    private readonly Lock _lock = new();

    private readonly DataLocationArgs _args;
    private readonly ILogger<DataLocation> _logger;
    private readonly DataBlockMemoryRefValue<DataLocationValue> _value;

    private readonly ConcurrentDictionary<String64, Lazy<IDataLocation>> _cache = new();

    private readonly Lazy<IDataBlockAccessor> _lazy;

    public DataLocation(DataLocationArgs args, ILogger<DataLocation> logger)
    {
        _args = args;
        _logger = logger;
        var reader = args.DataAccess.Block.AsReader(args.Id)
            .ReadRefValue(out _value);
        _lazy = new(() =>
            {
                reader.Read(RefValue.Length, out IDataBlockAccessor accessor);
                return accessor;
            },
            LazyThreadSafetyMode.ExecutionAndPublication);
        LogDebug($"location id:{args.Id} created {this}");
    }

    public long Id => _args.Id;
    public IDataAccess DataAccess => _args.DataAccess;

    public ref DataLocationValue RefValue => ref _value.RefValue;
    public ref String64 RefKey => ref RefValue.Key;

    public IDataBlockAccessor Data => _lazy.Value;

    public IDataLocation? Child =>
        RefValue.Child.IsNotDefault ? _args.DataAccess.LocationFactory.GetOrCreate(RefValue.Child) : null;

    public IDataLocation? Next =>
        RefValue.Next.IsNotDefault ? _args.DataAccess.LocationFactory.GetOrCreate(RefValue.Next) : null;

    public bool TryGet(String64 key, [MaybeNullWhen(false)] out IDataLocation result)
    {
        if (_cache.TryGetValue(key, out var lazy))
        {
            result = lazy.Value;
            return true;
        }

        result = this.FirstOrDefault(x => x.RefKey == key);
        if (result is not null)
        {
            _cache.TryAdd(key, new(result));
            return true;
        }

        return false;
    }

    public IDataLocation GetOrAdd(String64 key, int size = 0)
    {
        LogDebug($"start create location:{_args.Id} {key} {size}");
        var location = _cache.GetOrAdd(key,
                static (key, arg) =>
                    new(() =>
                            arg.location.FirstOrDefault(x => x.RefKey == key) ?? arg.location.ForceAdd(key, arg.size),
                        LazyThreadSafetyMode.ExecutionAndPublication),
                (location: this, size))
            .Value;
        LogDebug($"end create location:{_args.Id} {key} {size}");
        return location;
    }


    public IDataLocation ForceAdd(String64 key, int dataLength)
    {
        using var scope = _lock.EnterScope();
        var length = Unsafe.SizeOf<DataLocationValue>() + dataLength;
        var id = _args.DataAccess.GenerateId(length);
        var location = _args.DataAccess.LocationFactory.GetOrCreate(id);
        location.RefValue.Key = key;
        location.RefValue.Length = dataLength;
        return ForceAdd(location);
    }

    public IDataLocation ForceAdd(IDataLocation location)
    {
        location.RefValue.Next = RefValue.Child;
        RefValue.Child = location.Id;
        return location;
    }

    public bool TryRemove(String64 key, [MaybeNullWhen(false)] out IDataLocation item)
    {
        using var _ = _lock.EnterScope();
        item = this.FirstOrDefault();
        if (item?.RefKey == key)
        {
            RefValue.Child = item.RefValue.Next;
            _args.DataAccess.Root.GetOrAdd("_deleted").ForceAdd(item);
            return true;
        }

        item = this.FirstOrDefault(x => x.Next?.RefKey == key);
        if (item is not null)
        {
            item.RefValue.Next = item.Next?.RefValue.Next ?? 0;
            _args.DataAccess.Root.GetOrAdd("_deleted").ForceAdd(item);
            return true;
        }

        return false;
    }

    public IEnumerator<IDataLocation> GetEnumerator()
    {
        var current = Child;
        while (current is not null)
        {
            yield return current;
            current = current.Next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    public void Save()
    {
        _value.BlockMemory.Accessor.Save();
        if (_lazy.IsValueCreated)
            _lazy.Value.Save();
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }

    public override string ToString()
    {
        return _value.RefValue.ToString();
    }
}