namespace InterfaceFactory;

/// <summary>
/// Provides container adapter functionality independent of a specific IoC container.
/// </summary>
public interface IContainerResolveAdapter
{
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
