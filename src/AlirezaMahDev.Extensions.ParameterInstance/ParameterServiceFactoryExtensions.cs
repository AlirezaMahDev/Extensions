using AlirezaMahDev.Extensions.ParameterInstance.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace AlirezaMahDev.Extensions.ParameterInstance;

public static class ParameterInstanceFactoryExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddParameterInstanceFactory<TFactory>()
        {
            return AddParameterInstanceFactory(services, typeof(TFactory));
        }

        public IServiceCollection AddParameterInstanceFactory(Type factoryType)
        {
            return services.AddSingleton(factoryType);
        }
    }


    public static bool TryRemoveAllInstanceCreatedByThis<TInstance, TParameter>(
        this IParameterInstanceFactory<TInstance, TParameter> factoryService,
        TInstance instance)
        where TInstance : notnull
        where TParameter : notnull

    {
        return factoryService.Options.GetFactories<TInstance>()
            .Aggregate(false,
                (current, parameterInstanceFactory) =>
                    current || parameterInstanceFactory.TryRemove(instance));
    }
}