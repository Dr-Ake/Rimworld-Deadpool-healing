# Build script for Deadpool's Healing Factor mod
# Edit the RimWorldDir path below to match your Rimworld installation

# Set your Rimworld installation path here
$RimWorldDir = "C:\Program Files\Steam\steamapps\common\RimWorld"

# Alternative common paths - uncomment and modify as needed:
# $RimWorldDir = "C:\Program Files (x86)\Steam\steamapps\common\RimWorld"
# $RimWorldDir = "D:\Steam\steamapps\common\RimWorld"

Write-Host "Building Deadpool's Healing Factor mod..." -ForegroundColor Green
Write-Host "Using Rimworld path: $RimWorldDir" -ForegroundColor Yellow

# Check if Rimworld path exists
if (-not (Test-Path $RimWorldDir)) {
    Write-Host "ERROR: Rimworld path not found: $RimWorldDir" -ForegroundColor Red
    Write-Host "Please edit this file and set the correct Rimworld installation path" -ForegroundColor Red
    Read-Host "Press Enter to continue"
    exit 1
}

# Build the project
$MSBuildPath = "C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe"

if (-not (Test-Path $MSBuildPath)) {
    Write-Host "ERROR: MSBuild not found at: $MSBuildPath" -ForegroundColor Red
    Write-Host "Please ensure Visual Studio Build Tools are installed" -ForegroundColor Red
    Read-Host "Press Enter to continue"
    exit 1
}

Write-Host "Starting build..." -ForegroundColor Green

& $MSBuildPath "Source\DeadpoolsHealingFactor\DeadpoolsHealingFactor.csproj" /p:Configuration=Release /p:Platform="Any CPU" /p:RimWorldDir="$RimWorldDir"

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "Build successful! DLL should be in: 1.6\Assemblies\" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "Build failed with error code: $LASTEXITCODE" -ForegroundColor Red
}

Read-Host "Press Enter to continue"
