using Microsoft.Extensions.DependencyInjection;

using Parto.Extensions.Abstractions;

namespace Parto.Extensions;

public static class ParameterServiceFactoryExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddSingletonFactory<TFactory>()
        {
            return AddSingletonFactory(services, typeof(TFactory));
        }

        public IServiceCollection AddSingletonFactory(Type factoryType)
        {
            return services.AddSingleton(factoryType);
        }
    }


    public static bool TryRemoveAllInstanceCreatedByThis<TInstance, TParameter>(
        this IParameterServiceFactory<TInstance, TParameter> factoryService,
        TInstance instance)
        where TInstance : notnull
        where TParameter : notnull

    {
        return factoryService.Options.GetFactories<TInstance>()
            .Aggregate(false,
                (current, parameterServiceFactory) =>
                    current || parameterServiceFactory.TryRemove(instance));
    }
}