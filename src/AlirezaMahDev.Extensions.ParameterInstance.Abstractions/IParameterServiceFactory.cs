namespace AlirezaMahDev.Extensions.ParameterInstance.Abstractions;

public interface IParameterInstanceFactory<
    out TInstance,
    in TParameter> :
    IParameterInstanceFactory<TParameter>,
    IEnumerable<TInstance>
    where TParameter : notnull

{
    TInstance GetOrCreate(TParameter parameter);
}