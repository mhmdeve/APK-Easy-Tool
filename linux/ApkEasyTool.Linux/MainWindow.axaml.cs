using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using ApkEasyTool.Linux.Services;

namespace ApkEasyTool.Linux;

public partial class MainWindow : Window
{
    private readonly AndroidToolService _tools = new(new CommandRunner(), new ToolLocator());
    private CancellationTokenSource? _operationCancellation;
    private bool _busy;

    public MainWindow()
    {
        InitializeComponent();
        LinuxPaths.EnsureDirectories();
        Append("Ready. Workspace: " + LinuxPaths.WorkingDirectory);
        Append("Folders for Decompiles, Compile, Rebuild, Signed, ZipAligned, Extracted, Frameworks, Logs and Temp are created automatically.");
        SetStatus("Ready");
    }

    private void Append(string message)
    {
        OutputBox.Text = string.IsNullOrEmpty(OutputBox.Text)
            ? message
            : OutputBox.Text + Environment.NewLine + message;
        OutputBox.CaretIndex = OutputBox.Text.Length;
    }

    private void SetStatus(string message) => StatusText.Text = message;

    private void BeginOperation(string name)
    {
        _busy = true;
        _operationCancellation = new CancellationTokenSource();
        WorkflowPanel.IsEnabled = false;
        OperationProgress.IsVisible = true;
        OperationProgress.IsIndeterminate = true;
        CancelButton.IsVisible = true;
        CancelButton.IsEnabled = true;
        SetStatus($"{name} in progress…");
        Append($"\n--- {name} ---");
    }

    private void EndOperation(string status)
    {
        _busy = false;
        _operationCancellation?.Dispose();
        _operationCancellation = null;
        WorkflowPanel.IsEnabled = true;
        OperationProgress.IsVisible = false;
        CancelButton.IsVisible = false;
        CancelButton.IsEnabled = false;
        SetStatus(status);
    }

    private async Task RunOperationAsync(string name, Func<CancellationToken, IProgress<string>, Task<CommandResult>> operation)
    {
        if (_busy) return;
        BeginOperation(name);
        var progress = new Progress<string>(Append);
        try
        {
            var result = await operation(_operationCancellation!.Token, progress);
            var status = result.Success ? "Completed" : "Failed";
            Append(result.Success ? $"Completed successfully ({result.Duration.TotalSeconds:F1}s)." : $"Failed with exit code {result.ExitCode}.");
            SetStatus(status);
        }
        catch (OperationCanceledException)
        {
            Append("Operation cancelled.");
            SetStatus("Cancelled");
        }
        catch (Exception ex)
        {
            Append("ERROR: " + ex.Message);
            SetStatus("Error");
        }
        finally { EndOperation(StatusText.Text ?? "Ready"); }
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
        var projectDirectory = Path.Combine(LinuxPaths.DecompilesDirectory, Path.GetFileNameWithoutExtension(apk));
        await RunOperationAsync("Decode APK", (token, progress) => _tools.DecodeAsync(apk, projectDirectory, cancellationToken: token, output: progress));
        if (Directory.Exists(projectDirectory)) ProjectPathBox.Text = projectDirectory;
    }

    private async void Build_Click(object? sender, RoutedEventArgs e)
    {
        var project = ProjectPathBox.Text;
        if (string.IsNullOrWhiteSpace(project)) { Append("Select an Apktool project first."); return; }
        var name = Path.GetFileName(project.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        var output = Path.Combine(LinuxPaths.RebuildDirectory, name + ".apk");
        await RunOperationAsync("Build APK", (token, progress) => _tools.BuildAsync(project, output, cancellationToken: token, output: progress));
        Append("Rebuilt APK destination: " + output);
    }

    private async void Sign_Click(object? sender, RoutedEventArgs e)
    {
        var apk = ApkPathBox.Text;
        if (string.IsNullOrWhiteSpace(apk)) { Append("Select an APK first."); return; }
        var key = await PickFileAsync("Select PK8 private key", "*.pk8");
        if (key is null) return;
        var cert = await PickFileAsync("Select PEM certificate", "*.pem");
        if (cert is null) return;
        var output = Path.Combine(LinuxPaths.SignedDirectory, Path.GetFileNameWithoutExtension(apk) + "-signed.apk");
        await RunOperationAsync("Sign APK", (token, progress) => _tools.SignAsync(apk, output, key, cert, cancellationToken: token, output: progress));
        Append("Signed APK destination: " + output);
    }

    private async void ZipAlign_Click(object? sender, RoutedEventArgs e)
    {
        var apk = ApkPathBox.Text;
        if (string.IsNullOrWhiteSpace(apk)) { Append("Select an APK first."); return; }
        var output = Path.Combine(LinuxPaths.ZipAlignedDirectory, Path.GetFileNameWithoutExtension(apk) + "-aligned.apk");
        await RunOperationAsync("Zipalign APK", (token, progress) => _tools.ZipAlignAsync(apk, output, cancellationToken: token, output: progress));
        Append("Zipaligned APK destination: " + output);
    }

    private async void VerifyAlignment_Click(object? sender, RoutedEventArgs e)
    {
        var apk = ApkPathBox.Text;
        if (string.IsNullOrWhiteSpace(apk)) { Append("Select an APK first."); return; }
        await RunOperationAsync("Verify zip alignment", (token, progress) => _tools.VerifyAlignmentAsync(apk, cancellationToken: token, output: progress));
    }

    private async void Install_Click(object? sender, RoutedEventArgs e)
    {
        var apk = ApkPathBox.Text;
        if (string.IsNullOrWhiteSpace(apk)) { Append("Select an APK first."); return; }
        await RunOperationAsync("Install APK with ADB", (token, progress) => _tools.InstallAsync(apk, cancellationToken: token, output: progress));
    }

    private async void ApkInfo_Click(object? sender, RoutedEventArgs e)
    {
        var apk = ApkPathBox.Text;
        if (string.IsNullOrWhiteSpace(apk)) { Append("Select an APK first."); return; }
        await RunOperationAsync("Read APK metadata", (token, progress) => _tools.GetApkInformationAsync(apk, cancellationToken: token, output: progress));
    }

    private async void ExtractBundle_Click(object? sender, RoutedEventArgs e)
    {
        if (_busy) return;
        var bundle = await PickFileAsync("Select APKs bundle", "*.apks");
        if (bundle is null) return;
        var output = Path.Combine(LinuxPaths.ExtractedDirectory, Path.GetFileNameWithoutExtension(bundle));
        BeginOperation("Extract .apks bundle");
        try
        {
            await _tools.ExtractBundleAsync(bundle, output, _operationCancellation!.Token);
            Append("Bundle extracted successfully: " + output);
            SetStatus("Completed");
        }
        catch (OperationCanceledException)
        {
            Append("Extraction cancelled.");
            SetStatus("Cancelled");
        }
        catch (Exception ex)
        {
            Append("ERROR: " + ex.Message);
            SetStatus("Error");
        }
        finally { EndOperation(StatusText.Text ?? "Ready"); }
    }

    private void CancelOperation_Click(object? sender, RoutedEventArgs e)
    {
        if (_operationCancellation is { IsCancellationRequested: false })
        {
            CancelButton.IsEnabled = false;
            SetStatus("Cancelling…");
            _operationCancellation.Cancel();
        }
    }

    private void ThemeSelector_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (Application.Current is null) return;
        Application.Current.RequestedThemeVariant = ThemeSelector.SelectedIndex switch
        {
            1 => ThemeVariant.Light,
            2 => ThemeVariant.Dark,
            _ => ThemeVariant.Default
        };
    }

    private void ClearLog_Click(object? sender, RoutedEventArgs e) => OutputBox.Text = string.Empty;
}
