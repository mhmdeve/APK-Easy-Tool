# Debian / Ubuntu package

The CI workflow creates an amd64 `.deb` package for Debian- and Ubuntu-based distributions.

The package declares these APT dependencies, so installing the application also installs its command-line requirements:

- `default-jre | java-runtime` — Java runtime for Apktool and Android utilities
- `apktool` — APK decode and rebuild
- `adb` — device installation
- `android-sdk-build-tools` — `aapt`, `apksigner`, and `zipalign`
- X11/font runtime libraries used by the Avalonia desktop application

Enable the Ubuntu/Debian `universe` component (Ubuntu) if these packages are not available in your configured repositories.

Install the downloaded package:

```bash
sudo apt update
sudo apt install ./apk-easy-tool_0.1.0_amd64.deb
```

APT resolves and installs the dependencies automatically. Then launch **APK Easy Tool** from the desktop application menu, or run:

```bash
apk-easy-tool
```

Verify the external tools are discoverable:

```bash
java -version
apktool --version
adb version
aapt version
apksigner version
zipalign -h
```

If a tool is installed in a custom Android SDK directory rather than a system PATH directory, add its directory to PATH before launching the app. The package does not download Google's SDK binaries itself; it uses the distribution's APT packages.

Uninstall the application with:

```bash
sudo apt remove apk-easy-tool
```
