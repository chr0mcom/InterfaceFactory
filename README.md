# InterfaceFactory

InterfaceFactory is a modular C# library that streamlines service registration and resolution using a dependency injection (DI) container. By leveraging default implementations in interfaces (via IFactory<T>), it provides a clean, centralized, and container-agnostic API for retrieving services—all while preserving strong typing and sound architectural principles.

> NOTE: This README is for the core InterfaceFactory project only. The adapter project (InterfaceFactory.ContainerAdapter.DependencyInjection) will have its own README.

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
- [Usage Examples](#usage-examples)
  - [Basic Service Resolution](#example-1-basic-service-resolution)
  - [Multiple Implementations](#example-2-multiple-implementations)
  - [Custom IoC Container Adapter](#writing-your-own-ioc-container-adapter)
- [Default Implementations & Software Architecture](#default-implementations--software-architecture)
- [NuGet Packages](#nuget-packages)
- [Contributing](#contributing)
- [License](#license)

---

## Overview

InterfaceFactory simplifies the process of registering and resolving services within your application. It automatically scans assemblies for service implementations (marked with a [ContainerRegistration] attribute) and registers them with standardized lifetimes (Singleton, Scoped, or Transient).

Key benefits include:

- **Automatic Service Registration**: Scans for implementations conforming to common contracts and registers them.
- **Centralized DI Integration**: Encapsulates DI operations via a container adapter, making it easy to swap underlying DI containers.
- **Default Interface Implementations**: Provides default methods on factory interfaces (e.g., GetInstance(), GetRequiredInstance(), etc.) that delegate service resolution to the configured container adapter.

---

## Features

- **Dynamic Assembly Scanning**: Automatically detects and registers services from all loaded (or optionally unloaded) assemblies.
- **Flexible Lifetimes**: Supports Singleton, Scoped, and Transient service lifetimes using the ServiceLifetime enum.
- **Keyed Registrations**: Facilitates the registration of multiple implementations of the same interface by associating them with keys.
- **Unified Service Resolution**: Leverages default interface implementations (via IFactory<T>) for a container-agnostic approach to accessing services.

---

## Architecture

InterfaceFactory is built around several core concepts:

1. **ContainerRegistrationAttribute & ServiceLifetime**  
   - These allow you to configure service registration metadata (optionally including a key and lifetime).

2. **IContainerAdapter Interface**  
   - Comprised of IContainerResolveAdapter and IContainerRegisterAdapter, these interfaces abstract the underlying DI container operations so that InterfaceFactory can work with any IoC container.

3. **IFactory<T> Interface**  
   - This interface serves as both a contract and a gateway for resolving services. Its default implementations (e.g., GetInstance(), GetRequiredInstance()) encapsulate the lookup logic.

4. **Central Registration & Resolution**  
   - A static helper, ContainerRegistration, scans for types that implement IFactory<T> and registers them with the specified lifetimes using a container adapter.

---

## Getting Started

To integrate InterfaceFactory into your application:

1. **Install the NuGet Packages**
   - For core functionality, add the **InterfaceFactory.Core** package.
   - Optionally, if you wish to use the built-in adapter, add **InterfaceFactory.DependencyInjection** (Note: this project has its own README).

2. **Register Services**
   - Extend your DI container’s ServiceCollection by calling:
     ```csharp
     serviceCollection.RegisterInterfaceFactories(includeUnloadedAssemblies: true);
     ```

3. **Configure the Service Provider**
   - After building your IServiceProvider, set it for the factory by calling:
     ```csharp
     serviceProvider.UseInterfaceFactory();
     ```

4. **Resolve Services via IFactory<T>**
   - Retrieve services using the default methods:
     ```csharp
     var myService = IExample.GetInstance();
     var keyedService = IExample.GetRequiredKeyedInstance("MyServiceKey");
     ```

---

## Usage Examples

### Example 1: Basic Service Resolution

```csharp
public interface IExample : IFactory<IExample> { }

[ContainerRegistration(ServiceLifetime.Scoped, "MyExample")]
public class MyExample : IExample { }

// Service registration and usage:
var serviceCollection = new ServiceCollection();
serviceCollection.RegisterInterfaceFactories(includeUnloadedAssemblies: true);
var serviceProvider = serviceCollection.BuildServiceProvider();
serviceProvider.UseInterfaceFactory();

IExample? instance = IExample.GetInstance();
IExample requiredInstance = IExample.GetRequiredKeyedInstance("MyExample");
```

### Example 2: Multiple Implementations

```csharp
public interface IReport : IFactory<IReport> { }

[ContainerRegistration(ServiceLifetime.Transient, "SimpleReport")]
public class SimpleReport : IReport { }

[ContainerRegistration(ServiceLifetime.Transient, "DetailedReport")]
public class DetailedReport : IReport { }

// Retrieve specific implementations by key:
IReport simpleReport = IReport.GetRequiredKeyedInstance("SimpleReport");
IReport detailedReport = IReport.GetRequiredKeyedInstance("DetailedReport");
```

### Writing Your Own IoC Container Adapter

InterfaceFactory is designed to be container-agnostic. If you wish to use a different IoC container, you can create your own adapter by implementing the following interfaces:
- IContainerResolveAdapter
- IContainerRegisterAdapter

Below is an example of a custom adapter for a hypothetical IoC container called "CustomIoCContainer":

```csharp
// Example: Creating a custom IoC Container adapter.
public class CustomIoCContainerAdapter : IContainerResolveAdapter, IContainerRegisterAdapter
{
    private readonly CustomIoCContainer _container;

    public CustomIoCContainerAdapter(CustomIoCContainer container)
    {
        _container = container;
    }

    public void Register(Type serviceType, Type implementationType, ServiceLifetime lifetime)
    {
        // Use your container's API to register the service.
        _container.Register(serviceType, implementationType, ConvertLifetime(lifetime));
    }

    public void RegisterKeyed(Type serviceType, Type implementationType, ServiceLifetime lifetime, string? key)
    {
        if (key is null)
        {
            Register(serviceType, implementationType, lifetime);
        }
        else
        {
            _container.RegisterKeyed(serviceType, implementationType, ConvertLifetime(lifetime), key);
        }
    }

    public T? Resolve<T>()
    {
        return _container.Resolve<T>();
    }

    public T? ResolveKeyed<T>(string key)
    {
        return _container.ResolveKeyed<T>(key);
    }

    public T ResolveRequired<T>() where T : notnull
    {
        var instance = Resolve<T>();
        if (instance is null)
            throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered.");
        return instance;
    }

    public T ResolveKeyedRequired<T>(string key) where T : notnull
    {
        var instance = ResolveKeyed<T>(key);
        if (instance is null)
            throw new InvalidOperationException($"Keyed service of type {typeof(T).Name} with key '{key}' is not registered.");
        return instance;
    }

    private static CustomLifetime ConvertLifetime(ServiceLifetime lifetime)
    {
        // Convert InterfaceFactory's ServiceLifetime to your container's lifetime type.
        return lifetime switch
        {
            ServiceLifetime.Singleton => CustomLifetime.Singleton,
            ServiceLifetime.Scoped    => CustomLifetime.Scoped,
            ServiceLifetime.Transient => CustomLifetime.Transient,
            _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
        };
    }
}

// Dummy classes representing a custom IoC container and its lifetime settings.
// Replace these with the actual implementations from your IoC container.
public class CustomIoCContainer
{
    public void Register(Type serviceType, Type implementationType, CustomLifetime lifetime)
    {
        // Registration implementation goes here.
    }

    public void RegisterKeyed(Type serviceType, Type implementationType, CustomLifetime lifetime, string key)
    {
        // Keyed registration implementation goes here.
    }

    public T? Resolve<T>()
    {
        // Service resolution implementation goes here.
        return default;
    }

    public T? ResolveKeyed<T>(string key)
    {
        // Keyed resolution implementation goes here.
        return default;
    }
}

public enum CustomLifetime
{
    Singleton,
    Scoped,
    Transient
}
```

By implementing these adapters, you can integrate InterfaceFactory with virtually any DI container.

---

## Default Implementations & Software Architecture

Default implementations in interfaces have sparked significant discussion within the developer community. Below is a detailed look at the common criticisms and a thorough explanation of how InterfaceFactory is designed to address and mitigate these concerns.

### Common Criticisms of Default Implementations in Interfaces

1. **Blurring of Responsibilities**
   - **Criticism:**  
     Interfaces traditionally serve as pure contracts that define behavior without enforcing how that behavior is achieved. Including default implementations can mix the role of a contract with that of an implementation provider, which some argue violates the Single Responsibility Principle.
   - **Concern:**  
     Developers may inadvertently embed logic that should belong to a dedicated service layer or DI container, leading to confusion about where the actual business logic resides.

2. **Potential for Naming Conflicts**
   - **Criticism:**  
     When multiple interfaces offer default implementations for similar or same-named methods, it can result in ambiguity during multiple inheritance or when implementing multiple interfaces in a single class. This may lead to unexpected behavior or increased complexity in resolving which method should be executed.
   - **Concern:**  
     Overlapping implementations can create hard-to-diagnose bugs, especially in complex systems where multiple interfaces are composed.

3. **Hidden Dependencies**
   - **Criticism:**  
     Default implementations might introduce dependencies on global state or other hidden elements that are not explicitly declared in the interface. This can make the behavior of the interface less predictable and harder to understand at a glance.
   - **Concern:**  
     Such hidden dependencies can lead to issues during testing or when trying to refactor code, as the dependencies are not as transparent as those declared via constructor injection or method parameters.

4. **Testability Challenges**
   - **Criticism:**  
     Integrating logic directly into an interface's default methods can make it harder to isolate behavior during unit tests. Instead of testing a clean separation of concerns, developers might find themselves testing the intertwined logic of both the contract and its default implementation.
   - **Concern:**  
     This can lead to more complicated test setups and hinder efforts to properly mock or simulate different scenarios during testing.

### How InterfaceFactory Mitigates These Concerns

InterfaceFactory has been carefully designed to address and overcome the above challenges:

1. **Clear Separation of Responsibilities**
   - The default implementations in IFactory<T> are intentionally kept extremely thin:
     - They act purely as delegates, forwarding calls to a centralized container adapter.
     - No business logic or domain-specific behavior is embedded within the interface defaults.
   - This design preserves the role of the interface as a contract, ensuring that the only responsibility of the default implementation is to facilitate dependency resolution.

2. **Avoidance of Naming Conflicts**
   - **Static, Explicit Invocation:**  
     Default methods are invoked statically (for example, IExample.GetInstance()), which eliminates ambiguity. Each interface’s methods are clearly associated with that specific contract.
   - **Single-Purpose Methods:**  
     Since the default implementations do nothing more than delegate to the container adapter, there is no overlap with any other implementation. This explicit behavior prevents conflicts even when multiple interfaces are implemented by the same class.

3. **Transparency of Dependencies**
   - **Single Point of Integration:**  
     All dependencies used by the default implementations are funneled through a centrally configured container adapter.  
   - **Explicit Configuration:**  
     Before any resolution can occur, the container adapter must be explicitly set using methods like UseInterfaceFactory(). This enforces a clear initialization path and makes all external dependencies visible.
   - **No Hidden Business Logic:**  
     The default methods do not contain any logic beyond delegation, ensuring that there are no concealed side effects or additional dependencies introduced unexpectedly.

4. **Enhanced Testability**
   - **Mockable Container Adapter:**  
     The container adapter (implementing IContainerResolveAdapter) can be easily substituted with a mock or stub during tests. This decouples the resolution mechanism from the actual services and enables precise unit testing.
   - **Consistent, Predictable Behavior:**  
     By centralizing all logic in the container adapter (which is designed to be minimal and focused solely on DI operations), tests can be written to target either the adapter or the consumer code without having to account for complex default logic residing in each interface.
   - **Isolated Delegation:**  
     Since default methods serve only as pass-through calls, they are straightforward to test and do not introduce unexpected behavior into the consuming classes.

In summary, InterfaceFactory leverages default interface implementations to simplify DI usage while maintaining a strict separation of concerns. The default methods are deliberately minimalistic, designed only to delegate to a configurable container adapter. This approach:
- Keeps interfaces as true contracts.
- Eliminates potential naming conflicts.
- Avoids hidden dependencies.
- Simplifies testing by isolating DI logic from business functionality.

---

## NuGet Packages

InterfaceFactory is available as two NuGet packages:

- **InterfaceFactory**  
  Contains the core abstractions, attributes, and default factory implementations.
  
- **InterfaceFactory.ContainerAdapter.DependencyInjection**  
  Provides integration and extension methods for Microsoft.Extensions.DependencyInjection.

---

## Contributing

Contributions are welcome! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details on how to propose improvements or report issues.

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
