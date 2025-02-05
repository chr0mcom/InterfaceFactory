namespace InterfaceFactory;

/// <summary>
/// Holds the current container adapter instance.
/// </summary>
public static class ContainerAdapterContainer
{
  private static IContainerAdapter? _current;

  /// <summary>
  /// Gets or sets the current container adapter.
  /// </summary>
  public static IContainerAdapter Instance
  {
    get => _current ?? throw new ArgumentNullException($"The IContainerAdapter was not set. Please use a ContainerAdapter, e.g. InterfaceFactory.ContainerAdapter.DependencyInjection.");
    set => _current = value;
  }
}
