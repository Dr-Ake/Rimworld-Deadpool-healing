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

    [HarmonyPatch(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.Heal))]
    public static class Patch_Pawn_HealthTracker_Heal
    {
        public static void Postfix(Pawn ___pawn, Hediff_Injury injury, float amount)
        {
            Log.Message($"[DeadpoolsHealingFactor] {___pawn.LabelShort} healed {amount} damage.");
        }
    }
}
