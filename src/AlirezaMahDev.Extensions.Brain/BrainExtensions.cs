namespace AlirezaMahDev.Extensions.Brain;

public static class BrainExtensions
{
    extension(IExtensionsBuilder builder)
    {
        public BrainBuilder AddBrainService()
        {
            return new(builder);
        }

        public IExtensionsBuilder AddBrainService(Action<BrainBuilder> action)
        {
            action(AddBrainService(builder));
            return builder;
        }
    }
}