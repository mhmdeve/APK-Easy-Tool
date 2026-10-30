# APK Easy Tool for Linux

Linux desktop port of APK Easy Tool. This implementation is maintained separately from the original Windows Forms application so Windows users and build workflows remain unaffected.

## Status

The Linux client is under active development. See the repository-level [PROGRESS.md](../PROGRESS.md) for the authoritative implementation checklist and session handoff.

## Planned stack

- .NET 8+ and Avalonia UI
- Cross-platform, argument-safe process execution
- Apktool, Java, Android SDK build-tools and ADB integration
- Linux-native application data and configuration paths

## Build and test

From the repository root:

```bash
dotnet restore linux/ApkEasyTool.Linux/ApkEasyTool.Linux.csproj
dotnet build linux/ApkEasyTool.Linux/ApkEasyTool.Linux.csproj --configuration Release
dotnet test linux/ApkEasyTool.Linux.Tests/ApkEasyTool.Linux.Tests.csproj --configuration Release
dotnet publish linux/ApkEasyTool.Linux/ApkEasyTool.Linux.csproj --configuration Release --runtime linux-x64 --self-contained true --output artifacts/linux-x64
```

The test project currently covers process argument preservation, non-zero exit codes and cancellation on Linux. The GitHub Actions workflow runs the same test project before publishing.

## Install on Debian / Ubuntu

Download the `apk-easy-tool-linux-x64-deb` artifact from the GitHub Actions run and install it:

```bash
sudo apt update
sudo apt install ./apk-easy-tool_0.1.0_amd64.deb
```

The Debian package declares APT dependencies for Java, Apktool, ADB and
`android-sdk-build-tools` (which provides `aapt`, `apksigner` and
`zipalign`). It also declares the Linux desktop runtime libraries. APT
installs these dependencies during package installation. See
[package installation notes](package/README.md).

## Attribution

APK Easy Tool was originally created by [Evildog1](https://github.com/evildog1). This project is a continuation and Linux port. Preserve original attribution when redistributing.

## Disclaimer

Use the tool only for applications you own or are authorized to inspect, modify, test or distribute.
