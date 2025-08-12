using System;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using System.Collections.Generic;

namespace DeadpoolsHealingFactor
{
    [DefOf]
    public static class DPDefOf
    {
        public static HediffDef DP_HealingFactor;
        public static HediffDef DP_regrowing;
        public static HediffDef DP_adjusting;

        static DPDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(DPDefOf));
        }
    }
    [StaticConstructorOnStartup]
    public static class DeadpoolsHealingFactor
    {
        static DeadpoolsHealingFactor()
        {
            var harmony = new Harmony("CodexAgent.DeadpoolsHealingFactor");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Log.Message("[DeadpoolsHealingFactor] Initialized");
        }
    }

    [HarmonyPatch(typeof(Pawn_HealthTracker), "HealthTick")]
    public static class Patch_Pawn_HealthTracker_HealthTick
    {
        // default values are kept in Settings.cs

        public static void Postfix(Pawn_HealthTracker __instance, Pawn ___pawn)
        {
            if (___pawn == null)
            {
                return;
            }

            Hediff hediff = ___pawn.health?.hediffSet?.GetFirstHediffOfDef(DPDefOf.DP_HealingFactor);
            if (hediff == null)
            {
                return;
            }

            int interval = DeadpoolsHealingFactorMod.settings.ticksBetweenHeals;
            if (interval <= 0)
            {
                interval = 250;
            }
            // Pawn.HealthTick only runs for living pawns; keep the modulo for live case
            if (___pawn.Spawned && !___pawn.Dead)
            {
                if (Find.TickManager.TicksGame % interval != 0)
                {
                    return;
                }
            }

            // Keep mood maxed (via reflection to handle non-public setters across versions) and ensure the pawn is a psychopath
            if (DeadpoolsHealingFactorMod.settings.boostMood && ___pawn.needs?.mood != null)
            {
                try
                {
                    var need = ___pawn.needs.mood;
                    var curLevelProp = typeof(Need).GetProperty("CurLevel", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    curLevelProp?.SetValue(need, 1f, null);
                }
                catch
                {
                    // Ignore if API changes; mood boost is optional
                }
            }
            if (DeadpoolsHealingFactorMod.settings.forcePsychopath
                && ___pawn.RaceProps?.Humanlike == true
                && ___pawn.story?.traits != null
                && !___pawn.story.traits.HasTrait(TraitDefOf.Psychopath))
            {
                ___pawn.story.traits.GainTrait(new Trait(TraitDefOf.Psychopath));
            }

            float severityFactor = hediff.Severity <= 0f ? 1f : hediff.Severity;
            float healAmount = DeadpoolsHealingFactorMod.settings.baseHealAmount * severityFactor;

            if (DeadpoolsHealingFactorMod.settings.enableHealing)
            {
                // Heal or remove injuries and bad hediffs (including diseases and scars)
                List<Hediff> allHediffs = ___pawn.health.hediffSet.hediffs;
                List<Hediff> toRemove = new List<Hediff>();
                foreach (var h in allHediffs)
                {
                    if (h == null) continue;
                    if (h.def == DPDefOf.DP_HealingFactor || h.def == DPDefOf.DP_regrowing || h.def == DPDefOf.DP_adjusting)
                    {
                        continue;
                    }
                    // Let regrowth system handle missing parts
                    if (h is Hediff_MissingPart) continue;
                    // Never remove added parts (prosthetics/bionics)
                    if (h is Hediff_AddedPart) continue;

                    if (h is Hediff_Injury inj)
                    {
                        if (inj.IsPermanent())
                        {
                            toRemove.Add(inj); // treat scars as removable
                        }
                        else if (inj.Severity > 0 && inj.CanHealNaturally())
                        {
                            inj.Heal(healAmount);
                        }
                        continue;
                    }

                    // General bad hediffs (e.g., diseases like asthma)
                    if (h.def.isBad)
                    {
                        if (h.Severity > 0f)
                        {
                            h.Severity = (float)Math.Max(0f, h.Severity - healAmount);
                        }
                        if (h.Severity <= 0f)
                        {
                            toRemove.Add(h);
                        }
                    }
                }
                foreach (var h in toRemove)
                {
                    ___pawn.health.RemoveHediff(h);
                }
            }

            // ---------- Limb regrowth logic ----------
            HediffDef regrowingDef = DPDefOf.DP_regrowing;

            var regrowingHediffs = ___pawn.health.hediffSet.hediffs
                .Where(h => h.def == regrowingDef)
                .ToList();

            if (DeadpoolsHealingFactorMod.settings.enableRegrowth)
            {
                foreach (var regrow in regrowingHediffs)
                {
                    regrow.Severity += DeadpoolsHealingFactorMod.settings.regrowSpeed * severityFactor;
                    if (regrow.Severity >= 1.0f)
                    {
                        BodyPartRecord partToRestore = regrow.Part;
                        ___pawn.health.RemoveHediff(regrow);
                        ___pawn.health.RestorePart(partToRestore);
                        // Apply a short-lived adjustment debuff to the restored part to simulate recovery
                        ___pawn.health.AddHediff(DPDefOf.DP_adjusting, partToRestore);
                    if (Prefs.DevMode)
                    {
                        Log.Message($"[DeadpoolsHealingFactor] Restored {partToRestore.Label} on {___pawn.LabelShort}.");
                    }
                }
                }

                if (regrowingHediffs.Count < DeadpoolsHealingFactorMod.settings.maxRegrowingParts)
                {
                    var missingParts = ___pawn.health.hediffSet.GetMissingPartsCommonAncestors();
                    foreach (var missing in missingParts)
                    {
                        BodyPartRecord targetPart = missing.Part; // regrow the missing part itself (handles head, etc.)
                        if (targetPart != null && !___pawn.health.hediffSet.hediffs.Any(h => h.def == regrowingDef && h.Part == targetPart))
                        {
                            ___pawn.health.AddHediff(regrowingDef, targetPart);
                            if (Prefs.DevMode)
                            {
                                Log.Message($"[DeadpoolsHealingFactor] Started regrowing {targetPart.Label} on {___pawn.LabelShort}.");
                            }
                            break;
                        }
                    }
                }
            }
        }
    }

    // Continue healing/regrowth while dead, and resurrect when fully restored
    [HarmonyPatch(typeof(Corpse), "TickRare")]
    public static class Patch_Corpse_TickRare
    {
        public static void Postfix(Corpse __instance)
        {
            if (__instance == null)
            {
                return;
            }

            Pawn pawn = __instance.InnerPawn;
            if (pawn == null)
            {
                return;
            }

            // Must have the hediff to process
            Hediff factor = pawn.health?.hediffSet?.GetFirstHediffOfDef(DPDefOf.DP_HealingFactor);
            if (factor == null)
            {
                return;
            }

            int interval = DeadpoolsHealingFactorMod.settings.ticksBetweenHeals;
            if (interval <= 0)
            {
                interval = 250;
            }
            // Corpse.TickRare runs every 250 ticks. Always process here and scale by step factor
            float stepFactor = 250f / Math.Max(1, interval);

            float severityFactor = factor.Severity <= 0f ? 1f : factor.Severity;

            if (DeadpoolsHealingFactorMod.settings.enableHealing)
            {
                float healAmount = DeadpoolsHealingFactorMod.settings.baseHealAmount * severityFactor * stepFactor;
                List<Hediff> all = pawn.health.hediffSet.hediffs;
                List<Hediff> toRemove = new List<Hediff>();
                foreach (var h in all)
                {
                    if (h == null) continue;
                    if (h.def == DPDefOf.DP_HealingFactor || h.def == DPDefOf.DP_regrowing || h.def == DPDefOf.DP_adjusting) continue;
                    if (h is Hediff_MissingPart) continue;
                    if (h is Hediff_AddedPart) continue;

                    if (h is Hediff_Injury inj)
                    {
                        if (inj.IsPermanent())
                        {
                            toRemove.Add(inj);
                        }
                        else if (inj.Severity > 0)
                        {
                            inj.Severity = (float)Math.Max(0f, inj.Severity - healAmount);
                            if (inj.Severity <= 0f)
                            {
                                toRemove.Add(inj);
                            }
                        }
                        continue;
                    }

                    if (h.def.isBad)
                    {
                        if (h.Severity > 0f)
                        {
                            h.Severity = (float)Math.Max(0f, h.Severity - healAmount);
                        }
                        if (h.Severity <= 0f)
                        {
                            toRemove.Add(h);
                        }
                    }
                }
                foreach (var h in toRemove)
                {
                    pawn.health.RemoveHediff(h);
                }
            }

            if (DeadpoolsHealingFactorMod.settings.enableRegrowth)
            {
                HediffDef regrowingDef = DPDefOf.DP_regrowing;
                var regrowingHediffs = pawn.health.hediffSet.hediffs
                    .Where(h => h.def == regrowingDef)
                    .ToList();

                foreach (var regrow in regrowingHediffs)
                {
                    regrow.Severity += DeadpoolsHealingFactorMod.settings.regrowSpeed * severityFactor * stepFactor;
                    if (regrow.Severity >= 1.0f)
                    {
                        BodyPartRecord partToRestore = regrow.Part;
                        pawn.health.RemoveHediff(regrow);
                        pawn.health.RestorePart(partToRestore);
                        pawn.health.AddHediff(DPDefOf.DP_adjusting, partToRestore);
                    }
                }

                if (regrowingHediffs.Count < DeadpoolsHealingFactorMod.settings.maxRegrowingParts)
                {
                    var missingParts = pawn.health.hediffSet.GetMissingPartsCommonAncestors();
                    foreach (var missing in missingParts)
                    {
                        BodyPartRecord targetPart = missing.Part;
                        if (targetPart != null && !pawn.health.hediffSet.hediffs.Any(h => h.def == regrowingDef && h.Part == targetPart))
                        {
                            pawn.health.AddHediff(regrowingDef, targetPart);
                            break;
                        }
                    }
                }
            }

            // If fully healed (no non-permanent injuries and no missing parts), resurrect
            bool hasOpenInjury = pawn.health.hediffSet.hediffs
                .OfType<Hediff_Injury>()
                .Any(h => !h.IsPermanent() && h.Severity > 0);

            bool hasMissingParts = pawn.health.hediffSet.GetMissingPartsCommonAncestors().Any();

            if (!hasOpenInjury && !hasMissingParts)
            {
                try
                {
                    ResurrectionUtility.TryResurrect(pawn);
                }
                catch
                {
                    // ignore if API differs; resurrection is best-effort
                }
            }
        }
    }
}
