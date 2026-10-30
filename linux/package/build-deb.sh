#!/usr/bin/env bash
set -euo pipefail

PUBLISH_DIR="${1:-artifacts/linux-x64}"
OUTPUT_DIR="${2:-artifacts/deb}"
PACKAGE_ROOT="artifacts/deb-root"
VERSION="${PACKAGE_VERSION:-0.1.0}"
ARCH="${PACKAGE_ARCH:-amd64}"
PACKAGE_NAME="apk-easy-tool"

if [[ ! -x "$PUBLISH_DIR/apk-easy-tool" ]]; then
  echo "Published apphost not found: $PUBLISH_DIR/apk-easy-tool" >&2
  exit 1
fi

rm -rf "$PACKAGE_ROOT"
mkdir -p "$PACKAGE_ROOT/DEBIAN" "$PACKAGE_ROOT/opt/$PACKAGE_NAME" \
  "$PACKAGE_ROOT/usr/bin" "$PACKAGE_ROOT/usr/share/applications"

cp -a "$PUBLISH_DIR/." "$PACKAGE_ROOT/opt/$PACKAGE_NAME/"
chmod 0755 "$PACKAGE_ROOT/opt/$PACKAGE_NAME/apk-easy-tool"

cat > "$PACKAGE_ROOT/DEBIAN/control" <<CONTROL
Package: $PACKAGE_NAME
Version: $VERSION
Section: devel
Priority: optional
Architecture: $ARCH
Maintainer: APK Easy Tool Linux contributors
Depends: default-jre | java-runtime, apktool, adb, android-sdk-build-tools, libx11-6, libice6, libsm6, libfontconfig1, libfreetype6, libxkbcommon0
Homepage: https://github.com/mhmdeve/APK-Easy-Tool
Description: APK inspection and modification desktop utility
 Linux desktop port of APK Easy Tool. Provides APK decode, build,
 signing, zip alignment, metadata inspection and ADB installation.
 Android command-line dependencies are installed automatically by APT.
CONTROL

cat > "$PACKAGE_ROOT/usr/bin/apk-easy-tool" <<'LAUNCHER'
#!/bin/sh
exec /opt/apk-easy-tool/apk-easy-tool "$@"
LAUNCHER
chmod 0755 "$PACKAGE_ROOT/usr/bin/apk-easy-tool"

cat > "$PACKAGE_ROOT/usr/share/applications/apk-easy-tool.desktop" <<'DESKTOP'
[Desktop Entry]
Type=Application
Name=APK Easy Tool
Comment=Decode, build, sign and inspect Android APK files
Exec=/usr/bin/apk-easy-tool
Terminal=false
Categories=Development;Utility;
StartupNotify=true
DESKTOP

mkdir -p "$OUTPUT_DIR"
dpkg-deb --root-owner-group --build "$PACKAGE_ROOT" \
  "$OUTPUT_DIR/${PACKAGE_NAME}_${VERSION}_${ARCH}.deb"

echo "Created $OUTPUT_DIR/${PACKAGE_NAME}_${VERSION}_${ARCH}.deb"
