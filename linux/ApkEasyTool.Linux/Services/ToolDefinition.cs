namespace ApkEasyTool.Linux.Services;

public sealed record ToolDefinition(string Name, IReadOnlyList<string> Candidates, string? VersionArguments = "--version");
