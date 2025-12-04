using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AlirezaMahDev.Extensions.Abstractions;

public interface IBuilderBase
{
    IServiceCollection Services { get; }
}

public interface IBuilderBase<TOptions> : IBuilderBase
    where TOptions : class, IOptionsBase
{
    OptionsBuilder<TOptions> OptionsBuilder { get; }

    OptionsBuilder<TSubOptions> AddSubOptions<TSubOptions>()
        where TSubOptions : class, IOptionsBase;
}