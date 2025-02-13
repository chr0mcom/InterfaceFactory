namespace InterfaceFactory;

/// <summary>
/// Provides container adapter functionality independent of a specific IoC container.
/// </summary>
public interface IContainerRegisterAdapter
{
  /// <summary>
  /// Registers a service type with a specific implementation type and lifetime.
  /// </summary>
  /// <param name="serviceType">The type of the service to register.</param>
  /// <param name="implementationType">The type that implements the service.</param>
  /// <param name="lifetime">The lifetime of the service (e.g., Singleton, Scoped, or Transient).</param>
  /// <exception cref="InvalidOperationException">
  /// Thrown if the service collection is not initialized.
  /// </exception>
  /// <exception cref="ArgumentOutOfRangeException">
  /// Thrown if the specified <paramref name="lifetime"/> is not valid.
  /// </exception>
  void Register(Type serviceType, Type implementationType, ServiceLifetime lifetime);
  /// <summary>
  /// Registers a service type with a specific implementation type and lifetime, 
  /// optionally associating it with a unique key.
  /// </summary>
  /// <param name="serviceType">The type of the service to register.</param>
  /// <param name="implementationType">The type that implements the service.</param>
  /// <param name="lifetime">The lifetime of the service (e.g., Singleton, Scoped, or Transient).</param>
  /// <param name="key">
  /// An optional key to associate with the service registration. 
  /// If <c>null</c>, the service is registered without a key.
  /// </param>
  /// <exception cref="InvalidOperationException">
  /// Thrown if the service collection is not initialized.
  /// </exception>
  /// <exception cref="ArgumentOutOfRangeException">
  /// Thrown if the specified <paramref name="lifetime"/> is not valid.
  /// </exception>
  // ReSharper disable once TooManyArguments
  void RegisterKeyed(Type serviceType, Type implementationType, ServiceLifetime lifetime, string? key);
}
