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


## Automatic workspace folders

On first launch, APK Easy Tool creates this workspace automatically:

```text
~/Documents/APKEasyTool/
├── Compile/
├── Decompiles/
├── Rebuild/
├── Signed/
├── ZipAligned/
├── Extracted/
├── Frameworks/
├── Logs/
└── Temp/
```

Operations use these locations automatically:

- Decode APK → `Decompiles/<apk-name>/`
- Build/rebuild APK → `Rebuild/<project-name>.apk`
- Sign APK → `Signed/<apk-name>-signed.apk`
- Zipalign APK → `ZipAligned/<apk-name>-aligned.apk`
- Extract `.apks` bundle → `Extracted/<bundle-name>/`

The selected source APK and custom project folder remain wherever you choose them. You no longer need to select an output folder or output filename for these operations. The `Compile`, `Frameworks`, `Logs`, and `Temp` directories are also created ready for the next workflow additions.

## Attribution

APK Easy Tool was originally created by [Evildog1](https://github.com/evildog1). This project is a continuation and Linux port. Preserve original attribution when redistributing.

## Disclaimer

Use the tool only for applications you own or are authorized to inspect, modify, test or distribute.


## User interface and appearance

The Linux app follows the desktop's default theme when **System** is selected. You can override it with **Light** or **Dark** from the Appearance selector in the header. During an operation, the Activity panel streams tool output, shows the current state and displays an indeterminate progress bar. Use **Cancel operation** to terminate a running external tool; other workflow actions are disabled until it finishes.

The progress bar is intentionally indeterminate because Apktool, ADB, apksigner and zipalign do not expose a consistent machine-readable percentage for all operations. The app does not invent percentage values.

## CI trigger

The Linux build workflow is manual-only. Open **Actions → Build Linux APK Easy Tool → Run workflow** and select the `linux` branch when you want to build and test. Commits and pull requests do not start this workflow automatically.
