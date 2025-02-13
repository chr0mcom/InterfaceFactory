namespace InterfaceFactory;

/// <summary>
/// Provides a static container for managing the resolution of dependencies using an implementation
/// of <see cref="IContainerResolveAdapter"/>.
/// </summary>
/// <remarks>
/// The <see cref="ContainerAdapterContainer"/> class acts as a central point for setting and retrieving
/// the current dependency resolution adapter. It is primarily used to facilitate dependency injection
/// and service resolution within the application.
/// </remarks>
public static class ContainerAdapterContainer
{
  private static IContainerResolveAdapter? _resolveInstance;

  /// <summary>
  /// Gets or sets the instance of <see cref="IContainerResolveAdapter"/> used for resolving dependencies.
  /// </summary>
  /// <exception cref="ArgumentNullException">
  /// Thrown when attempting to get the property and the <see cref="IContainerResolveAdapter"/> instance has not been set.
  /// </exception>
  /// <remarks>
  /// This property must be initialized with a valid <see cref="IContainerResolveAdapter"/> implementation 
  /// before it can be used. It serves as the central point for resolving dependencies within the application.
  /// </remarks>
  public static IContainerResolveAdapter ResolveInstance
  {
    get => _resolveInstance ?? throw new ArgumentNullException($"The IContainerResolveAdapter was not set. Please use a ContainerAdapter, e.g. InterfaceFactory.ContainerAdapter.DependencyInjection.");
    set => _resolveInstance = value;
  }
}
