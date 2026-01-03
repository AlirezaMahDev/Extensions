using Microsoft.Extensions.DependencyInjection;

namespace AlirezaMahDev.Extensions.Brain;

public static class BrainExtensions
{
    extension(IServiceCollection services)
    {
        public BrainBuilder AddBrain()
        {
            return new(services);
        }

        public IServiceCollection AddBrain(Action<BrainBuilder> action)
        {
            action(services.AddBrain());
            return services;
        }
    }
}