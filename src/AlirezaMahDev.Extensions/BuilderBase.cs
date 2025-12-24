using System.Diagnostics.CodeAnalysis;

using AlirezaMahDev.Extensions.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AlirezaMahDev.Extensions;

public abstract class BuilderBase(IServiceCollection services) : IBuilderBase
{
    public IServiceCollection Services { get; } = services;
}

public abstract class BuilderBase<TOptions> : BuilderBase, IBuilderBase<TOptions>
    where TOptions : class, IOptionsBase
{
    protected BuilderBase(IServiceCollection services) : this(services, TOptions.Key)
    {
    }

    protected internal BuilderBase(IServiceCollection services, string key) : base(services)
    {
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
        return Services.AddOptions<TSubOptions>().BindConfiguration($"{Key}:{TSubOptions.Key}");
    }
}

public abstract class BuilderBase<TOptions, TParent, TParentOptions>(IServiceCollection services, TParent parent)
    : BuilderBase<TOptions>(services, $"{parent.Key}:{TOptions.Key}")
    where TOptions : class, IOptionsBase
    where TParent : BuilderBase<TParentOptions>
    where TParentOptions : class, IOptionsBase
{
    protected TParent Parent { get; } = parent;
}