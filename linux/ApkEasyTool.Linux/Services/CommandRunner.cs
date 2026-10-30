using System.Diagnostics;
using System.Text;

namespace ApkEasyTool.Linux.Services;

/// <summary>
/// Runs tools directly (without a shell), preserving paths and arguments containing spaces.
/// Captures complete output while forwarding each line for live UI feedback.
/// </summary>
public sealed class CommandRunner
{
    public async Task<CommandResult> RunAsync(
        string executable,
        IEnumerable<string> arguments,
        string? workingDirectory = null,
        CancellationToken cancellationToken = default,
        IProgress<string>? output = null)
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

            using var cancellationRegistration = cancellationToken.Register(() =>
            {
                try
                {
                    if (!process.HasExited)
                        process.Kill(entireProcessTree: true);
                }
                catch
                {
                    // The process may have exited between the check and Kill.
                }
            });

            var stdoutTask = ReadStreamAsync(process.StandardOutput, "OUT", output);
            var stderrTask = ReadStreamAsync(process.StandardError, "ERR", output);
            await process.WaitForExitAsync();
            var streams = await Task.WhenAll(stdoutTask, stderrTask);
            cancellationToken.ThrowIfCancellationRequested();
            timer.Stop();

            return new CommandResult(executable, args, process.ExitCode, streams[0], streams[1], timer.Elapsed);
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

    private static async Task<string> ReadStreamAsync(
        StreamReader reader,
        string label,
        IProgress<string>? output)
    {
        var buffer = new StringBuilder();
        while (await reader.ReadLineAsync() is { } line)
        {
            buffer.AppendLine(line);
            output?.Report($"{label}: {line}");
        }

        return buffer.ToString().TrimEnd('\r', '\n');
    }
}
