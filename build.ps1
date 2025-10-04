#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Build script for DanielsWpfCoaster

.DESCRIPTION
    This script builds the project, runs tests (if any), and creates NuGet packages.
    Version is determined from git tags (e.g., v1.2.3).

.PARAMETER Configuration
    Build configuration (Debug or Release). Default: Release

.PARAMETER Target
    Build target: Clean, Build, Test, Package, or Publish. Default: Package

.PARAMETER SkipClean
    Skip the clean step

.PARAMETER NuGetApiKey
    NuGet API key for publishing (can also be set via NUGET_API_KEY environment variable)

.EXAMPLE
    .\build.ps1
    .\build.ps1 -Configuration Debug
    .\build.ps1 -Target Publish -NuGetApiKey "your-key"
#>

[CmdletBinding()]
param(
    [Parameter()]
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [Parameter()]
    [ValidateSet('Clean', 'Build', 'Test', 'Package', 'Publish')]
    [string]$Target = 'Package',

    [Parameter()]
    [switch]$SkipClean,

    [Parameter()]
    [string]$NuGetApiKey = $env:NUGET_API_KEY
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

# Paths
$RootDir = $PSScriptRoot
$SourceDir = Join-Path $RootDir "source"
$StageDir = Join-Path $RootDir "stage"
$PackageDir = Join-Path $StageDir "package"
$PublishDir = Join-Path $StageDir "publish"
$SolutionFile = Join-Path $RootDir "DanielsWpfCoaster.sln"
$ProjectFile = Join-Path $SourceDir "DanielsWpfCoaster\DanielsWpfCoaster.csproj"

# Functions
function Write-Banner {
    param([string]$Message)
    
    $line = "=" * 80
    Write-Host ""
    Write-Host $line -ForegroundColor Cyan
    Write-Host "  $Message" -ForegroundColor Cyan
    Write-Host $line -ForegroundColor Cyan
    Write-Host ""
}

function Invoke-CleanTask {
    Write-Banner "CLEAN"
    
    # Clean bin/obj directories
    Get-ChildItem -Path $SourceDir -Include bin,obj -Recurse -Directory | 
        Where-Object { $_.FullName -like "*\$Configuration" -or $_.Name -eq 'obj' } |
        Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
    
    # Clean stage directory
    if (Test-Path $StageDir) {
        Remove-Item $StageDir -Recurse -Force -ErrorAction SilentlyContinue
    }
    
    # Recreate stage directories
    New-Item -ItemType Directory -Path $PackageDir -Force | Out-Null
    New-Item -ItemType Directory -Path $PublishDir -Force | Out-Null
    
    Write-Host "✓ Clean completed" -ForegroundColor Green
}

function Invoke-BuildTask {
    Write-Banner "BUILD"
    
    Write-Host "Configuration: $Configuration" -ForegroundColor Cyan
    Write-Host "MinVer will determine version from git tags" -ForegroundColor Cyan
    Write-Host ""
    
    # Build the solution - MinVer will handle versioning
    $buildArgs = @(
        $SolutionFile,
        "/t:Restore;Rebuild",
        "/p:Configuration=$Configuration",
        "/m",
        "/v:minimal"
    )
    
    & dotnet msbuild @buildArgs
    
    if ($LASTEXITCODE -ne 0) {
        throw "Build failed with exit code $LASTEXITCODE"
    }
    
    Write-Host "✓ Build completed" -ForegroundColor Green
}

function Invoke-TestTask {
    Write-Banner "TEST"
    
    # Find test projects
    $testProjects = @(Get-ChildItem -Path $SourceDir -Filter "*.Tests.csproj" -Recurse -ErrorAction SilentlyContinue)
    
    if ($testProjects.Count -eq 0) {
        Write-Host "No test projects found. Skipping tests." -ForegroundColor Yellow
        return
    }
    
    foreach ($testProject in $testProjects) {
        Write-Host "Running tests in $($testProject.Name)..." -ForegroundColor Cyan
        
        dotnet test $testProject.FullName `
            --configuration $Configuration `
            --no-build `
            --logger "trx;LogFileName=TestResult.xml" `
            --results-directory $StageDir
        
        if ($LASTEXITCODE -ne 0) {
            throw "Tests failed with exit code $LASTEXITCODE"
        }
    }
    
    Write-Host "✓ Tests completed" -ForegroundColor Green
}

function Invoke-PackageTask {
    Write-Banner "PACKAGE"
    
    Write-Host "Creating NuGet package (version from MinVer)..." -ForegroundColor Cyan
    
    # Use dotnet pack - MinVer will set the version
    dotnet pack $ProjectFile `
        --configuration $Configuration `
        --no-build `
        --output $PublishDir
    
    if ($LASTEXITCODE -ne 0) {
        throw "Pack failed with exit code $LASTEXITCODE"
    }
    
    # List created packages
    $packages = Get-ChildItem -Path $PublishDir -Filter "*.nupkg"
    Write-Host ""
    Write-Host "Created packages:" -ForegroundColor Green
    foreach ($package in $packages) {
        Write-Host "  - $($package.Name)" -ForegroundColor White
    }
    
    Write-Host "✓ Package completed" -ForegroundColor Green
}

function Invoke-PublishTask {
    Write-Banner "PUBLISH"
    
    if ([string]::IsNullOrWhiteSpace($NuGetApiKey)) {
        Write-Warning "NuGet API key not provided. Skipping publish."
        Write-Host "Set NUGET_API_KEY environment variable or use -NuGetApiKey parameter" -ForegroundColor Yellow
        return
    }
    
    # Check if we're on a release tag (not a pre-release)
    $package = Get-ChildItem -Path $PublishDir -Filter "*.nupkg" | Where-Object { $_.Name -notmatch "preview|alpha|beta|rc" } | Select-Object -First 1
    
    if (-not $package) {
        Write-Warning "No release package found (only pre-release packages). Skipping publish to nuget.org"
        Write-Host "Pre-release packages are not automatically published to nuget.org" -ForegroundColor Yellow
        Write-Host "Create a release git tag (e.g., 'git tag 1.0.0') to publish a release version" -ForegroundColor Yellow
        return
    }
    
    Write-Host "Publishing $($package.Name) to NuGet.org..." -ForegroundColor Cyan
    
    dotnet nuget push $package.FullName `
        --source "https://api.nuget.org/v3/index.json" `
        --api-key $NuGetApiKey `
        --skip-duplicate
    
    if ($LASTEXITCODE -ne 0) {
        throw "Publish failed with exit code $LASTEXITCODE"
    }
    
    Write-Host "✓ Publish completed" -ForegroundColor Green
}

# Main execution
try {
    Write-Host ""
    Write-Host "╔════════════════════════════════════════════════════════════════════════════════╗" -ForegroundColor Magenta
    Write-Host "║                        DanielsWpfCoaster Build Script                          ║" -ForegroundColor Magenta
    Write-Host "╚════════════════════════════════════════════════════════════════════════════════╝" -ForegroundColor Magenta
    Write-Host ""
    
    Write-Host "Build Information:" -ForegroundColor Cyan
    Write-Host "  Configuration: $Configuration" -ForegroundColor White
    Write-Host "  Target: $Target" -ForegroundColor White
    Write-Host "  Versioning: MinVer (git tag-based)" -ForegroundColor White
    
    # Execute tasks based on target
    if (-not $SkipClean -and $Target -ne 'Clean') {
        Invoke-CleanTask
    }
    
    switch ($Target) {
        'Clean' {
            Invoke-CleanTask
        }
        'Build' {
            Invoke-BuildTask
        }
        'Test' {
            Invoke-BuildTask
            Invoke-TestTask
        }
        'Package' {
            Invoke-BuildTask
            Invoke-TestTask
            Invoke-PackageTask
        }
        'Publish' {
            Invoke-BuildTask
            Invoke-TestTask
            Invoke-PackageTask
            Invoke-PublishTask
        }
    }
    
    Write-Host ""
    Write-Host "════════════════════════════════════════════════════════════════════════════════" -ForegroundColor Green
    Write-Host "  ✓ BUILD SUCCEEDED" -ForegroundColor Green
    Write-Host "════════════════════════════════════════════════════════════════════════════════" -ForegroundColor Green
    Write-Host ""
}
catch {
    Write-Host ""
    Write-Host "════════════════════════════════════════════════════════════════════════════════" -ForegroundColor Red
    Write-Host "  ✗ BUILD FAILED" -ForegroundColor Red
    Write-Host "════════════════════════════════════════════════════════════════════════════════" -ForegroundColor Red
    Write-Host ""
    Write-Error $_
    exit 1
}
