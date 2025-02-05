using System.Reflection;

namespace InterfaceFactory;

/// <summary>
/// Provides methods to scan and register services using the container adapter.
/// </summary>
public static class ContainerRegistration
{
  /// <summary>
  /// Scans the current assemblies for classes implementing IFactory and decorated with ContainerRegistrationAttribute, then registers them.
  /// </summary>
  /// <param name="includeUnloadedAssemblies">Whether to load assemblies from the current folder that are not already loaded.</param>
  // ReSharper disable once MethodTooLong
  // ReSharper disable once FlagArgument
  public static void RegisterInterfaceFactories(bool includeUnloadedAssemblies = false)
  {
    if (includeUnloadedAssemblies) LoadAllAssembliesFromCurrentFolder();

    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
    // ReSharper disable once ComplexConditionExpression
    var concreteTypes = assemblies.SelectMany(GetTypesSafe)
      .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition);
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
          ContainerAdapterContainer.Instance.RegisterKeyed(serviceInterface, concreteType, registrationAttribute.Lifetime, registrationAttribute.Key);
        }
        else
        {
          ContainerAdapterContainer.Instance.Register(serviceInterface, concreteType, registrationAttribute?.Lifetime ?? ServiceLifetime.Scoped);
        }
      }
    }
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
