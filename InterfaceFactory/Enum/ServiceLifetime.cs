namespace InterfaceFactory;

/// <summary>
/// Specifies the lifetime of a service in a dependency injection container.
/// </summary>
/// <summary>
/// A single instance of the service is created and shared across the application.
/// </summary>
/// <summary>
/// A new instance of the service is created for each scope, typically per request.
/// </summary>
/// <summary>
/// A new instance of the service is created each time it is requested.
/// </summary>
public enum ServiceLifetime
{
  /// <summary>
  /// Specifies that a single instance of the service is created and shared 
  /// across the application. This is typically used for services that are 
  /// stateless or thread-safe and can be reused throughout the application's lifetime.
  /// </summary>
  Singleton,
  /// <summary>
  /// A new instance of the service is created for each scope, typically per request.
  /// </summary>
  Scoped,
  /// <summary>
  /// A new instance of the service is created each time it is requested.
  /// </summary>
  Transient
}
