using System;
using System.Text.RegularExpressions;

namespace HIS.ActivityLogs;

/// <summary>
/// Service to parse UserAgent strings and extract device/browser information.
/// </summary>
public static class UserAgentParser
{
    /// <summary>
    /// Parses a UserAgent string and returns device information.
    /// </summary>
    public static (string DeviceType, string BrowserName, string BrowserVersion, string OperatingSystem) Parse(string? userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
            return ("Unknown", "Unknown", "", "Unknown");

        var deviceType = DetectDeviceType(userAgent);
        var (browserName, browserVersion) = DetectBrowser(userAgent);
        var operatingSystem = DetectOS(userAgent);

        return (deviceType, browserName, browserVersion, operatingSystem);
    }

    private static string DetectDeviceType(string userAgent)
    {
        var ua = userAgent.ToLower();
        
        if (ua.Contains("mobile") || ua.Contains("android") && !ua.Contains("tablet"))
            return "Mobile";
        if (ua.Contains("tablet") || ua.Contains("ipad"))
            return "Tablet";
        
        return "Desktop";
    }

    private static (string Name, string Version) DetectBrowser(string userAgent)
    {
        // Edge (must check before Chrome as Edge contains "Chrome")
        var edgeMatch = Regex.Match(userAgent, @"Edg[ea]?/(\d+\.?\d*)");
        if (edgeMatch.Success)
            return ("Edge", edgeMatch.Groups[1].Value);

        // Chrome
        var chromeMatch = Regex.Match(userAgent, @"Chrome/(\d+\.?\d*)");
        if (chromeMatch.Success && !userAgent.Contains("Edg"))
            return ("Chrome", chromeMatch.Groups[1].Value);

        // Firefox
        var firefoxMatch = Regex.Match(userAgent, @"Firefox/(\d+\.?\d*)");
        if (firefoxMatch.Success)
            return ("Firefox", firefoxMatch.Groups[1].Value);

        // Safari (must check after Chrome)
        var safariMatch = Regex.Match(userAgent, @"Version/(\d+\.?\d*).*Safari");
        if (safariMatch.Success)
            return ("Safari", safariMatch.Groups[1].Value);

        // IE
        var ieMatch = Regex.Match(userAgent, @"MSIE\s(\d+\.?\d*)|Trident.*rv:(\d+\.?\d*)");
        if (ieMatch.Success)
            return ("Internet Explorer", ieMatch.Groups[1].Success ? ieMatch.Groups[1].Value : ieMatch.Groups[2].Value);

        return ("Unknown", "");
    }

    private static string DetectOS(string userAgent)
    {
        var ua = userAgent.ToLower();

        if (ua.Contains("windows nt 10"))
            return "Windows 10/11";
        if (ua.Contains("windows nt 6.3"))
            return "Windows 8.1";
        if (ua.Contains("windows nt 6.2"))
            return "Windows 8";
        if (ua.Contains("windows nt 6.1"))
            return "Windows 7";
        if (ua.Contains("windows"))
            return "Windows";
        if (ua.Contains("iphone"))
            return "iOS (iPhone)";
        if (ua.Contains("ipad"))
            return "iOS (iPad)";
        if (ua.Contains("mac os"))
            return "macOS";
        if (ua.Contains("android"))
            return "Android";
        if (ua.Contains("linux"))
            return "Linux";

        return "Unknown";
    }
}
