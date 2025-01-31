namespace Microsoft.Extensions.DependencyInjection.InterfaceFactory;

public static class ServiceProviderExtensions
{
    /// <summary>
    /// Sets the current <see cref="IServiceProvider"/> to be used by the interface factory.
    /// </summary>
    /// <param name="serviceProvider">
    /// The <see cref="IServiceProvider"/> instance to be set as the current service provider.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceProvider"/> instance passed as the parameter.
    /// </returns>
    /// <remarks>
    /// This method assigns the provided <see cref="IServiceProvider"/> to a static container, making it accessible
    /// for resolving services via the interface factory. It is essential to call this method before attempting
    /// to retrieve instances of interfaces that extend <see cref="IFactory{T}"/>.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown when attempting to access the current <see cref="IServiceProvider"/> without calling this method first.
    /// </exception>
    public static IServiceProvider UseInterfaceFactory(this IServiceProvider serviceProvider) => ServiceProviderContainer.Current = serviceProvider;
}
