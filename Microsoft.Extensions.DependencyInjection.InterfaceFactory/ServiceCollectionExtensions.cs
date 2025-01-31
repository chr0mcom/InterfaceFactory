using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection.InterfaceFactory;

public static class ServiceCollectionExtensions
{
  /// <summary>
  ///   Registers interfaces that extend <see cref="IFactory{T}" /> in a self-referencing manner with their respective
  ///   concrete implementations.
  /// </summary>
  /// <param name="serviceCollection">
  ///   The <see cref="IServiceCollection" /> to which the services will be added.
  /// </param>
  /// <param name="includeUnloadedAssemblies">
  ///   A boolean value indicating whether to include assemblies that are not yet loaded into the current
  ///   <see cref="AppDomain" />.
  ///   If <c>true</c>, all assemblies in the current folder will be loaded before registration.
  /// </param>
  /// <returns>
  ///   The updated <see cref="IServiceCollection" /> with the registered services.
  /// </returns>
  /// <remarks>
  ///   This method scans all loaded assemblies (and optionally loads additional assemblies from the current folder)
  ///   to find concrete types implementing interfaces that extend <see cref="IFactory{T}" />. It registers these interfaces
  ///   with their corresponding concrete implementations using the <c>AddScoped</c> method.
  ///   For example, given the following interface and implementation:
  ///   <code>
  /// public interface IExample : IFactory{IExample} {}
  /// public class MyExample : IExample {}
  /// </code>
  ///   This method registers the service as:
  ///   <c>serviceCollection.AddScoped&lt;IExample, MyExample&gt;()</c>.
  /// </remarks>
  /// <exception cref="ArgumentNullException">
  ///   Thrown if <paramref name="serviceCollection" /> is <c>null</c>.
  /// </exception>
  // ReSharper disable once FlagArgument
  public static IServiceCollection RegisterInterfaceFactories(this ServiceCollection serviceCollection,
    bool includeUnloadedAssemblies = false)
  {
    if (includeUnloadedAssemblies) LoadAllAssembliesFromCurrentFolder();

    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
    var concreteTypes = assemblies.SelectMany(GetTypesSafe)
      .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition);

    foreach (var concreteType in concreteTypes)
    {
      var interfacesToRegister = concreteType.GetInterfaces().Where(iface =>
        iface.IsInterface && iface.GetInterfaces().Any(baseIface =>
          baseIface.IsGenericType &&
          baseIface.GetGenericTypeDefinition() == typeof(IFactory<>) &&
          baseIface.GetGenericArguments()[0] == iface));

      foreach (var iface in interfacesToRegister)
      {
        var keyNameProperty = concreteType.GetProperty("FactoryKeyName", BindingFlags.Public | BindingFlags.Static);
        object? keyValue = keyNameProperty?.GetValue(null);

        if (keyValue is not null)
        {
          serviceCollection.AddKeyedScoped(iface, keyValue, concreteType);
        }
        else
        {
          serviceCollection.AddScoped(iface, concreteType);
        }
      }
    }

    return serviceCollection;
  }

  private static void LoadAllAssembliesFromCurrentFolder()
  {
    var assemblies = new HashSet<Assembly>(AppDomain.CurrentDomain.GetAssemblies());
    var loadedPaths = assemblies.Select(a => a.Location).Where(p => !string.IsNullOrEmpty(p)).ToArray();

    var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    if (basePath == null) return;

    var dllFiles = Directory.GetFiles(basePath, "*.dll", SearchOption.TopDirectoryOnly)
      .Where(dll => !loadedPaths.Contains(dll, StringComparer.InvariantCultureIgnoreCase))
      .ToList();

    foreach (var dllPath in dllFiles)
    {
      try
      {
        var asmName = AssemblyName.GetAssemblyName(dllPath);
        if (asmName != null && !assemblies.Any(a => a.FullName == asmName.FullName))
        {
          var loadedAssembly = Assembly.LoadFrom(dllPath);
          AppDomain.CurrentDomain.Load(loadedAssembly.GetName());
        }
      }
      catch
      {
        // There is no real reason to do anything here. If an error is thrown, then this is surely only because the dll does not belong to the application.
      }
    }
  }

  private static Type[] GetTypesSafe(Assembly assembly)
  {
    try
    {
      return assembly.GetTypes();
    }
    catch
    {
      // If types cannot be loaded, return an empty array so the remaining types
      // in other assemblies can still be processed.
      return [];
    }
  }
}
