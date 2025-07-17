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

    // We are changing the target method to "HealthTick"
    [HarmonyPatch(typeof(Pawn_HealthTracker), "HealthTick")]
    public static class Patch_Pawn_HealthTracker_HealthTick
    {
        // Healing factor settings
        private const float BaseHealAmount = 0.5f;
        private const int TicksBetweenHeals = 250;

        // Regrowth settings (could be replaced by user-configurable settings)
        private const float RegrowSpeed = 0.01f;
        private const int MaxRegrowingParts = 2;

        // Access the private 'pawn' field by naming the parameter with three underscores
        public static void Postfix(Pawn_HealthTracker __instance, Pawn ___pawn)
        {
            // Safety first: make sure the pawn exists and isn't dead
            if (___pawn == null || ___pawn.Dead)
            {
                return;
            }

            // Check if the pawn has the healing factor hediff
            Hediff hediff = ___pawn.health?.hediffSet?.GetFirstHediffOfDef(HediffDef.Named("DP_HealingFactor"));
            if (hediff == null)
            {
                return;
            }

            // Run the healing logic periodically
            if (Find.TickManager.TicksGame % TicksBetweenHeals != 0)
            {
                return;
            }

            // Determine the amount to heal based on the hediff severity
            float healAmount = BaseHealAmount * hediff.Severity;

            // Find the most severe healable injury
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

            // Advance existing regrowth
            var regrowingParts = ___pawn.health.hediffSet.hediffs
                .Where(h => h.def == regrowingDef)
                .ToList();

            foreach (var regrow in regrowingParts.ToList())
            {
                regrow.Severity += RegrowSpeed * hediff.Severity;
                if (regrow.Severity >= 1.0f)
                {
                    BodyPartRecord part = regrow.Part;
                    ___pawn.health.RemoveHediff(regrow);
                    ___pawn.health.RestorePart(part);
                    if (Prefs.DevMode)
                    {
                        Log.Message($"[DeadpoolsHealingFactor] Restored {part.Label} on {___pawn.LabelShort}.");
                    }
                }
            }

            // Start new regrowth if below limit
            if (regrowingParts.Count < MaxRegrowingParts)
            {
                var missingParts = ___pawn.health.hediffSet.GetMissingPartsCommonAncestors();
                foreach (var missing in missingParts)
                {
                    BodyPartRecord parent = missing.parent;
                    if (parent != null && ___pawn.health.hediffSet.GetFirstHediffOfDef(regrowingDef, parent) == null)
                    {
                        // Skip if parent already has regrowing hediff
                        BodyPartRecord attachPart = parent;
                        while (attachPart != null && ___pawn.health.hediffSet.PartIsMissing(attachPart))
                        {
                            attachPart = attachPart.parent;
                        }
                        if (attachPart != null)
                        {
                            var newHediff = HediffMaker.MakeHediff(regrowingDef, ___pawn, attachPart);
                            newHediff.Severity = 0f;
                            ___pawn.health.AddHediff(newHediff);
                            if (Prefs.DevMode)
                            {
                                Log.Message($"[DeadpoolsHealingFactor] Started regrowing {missing.Label} on {___pawn.LabelShort}.");
                            }
                        }
                        break;
                    }
                }
            }
        }
    }
}
