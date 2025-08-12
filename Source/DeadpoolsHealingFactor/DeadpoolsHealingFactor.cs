using System.Reflection;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

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
            if (___pawn == null || ___pawn.Dead)
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
            if (Find.TickManager.TicksGame % interval != 0)
            {
                return;
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

            float healAmount = DeadpoolsHealingFactorMod.settings.baseHealAmount * hediff.Severity;

            Hediff_Injury injury = ___pawn.health.hediffSet.hediffs
                .OfType<Hediff_Injury>()
                .Where(h => !h.IsPermanent() && h.CanHealNaturally() && h.Severity > 0)
                .OrderByDescending(h => h.Severity)
                .FirstOrDefault();

            if (DeadpoolsHealingFactorMod.settings.enableHealing && injury != null)
            {
                injury.Heal(healAmount);
                if (Prefs.DevMode)
                {
                    Log.Message($"[DeadpoolsHealingFactor] Healed {injury.Label} on {___pawn.LabelShort} for {healAmount}.");
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
                    regrow.Severity += DeadpoolsHealingFactorMod.settings.regrowSpeed * hediff.Severity;
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
                        // A Hediff_MissingPart's 'Part' property is the BodyPartRecord
                        BodyPartRecord parent = missing.Part.parent;

                        // Check if the parent part is already growing
                        if (parent != null && !___pawn.health.hediffSet.hediffs.Any(h => h.def == regrowingDef && h.Part == parent))
                        {
                            ___pawn.health.AddHediff(regrowingDef, parent);
                            if (Prefs.DevMode)
                            {
                                Log.Message($"[DeadpoolsHealingFactor] Started regrowing a child of {parent.Label} on {___pawn.LabelShort}.");
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}
