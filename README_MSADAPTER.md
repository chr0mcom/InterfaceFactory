# InterfaceFactory.ContainerAdapter.DependencyInjection

InterfaceFactory.ContainerAdapter.DependencyInjection is the Microsoft.Extensions.DependencyInjection adapter for InterfaceFactory. This package bridges the gap between InterfaceFactory's container-agnostic service registration and resolution, and the native dependency injection provided by Microsoft's DI container.

> NOTE: This adapter is intended to be used with the core InterfaceFactory project. For details on the core concepts and usage, please see the [InterfaceFactory README](https://github.com/yourrepo).

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Installation](#installation)
- [Getting Started](#getting-started)
- [Usage Examples](#usage-examples)
- [Configuration & Customizations](#configuration--customizations)
- [Contributing](#contributing)
- [License](#license)

---

## Overview

This adapter package integrates InterfaceFactory with Microsoft.Extensions.DependencyInjection, enabling you to:

- Automatically scan assemblies and register services with your DI container.
- Use extension methods to streamline the configuration of the interface factory using Microsoft’s ServiceCollection.
- Leverage keyed and non-keyed registrations with minimal boilerplate code.

Under the hood, the adapter implements the IContainerResolveAdapter and IContainerRegisterAdapter interfaces. It delegates all DI operations (such as singleton, scoped, and transient registrations) directly to the Microsoft DI container using extension methods like AddKeyedSingleton, AddKeyedScoped, and AddKeyedTransient.

---

## Features

- **Seamless Integration**: Easily register all interface factories into your Microsoft DI container.
- **Dynamic Assembly Scanning**: Automatically detects and registers services from all loaded (or optionally unloaded) assemblies.
- **Keyed Registrations Support**: Register multiple implementations of the same interface under different keys.
- **Centralized Resolution**: Configure a single point of service resolution through the central container adapter.

---

## Installation

You can install the adapter via NuGet. Use one of the following commands:

Using .NET CLI:
> dotnet add package InterfaceFactory.ContainerAdapter.DependencyInjection

Using Package Manager:
> Install-Package InterfaceFactory.ContainerAdapter.DependencyInjection

---

## Getting Started

To set up the adapter in your application, follow these steps:

1. **Add the NuGet package**  
   Install the adapter package into your project.

2. **Register Interface Factories**  
   In your startup or composition root, configure your IServiceCollection as follows:
   ```csharp
   using Microsoft.Extensions.DependencyInjection;
   using InterfaceFactory.ContainerAdapter.DependencyInjection;

   // Create a new service collection
   var serviceCollection = new ServiceCollection();

   // Register all interface factories
   // The optional 'includeUnloadedAssemblies' flag will scan for services in assemblies not yet loaded.
   serviceCollection.RegisterInterfaceFactories(includeUnloadedAssemblies: true);

   // Build the ServiceProvider
   var serviceProvider = serviceCollection.BuildServiceProvider();
   ```

3. **Configure the Container Adapter**  
   Set the built ServiceProvider as the active container adapter:
   ```csharp
   // Make the service provider available to InterfaceFactory
   serviceProvider.UseInterfaceFactory();
   ```

Once configured, you can resolve your services via your interfaces extending IFactory<T> (as defined in the core InterfaceFactory package).

---

## Usage Examples

### Example 1: Basic Service Registration & Resolution

```csharp
// Define an interface that extends IFactory<T>
public interface IExample : IFactory<IExample> { }

// Implement the interface with a class decorated with ContainerRegistration attribute
[ContainerRegistration(ServiceLifetime.Scoped, "MyExample")]
public class MyExample : IExample { }

// In your application startup:
using Microsoft.Extensions.DependencyInjection;

var serviceCollection = new ServiceCollection();
serviceCollection.RegisterInterfaceFactories(includeUnloadedAssemblies: false);
var serviceProvider = serviceCollection.BuildServiceProvider();
serviceProvider.UseInterfaceFactory();

// Resolve service using the default method extended through IFactory<T>
IExample instance = IExample.GetRequiredKeyedInstance("MyExample");
```

### Example 2: Keyed Registrations with Multiple Implementations

```csharp
// Define the interface
public interface IReport : IFactory<IReport> { }

// Implement two separate versions of the interface
[ContainerRegistration(ServiceLifetime.Transient, "SimpleReport")]
public class SimpleReport : IReport { }

[ContainerRegistration(ServiceLifetime.Transient, "DetailedReport")]
public class DetailedReport : IReport { }

// In your configuration:
var serviceCollection = new ServiceCollection();
serviceCollection.RegisterInterfaceFactories(includeUnloadedAssemblies: true);
var serviceProvider = serviceCollection.BuildServiceProvider();
serviceProvider.UseInterfaceFactory();

// Resolve services by their key
IReport simpleReport = IReport.GetRequiredKeyedInstance("SimpleReport");
IReport detailedReport = IReport.GetRequiredKeyedInstance("DetailedReport");
```

---

## Configuration & Customizations

### Keyed Registrations

The adapter supports keyed registrations. When you decorate your implementation with the ContainerRegistration attribute and specify a key, the adapter uses extension methods like `AddKeyedSingleton`, `AddKeyedScoped`, or `AddKeyedTransient` to register the service in the Microsoft DI container.  
*Note:* The actual implementation of these keyed extension methods must be available in your solution. If you’re using a third-party library that provides keyed registrations, ensure that it is referenced accordingly.

### Adjusting Scanning Behavior

The extension method `RegisterInterfaceFactories` accepts a boolean parameter `includeUnloadedAssemblies`. Set this flag to true if you want the adapter to scan for additional assemblies in the current folder that have not yet been loaded. This allows for a more comprehensive registration but may impact startup performance if many assemblies are present.

### Further Customizations

If your project has specific DI requirements or you want to hook into the registration or resolution process, consider creating your own adapter that extends IContainerResolveAdapter and IContainerRegisterAdapter. The Microsoft adapter provided in this package is designed to be simple and straightforward, but it is flexible enough to support customization if needed.

---

## Contributing

Contributions are welcome! Please refer to the [Contributing Guidelines](https://github.com/yourrepo/CONTRIBUTING.md) in the main repository for more details on how to contribute, report issues, or propose improvements.

---

## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/yourrepo/LICENSE) file for full license information.
```
