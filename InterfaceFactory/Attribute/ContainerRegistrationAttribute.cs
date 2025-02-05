namespace InterfaceFactory;

/// <summary>
/// Attribute to configure service registration.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ContainerRegistrationAttribute : Attribute
{
  /// <summary>
  /// Gets the optional registration key.
  /// </summary>
  public string? Key { get; }

  /// <summary>
  /// Gets the desired lifetime.
  /// </summary>
  public ServiceLifetime Lifetime { get; }

  /// <summary>
  /// Initializes a new instance with a specified lifetime.
  /// </summary>
  /// <param name="lifetime">The lifetime.</param>
  public ContainerRegistrationAttribute(ServiceLifetime lifetime)
  {
    Lifetime = lifetime;
  }

  /// <summary>
  /// Initializes a new instance with a specified lifetime and key.
  /// </summary>
  /// <param name="lifetime">The lifetime.</param>
  /// <param name="key">The registration key.</param>
  public ContainerRegistrationAttribute(ServiceLifetime lifetime, string key)
  {
    Lifetime = lifetime;
    Key = key;
  }
}
