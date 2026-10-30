using System.Diagnostics;

namespace ApkEasyTool.Linux.Services;

/// <summary>Locates external Android tooling without assuming Windows executable suffixes.</summary>
public sealed class ToolLocator
{
    public static IReadOnlyList<ToolDefinition> KnownTools { get; } =
    [
        new("Java", ["java"]),
        new("ADB", ["adb"]),
        new("Apktool", ["apktool"]),
        new("Zipalign", ["zipalign"]),
        new("Apksigner", ["apksigner"]),
        new("AAPT", ["aapt"]),
        new("AAPT2", ["aapt2"])
    ];

    public string? Find(string executable)
    {
        if (string.IsNullOrWhiteSpace(executable))
            return null;

        if (Path.IsPathRooted(executable))
            return File.Exists(executable) ? executable : null;

        var pathValue = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        foreach (var directory in pathValue.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
        {
            var candidate = Path.Combine(directory, executable);
            if (File.Exists(candidate) && IsExecutable(candidate))
                return candidate;
        }

        return null;
    }

    public bool IsAvailable(ToolDefinition tool) => tool.Candidates.Any(candidate => Find(candidate) is not null);

    private static bool IsExecutable(string path)
    {
        if (!OperatingSystem.IsLinux())
            return true;

        try
        {
            var startInfo = new ProcessStartInfo("test")
            {
                UseShellExecute = false
            };
            startInfo.ArgumentList.Add("-x");
            startInfo.ArgumentList.Add(path);
            using var process = Process.Start(startInfo);
            process?.WaitForExit();
            return process?.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }
}
