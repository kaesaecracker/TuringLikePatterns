using Microsoft.Extensions.DependencyInjection;

namespace TuringLikePatterns;

internal static class DependencyInjectionExtensions
{
    internal static IServiceCollection AddQuantity(this IServiceCollection serviceCollection, Quantity instance) =>
        serviceCollection
            .AddSingleton(instance)
            .AddKeyedSingleton(instance.Name, instance);

    public static IServiceCollection AddMutationGenerator<T>(this IServiceCollection serviceCollection,
        params object[] args)
        where T : class, IMutationGenerator
    {
        return serviceCollection
            .AddSingleton<T>(sp => ActivatorUtilities.CreateInstance<T>(sp, args))
            .AddSingleton<IMutationGenerator, T>(sp => sp.GetRequiredService<T>());
    }
}
