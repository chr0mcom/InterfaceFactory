namespace InterfaceFactory;

/// <summary>
/// Provides container adapter functionality independent of a specific IoC container.
/// </summary>
public interface IContainerAdapter
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
  /// <summary>
  /// Resolves a service of type <typeparamref name="T"/> from the current <see cref="IServiceProvider"/>.
  /// </summary>
  /// <typeparam name="T">The type of the service to resolve. Can be a reference or value type.</typeparam>
  /// <returns>
  /// An instance of the service of type <typeparamref name="T"/>, or <c>null</c> if the service is not available.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  /// Thrown if the <see cref="IServiceProvider"/> has not been set.
  /// </exception>
  T? Resolve<T>();
  /// <summary>
  /// Resolves a service of type <typeparamref name="T"/> associated with the specified key
  /// from the current <see cref="IServiceProvider"/>.
  /// </summary>
  /// <typeparam name="T">The type of the service to resolve. Can be a reference or value type.</typeparam>
  /// <param name="key">The key associated with the service to resolve.</param>
  /// <returns>
  /// An instance of the service of type <typeparamref name="T"/>, or <c>null</c> if the service
  /// associated with the specified key is not available.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  /// Thrown if the <see cref="IServiceProvider"/> has not been set.
  /// </exception>
  T? ResolveKeyed<T>(string key);
  /// <summary>
  /// Resolves a required service of type <typeparamref name="T"/> from the current <see cref="IServiceProvider"/>.
  /// </summary>
  /// <typeparam name="T">The type of the service to resolve. Must be a non-nullable reference or value type.</typeparam>
  /// <returns>An instance of the service of type <typeparamref name="T"/>.</returns>
  /// <exception cref="InvalidOperationException">
  /// Thrown if the service of type <typeparamref name="T"/> is not available.
  /// </exception>
  /// <exception cref="ArgumentNullException">
  /// Thrown if the <see cref="IServiceProvider"/> has not been set.
  /// </exception>
  T ResolveRequired<T>() where T : notnull;
  /// <summary>
  /// Resolves a required service of type <typeparamref name="T"/> associated with the specified key
  /// from the current <see cref="IServiceProvider"/>.
  /// </summary>
  /// <typeparam name="T">The type of the service to resolve. Must be a non-nullable reference or value type.</typeparam>
  /// <param name="key">The key associated with the service to resolve.</param>
  /// <returns>An instance of the service of type <typeparamref name="T"/>.</returns>
  /// <exception cref="InvalidOperationException">
  /// Thrown if the service of type <typeparamref name="T"/> associated with the specified key is not available.
  /// </exception>
  /// <exception cref="ArgumentNullException">
  /// Thrown if the <see cref="IServiceProvider"/> has not been set.
  /// </exception>
  T ResolveKeyedRequired<T>(string key) where T : notnull;
}
