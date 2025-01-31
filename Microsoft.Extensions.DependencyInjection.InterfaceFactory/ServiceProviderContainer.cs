namespace Microsoft.Extensions.DependencyInjection.InterfaceFactory;

internal static class ServiceProviderContainer
{
    private static IServiceProvider? _current;

    public static IServiceProvider Current
    {
        get => _current ?? throw new ArgumentNullException($"The ServiceProvider was not set. Please call the extension method 'UseInterfaceFactory' on the ServiceProvider.");
        set => _current = value;
    }
}