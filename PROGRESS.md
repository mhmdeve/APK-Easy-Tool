# Linux Port — Progress & Handoff

## Goal

Port APK Easy Tool to Linux while preserving the existing Windows application and its source. Linux work happens only on the `linux` branch.

## Baseline inspection

- [x] Created `linux` from `master` (baseline commit: `19bb5104122a6fe33780c9223bbda04ab1606e45`).
- [x] Confirmed the current desktop application targets .NET Framework 4.8 and Windows Forms.
- [x] Confirmed the project uses Windows-specific APIs and tooling, including WinForms, Windows Explorer integration, Windows shell APIs, and Windows command invocation.
- [x] Identified the separate Explorer context-menu extension as Windows-only; it must not be part of the Linux application.
- [x] Reviewed the existing README and build workflow. The current release pipeline is Windows/MSBuild-specific.

## Architecture decision

Do not attempt to run the existing WinForms project on Linux or rewrite every Windows-bound form in place. Keep the Windows code intact and introduce a separate Linux desktop application with a cross-platform .NET target and UI framework.

Proposed implementation:
- .NET 8 (or newer supported LTS at implementation time) cross-platform application.
- Avalonia UI for native desktop support on Linux, with Windows portability retained as an option.
- Shared command/process layer for Apktool, Java, ADB, zipalign, apksigner, aapt/aapt2, smali and baksmali.
- Linux-native paths, process discovery, file dialogs, application data directories and executable permission handling.
- Preserve the primary workflows: decode, build, sign, zipalign, install, extract, APK information, framework management and logs.
- Use system-installed Android tools where practical and provide a clear managed-tools option.
- Linux packaging: portable publish directory/AppImage or .deb first; Flatpak can follow.

## Progress

### Phase 0 — Branch and handoff
- [x] Create `linux` branch.
- [x] Add this progress/handoff document.
- [ ] Add Linux project and architecture.
- [ ] Add shared command runner and tool detection.
- [ ] Implement core APK workflows.
- [ ] Build Avalonia UI.
- [ ] Add automated Linux CI and distributable package.
- [ ] Test on a real Linux desktop environment.

## Important compatibility notes

- Existing source uses Windows Forms and .NET Framework 4.8; that UI cannot be built natively for Linux.
- Existing paths frequently contain Windows separators and executable names (for example `cmd.exe`, `java.exe`, `explorer.exe`). Linux support requires platform-aware replacements and argument-safe process launching.
- Windows Explorer shell extension under `AETShellExt` is not portable. Leave it available for Windows builds; do not attempt to install it on Linux.
- Do not modify or remove Windows resources, forms, project files or release workflows as part of the Linux implementation unless a later shared-code refactor is clearly justified.
- Preserve original creator attribution (Evildog1) in all new documentation and UI credits.

## Next session — exact starting point

1. Read this file first.
2. Inspect `APKEasyTool/Utils`, `APKEasyTool/Classes`, `Resources`, and the remaining APK workflow classes to inventory command semantics and bundled tools.
3. Inspect existing target framework, package versions, signing and APKtool compatibility behavior.
4. Create a new directory (recommended: `linux/ApkEasyTool.Linux`) containing an Avalonia .NET project, keeping it isolated from the existing Windows solution.
5. Implement a tested process runner using `ProcessStartInfo.ArgumentList`, asynchronous stdout/stderr capture, cancellation, exit-code handling and logging. Never construct shell command strings from user-provided paths.
6. Implement tool discovery for Java, Apktool versions, ADB, zipalign, apksigner and aapt/aapt2.
7. Add unit tests for executable discovery, quoting/arguments, output capture and error handling.
8. Implement workflows one by one, beginning with decode and rebuild, then signing/alignment and ADB installation.
9. Add Linux GitHub Actions build/test workflow and package artifact.
10. Update this document after each coherent milestone and commit each independently.

## Commit discipline

- Commit each independently useful change with a focused message.
- Update this file in the same commit when a milestone is completed or a technical decision changes.
- Keep this file actionable: record completed work, tests and results, blockers, and the next exact task.
- Do not claim Linux compatibility until the Linux build and core workflows have been tested on Linux.

## Current status

**Branch:** `linux`  
**Baseline:** `master` at `19bb5104122a6fe33780c9223bbda04ab1606e45`  
**Status:** Branch and handoff created; Linux implementation not yet started.
