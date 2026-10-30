using ApkEasyTool.Linux.Services;

namespace ApkEasyTool.Linux.Tests;

public sealed class CommandRunnerTests
{
    private readonly CommandRunner _runner = new();

    [Fact]
    public async Task RunAsync_PreservesArgumentsContainingSpaces()
    {
        var result = await _runner.RunAsync(
            "/usr/bin/printf",
            ["%s|%s", "hello world", "folder with spaces/app.apk"]);

        Assert.True(result.Success, result.StandardError);
        Assert.Equal("hello world|folder with spaces/app.apk", result.StandardOutput);
        Assert.Empty(result.StandardError);
    }

    [Fact]
    public async Task RunAsync_ReturnsNonZeroExitCode()
    {
        var result = await _runner.RunAsync("/usr/bin/false", []);

        Assert.False(result.Success);
        Assert.Equal(1, result.ExitCode);
    }

    [Fact]
    public async Task RunAsync_CancellationTerminatesChildProcess()
    {
        using var cancellation = new CancellationTokenSource(TimeSpan.FromMilliseconds(150));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            _runner.RunAsync("/usr/bin/sleep", ["10"], cancellationToken: cancellation.Token));
    }
}
