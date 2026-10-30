namespace ApkEasyTool.Linux.Services;

public sealed record CommandResult(
    string FileName,
    IReadOnlyList<string> Arguments,
    int ExitCode,
    string StandardOutput,
    string StandardError,
    TimeSpan Duration)
{
    public bool Success => ExitCode == 0;
}
