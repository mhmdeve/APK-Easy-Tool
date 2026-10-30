namespace ApkEasyTool.Linux.Services;

/// <summary>First platform-neutral façade for APK operations; workflows are added incrementally.</summary>
public sealed class AndroidToolService(CommandRunner runner, ToolLocator locator)
{
    public async Task<CommandResult> DecodeAsync(string apkPath, string outputDirectory, string apktool = "apktool", CancellationToken cancellationToken = default)
    {
        EnsureFile(apkPath, ".apk");
        Directory.CreateDirectory(outputDirectory);
        return await runner.RunAsync(apktool, ["d", "-f", apkPath, "-o", outputDirectory], cancellationToken: cancellationToken);
    }

    public async Task<CommandResult> BuildAsync(string projectDirectory, string outputApk, string apktool = "apktool", CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(projectDirectory))
            throw new DirectoryNotFoundException($"Project directory not found: {projectDirectory}");

        var outputDirectory = Path.GetDirectoryName(Path.GetFullPath(outputApk));
        if (!string.IsNullOrEmpty(outputDirectory))
            Directory.CreateDirectory(outputDirectory);

        return await runner.RunAsync(apktool, ["b", projectDirectory, "-o", outputApk], cancellationToken: cancellationToken);
    }

    public async Task<CommandResult> InstallAsync(string apkPath, string adb = "adb", CancellationToken cancellationToken = default)
    {
        EnsureFile(apkPath, ".apk");
        var resolved = locator.Find(adb) ?? throw new FileNotFoundException($"ADB executable was not found: {adb}");
        return await runner.RunAsync(resolved, ["install", "-r", apkPath], cancellationToken: cancellationToken);
    }

    private static void EnsureFile(string path, string expectedExtension)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("A file path is required.", nameof(path));
        if (!File.Exists(path))
            throw new FileNotFoundException("Input file was not found.", path);
        if (!string.Equals(Path.GetExtension(path), expectedExtension, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException($"Expected a {expectedExtension} file.", nameof(path));
    }
}
