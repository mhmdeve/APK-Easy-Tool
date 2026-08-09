# APK Easy Tool

A Windows desktop utility for simplifying common Android APK reverse-engineering and modification workflows.

> **Original project credit:** APK Easy Tool was created by **[@evildog1](https://github.com/evildog1)**. This repository is a continuation/fork of the original project. Please preserve attribution to the original author when redistributing or modifying the project.

## Overview

APK Easy Tool provides a graphical workflow around APK tooling so common operations can be performed without manually entering every command in a terminal.

Typical workflows include:

- Decode APK files
- Build/rebuild decoded APK projects
- Sign APKs
- Install APKs through ADB
- Manage APKtool versions
- Work with different APKtool generations from one application

## Version 1.61

This repository currently targets **APK Easy Tool v1.61**.

### APKtool support

Version 1.61 includes compatibility for both major APKtool generations:

- **APKtool 2.12.1** — APKtool 2.x compatibility
- **APKtool 3.0.3** — APKtool 3.x compatibility

APK Easy Tool historically uses command-line conventions from APKtool 2.x. APKtool 3.x changed and removed several command-line options, so this release includes a compatibility launcher that translates the legacy options used by APK Easy Tool into their APKtool 3.x equivalents where applicable.

The bundled versions are tested during the GitHub Actions build before the application is packaged.

## Requirements

### Runtime

- Windows
- Java Runtime Environment suitable for the selected APKtool version
- Android Debug Bridge (ADB) for device/install operations

### Development

- Visual Studio 2019 or newer
- C# / .NET Framework development tools
- Java 17 for building/testing the APKtool 3.x compatibility components

## Building from Source

1. Clone the repository:

   ```powershell
   git clone https://github.com/mhmdeve/APK-Easy-Tool.git
   cd APK-Easy-Tool
   ```

2. Open `APKEasyTool.sln` in Visual Studio 2019 or newer.

3. Select `Release` and `Any CPU`.

4. Build the solution.

The release build is also automated through GitHub Actions and produces an **Any CPU** ZIP package.

## GitHub Actions

The repository includes an automated Windows build pipeline that restores dependencies, configures MSBuild and Java 17, downloads and verifies the supported APKtool versions, builds the APKtool 3.x compatibility launcher, builds APK Easy Tool as **Any CPU**, verifies the executable and bundled tools, and packages the release ZIP.

A version tag (`v*`) triggers the release packaging workflow.

## Project Structure

```text
APKEasyTool.sln       Visual Studio solution
APKEasyTool/           Main application source
Resources/             Runtime resources and bundled tooling
build/                 Build-time compatibility components
.github/workflows/     Continuous integration and release workflow
```

## Attribution

### Original Creator

**Evildog1** is the original creator and source author of APK Easy Tool.

GitHub: https://github.com/evildog1

The original project explicitly permits continued development under the same name provided that credit is given to the original author. This repository preserves that attribution.

### Project Continuation

This repository contains subsequent maintenance, build-system improvements, APKtool compatibility work, and release packaging while retaining the original project's identity and attribution.

## Credits

- **Original creator:** [Evildog1](https://github.com/evildog1)
- **APKtool:** [iBotPeaches / Apktool](https://github.com/iBotPeaches/Apktool)
- **Project repository:** [mhmdeve/APK-Easy-Tool](https://github.com/mhmdeve/APK-Easy-Tool)

## Disclaimer

APK Easy Tool is intended for legitimate Android application development, testing, research, debugging, and modification of software for which you have permission to work.

Always respect software licenses, copyright, application terms of service, and applicable laws.

## License / Original Project Terms

Please review the original project files and attribution requirements before redistributing modified versions. Where the original project specifies attribution requirements, those requirements remain applicable to derivative work.
