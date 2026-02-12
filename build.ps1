<#
.SYNOPSIS
    Cartridge Build Script for Windows

.DESCRIPTION
    Builds the Cartridge NFS client solution with idempotent, reproducible builds.

.PARAMETER Configuration
    Build configuration: Debug or Release (default: Release)

.PARAMETER Clean
    Clean before building

.PARAMETER Restore
    Force restore even if packages are present

.EXAMPLE
    .\build.ps1
    .\build.ps1 -Configuration Debug
    .\build.ps1 -Clean -Configuration Release
#>

param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",

    [switch]$Clean,
    [switch]$Restore,
    [switch]$CI
)

$ErrorActionPreference = "Stop"

# Locate script directory
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $ScriptDir

try {
    Write-Host "[INFO] Building Cartridge ($Configuration)" -ForegroundColor Cyan

    # Verify SDK
    $sdkVersion = dotnet --version
    if (-not $sdkVersion.StartsWith("8.")) {
        Write-Warning "Expected .NET 8.x SDK, found $sdkVersion"
    }
    Write-Host "[INFO] Using .NET SDK $sdkVersion" -ForegroundColor Gray

    # Restore packages
    if ($Restore -or $Clean -or -not (Test-Path "src\obj")) {
        Write-Host "[INFO] Restoring packages..." -ForegroundColor Cyan
        dotnet restore --verbosity quiet
        if ($LASTEXITCODE -ne 0) { throw "Restore failed" }
    }

    # Clean if requested
    if ($Clean) {
        Write-Host "[INFO] Cleaning build artifacts..." -ForegroundColor Cyan
        dotnet clean -c $Configuration --verbosity quiet
    }

    # Set CI environment variable if requested
    if ($CI) {
        $env:CI = "true"
    }

    # Build
    Write-Host "[INFO] Building solution..." -ForegroundColor Cyan
    $buildArgs = @(
        "build",
        "-c", $Configuration,
        "--no-restore",
        "-v", "minimal"
    )

    dotnet @buildArgs
    if ($LASTEXITCODE -ne 0) {
        throw "Build failed with exit code $LASTEXITCODE"
    }

    Write-Host "`n[SUCCESS] Build completed successfully!" -ForegroundColor Green
    Write-Host "[INFO] Output: src\bin\$Configuration\net8.0-windows\" -ForegroundColor Gray
}
catch {
    Write-Host "[ERROR] $_" -ForegroundColor Red
    exit 1
}
finally {
    Pop-Location
}
