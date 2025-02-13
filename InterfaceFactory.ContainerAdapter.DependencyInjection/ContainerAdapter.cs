using Microsoft.Extensions.DependencyInjection;

namespace InterfaceFactory.ContainerAdapter.DependencyInjection;

internal class ContainerAdapter : IContainerResolveAdapter, IContainerRegisterAdapter
{
  internal static IServiceProvider? ServiceProvider;
  internal static IServiceCollection? ServiceCollection;

  /// <inheritdoc />
  public void Register(Type serviceType, Type implementationType, ServiceLifetime lifetime) => RegisterKeyed(serviceType, implementationType, lifetime, null);

  /// <inheritdoc />
  // ReSharper disable once TooManyArguments
  // ReSharper disable once MethodTooLong
  public void RegisterKeyed(Type serviceType, Type implementationType, ServiceLifetime lifetime, string? key)
  {
    if (ServiceCollection == null) throw new InvalidOperationException("Service collection is not initialized. Please call the extension method 'RegisterInterfaceFactories' on the ServiceCollection.");

    switch (lifetime)
    {
      case ServiceLifetime.Singleton:
        if (key == null) ServiceCollection.AddSingleton(serviceType, implementationType);
        else ServiceCollection.AddKeyedSingleton(serviceType, key, implementationType);
        break;
      case ServiceLifetime.Scoped:
        if (key == null) ServiceCollection.AddScoped(serviceType, implementationType);
        else ServiceCollection.AddKeyedScoped(serviceType, key, implementationType);
        break;
      case ServiceLifetime.Transient:
        if (key == null) ServiceCollection.AddTransient(serviceType, implementationType);
        else ServiceCollection.AddKeyedTransient(serviceType, key, implementationType);
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, "Invalid service lifetime.");
    }
  }

  /// <inheritdoc />
  public T? Resolve<T>() => ServiceProvider != null ? ServiceProvider.GetService<T>() : throw new ArgumentNullException($"The ServiceProvider was not set. Please call the extension method 'UseInterfaceFactory' on the ServiceProvider.");
  /// <inheritdoc />
  public T? ResolveKeyed<T>(string key) => ServiceProvider != null ? ServiceProvider.GetKeyedService<T>(key) : throw new ArgumentNullException($"The ServiceProvider was not set. Please call the extension method 'UseInterfaceFactory' on the ServiceProvider.");
  /// <inheritdoc />
  public T ResolveRequired<T>() where T : notnull => ServiceProvider != null ? ServiceProvider.GetRequiredService<T>() : throw new ArgumentNullException($"The ServiceProvider was not set. Please call the extension method 'UseInterfaceFactory' on the ServiceProvider.");
  /// <inheritdoc />
  public T ResolveKeyedRequired<T>(string key) where T : notnull => ServiceProvider != null ? ServiceProvider.GetRequiredKeyedService<T>(key) : throw new ArgumentNullException($"The ServiceProvider was not set. Please call the extension method 'UseInterfaceFactory' on the ServiceProvider.");
}
