# Build Instructions for Deadpool's Healing Factor Mod

## Prerequisites

1. **Visual Studio Build Tools 2019** (already detected on your system)
2. **Rimworld installation** (Steam or standalone)
3. **Harmony mod** installed in your Rimworld mods folder

## Quick Build

### Option 1: Use the PowerShell script (Recommended)
1. Edit `build.ps1` and set the correct `$RimWorldDir` path
2. Right-click `build.ps1` → "Run with PowerShell"
3. Or run: `powershell -ExecutionPolicy Bypass -File build.ps1`

### Option 2: Use the batch file
1. Edit `build.bat` and set the correct `RimWorldDir` path
2. Double-click `build.bat`

### Option 3: Manual build
1. Set environment variable: `set RimWorldDir=C:\Path\To\Your\Rimworld`
2. Run: `"C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe" "Source\DeadpoolsHealingFactor\DeadpoolsHealingFactor.csproj" /p:Configuration=Release /p:Platform="Any CPU"`

## Finding Your Rimworld Path

### Steam Installation
- **Default**: `C:\Program Files\Steam\steamapps\common\RimWorld`
- **Alternative**: `C:\Program Files (x86)\Steam\steamapps\common\RimWorld`
- **Custom Library**: `D:\Steam\steamapps\common\RimWorld` (or other drive)

### Standalone Installation
- Check where you installed Rimworld manually

## What Gets Built

When successful, the compiled DLL will be placed in:
```
1.6\Assemblies\DeadpoolsHealingFactor.dll
```

## Troubleshooting

### "Rimworld path not found"
- Edit the build script and set the correct path
- Make sure the path points to the main Rimworld folder (contains RimWorldWin64.exe)

### "MSBuild not found"
- Ensure Visual Studio Build Tools 2019 are installed
- The script looks for MSBuild at: `C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe`

### Build errors
- Check that all required Rimworld assemblies exist in the specified path
- Ensure Harmony mod is installed and contains `0Harmony.dll`

## Mod Installation

After successful build:
1. Copy the entire `1.6` folder to your Rimworld mods directory
2. Enable the mod in Rimworld's mod settings
3. The mod will add a new hediff (health condition) that provides Deadpool-style healing abilities

## Features

- **Healing Factor**: Gradually heals injuries over time
- **Limb Regrowth**: Regrows missing body parts
- **Mood Boost**: Keeps pawns at maximum mood
- **Psychopath Trait**: Automatically applies psychopath trait
- **Configurable Settings**: Adjustable healing rates and limits
