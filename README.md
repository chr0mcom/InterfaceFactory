# InterfaceFactory

InterfaceFactory is a lightweight, extensible factory solution for C# that integrates with Microsoft’s Dependency Injection container. It provides a factory abstraction over DI by:

- **Defining a Generic Factory Interface**: `IFactory<T>` with static helper methods to resolve services.  
- **Container Adapter Abstraction**: An abstraction via `IContainerAdapter` to support registration and resolution from various IoC containers.
- **Attribute-Based Registration**: Using `ContainerRegistrationAttribute` to annotate concrete classes for automatic registration with correct lifetimes and keys.
- **Automated Assembly Scanning**: Automatically scans assemblies to register eligible services with minimal configuration.

---

## Features

- **Generic Factory Interface – IFactory<T>**  
  Provides static helper methods (`GetInstance`, `GetRequiredInstance`, `GetKeyedInstance`, and `GetRequiredKeyedInstance`) to resolve services from the DI container.

- **Container Adapter Abstraction – IContainerAdapter**  
  Decouples application code from the specific DI container implementation. Currently, it supports Microsoft.Extensions.DependencyInjection.

- **Attribute-Based Registration – ContainerRegistrationAttribute**  
  Simplifies service registration by decorating concrete classes with the desired service lifetime (`Singleton`, `Scoped`, or `Transient`) and an optional registration key.

- **Automated Assembly Scanning**  
  Uses the `ContainerRegistration.RegisterInterfaceFactories` method to scan assemblies and automatically register services based on defined conventions.

---

## Installation

Install the NuGet package via the Package Manager Console:

```powershell
Install-Package InterfaceFactory
```

Or via the .NET CLI:

```bash
dotnet add package InterfaceFactory
```

---

## Usage

### 1. Registering Services

In your application startup, create and configure your `ServiceCollection` as usual. Then automatically register your factories by scanning the loaded (or local folder) assemblies:

```csharp
using InterfaceFactory;
using InterfaceFactory.ContainerAdapter.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

// Create the ServiceCollection
ServiceCollection serviceCollection = new ServiceCollection();

// Optionally, include additional assemblies from the current folder
serviceCollection.RegisterInterfaceFactories(includeUnloadedAssemblies: true);

// Build the ServiceProvider
ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

// Set the active container adapter (this wires up ContainerAdapterContainer.Instance)
serviceProvider.UseInterfaceFactory();
```

### 2. Defining a Factory Interface and Concrete Implementation

Define an interface that extends `IFactory<T>` and implement it. Optionally add the `ContainerRegistrationAttribute` to control lifetime and key registration:

```csharp
public interface IExample : IFactory<IExample> { }

[ContainerRegistration(ServiceLifetime.Scoped, nameof(MyExample))]
public class MyExample : IExample { }
```

### 3. Resolving Services

Once registered, resolve your service using the static factory methods defined on `IFactory<T>`:

```csharp
// Returns null if the service is not registered
IExample? example1 = IExample.GetInstance();

// Throws an exception if the keyed service is not available
IExample example2 = IExample.GetRequiredKeyedInstance(nameof(MyExample));
```

---

## Configuration & Conventions

- **Automatic Service Registration**  
  The `ContainerRegistration.RegisterInterfaceFactories` method scans all loaded assemblies and locates concrete classes that implement an interface extending `IFactory<T>`. It then registers these services using the provided lifetime from the `ContainerRegistrationAttribute`.

- **Customizing Container Behavior**  
  The container adapter used by InterfaceFactory is managed via the static `ContainerAdapterContainer.Instance`. An implementation integrating with `Microsoft.Extensions.DependencyInjection` is provided by default. You can implement and set your own `IContainerAdapter` if needed.

---

## Contributing

Contributions are welcome! If you find bugs or have feature requests, please open an issue or submit a pull request on GitHub.

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

---

## Contact

For questions or additional information, please contact [Your Name or Support Email].

---

## Notes

- Ensure the container adapter is properly set (e.g., by calling `serviceProvider.UseInterfaceFactory()`) before resolving services via the static methods on `IFactory<T>`.  
- Including unloaded assemblies is optional. Note that scanning additional assemblies may affect startup performance, so use this feature based on your project's needs.

Happy coding with InterfaceFactory!
