namespace InterfaceFactory;

/// <summary>
/// Defines a factory interface for retrieving services of type <typeparamref name="T"/> 
/// from the current <see cref="IServiceProvider"/>.
/// To use the factory, the services must have been registered in the ServiceCollection
/// and the extension method UseInterfaceFactory must have been called on the ServiceProvider.
/// </summary>
/// <typeparam name="T">The type of the service to be retrieved.</typeparam>
public interface IFactory<T>
{
  /// <summary>
  /// Retrieves a service of type <typeparamref name="T"/> from the current <see cref="IServiceProvider"/>.
  /// </summary>
  /// <returns>
  /// An instance of the service of type <typeparamref name="T"/>, or <c>null</c> if the service is not available.
  /// </returns>
  public static T? GetInstance()
  {
    try
    {
      return ContainerRegistration.ResolveInstance.Resolve<T>();
    }
    catch (InvalidOperationException)
    {
      return ContainerRegistration.ResolveInstance.ResolveKeyed<T>(ContainerRegistration.TransientKey);
    }
  }

  /// <summary>
  /// Retrieves a required service of type <typeparamref name="T"/> from the current <see cref="IServiceProvider"/>.
  /// </summary>
  /// <returns>An instance of the service of type <typeparamref name="T"/>.</returns>
  /// <exception cref="InvalidOperationException">
  /// Thrown if the service of type <typeparamref name="T"/> is not available.
  /// </exception>
  public static T GetRequiredInstance()
  {
    try
    {
      return ContainerRegistration.ResolveInstance.ResolveRequired<T>();
    }
    catch (InvalidOperationException)
    {
      return ContainerRegistration.ResolveInstance.ResolveKeyedRequired<T>(ContainerRegistration.TransientKey);
    }
  }
  /// <summary>
  /// Retrieves a service of type <typeparamref name="T"/> associated with the specified key
  /// from the current <see cref="IServiceProvider"/>.
  /// </summary>
  /// <param name="key">The key associated with the service to retrieve.</param>
  /// <returns>
  /// An instance of the service of type <typeparamref name="T"/>, or <c>null</c> if the service
  /// associated with the specified key is not available.
  /// </returns>
  public static T? GetKeyedInstance(string key)
  {
    try
    {
      return ContainerRegistration.ResolveInstance.ResolveKeyed<T>(key);
    }
    catch (InvalidOperationException)
    {
      return ContainerRegistration.ResolveInstance.ResolveKeyed<T>(ContainerRegistration.TransientKey+key);
    }
  }
  /// <summary>
  /// Retrieves a required service of type <typeparamref name="T"/> associated with the specified key
  /// from the current <see cref="IServiceProvider"/>.
  /// </summary>
  /// <param name="key">The key associated with the service to retrieve.</param>
  /// <returns>An instance of the service of type <typeparamref name="T"/>.</returns>
  /// <exception cref="InvalidOperationException">
  /// Thrown if the service of type <typeparamref name="T"/> associated with the specified key is not available.
  /// </exception>
  public static T GetRequiredKeyedInstance(string key)
  {
    try
    {
      return ContainerRegistration.ResolveInstance.ResolveKeyedRequired<T>(key);
    }
    catch (InvalidOperationException)
    {
      return ContainerRegistration.ResolveInstance.ResolveKeyedRequired<T>(ContainerRegistration.TransientKey+key);
    }
  }
}
