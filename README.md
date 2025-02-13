# InterfaceFactory

InterfaceFactory is a modular library designed to simplify service registration and resolution using a dependency injection (DI) container. This project leverages the power of default implementations in interfaces to provide a clean, centralized, and container-agnostic interface—while still respecting core software architecture principles.

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
- [Usage Examples](#usage-examples)
- [Default Implementations & Software Architecture](#default-implementations--software-architecture)
- [NuGet Packages](#nuget-packages)
- [Contributing](#contributing)
- [License](#license)

---

## Overview

InterfaceFactory aims to streamline the process of registering and resolving services within an application. By introducing a set of abstraction layers and default implementations via interfaces (notably through `IFactory<T>`), it provides an easy-to-use API for service retrieval without sacrificing type safety or architectural clarity.

Key aspects include:

- **Automatic Service Registration**: Scans assemblies for implementations of interfaces conforming to specified contracts (using the `ContainerRegistration` attribute) and registers them with appropriate lifetimes.
- **Centralized DI Integration**: Encapsulates the DI container within a central adapter, making it simple to swap out underlying implementations if needed.
- **Default Interface Implementations**: Provides default methods within factory interfaces that delegate service resolution requests directly to the DI container.

---

## Features

- **Dynamic Assembly Scanning**: Automatically detects and registers services by scanning loaded (and optionally unloaded) assemblies.
- **Flexible Service Lifetimes**: Supports Singleton, Scoped, and Transient lifetimes via the `ServiceLifetime` enumeration.
- **Keyed Registrations**: Enables associating services with keys to distinguish between multiple implementations of the same interface.
- **Unified Service Resolution**: Default implementations in `IFactory<T>` allow for a unified, container-agnostic method of accessing services.

---

## Architecture

The project is structured around a few core components:

1. **ContainerRegistrationAttribute & ServiceLifetime**  
   - These define configuration metadata for service registration.
   - The attribute accepts an optional key and the desired lifetime, ensuring that each service is registered with the correct scope.

2. **IContainerAdapter & ContainerAdapterContainer**  
   - `IContainerAdapter` abstracts the underlying DI container operations such as registration and resolution.
   - `ContainerAdapterContainer` is a static holder that manages the current DI adapter instance.

3. **IFactory<T>**  
   - Acts as both a contract and a gateway for resolving services.
   - Default methods like `GetInstance()` and `GetRequiredInstance()` delegate resolution requests to the central container adapter, ensuring that the business logic remains completely decoupled from the DI setup.

4. **Container Registration Mechanism**  
   - Scans and registers all matching service interfaces automatically.  
   - Integration with the Microsoft.Extensions.DependencyInjection container is provided via extension methods, ensuring seamless DI container registration and usage.

---

## Getting Started

To integrate InterfaceFactory into your project:

1. Install the provided NuGet packages.
2. Register services with your DI container by extending your `ServiceCollection` using:
   ```csharp
   serviceCollection.RegisterInterfaceFactories(includeUnloadedAssemblies: true);
   ```
3. Once the `ServiceProvider` is built, set it by calling:
   ```csharp
   serviceProvider.UseInterfaceFactory();
   ```
4. Retrieve services through the default methods in `IFactory<T>`, e.g.:
   ```csharp
   var myService = IExample.GetInstance();
   var myKeyedService = IExample.GetRequiredKeyedInstance("MyServiceKey");
   ```

---

## Usage Examples

### Example 1: Basic Service Resolution

```csharp
public interface IExample : IFactory<IExample> { }

[ContainerRegistration(ServiceLifetime.Scoped, "MyExample")]
public class MyExample : IExample { }

// Service registration and usage:
ServiceCollection serviceCollection = new();
serviceCollection.RegisterInterfaceFactories(includeUnloadedAssemblies: true);
IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
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

// Retrieve specific implementation by key:
IReport simpleReport = IReport.GetRequiredKeyedInstance("SimpleReport");
IReport detailedReport = IReport.GetRequiredKeyedInstance("DetailedReport");
```

---

## Default Implementations & Software Architecture

### Addressing Architectural Concerns

Default implementations in interfaces have often been criticized for several reasons:
- **Blurring of Responsibility**: Critics argue that default logic in an interface can mix contract definition with implementation details.
- **Potential for Naming Conflicts**: Multiple inheritance-like issues might arise if two interfaces provide default implementations for the same members.
- **Hidden Dependencies**: Developers might not notice that default code introduces hidden dependencies or side effects.
- **Testability Challenges**: The embedded logic might complicate the unit testing of components using such interfaces.

### How InterfaceFactory Mitigates These Concerns

In InterfaceFactory, the default implementations in `IFactory<T>` are intentionally kept minimalistic and serve only as a direct delegation mechanism. They provide a clear and declarative way to access services from the central DI container without introducing any business logic or hidden side effects.

#### Clear Separation of Concerns
- **Interface as a Contract**: The `IFactory<T>` interface strictly serves as a contract for service retrieval. The default methods (`GetInstance()`, `GetRequiredInstance()`, etc.) do not contain complex logic; they simply delegate to the central container adapter.
- **Centralized Logic**: All actual service resolution and DI-related logic is encapsulated within the `ContainerAdapter`. This keeps the interface free from domain-specific dependencies.

#### Avoiding Naming Conflicts
- **Explicit Calling Convention**: Default methods are invoked statically (e.g., `IExample.GetInstance()`), which avoids issues related to multiple inheritances seen in instance method conflicts. Each interface declares its own default methods, and there is no ambiguity about which implementation is used.
- **Scoped Usage**: Since the default implementations serve purely as helpers, they are not extended with additional behavior that could lead to conflict.

#### Enhancing Testability and Modularity
- **Mockable Container Adapter**: Because the default methods delegate to a static container adapter instance, you can easily provide a mock or a specialized test instance of the DI container. This allows tests to substitute the underlying service resolution logic without altering the interface itself.
- **Consistent Behavior**: The default implementation establishes a uniform entry point for service resolution. This reduces boilerplate code and minimizes the risk of implementation discrepancies across different services.

### Practical Example

Imagine an application where you have several service implementations that need to be resolved dynamically. The default implementations in `IFactory<T>` provide a concise, standardized way to access the DI container:

1. **Without Default Implementations**:  
   Each service consumer would have to manually acquire a reference to the DI container, leading to repeated boilerplate and potential inconsistencies.

2. **With Default Implementations (InterfaceFactory Approach)**:  
   By calling `IExample.GetInstance()`, the consumer seamlessly accesses the registered service without having to worry about the DI container's internal details. The central adapter handles all nuances of service resolution, ensuring that the calling code remains clean and focused on domain logic.

In essence, the design of InterfaceFactory demonstrates that default implementations—when limited to delegation and devoid of business logic—can coexist with sound software architecture principles such as separation of concerns, modularity, and testability.

---

## NuGet Packages

InterfaceFactory is available as two NuGet packages:

- **InterfaceFactory.Core**  
  Contains the core abstractions, attributes, and default factory implementations.
  
- **InterfaceFactory.DependencyInjection**  
  Provides integration and extensions for Microsoft.Extensions.DependencyInjection.

For more details, refer to the [GitHub Repository](https://github.com/yourrepo).

---

## Contributing

Contributions are welcome! Please see the [Contributing Guidelines](CONTRIBUTING.md) for details on how to propose improvements or report issues.

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

We invite you to explore the repository and try out the NuGet packages. Should you have any questions or suggestions regarding the approach to Default Interface Implementations in InterfaceFactory, feel free to open an issue or reach out directly.
