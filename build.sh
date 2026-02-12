#!/bin/bash
#
# Cartridge Build Script
# Syncs source to Windows build server and runs a .NET build
#
# Usage: ./build.sh [clean|release|debug]
#   clean   - Clean and rebuild
#   release - Build in Release configuration (default)
#   debug   - Build in Debug configuration
#
set -e

# Configuration
BUILD_HOST="winbuild"
BUILD_DIR="C:/Build/Cartridge"
LOCAL_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
CONFIGURATION="${1:-release}"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

info()  { echo -e "${BLUE}[INFO]${NC} $1"; }
success() { echo -e "${GREEN}[SUCCESS]${NC} $1"; }
warn()  { echo -e "${YELLOW}[WARN]${NC} $1"; }
error() { echo -e "${RED}[ERROR]${NC} $1" >&2; }

# Parse arguments
CLEAN=false
case "$CONFIGURATION" in
    clean)
        CLEAN=true
        CONFIGURATION="Release"
        ;;
    release|Release)
        CONFIGURATION="Release"
        ;;
    debug|Debug)
        CONFIGURATION="Debug"
        ;;
    *)
        error "Unknown configuration: $CONFIGURATION"
        echo "Usage: $0 [clean|release|debug]"
        exit 1
        ;;
esac

info "Building Cartridge ($CONFIGURATION configuration)"

# Test SSH connectivity
info "Testing connection to build server..."
if ! ssh -o ConnectTimeout=10 "$BUILD_HOST" "echo OK" >/dev/null 2>&1; then
    error "Cannot connect to build server: $BUILD_HOST"
    exit 1
fi
success "Connected to $BUILD_HOST"

# Prepare build directory (clean source files, preserve build caches)
info "Preparing build directory..."
ssh "$BUILD_HOST" "powershell -Command \"\
    if (!(Test-Path '$BUILD_DIR')) { \
        New-Item -ItemType Directory -Path '$BUILD_DIR' -Force | Out-Null; \
    } else { \
        Get-ChildItem '$BUILD_DIR' -Exclude 'bin','obj','packages','.nuget' | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue; \
    }\""

# Sync source files using tar over SSH (portable, no rsync needed)
info "Syncing source files..."
cd "$LOCAL_DIR"
tar -cf - \
    --exclude='./bin' \
    --exclude='./obj' \
    --exclude='./.git' \
    --exclude='*.user' \
    --exclude='./.vs' \
    --exclude='./TestResults' \
    --exclude='./packages' \
    --exclude='./src/bin' \
    --exclude='./src/obj' \
    --exclude='./src/Cartridge.Core/bin' \
    --exclude='./src/Cartridge.Core/obj' \
    . | ssh "$BUILD_HOST" "cd /d \"$BUILD_DIR\" && tar -xf -"

success "Source synced to build server"

# Build on Windows
info "Running dotnet restore..."
ssh "$BUILD_HOST" "cd /d \"$BUILD_DIR\" && dotnet restore --verbosity quiet" || {
    error "Restore failed"
    exit 1
}

if $CLEAN; then
    info "Cleaning build artifacts..."
    ssh "$BUILD_HOST" "cd /d \"$BUILD_DIR\" && dotnet clean -c $CONFIGURATION --verbosity quiet" 2>/dev/null || true
fi

info "Building solution..."
if ! ssh "$BUILD_HOST" "cd /d \"$BUILD_DIR\" && dotnet build -c $CONFIGURATION --no-restore"; then
    error "Build failed"
    exit 1
fi

success "Build completed successfully!"
info "Artifacts: $BUILD_DIR/src/bin/$CONFIGURATION/net8.0-windows/"
