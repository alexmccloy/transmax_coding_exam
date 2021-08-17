using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EagleRock
{
    /// <summary>
    /// Extension methods to apply configuration for this application
    /// </summary>
    public static class HostBuilderExtensions
    {

        /// <summary>
        /// Registers a class as both an interface dependency and a hosted service
        /// </summary>
        /// <summary>
        /// Registers a class as both an interface dependency and a hosted service
        /// </summary>
        /// 
        /// <param name="serviceCollection">
        /// The service collection to modify
        /// </param>
        /// <typeparam name="TInterface">
        /// The interface to register as a dependency
        /// </typeparam>
        /// <typeparam name="TClass">
        /// The concrete implementation of the dependency. This must implement <see cref="IHostedService"/>
        /// </typeparam>
        /// 
        /// <returns>
        /// The modified service collection
        /// </returns>
        public static IServiceCollection AddHostedSingleton<TInterface, TClass>(this IServiceCollection serviceCollection)
            where TInterface : class
            where TClass : class, TInterface, IHostedService
        {
            serviceCollection.AddSingleton<TClass>();
            serviceCollection.AddSingleton<TInterface>(provider => provider.GetRequiredService<TClass>());
            serviceCollection.AddSingleton<IHostedService>(provider => provider.GetRequiredService<TClass>());

            return serviceCollection;
        }


        /// <summary>
        /// Registers a class as both a dependency and a hosted service
        /// </summary>
        /// <summary>
        /// Registers a class as both an interface dependency and a hosted service
        /// </summary>
        /// 
        /// <param name="serviceCollection">
        /// The service collection to modify
        /// </param>
        /// <typeparam name="TClass">
        /// The concrete implementation of the dependency. This must implement <see cref="IHostedService"/>
        /// </typeparam>
        /// 
        /// <returns>
        /// The modified service collection
        /// </returns>
        public static IServiceCollection AddHostedSingleton<TClass>(this IServiceCollection serviceCollection)
            where TClass : class, IHostedService
        {
            serviceCollection.AddSingleton<TClass>();
            serviceCollection.AddSingleton<IHostedService>(provider => provider.GetRequiredService<TClass>());

            return serviceCollection;
        }
    }
}