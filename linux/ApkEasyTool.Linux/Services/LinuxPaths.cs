namespace ApkEasyTool.Linux.Services;

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

    public static string WorkingDirectory
    {
        get
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(home, "APK Easy Tool");
        }
    }

    public static void EnsureDirectories()
    {
        Directory.CreateDirectory(ApplicationDataDirectory);
        Directory.CreateDirectory(WorkingDirectory);
    }
}
