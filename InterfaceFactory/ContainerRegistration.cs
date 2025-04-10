using System.Reflection;

namespace InterfaceFactory;

/// <summary>
/// Provides methods to scan and register services using the container adapter.
/// </summary>
public static class ContainerRegistration
{
  internal const string TransientKey = "InterfaceFactoryTransientKey";
  
  private static IContainerResolveAdapter? _resolveInstance;
  internal static IContainerResolveAdapter ResolveInstance => _resolveInstance ?? throw new ArgumentNullException($"The IContainerResolveAdapter was not set. Please use a ContainerAdapter, e.g. InterfaceFactory.ContainerAdapter.DependencyInjection.");

  /// <summary>
  /// Sets the container resolve adapter to be used for resolving services.
  /// </summary>
  /// <param name="resolveAdapter">
  /// An instance of <see cref="IContainerResolveAdapter"/> that provides the functionality 
  /// for resolving services from the container.
  /// </param>
  /// <returns>
  /// The provided <see cref="IContainerResolveAdapter"/> instance, allowing for method chaining.
  /// </returns>
  /// <remarks>
  /// This method allows you to specify the container resolve adapter that will be used 
  /// throughout the application. Ensure that the provided <paramref name="resolveAdapter"/> 
  /// is properly configured before calling this method.
  /// </remarks>
  /// <exception cref="ArgumentNullException">
  /// Thrown if the <paramref name="resolveAdapter"/> is <c>null</c>.
  /// </exception>
  public static T SetContainerResolveAdapter<T>(this T resolveAdapter) where T : IContainerResolveAdapter => (T)(_resolveInstance = resolveAdapter);
  
  /// <summary>
  /// Scans the current application domain for concrete classes implementing interfaces derived 
  /// from <see cref="IFactory{T}"/> and registers them with the provided container adapter.
  /// </summary>
  /// <param name="registerAdapter">
  /// The container adapter used to register the discovered interfaces and their implementations.
  /// </param>
  /// <param name="includeUnloadedAssemblies">
  /// A boolean value indicating whether to load and scan assemblies from the current folder 
  /// that are not already loaded into the application domain.
  /// </param>
  /// <returns>
  /// The same <see cref="IContainerRegisterAdapter"/> instance passed as the <paramref name="registerAdapter"/> parameter.
  /// </returns>
  /// <remarks>
  /// This method identifies concrete classes that implement interfaces derived from 
  /// <see cref="IFactory{T}"/>. If a <see cref="ContainerRegistrationAttribute"/> is present 
  /// on the class, its properties (e.g., lifetime and key) are used to customize the registration.
  /// </remarks>
  // ReSharper disable once MethodTooLong
  // ReSharper disable once FlagArgument
  public static T RegisterInterfaceFactories<T>(this T registerAdapter, bool includeUnloadedAssemblies = false) where T : IContainerRegisterAdapter
  {
    if (includeUnloadedAssemblies) LoadAllAssembliesFromCurrentFolder();

    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
    // ReSharper disable once ComplexConditionExpression
    var concreteTypes = assemblies.SelectMany(GetTypesSafe)
      .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition && t.GetCustomAttribute<IgnoreContainerRegistrationAttribute>() is null);
    foreach (var concreteType in concreteTypes)
    {
      // ReSharper disable once ComplexConditionExpression
      var interfacesToRegister = concreteType.GetInterfaces()
        .Where(iface =>
          iface.IsInterface &&
          iface.GetInterfaces().Any(baseIface =>
            baseIface.IsGenericType &&
            baseIface.GetGenericTypeDefinition() == typeof(IFactory<>) &&
            baseIface.GetGenericArguments()[0] == iface));
      
      foreach (var serviceInterface in interfacesToRegister)
      {
        var registrationAttribute = concreteType.GetCustomAttribute<ContainerRegistrationAttribute>();

        if (registrationAttribute?.Key is not null)
        {
          registerAdapter.RegisterKeyed(serviceInterface, concreteType, registrationAttribute.Lifetime, registrationAttribute.Key);
          if (registrationAttribute.Lifetime == ServiceLifetime.Scoped)
            registerAdapter.RegisterKeyed(serviceInterface, concreteType, ServiceLifetime.Transient, TransientKey+ registrationAttribute.Key);
        }
        else
        {
          registerAdapter.Register(serviceInterface, concreteType, registrationAttribute?.Lifetime ?? ServiceLifetime.Scoped);
          if (registrationAttribute == null || registrationAttribute.Lifetime == ServiceLifetime.Scoped)
            registerAdapter.RegisterKeyed(serviceInterface, concreteType, ServiceLifetime.Transient, TransientKey);
        }

      }
    }

    return registerAdapter;
  }

  // ReSharper disable once TooManyDeclarations
  private static void LoadAllAssembliesFromCurrentFolder()
  {
    var loadedAssemblies = new HashSet<Assembly>(AppDomain.CurrentDomain.GetAssemblies());
    var loadedPaths = loadedAssemblies.Select(a => a.Location)
      .Where(p => !string.IsNullOrEmpty(p));
    var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
    var dllFiles = Directory.GetFiles(basePath, "*.dll", SearchOption.TopDirectoryOnly)
      .Where(dll => !loadedPaths.Contains(dll, StringComparer.InvariantCultureIgnoreCase));
    foreach (var dllPath in dllFiles)
    {
      try
      {
        var asmName = AssemblyName.GetAssemblyName(dllPath);
        if (loadedAssemblies.All(a => a.FullName != asmName.FullName))
        {
          Assembly.LoadFrom(dllPath);
        }
      }
      catch
      {
        // There is no real reason to do anything here.
        // If an error is thrown, then this is surely only because the dll does not belong to the application.
      }
    }
  }

  private static IEnumerable<Type> GetTypesSafe(Assembly assembly)
  {
    try
    {
      return assembly.GetTypes();
    }
    catch
    {
      return Array.Empty<Type>();
    }
  }
}
