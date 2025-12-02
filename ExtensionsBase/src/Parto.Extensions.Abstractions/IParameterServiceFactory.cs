using System.Diagnostics.CodeAnalysis;

namespace Parto.Extensions.Abstractions;

public interface IParameterServiceFactory<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] in TParameter>
    where TParameter : notnull
{
    ParameterServiceFactoryOptions Options { get; }
    bool TryRemove(TParameter parameter);
}

public interface IParameterServiceFactory<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] out TInstance,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] in TParameter> :
    IParameterServiceFactory<TParameter>,
    IEnumerable<TInstance>
    where TParameter : notnull

{
    TInstance GetOrCreate(TParameter parameter);
}