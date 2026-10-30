using System.IO.Compression;

namespace ApkEasyTool.Linux.Services;

/// <summary>APK operations implemented using host-installed Android command-line tools.</summary>
public sealed class AndroidToolService(CommandRunner runner, ToolLocator locator)
{
    public async Task<CommandResult> DecodeAsync(string apkPath, string outputDirectory, string apktool = "apktool", CancellationToken cancellationToken = default)
    {
        EnsureApk(apkPath);
        Directory.CreateDirectory(outputDirectory);
        return await RunToolAsync(apktool, ["d", "-f", apkPath, "-o", outputDirectory], cancellationToken);
    }

    public async Task<CommandResult> BuildAsync(string projectDirectory, string outputApk, string apktool = "apktool", CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(projectDirectory))
            throw new DirectoryNotFoundException($"Project directory not found: {projectDirectory}");
        var outputDirectory = Path.GetDirectoryName(Path.GetFullPath(outputApk));
        if (!string.IsNullOrEmpty(outputDirectory)) Directory.CreateDirectory(outputDirectory);
        return await RunToolAsync(apktool, ["b", projectDirectory, "-o", outputApk], cancellationToken);
    }

    public async Task<CommandResult> InstallAsync(string apkPath, string adb = "adb", CancellationToken cancellationToken = default)
    {
        EnsureApk(apkPath);
        return await RunToolAsync(adb, ["install", "-r", apkPath], cancellationToken);
    }

    public async Task<CommandResult> SignAsync(string apkPath, string outputApk, string keyFile, string certificateFile, string apksigner = "apksigner", CancellationToken cancellationToken = default)
    {
        EnsureApk(apkPath);
        if (!File.Exists(keyFile)) throw new FileNotFoundException("PK8 private key not found.", keyFile);
        if (!File.Exists(certificateFile)) throw new FileNotFoundException("PEM certificate not found.", certificateFile);
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outputApk))!);
        return await RunToolAsync(apksigner, ["sign", "--key", keyFile, "--cert", certificateFile, "--out", outputApk, apkPath], cancellationToken);
    }

    public async Task<CommandResult> SignWithKeystoreAsync(string apkPath, string outputApk, string keystore, string alias, string? storePassword = null, string apksigner = "apksigner", CancellationToken cancellationToken = default)
    {
        EnsureApk(apkPath);
        if (!File.Exists(keystore)) throw new FileNotFoundException("Keystore not found.", keystore);
        var args = new List<string> { "sign", "--ks", keystore, "--ks-key-alias", alias };
        if (storePassword is not null) args.AddRange(["--ks-pass", "pass:" + storePassword]);
        args.AddRange(["--out", outputApk, apkPath]);
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outputApk))!);
        return await RunToolAsync(apksigner, args, cancellationToken);
    }

    public async Task<CommandResult> ZipAlignAsync(string apkPath, string outputApk, string zipalign = "zipalign", CancellationToken cancellationToken = default)
    {
        EnsureApk(apkPath);
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outputApk))!);
        return await RunToolAsync(zipalign, ["-f", "4", apkPath, outputApk], cancellationToken);
    }

    public async Task<CommandResult> VerifyAlignmentAsync(string apkPath, string zipalign = "zipalign", CancellationToken cancellationToken = default)
    {
        EnsureApk(apkPath);
        return await RunToolAsync(zipalign, ["-c", "-v", "4", apkPath], cancellationToken);
    }

    public async Task ExtractBundleAsync(string bundlePath, string outputDirectory, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(bundlePath)) throw new FileNotFoundException("Bundle file not found.", bundlePath);
        if (!string.Equals(Path.GetExtension(bundlePath), ".apks", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Expected an .apks bundle.", nameof(bundlePath));
        cancellationToken.ThrowIfCancellationRequested();
        if (Directory.Exists(outputDirectory)) Directory.Delete(outputDirectory, recursive: true);
        Directory.CreateDirectory(outputDirectory);
        await Task.Run(() => ZipFile.ExtractToDirectory(bundlePath, outputDirectory, overwriteFiles: true), cancellationToken);
    }

    public async Task<CommandResult> GetApkInformationAsync(string apkPath, string aapt = "aapt", CancellationToken cancellationToken = default)
    {
        EnsureApk(apkPath);
        return await RunToolAsync(aapt, ["dump", "badging", apkPath], cancellationToken);
    }

    private async Task<CommandResult> RunToolAsync(string executable, IEnumerable<string> args, CancellationToken token)
    {
        var resolved = locator.Find(executable) ?? throw new FileNotFoundException($"Required tool '{executable}' was not found in PATH. Install it or configure its full path.");
        return await runner.RunAsync(resolved, args, cancellationToken: token);
    }

    private static void EnsureApk(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("An APK path is required.", nameof(path));
        if (!File.Exists(path)) throw new FileNotFoundException("APK file not found.", path);
        if (!string.Equals(Path.GetExtension(path), ".apk", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Expected an .apk file.", nameof(path));
    }
}
