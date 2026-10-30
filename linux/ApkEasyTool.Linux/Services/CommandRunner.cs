using System.Diagnostics;

namespace ApkEasyTool.Linux.Services;

/// <summary>
/// Runs tools directly (without a shell), preserving paths and arguments containing spaces.
/// </summary>
public sealed class CommandRunner
{
    public async Task<CommandResult> RunAsync(
        string executable,
        IEnumerable<string> arguments,
        string? workingDirectory = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(executable);
        ArgumentNullException.ThrowIfNull(arguments);

        var args = arguments.ToArray();
        var startInfo = new ProcessStartInfo
        {
            FileName = executable,
            WorkingDirectory = string.IsNullOrWhiteSpace(workingDirectory)
                ? Environment.CurrentDirectory
                : workingDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        foreach (var argument in args)
            startInfo.ArgumentList.Add(argument ?? string.Empty);

        using var process = new Process { StartInfo = startInfo };
        var timer = Stopwatch.StartNew();

        try
        {
            if (!process.Start())
                throw new InvalidOperationException($"Could not start process: {executable}");

            var stdoutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
            var stderrTask = process.StandardError.ReadToEndAsync(cancellationToken);

            await process.WaitForExitAsync(cancellationToken);
            var stdout = await stdoutTask;
            var stderr = await stderrTask;
            timer.Stop();

            return new CommandResult(executable, args, process.ExitCode, stdout, stderr, timer.Elapsed);
        }
        catch
        {
            try
            {
                if (!process.HasExited)
                    process.Kill(entireProcessTree: true);
            }
            catch
            {
                // Preserve the original exception.
            }

            throw;
        }
    }
}
