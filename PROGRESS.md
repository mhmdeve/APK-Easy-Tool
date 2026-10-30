# Linux Port — Progress & Handoff

## Goal

Port APK Easy Tool to Linux while preserving the existing Windows application and its source. Linux work happens only on the `linux` branch.

## Architecture

- Separate .NET 8 / Avalonia desktop app in `linux/ApkEasyTool.Linux`.
- Existing Windows Forms project and Windows workflows remain unchanged.
- Host-installed Java, Apktool, ADB and Android build-tools are discovered from PATH.
- Linux workspace: `~/Documents/APKEasyTool`, with operation-specific subfolders.
- Preserve original creator attribution (Evildog1).

## Completed Linux functionality

- [x] APK selection and Apktool project selection.
- [x] Decode, build/rebuild, PK8/PEM signing, zipalign, alignment verification, ADB install, aapt metadata and .apks extraction.
- [x] Automatic workspace directories: Compile, Decompiles, Rebuild, Signed, ZipAligned, Extracted, Frameworks, Logs and Temp.
- [x] Debian package build and Ubuntu CI artifact publishing.
- [x] xUnit process-runner tests for argument preservation, exit codes and cancellation.
- [x] Fluent UI layout refresh with Light, Dark and System appearance choices.
- [x] Indeterminate per-operation progress indicator, live stdout/stderr log streaming, status text and cancellation.
- [x] CI workflow now uses workflow_dispatch only; pushes and pull requests no longer trigger it automatically.

## UX / release-readiness follow-up

- [ ] Verify the manual workflow from GitHub Actions and inspect build/test/package results.
- [ ] Run the app on a graphical Linux desktop in both light and dark modes.
- [ ] Test cancellation against a long-running Apktool operation and confirm the child process exits.
- [ ] Add percentage progress only for tools that expose reliable progress; otherwise keep the honest indeterminate indicator.
- [ ] Add keystore signing UI, configurable tool paths, persistent preferences and framework management.
- [ ] Validate Debian package installation and runtime dependencies on a clean Ubuntu/Debian system.
- [ ] Add app icon, desktop metadata, version/about dialog and release notes before public release.

## Current status

The UX refresh adds system-following appearance by default, manual Light/Dark overrides, live process output, operation status, a cancellable indeterminate progress bar, and a redesigned workspace layout. The GitHub Actions workflow is manual-only. These changes still require CI verification and real Linux desktop testing; do not describe the app as release-ready until those checks pass.
