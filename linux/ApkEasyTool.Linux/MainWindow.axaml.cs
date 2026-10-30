using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using ApkEasyTool.Linux.Services;

namespace ApkEasyTool.Linux;

public partial class MainWindow : Window
{
    private readonly AndroidToolService _tools = new(new CommandRunner(), new ToolLocator());
    private bool _busy;

    public MainWindow()
    {
        InitializeComponent();
        LinuxPaths.EnsureDirectories();
        Append("Ready. Install Java, Apktool and Android SDK build-tools, then select an APK.");
    }

    private void Append(string message)
    {
        OutputBox.Text = string.IsNullOrEmpty(OutputBox.Text)
            ? message
            : OutputBox.Text + Environment.NewLine + message;
    }

    private async Task RunOperationAsync(string name, Func<Task<CommandResult>> operation)
    {
        if (_busy) return;
        _busy = true;
        Append($"\n--- {name} ---");
        try
        {
            var result = await operation();
            if (!string.IsNullOrWhiteSpace(result.StandardOutput)) Append(result.StandardOutput.TrimEnd());
            if (!string.IsNullOrWhiteSpace(result.StandardError)) Append(result.StandardError.TrimEnd());
            Append(result.Success ? $"Completed successfully ({result.Duration.TotalSeconds:F1}s)." : $"Failed with exit code {result.ExitCode}.");
        }
        catch (Exception ex)
        {
            Append("ERROR: " + ex.Message);
        }
        finally { _busy = false; }
    }

    private async Task<string?> PickFileAsync(string title, params string[] patterns)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("Supported files") { Patterns = patterns }]
        });
        return files.Count == 0 ? null : files[0].Path.LocalPath;
    }

    private async Task<string?> PickFolderAsync(string title)
    {
        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions { Title = title, AllowMultiple = false });
        return folders.Count == 0 ? null : folders[0].Path.LocalPath;
    }

    private async Task<string?> PickSaveFileAsync(string title, string suggestedName)
    {
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = title,
            SuggestedFileName = suggestedName,
            FileTypeChoices = [new FilePickerFileType("APK package") { Patterns = ["*.apk"] }]
        });
        return file?.Path.LocalPath;
    }

    private async void ChooseApk_Click(object? sender, RoutedEventArgs e)
    {
        var path = await PickFileAsync("Select APK", "*.apk");
        if (path is not null) ApkPathBox.Text = path;
    }

    private async void ChooseProject_Click(object? sender, RoutedEventArgs e)
    {
        var path = await PickFolderAsync("Select Apktool project");
        if (path is not null) ProjectPathBox.Text = path;
    }

    private async void Decode_Click(object? sender, RoutedEventArgs e)
    {
        var apk = ApkPathBox.Text;
        if (string.IsNullOrWhiteSpace(apk)) { Append("Select an APK first."); return; }
        var output = await PickFolderAsync("Choose output folder for decoded project");
        if (output is null) return;
        var name = Path.GetFileNameWithoutExtension(apk);
        await RunOperationAsync("Decode APK", () => _tools.DecodeAsync(apk, Path.Combine(output, name)));
        ProjectPathBox.Text = Path.Combine(output, name);
    }

    private async void Build_Click(object? sender, RoutedEventArgs e)
    {
        var project = ProjectPathBox.Text;
        if (string.IsNullOrWhiteSpace(project)) { Append("Select an Apktool project first."); return; }
        var output = await PickSaveFileAsync("Save rebuilt APK", "rebuilt.apk");
        if (output is null) return;
        await RunOperationAsync("Build APK", () => _tools.BuildAsync(project, output));
    }

    private async void Sign_Click(object? sender, RoutedEventArgs e)
    {
        var apk = ApkPathBox.Text;
        if (string.IsNullOrWhiteSpace(apk)) { Append("Select an APK first."); return; }
        var key = await PickFileAsync("Select PK8 private key", "*.pk8");
        if (key is null) return;
        var cert = await PickFileAsync("Select PEM certificate", "*.pem");
        if (cert is null) return;
        var output = await PickSaveFileAsync("Save signed APK", Path.GetFileNameWithoutExtension(apk) + "-signed.apk");
        if (output is null) return;
        await RunOperationAsync("Sign APK", () => _tools.SignAsync(apk, output, key, cert));
    }

    private async void ZipAlign_Click(object? sender, RoutedEventArgs e)
    {
        var apk = ApkPathBox.Text;
        if (string.IsNullOrWhiteSpace(apk)) { Append("Select an APK first."); return; }
        var output = await PickSaveFileAsync("Save zipaligned APK", Path.GetFileNameWithoutExtension(apk) + "-aligned.apk");
        if (output is null) return;
        await RunOperationAsync("Zipalign APK", () => _tools.ZipAlignAsync(apk, output));
    }

    private async void VerifyAlignment_Click(object? sender, RoutedEventArgs e)
    {
        var apk = ApkPathBox.Text;
        if (string.IsNullOrWhiteSpace(apk)) { Append("Select an APK first."); return; }
        await RunOperationAsync("Verify zip alignment", () => _tools.VerifyAlignmentAsync(apk));
    }

    private async void Install_Click(object? sender, RoutedEventArgs e)
    {
        var apk = ApkPathBox.Text;
        if (string.IsNullOrWhiteSpace(apk)) { Append("Select an APK first."); return; }
        await RunOperationAsync("Install APK with ADB", () => _tools.InstallAsync(apk));
    }

    private async void ApkInfo_Click(object? sender, RoutedEventArgs e)
    {
        var apk = ApkPathBox.Text;
        if (string.IsNullOrWhiteSpace(apk)) { Append("Select an APK first."); return; }
        await RunOperationAsync("Read APK metadata", () => _tools.GetApkInformationAsync(apk));
    }

    private async void ExtractBundle_Click(object? sender, RoutedEventArgs e)
    {
        var bundle = await PickFileAsync("Select APKs bundle", "*.apks");
        if (bundle is null) return;
        var output = await PickFolderAsync("Choose extraction folder");
        if (output is null) return;
        Append("\n--- Extract .apks bundle ---");
        try
        {
            await _tools.ExtractBundleAsync(bundle, Path.Combine(output, Path.GetFileNameWithoutExtension(bundle)));
            Append("Bundle extracted successfully.");
        }
        catch (Exception ex) { Append("ERROR: " + ex.Message); }
    }

    private void ClearLog_Click(object? sender, RoutedEventArgs e) => OutputBox.Text = string.Empty;
}
