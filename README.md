# Deadpool's Healing Factor

This repository contains the RimWorld mod **Deadpool's Healing Factor**. The mod is built on top of the Immortals framework and grants pawns an absurdly strong regenerative ability reminiscent of the Merc with a Mouth. The repository includes definitions and precompiled assemblies for RimWorld 1.0 through 1.6.

## Repository layout

- `About/` – mod metadata and preview image.
- `1.6/` – assemblies and definitions for RimWorld 1.6.
- `Defs/` – definitions used by older game versions.
- `Languages/` – English and partial Russian translations.
- `Textures/` – UI icons for beheading and extractor actions.
- `Source/obj/` – build artifacts; the original C# source code is not included.

## Features

- Replaces the standard Immortal hediff with **Healing Factor**, eliminating pain for affected pawns.
- Adds Immortals mechanics such as beheading jobs, immortal extractor building and the Mortalis drug.
- Includes a patch that marks the `Lactating` hediff as non harmful when present.
- Translation support for English with limited Russian entries.
- Provides a precompiled DLL (`Immortals 1.5.dll`) for RimWorld 1.6.

## Installation

1. Install the Harmony dependency.
2. Copy this mod folder into your RimWorld `Mods` directory.
3. Activate **Deadpool's Healing Factor** in the mod menu.

## Compatibility

The metadata lists support for RimWorld versions 1.0 through 1.6. Use the files inside `1.6/` for the latest game version.

## Building

Only compiled assemblies are provided. Rebuilding the DLL requires the original source code, which is not included in this repository.

