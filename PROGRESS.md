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
- [x] Add initial Avalonia Linux project and architecture skeleton (not yet build-verified).
- [x] Add initial argument-safe process runner and PATH-based tool discovery.
- [x] Add initial Apktool decode/build and ADB install service methods; signing/alignment and remaining workflows are still pending.
- [x] Wire Avalonia file/folder pickers and decode/build/sign/zipalign/verify/install/info/extract actions.
- [x] Add Ubuntu GitHub Actions restore/build/publish workflow; initial Linux x64 publish run passed.
- [x] Create the automatic ~/Documents/APKEasyTool workspace and standard operation folders at startup.\n- [ ] Test on a real Linux desktop environment.

## Phase 1 — Initial Linux project scaffold (commit `e7105272b77a8734db8ad10c49aafacda6fd98ab`)

- [x] Add isolated Avalonia desktop project under `linux/ApkEasyTool.Linux`.
- [x] Add initial navigation/workspace/log window layout.
- [x] Add `CommandRunner` based on `ProcessStartInfo.ArgumentList`, asynchronous output capture and cancellation.
- [x] Add Linux-aware executable lookup for Java, ADB, Apktool and Android build-tools.
- [x] Add first APK workflow service methods: Apktool decode/build and ADB install.
- [x] Add Ubuntu GitHub Actions restore/build/publish artifact workflow.
- [x] Compile and publish the Avalonia application on Ubuntu CI (run 41019820666, commit e221420). Runtime execution on a graphical Linux desktop is still pending.
- [ ] Add unit tests; manual Linux UI testing remains pending.

## Phase 2 — Functional desktop workflows (in progress)

- [x] Replace placeholder window actions with native Avalonia storage-provider file and folder pickers.
- [x] Connect decode and build to Apktool.
- [x] Connect PK8/PEM APK signing to apksigner.
- [x] Connect zipalign and alignment verification.
- [x] Connect ADB install, aapt badging metadata and .apks ZIP extraction.
- [ ] Add keystore UI, configurable tool paths and framework management.\n- [x] Add cancellable operation execution, live stdout/stderr output, status feedback and an indeterminate progress indicator.
- [x] Run Ubuntu CI: restore, build, Linux x64 publish and artifact upload passed after the XAML fix.
- [x] Correct the XAML property to use ScrollViewer.VerticalScrollBarVisibility; verified by successful Ubuntu CI run 41019820666.

## Phase 3 — Process-runner tests and CI (in progress)

- [x] Add linux/ApkEasyTool.Linux.Tests targeting .NET 8 with xUnit.
- [x] Add tests for argument preservation with spaces, non-zero exit codes and cancellation/child-process termination.
- [x] Add a CI test step before Linux publish.
- [x] Document local restore/build/test/publish commands in linux/README.md.
- [ ] Confirm the latest manual workflow passes. First test-enabled run 41020104176 built the app but failed compiling tests because the test file omitted using Xunit; fixed in commit 6f1a14c, awaiting CI verification.
- [ ] Expand tests to cover tool discovery and APK workflow argument construction.

## Important compatibility notes

- Existing source uses Windows Forms and .NET Framework 4.8; that UI cannot be built natively for Linux.
- Existing paths frequently contain Windows separators and executable names (for example `cmd.exe`, `java.exe`, `explorer.exe`). Linux support requires platform-aware replacements and argument-safe process launching.
- Windows Explorer shell extension under `AETShellExt` is not portable. Leave it available for Windows builds; do not attempt to install it on Linux.
- Do not modify or remove Windows resources, forms, project files or release workflows as part of the Linux implementation unless a later shared-code refactor is clearly justified.
- Preserve original creator attribution (Evildog1) in all new documentation and UI credits.

## Next session — exact starting point

1. Read this file and inspect the latest test-enabled GitHub Actions run.
2. If CI fails, inspect its test/build logs and fix the issue in a focused commit; do not assume the tests passed.
3. Run the Linux app in a real graphical desktop and manually validate file pickers, logs, decode/build, signing, alignment, metadata and ADB install against installed Android tools.
4. Improve ToolLocator: avoid spawning /usr/bin/test for every PATH candidate; add unit coverage and support configurable absolute tool paths.
5. Add keystore signing controls to the UI and validate PKCS12/JKS workflows using apksigner.
6. Add percentage progress only where tools expose reliable progress; otherwise retain the indeterminate progress indicator.
7. Inventory and port remaining Windows workflows: framework management, Smali/Baksmali, APK/AAB/APKS handling and preferences.
8. Add packaging (AppImage or .deb) after runtime behavior is stable; keep Linux packaging separate from the Windows release pipeline.
9. Update this document after each coherent milestone and commit each independently.

## Commit discipline

- Commit each independently useful change with a focused message.
- Update this file in the same commit when a milestone is completed or a technical decision changes.
- Keep this file actionable: record completed work, tests and results, blockers, and the next exact task.
- Do not claim Linux compatibility until the Linux build and core workflows have been tested on Linux.

## Current status

**Branch:** `linux`  
**Baseline:** `master` at `19bb5104122a6fe33780c9223bbda04ab1606e45`  
**Status:** Avalonia Linux x64 app builds and publishes successfully in Ubuntu CI. Process-runner tests and a test-enabled CI workflow have been added. The first test run found a missing Xunit import, fixed in commit 6f1a14c; verify the rerun. User runtime test exposed that the framework-dependent artifact required .NET 8; publish now uses self-contained mode (commit 832f5bf). No real desktop runtime test has been completed. Keystore UI, configurable tools, framework management and feature parity remain. The app now has system/light/dark appearance modes, live output, a cancellable progress indicator and a redesigned workspace. CI is manual-only (`workflow_dispatch`). Debian packaging has been added to CI but awaits CI and real-system installation validation. The app now creates ~/Documents/APKEasyTool with Compile, Decompiles, Rebuild, Signed, ZipAligned, Extracted, Frameworks, Logs and Temp subfolders; decode/build/sign/zipalign/APKS extraction use their default folders automatically.


## UX refresh (commit bdb8d2804e9cc4bccf6bafa178e3a8b91ae82383)

- [x] Redesign the Avalonia workspace with a clear action sidebar, input selection area and activity console.
- [x] Add System / Light / Dark appearance selection; System is the default.
- [x] Stream process stdout/stderr into the activity console while the operation is running.
- [x] Show operation status, an indeterminate progress bar and a cancellation action.
- [x] Disable workflow actions while a process is running to prevent overlapping operations.
- [x] Change Linux CI to manual-only; no push or pull-request trigger remains.
- [ ] Run manual GitHub Actions build and test; run-time verification on a Linux desktop is still required.
