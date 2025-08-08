# Deadpool's Healing Factor

**Version 0.0.2.0**

Whoa there, reader! Yep, it's me, narrating my own README. This repo packs my not-so-secret sauce straight into your RimWorld pawns. Install it and watch them shrug off decapitations faster than you can yell "Chimichanga!" No sprawling frameworks, no mystical X-Men tie-ins – just an absurd dose of regeneration wrapped in a tidy mod.

## What's in the box?

- **About/** – Metadata and a spiffy preview image so you know what you just downloaded.
- **1.6/** – All the juicy stuff for RimWorld 1.6: patch files and XML defs that keep the carnage running smoothly.
  - **Defs/** – Damage, hediff, and thought definitions that teach your pawns to ignore minor annoyances like death.
  - **Patches/** – Tiny tweaks for compatibility. I even care about lactation, apparently.
  - **Assemblies/** - Precompiled `Deadpool.dll` for immediate use.
- **Languages/** – Snarky English translations. Other languages? Maybe later when I hire a bilingual merc.
- **Source/DeadpoolsHealingFactor/** – The C# project with a simple Harmony patch that kicks everything off. Building places the DLL into `1.6/Assemblies`.

## Features

- Grants a beefy healing factor that laughs at wounds and disease.
- Settings galore: tweak healing ticks, regrowth speed, and more – all described in the most sarcastic tooltips you'll ever read.
- Custom thoughts and hediffs from `Hediffs_Deadpool.xml`, including your very own Death Nap and Uncanny Hunger.
- Precompiled DLLs so you can jump straight into the action without building a thing. Check `1.6/Assemblies` for the compiled `Deadpool.dll`.

## Installation

1. Grab [Harmony](https://github.com/pardeike/HarmonyRimWorld) if you don't already have it.
2. Drop this folder into your RimWorld `Mods` directory.
3. Fire up the game and enable **Deadpool's Healing Factor**. Boom – your colonists just went full mutant.

## Compatibility

Built for RimWorld 1.6.

## Building

Set the `RimWorldDir` environment variable to your RimWorld installation path before building. For example on Windows PowerShell:

```
$env:RimWorldDir = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\RimWorld"
```

Harmony's DLL is expected under `Mods/Harmony/Assemblies` inside that directory. Once configured, open the project in `Source/DeadpoolsHealingFactor` with your favorite .NET build tools and compile it. The resulting DLL will appear in `1.6/Assemblies`.

Now quit reading this file and go break the fourth wall somewhere else. Chimichangas await!

## License

This project is released under the [MIT License](LICENSE).

## Disclaimer

This mod is entirely unofficial and not endorsed by Marvel, Ludeon Studios, or anyone else who could send a cease-and-desist. Use at your own risk and remember: chimichangas not included.
