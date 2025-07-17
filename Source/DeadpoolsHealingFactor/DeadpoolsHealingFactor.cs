using System.Reflection;
using HarmonyLib;
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
        // The Postfix now takes the instance of the Pawn_HealthTracker
        public static void Postfix(Pawn_HealthTracker __instance)
        {
            // We need to get the pawn from the __instance
            Pawn pawn = __instance.pawn;
            if (pawn != null)
            {
                // Note: We can't get the specific amount healed here,
                // but we can confirm the tick is running for the pawn.
                Log.Message($"[DeadpoolsHealingFactor] HealthTick for {pawn.LabelShort}.");
            }
        }
    }
}
