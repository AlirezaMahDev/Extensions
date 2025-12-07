namespace AlirezaMahDev.Extensions.ParameterInstance.Abstractions;

public interface IParameterInstanceFactory<
    in TParameter>
    where TParameter : notnull
{
    ParameterInstanceFactoryOptions Options { get; }
    bool TryRemove(TParameter parameter);
}