namespace ApkEasyTool.Linux.Services;

/// <summary>Standard user workspace and application data paths.</summary>
public static class LinuxPaths
{
    public static string ApplicationDataDirectory
    {
        get
        {
            var xdg = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
            var root = !string.IsNullOrWhiteSpace(xdg)
                ? xdg
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config");

            return Path.Combine(root, "apk-easy-tool");
        }
    }

    /// <summary>All APK Easy Tool workspaces are kept in ~/Documents/APKEasyTool.</summary>
    public static string WorkingDirectory
    {
        get
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (string.IsNullOrWhiteSpace(documents))
                documents = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents");

            return Path.Combine(documents, "APKEasyTool");
        }
    }

    public static string CompileDirectory => Path.Combine(WorkingDirectory, "Compile");
    public static string DecompilesDirectory => Path.Combine(WorkingDirectory, "Decompiles");
    public static string RebuildDirectory => Path.Combine(WorkingDirectory, "Rebuild");
    public static string SignedDirectory => Path.Combine(WorkingDirectory, "Signed");
    public static string ZipAlignedDirectory => Path.Combine(WorkingDirectory, "ZipAligned");
    public static string ExtractedDirectory => Path.Combine(WorkingDirectory, "Extracted");
    public static string FrameworksDirectory => Path.Combine(WorkingDirectory, "Frameworks");
    public static string LogsDirectory => Path.Combine(WorkingDirectory, "Logs");
    public static string TempDirectory => Path.Combine(WorkingDirectory, "Temp");

    public static void EnsureDirectories()
    {
        Directory.CreateDirectory(ApplicationDataDirectory);
        foreach (var directory in new[]
        {
            WorkingDirectory,
            CompileDirectory,
            DecompilesDirectory,
            RebuildDirectory,
            SignedDirectory,
            ZipAlignedDirectory,
            ExtractedDirectory,
            FrameworksDirectory,
            LogsDirectory,
            TempDirectory
        })
        {
            Directory.CreateDirectory(directory);
        }
    }
}
