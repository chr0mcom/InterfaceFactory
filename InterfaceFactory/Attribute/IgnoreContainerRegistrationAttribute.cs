namespace InterfaceFactory;

/// <summary>
/// Attribute to ignore configure service registration.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class IgnoreContainerRegistrationAttribute : Attribute;
