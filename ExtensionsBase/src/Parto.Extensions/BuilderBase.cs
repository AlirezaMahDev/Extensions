using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Parto.Extensions.Abstractions;

namespace Parto.Extensions;

public abstract class BuilderBase<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    TOptions> : IBuilderBase<TOptions>
    where TOptions : class, IOptionsBase
{
    private readonly IServiceCollection _services;

    protected BuilderBase(IServiceCollection services) : this(services, TOptions.Key)
    {
    }

    protected internal BuilderBase(IServiceCollection services, string key)
    {
        _services = services;
        Key = key;
        OptionsBuilder = services.AddOptions<TOptions>().BindConfiguration(key);
    }

    protected internal string Key { get; }

    public OptionsBuilder<TOptions> OptionsBuilder { get; }

    public OptionsBuilder<TSubOptions> AddSubOptions<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        TSubOptions>()
        where TSubOptions : class, IOptionsBase
    {
        return _services.AddOptions<TSubOptions>().BindConfiguration($"{Key}:{TSubOptions.Key}");
    }
}

public abstract class BuilderBase<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    TOptions, TParent, TParentOptions>(IServiceCollection services, TParent parent)
    : BuilderBase<TOptions>(services, $"{parent.Key}:{TOptions.Key}")
    where TOptions : class, IOptionsBase
    where TParent : BuilderBase<TParentOptions>
    where TParentOptions : class, IOptionsBase
{
    protected TParent Parent { get; } = parent;
}