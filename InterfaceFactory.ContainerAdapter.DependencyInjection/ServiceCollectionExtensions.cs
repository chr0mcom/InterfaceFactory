using Microsoft.Extensions.DependencyInjection;

namespace InterfaceFactory.ContainerAdapter.DependencyInjection;

public static class ServiceCollectionExtensions
{
  /// <summary>
  ///   Registers interfaces that extend <see cref="IFactory{T}" /> in a self-referencing manner with their respective
  ///   concrete implementations.
  /// </summary>
  /// <param name="serviceCollection">
  ///   The <see cref="IServiceCollection" /> to which the services will be added.
  /// </param>
  /// <param name="includeUnloadedAssemblies">
  ///   A boolean value indicating whether to include assemblies that are not yet loaded into the current
  ///   <see cref="AppDomain" />.
  ///   If <c>true</c>, all assemblies in the current folder will be loaded before registration.
  /// </param>
  /// <returns>
  ///   The updated <see cref="IServiceCollection" /> with the registered services.
  /// </returns>
  /// <remarks>
  ///   This method scans all loaded assemblies (and optionally loads additional assemblies from the current folder)
  ///   to find concrete types implementing interfaces that extend <see cref="IFactory{T}" />. It registers these interfaces
  ///   with their corresponding concrete implementations using the <c>AddScoped</c> method.
  ///   For example, given the following interface and implementation:
  ///   <code>
  /// public interface IExample : IFactory{IExample} {}
  /// public class MyExample : IExample {}
  /// </code>
  ///   This method registers the service as:
  ///   <c>serviceCollection.AddScoped&lt;IExample, MyExample&gt;()</c>.
  /// </remarks>
  /// <exception cref="ArgumentNullException">
  ///   Thrown if <paramref name="serviceCollection" /> is <c>null</c>.
  /// </exception>
  // ReSharper disable once FlagArgument
  public static IServiceCollection RegisterInterfaceFactories(this IServiceCollection serviceCollection, bool includeUnloadedAssemblies = false)
  {
    ContainerAdapter.ServiceCollection = serviceCollection;
    ContainerAdapterContainer.Instance = new ContainerAdapter();
    ContainerRegistration.RegisterInterfaceFactories(includeUnloadedAssemblies);
    return serviceCollection;
  }
}
