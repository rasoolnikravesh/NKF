// This file ensures the assembly has public types for NuGet packaging
namespace NKF.Resources;

/// <summary>
/// Marker class for the NKF.Resources assembly.
/// This assembly contains embedded resources for localization.
/// </summary>
public static class ResourcesAssembly
{
    /// <summary>
    /// Gets the assembly version.
    /// </summary>
    public static string Version => typeof(ResourcesAssembly).Assembly.GetName().Version?.ToString() ?? "unknown";
}
