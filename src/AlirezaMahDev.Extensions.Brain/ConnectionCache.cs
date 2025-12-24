// using System.Collections;
// using System.Numerics;
//
// using AlirezaMahDev.Extensions.Brain.Abstractions;
//
// namespace AlirezaMahDev.Extensions.Brain;
//
// public class ConnectionCache<TData, TLink>(
//     Func<IEnumerable<IConnection<TData, TLink>>> loader,
//     Func<CancellationToken, IAsyncEnumerable<IConnection<TData, TLink>>> loaderAsync,
//     Func<long, bool> checker,
//     Func<long, CancellationToken, ValueTask<bool>> checkerAsync) : IEnumerable<IConnection<TData, TLink>>, IAsyncEnumerable<IConnection<TData, TLink>>
//     where TData : unmanaged,
//     IEquatable<TData>, IComparable<TData>, IAdditionOperators<TData, TData, TData>,
//     ISubtractionOperators<TData, TData, TData>
//     where TLink : unmanaged,
//     IEquatable<TLink>, IComparable<TLink>, IAdditionOperators<TLink, TLink, TLink>,
//     ISubtractionOperators<TLink, TLink, TLink>
// {
//     private readonly Lock _lock = new();
//     private IConnection<TData, TLink>[] _cache = [];
//
//     public IEnumerator<IConnection<TData, TLink>> GetEnumerator()
//     {
//         long? cacheKey = _cache.Length > 0 ? _cache[0].Offset : null;
//         if (!cacheKey.HasValue || !checker(cacheKey.Value))
//         {
//             using var scope = _lock.EnterScope();
//             cacheKey = _cache.Length > 0 ? _cache[0].Offset : null;
//             if (!cacheKey.HasValue || !checker(cacheKey.Value))
//             {
//                 Interlocked.Exchange(ref _cache,
//                 [.. loader().TakeWhile(x => x.Offset != cacheKey), .. _cache]
//                 );
//             }
//         }
//
//         return _cache.AsEnumerable().GetEnumerator();
//     }
//
//     IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
//
//     public async IAsyncEnumerator<IConnection<TData, TLink>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
//     {
//         long? cacheKey = _cache.Length > 0 ? _cache[0].Offset : null;
//         if (!cacheKey.HasValue || !await checkerAsync(cacheKey.Value, cancellationToken))
//         {
//             _lock.Enter();
//             cacheKey = _cache.Length > 0 ? _cache[0].Offset : null;
//             if (!cacheKey.HasValue || !await checkerAsync(cacheKey.Value, cancellationToken))
//             {
//                 Interlocked.Exchange(ref _cache,
//                         await loaderAsync(cancellationToken)
//                         .TakeWhile(x => x.Offset != cacheKey)
//                         .Concat(_cache.ToAsyncEnumerable())
//                         .ToArrayAsync(cancellationToken: cancellationToken)
//                 );
//             }
//             _lock.Exit();
//         }
//
//         foreach (var connnection in _cache)
//         {
//             yield return connnection;
//         }
//     }
// }