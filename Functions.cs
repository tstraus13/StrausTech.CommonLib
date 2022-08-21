using System.Diagnostics;

namespace StrausTech.CommonLib;

/// <summary>
/// Use tools or functions that can be used in any application
/// </summary>
public static class Tools
{
    /// <summary>
    /// Remote the trailing slave from a file path
    /// </summary>
    /// <param name="path">The file path</param>
    /// <returns>The file path without a trailing slash if it contained one</returns>
    public static string PathRemoveTrailingSlash(string path)
    {
        if (isWindows())
            return path.Last() == '\\' ? path.Remove(path.LastIndexOf(@"\"), 1) : path;
        else
            return path.Last() == '/' ? path.Remove(path.LastIndexOf(@"/"), 1) : path;
    }

    public static string RemoveWhitespace(string text)
    {
        return new string(text.ToCharArray().Where(c => !char.IsWhiteSpace(c)).ToArray());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lines"></param>
    /// <param name="numOfBlank"></param>
    public static void AddBlankLine(List<string> lines, int numOfBlank = 1)
    {
        for (int i = 0; i <= numOfBlank; i++)
        {
            lines.Add("");
        }
    }

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

    public static bool? isDevelopment()
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            return true;
        else if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
            return false;
        else
            return null;
    }

    public static bool? isProduction()
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            return false;
        else if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
            return true;
        else
            return null;
    }
}