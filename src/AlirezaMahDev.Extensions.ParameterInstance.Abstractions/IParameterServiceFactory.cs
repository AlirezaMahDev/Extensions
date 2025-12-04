namespace AlirezaMahDev.Extensions.ParameterInstance.Abstractions;

public interface IParameterInstanceFactory<
    in TParameter>
    where TParameter : notnull
{
    ParameterInstanceFactoryOptions Options { get; }
    bool TryRemove(TParameter parameter);
}

public interface IParameterInstanceFactory<
    out TInstance,
    in TParameter> :
    IParameterInstanceFactory<TParameter>,
    IEnumerable<TInstance>
    where TParameter : notnull

{
    TInstance GetOrCreate(TParameter parameter);
}