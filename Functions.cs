using System.Diagnostics;

namespace StrausTech.CommonLib;

/// <summary>
/// Use tools or functions that can be used in any application
/// </summary>
public static class Tools
{
    /// <summary>
    /// Detects if the system is Windows
    /// </summary>
    /// <returns>True if the it is a Windows System</returns>
    public static bool isWindows()
    {
        return System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
    }

    /// <summary>
    /// Detects if the system is Linux
    /// </summary>
    /// <returns>True if the it is a Linux System</returns>
    public static bool isLinux()
    {
        return System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux);
    }

    /// <summary>
    /// Detects if the system is MacOS
    /// </summary>
    /// <returns>True if the it is a MacOS System</returns>
    public static bool isMacOS()
    {
        return System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX);
    }

    public static bool isDevelopment()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (env.IsNullOrEmpty())
            return false;

        if (env?.ToLower() == "development")
            return true;

        return false;
    }

    public static bool isProduction()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (env.IsNullOrEmpty())
            return false;

        if (env?.ToLower() == "production")
            return true;

        return false;
    }

    public static int MaxSubArray(int[] values)
    {
        var max_cursor = values.Max();
        var max_current = values.Max();

        foreach (var value in values)
        {
            max_cursor = Math.Max(max_cursor, max_cursor + value);

            max_current = Math.Max(max_current, max_cursor);
        }

        return max_current;
    }
}