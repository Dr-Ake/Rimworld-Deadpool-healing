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
        // Access the private 'pawn' field by naming the parameter with three underscores
        public static void Postfix(Pawn ___pawn)
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
            if (!___pawn.IsHashIntervalTick(250))
            {
                return;
            }

            // Determine the amount to heal based on the hediff severity
            const float baseHeal = 1f;
            float healAmount = baseHeal * hediff.Severity;

            // Find the most severe healable injury
            Hediff_Injury injury = ___pawn.health.hediffSet.hediffs
                .OfType<Hediff_Injury>()
                .Where(h => !h.IsPermanent() && h.CanHealNaturally() && h.Severity > 0)
                .OrderByDescending(h => h.Severity)
                .FirstOrDefault();

            // Apply healing
            injury?.Heal(healAmount);
        }
    }
}
