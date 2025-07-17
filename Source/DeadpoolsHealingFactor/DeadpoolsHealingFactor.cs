using System.Reflection;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DeadpoolsHealingFactor
{
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
        // Healing factor settings
        private const float BaseHealAmount = 0.5f;
        private const int TicksBetweenHeals = 250;

        // Regrowth settings
        private const float RegrowSpeed = 0.01f;
        private const int MaxRegrowingParts = 2;

        public static void Postfix(Pawn_HealthTracker __instance, Pawn ___pawn)
        {
            if (___pawn == null || ___pawn.Dead)
            {
                return;
            }

            Hediff hediff = ___pawn.health?.hediffSet?.GetFirstHediffOfDef(HediffDef.Named("DP_HealingFactor"));
            if (hediff == null)
            {
                return;
            }

            // Keep mood maxed and ensure the pawn is a psychopath
            if (___pawn.needs?.mood != null)
            {
                ___pawn.needs.mood.CurInstantLevel = 1f; // effectively +100 mood
            }
            if (___pawn.story?.traits != null && !___pawn.story.traits.HasTrait(TraitDefOf.Psychopath))
            {
                ___pawn.story.traits.GainTrait(new Trait(TraitDefOf.Psychopath));
            }

            if (Find.TickManager.TicksGame % TicksBetweenHeals != 0)
            {
                return;
            }

            float healAmount = BaseHealAmount * hediff.Severity;

            Hediff_Injury injury = ___pawn.health.hediffSet.hediffs
                .OfType<Hediff_Injury>()
                .Where(h => !h.IsPermanent() && h.CanHealNaturally() && h.Severity > 0)
                .OrderByDescending(h => h.Severity)
                .FirstOrDefault();

            if (injury != null)
            {
                injury.Heal(healAmount);
                if (Prefs.DevMode)
                {
                    Log.Message($"[DeadpoolsHealingFactor] Healed {injury.Label} on {___pawn.LabelShort} for {healAmount}.");
                }
            }

            // ---------- Limb regrowth logic ----------
            HediffDef regrowingDef = HediffDef.Named("DP_regrowing");

            var regrowingHediffs = ___pawn.health.hediffSet.hediffs
                .Where(h => h.def == regrowingDef)
                .ToList();

            foreach (var regrow in regrowingHediffs)
            {
                regrow.Severity += RegrowSpeed * hediff.Severity;
                if (regrow.Severity >= 1.0f)
                {
                    BodyPartRecord partToRestore = regrow.Part;
                    ___pawn.health.RemoveHediff(regrow);
                    ___pawn.health.RestorePart(partToRestore);
                    if (Prefs.DevMode)
                    {
                        Log.Message($"[DeadpoolsHealingFactor] Restored {partToRestore.Label} on {___pawn.LabelShort}.");
                    }
                }
            }
            
            if (regrowingHediffs.Count < MaxRegrowingParts)
            {
                // **THE FIX IS HERE:** Changed 'missing.parent' to 'missing.Part.parent'
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
