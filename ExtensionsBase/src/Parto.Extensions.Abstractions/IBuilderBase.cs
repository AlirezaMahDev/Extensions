using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Options;

namespace Parto.Extensions.Abstractions;

public interface IBuilderBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TOptions>
    where TOptions : class, IOptionsBase
{
    OptionsBuilder<TOptions> OptionsBuilder { get; }

    OptionsBuilder<TSubOptions> AddSubOptions<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        TSubOptions>()
        where TSubOptions : class, IOptionsBase;
}