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
        // Access the private 'pawn' field by naming the parameter with three underscores
        public static void Postfix(Pawn ___pawn)
        {
            if (___pawn != null)
            {
                Log.Message($"[DeadpoolsHealingFactor] HealthTick for {___pawn.LabelShort}.");
            }
        }
    }
}
