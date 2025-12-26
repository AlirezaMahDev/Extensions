using AlirezaMahDev.Extensions.ParameterInstance.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AlirezaMahDev.Extensions.ParameterInstance;

public static class ParameterInstanceFactoryExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddSingletonParameterInstanceFactory<TFactory>()
        {
            return services.AddSingletonParameterInstanceFactory(typeof(TFactory));
        }

        public IServiceCollection AddSingletonParameterInstanceFactory(Type factoryType)
        {
            services.TryAddSingleton(factoryType);
            return services;
        }


        public IServiceCollection AddScopedParameterInstanceFactory<TFactory>()
        {
            return services.AddScopedParameterInstanceFactory(typeof(TFactory));
        }

        public IServiceCollection AddScopedParameterInstanceFactory(Type factoryType)
        {
            services.TryAddScoped(factoryType);
            return services;
        }

        public IServiceCollection AddTransientParameterInstanceFactory<TFactory>()
        {
            return services.AddTransientParameterInstanceFactory(typeof(TFactory));
        }

        public IServiceCollection AddTransientParameterInstanceFactory(Type factoryType)
        {
            services.TryAddTransient(factoryType);
            return services;
        }
    }


    extension<TInstance, TParameter>(IParameterInstanceFactory<TInstance, TParameter> factoryService)
        where TInstance : notnull
        where TParameter : notnull
    {
        public bool TryRemoveAllInstanceCreatedByThis(TInstance instance)
        {
            return factoryService.Options.GetFactories<TInstance>()
                .Aggregate(false,
                    (current, parameterInstanceFactory) =>
                        current || parameterInstanceFactory.TryRemove(instance));
        }
    }
}