using Microsoft.Extensions.DependencyInjection;
using TuringLikePatterns.Mutations;

namespace TuringLikePatterns;

internal static class DependencyInjectionExtensions
{
    internal static IServiceCollection AddQuantity(this IServiceCollection serviceCollection, Quantity instance) =>
        serviceCollection
            .AddSingleton(instance)
            .AddKeyedSingleton(instance.Name, instance);

    internal static IServiceCollection AddMutationGenerator<T>(this IServiceCollection serviceCollection,
        params object[] args)
        where T : class, IMutationGenerator
    {
        return serviceCollection
            .AddSingleton<T>(sp => ActivatorUtilities.CreateInstance<T>(sp, args))
            .AddSingleton<IMutationGenerator, T>(sp => sp.GetRequiredService<T>());
    }

    internal static IServiceCollection AddStatistic(this IServiceCollection serviceCollection, string name,
        Func<IServiceProvider, Func<string>> updateFunProvider)
    {
        return serviceCollection.AddSingleton<Statistic>(sp => new Statistic(name, updateFunProvider(sp)));
    }
}
