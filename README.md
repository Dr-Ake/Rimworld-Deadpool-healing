## Deadpool's Healing Factor

Version 0.0.2.0

Hi. It's me. The guy on the cover. I heal fast. Your pawns? Now they do too. Install this and watch limbs regrow, moods skyrocket, and death become a mild inconvenience. Fourth wall? What fourth wall.

![Preview](About/Preview.png)

### TL;DR
- **What**: A standalone, lightweight RimWorld 1.6 mod that gives pawns Deadpool-style regeneration.
- **Why**: Because losing arms is funny… until it's your crafter.
- **How**: Harmony patch that heals, regrows, and buffs on a configurable schedule — even while dead.

## Features (Maximum Effort Edition)
- **You can die… briefly**: If a pawn with the factor dies, healing and regrowth keep going on the corpse. When everything’s restored, they stand back up like nothing happened.
- **Healing factor**: Wounds heal automatically at intervals you set. Severity scales the healing.
- **Limb/part regrowth**: Missing parts regrow over time (including head). Cap how many grow at once and tune the speed.
- **Everything heals**: Scars fade, diseases tick down and clear, and general nasty stuff gets purged over time. Prosthetics/bionics are respected.
- **Recovery wobble**: Freshly regrown parts get a short “adjusting” period to keep it spicy.
- **Mood juice**: Optional mood max buff. Because happy pawns shoot straighter. Science.
- **Psychopath switch**: Optional trait application for humanlikes. You wanted Deadpool… you get Deadpool.
- **Snark with settings**: Sliders and toggles for tick rate, heal amount, regrow speed, and more.

## What's in the box?
- **About/**: Metadata and that handsome preview image up there.
- **1.6/**: All the defs for RimWorld 1.6.
  - **Defs/**: Hediffs that make the magic happen.
- **Source/DeadpoolsHealingFactor/**: The C# source code, if you're into that sort of thing. The pre-compiled DLL is already in `1.6/Assemblies`.

## Installation
1. Install Harmony first (`brrainz.harmony`). You already have it. If you don't, get it.
2. Drop this mod folder into `RimWorld/Mods`.
3. Activate it in the mod list. Profit.

## In‑Game Settings (aka the Chimichanga Menu)
- **Enable Healing**: Turns the passive healing on/off.
- **Enable Regrowth**: Let those missing parts make a comeback tour.
- **Boost Mood**: Keep mood topped off. Big smiles, fewer tantrums.
- **Force Psychopath**: Applies the Psychopath trait to humanlikes with the factor.
- **Ticks Between Heals**: How often the healing/regrowth logic runs.
- **Base Heal Amount**: How much gets healed per tick, scaled by severity.
- **Regrow Speed**: How fast parts regrow (severity multiplies this).
- **Max Regrowing Parts**: Parallel limbs, baby. Cap the simultaneous regrowths.

## How it works (without the technobabble)
- Pawns with `Healing Factor` get periodic healing based on a tick interval and severity.
- If they die, the healing/regrowth still runs on their corpse. Once all injuries are healed and missing parts are back, they resurrect automatically.
- Regrowth anchors on the parent and restores the missing child (so full limbs and even heads grow back).
- Optional: mood max and Psychopath trait application (humanlikes only) to stay on theme.

## Compatibility
- Built for RimWorld 1.6.
- Requires Harmony. Load after Harmony (it’s in the metadata already).
- Safe for existing saves. Remove at your own risk; I’m not resurrecting your colony.

## Building from Source (Don't)
Psst. Hey. You. Yeah, you with the compiler. Put it away. I already did the hard work for you. The `DeadpoolsHealingFactor.dll` is right there in the `1.6/Assemblies` folder, all shiny and pre-compiled. No need to get your hands dirty. Just grab the mod, drop it in, and let the healing begin. You're welcome.

## FAQ
- **Does this make pawns immortal?**
  Immortal? Please. That's a big word. Let's just say death is a coffee break. Chop the head off? It grows back. Turn them into paste or ashes? Okay, that’s cheating. Also, pro-tip: don’t let them get kidnapped — hard to heal your way out of the cargo hold.
- **Can I turn off the Psychopath thing?**
  Yes. Toggle it in settings.
- **How many limbs can grow back at once?**
  You choose with “Max Regrowing Parts.” Regrowth speed scales with severity.
- **Is it performance‑friendly?**
  Yep. Work happens on a configurable interval and uses cached defs.

## Credits
- Code and design: Dr-Ake
- Harmony magic: Brrainz and contributors
- Inspiration: The unkillable, unshut‑uppable Deadpool
- Also heavy inspiration and special thanks to the Immortal Mod/ Highlander Mod

## License
MIT. See `LICENSE`.

## Disclaimer
Unofficial fan work. Not endorsed by Marvel, Ludeon Studios, or anyone with expensive lawyers. Chimichangas sold separately.
