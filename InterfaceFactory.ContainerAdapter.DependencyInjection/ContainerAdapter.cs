using Microsoft.Extensions.DependencyInjection;

namespace InterfaceFactory.ContainerAdapter.DependencyInjection;

internal class ContainerAdapter(IServiceCollection serviceCollection) : IContainerResolveAdapter, IContainerRegisterAdapter
{
  private IServiceProvider? _serviceProvider;
  internal IServiceProvider SetServiceProvider(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

  /// <inheritdoc />
  public void Register(Type serviceType, Type implementationType, ServiceLifetime lifetime) => RegisterKeyed(serviceType, implementationType, lifetime, null);

  /// <inheritdoc />
  // ReSharper disable once TooManyArguments
  // ReSharper disable once MethodTooLong
  public void RegisterKeyed(Type serviceType, Type implementationType, ServiceLifetime lifetime, string? key)
  {
    switch (lifetime)
    {
      case ServiceLifetime.Singleton:
        if (key == null) serviceCollection.AddSingleton(serviceType, implementationType);
        else serviceCollection.AddKeyedSingleton(serviceType, key, implementationType);
        break;
      case ServiceLifetime.Scoped:
        if (key == null) serviceCollection.AddScoped(serviceType, implementationType);
        else serviceCollection.AddKeyedScoped(serviceType, key, implementationType);
        break;
      case ServiceLifetime.Transient:
        if (key == null) serviceCollection.AddTransient(serviceType, implementationType);
        else serviceCollection.AddKeyedTransient(serviceType, key, implementationType);
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, "Invalid service lifetime.");
    }
  }

  /// <inheritdoc />
  public T? Resolve<T>() => _serviceProvider != null ? _serviceProvider.GetService<T>() : throw new ArgumentNullException($"The ServiceProvider was not set. Please call the extension method 'UseInterfaceFactory' on the ServiceProvider.");
  /// <inheritdoc />
  public T? ResolveKeyed<T>(string key) => _serviceProvider != null ? _serviceProvider.GetKeyedService<T>(key) : throw new ArgumentNullException($"The ServiceProvider was not set. Please call the extension method 'UseInterfaceFactory' on the ServiceProvider.");
  /// <inheritdoc />
  public T ResolveRequired<T>() where T : notnull => _serviceProvider != null ? _serviceProvider.GetRequiredService<T>() : throw new ArgumentNullException($"The ServiceProvider was not set. Please call the extension method 'UseInterfaceFactory' on the ServiceProvider.");
  /// <inheritdoc />
  public T ResolveKeyedRequired<T>(string key) where T : notnull => _serviceProvider != null ? _serviceProvider.GetRequiredKeyedService<T>(key) : throw new ArgumentNullException($"The ServiceProvider was not set. Please call the extension method 'UseInterfaceFactory' on the ServiceProvider.");
}
