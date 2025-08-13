Param(
    [string]$RimWorldDir = 'C:\Program Files (x86)\Steam\steamapps\common\RimWorld',
    [string]$HarmonyDir  = 'C:\Program Files (x86)\Steam\steamapps\workshop\content\294100\2009463077\Current\Assemblies'
)

$ErrorActionPreference = 'Stop'

Write-Host "Building DeadpoolsHealingFactor.dll..." -ForegroundColor Cyan

$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path | Split-Path -Parent
Set-Location $repoRoot

$proj = Join-Path $repoRoot 'Source/DeadpoolsHealingFactor/DeadpoolsHealingFactor.csproj'
$outDir = Join-Path $repoRoot '1.6/Assemblies'
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

function Invoke-RoslynBuild {
    param(
        [string]$RimWorldDirParam,
        [string]$HarmonyDirParam,
        [string]$OutDirParam
    )

    $managed = Join-Path $RimWorldDirParam 'RimWorldWin64_Data/Managed'
    $roslyn2019 = 'C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\Roslyn\csc.exe'
    $roslyn2022 = 'C:\Program Files\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\Roslyn\csc.exe'
    $csc = $null
    if (Test-Path $roslyn2022) { $csc = $roslyn2022 }
    elseif (Test-Path $roslyn2019) { $csc = $roslyn2019 }
    if (-not $csc) { throw 'Roslyn csc.exe not found. Install Visual Studio Build Tools.' }

    $out = Join-Path $OutDirParam 'DeadpoolsHealingFactor.dll'
    if (Test-Path $out) { Remove-Item $out -Force }

    $refNames = @('mscorlib.dll','System.dll','System.Core.dll','System.Xml.dll','System.Xml.Linq.dll','System.Net.Http.dll','netstandard.dll','System.Runtime.dll','Assembly-CSharp.dll','UnityEngine.CoreModule.dll','UnityEngine.dll')
    $refs = @()
    foreach ($n in $refNames) { $refs += "/reference:`"$(Join-Path $managed $n)`"" }
    $refs += "/reference:`"$(Join-Path $HarmonyDirParam '0Harmony.dll')`""

    $src = @(
        'Source/DeadpoolsHealingFactor/DeadpoolsHealingFactor.cs',
        'Source/DeadpoolsHealingFactor/Settings.cs',
        'Source/DeadpoolsHealingFactor/UseEffectGiveHediff.cs'
    )

    & $csc /noconfig /nostdlib+ /target:library /optimize+ /langversion:latest ("/out:$out") $refs $src
    if ($LASTEXITCODE -ne 0) { throw "csc.exe failed with exit code $LASTEXITCODE" }
    Write-Host "Roslyn build complete." -ForegroundColor Green
}

# Try MSBuild first
$ms2022 = 'C:\Program Files\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe'
$ms2019 = 'C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe'
$msbuild = $null
if (Test-Path $ms2022) { $msbuild = $ms2022 }
elseif (Test-Path $ms2019) { $msbuild = $ms2019 }

if ($msbuild) {
    try {
        & $msbuild $proj /m /v:m /p:Configuration=Release /p:Platform=AnyCPU /p:RimWorldDir="$RimWorldDir" /p:HarmonyDir="$HarmonyDir"
        if ($LASTEXITCODE -ne 0) { throw "MSBuild failed with exit code $LASTEXITCODE" }
        Write-Host "MSBuild complete." -ForegroundColor Green
        Write-Host "Output: $outDir" -ForegroundColor Cyan
        exit 0
    } catch {
        Write-Host $_ -ForegroundColor Yellow
        Write-Host "Falling back to Roslyn csc.exe..." -ForegroundColor Yellow
        Invoke-RoslynBuild -RimWorldDirParam $RimWorldDir -HarmonyDirParam $HarmonyDir -OutDirParam $outDir
    }
} else {
    Write-Host "MSBuild not found. Falling back to Roslyn csc.exe..." -ForegroundColor Yellow
    Invoke-RoslynBuild -RimWorldDirParam $RimWorldDir -HarmonyDirParam $HarmonyDir -OutDirParam $outDir
}

Write-Host "Output: $outDir" -ForegroundColor Cyan


