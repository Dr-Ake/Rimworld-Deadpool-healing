using UnityEngine;
using Verse;

namespace DeadpoolsHealingFactor
{
    public class DeadpoolsHealingFactorSettings : ModSettings
    {
        public bool enableHealing = true;
        public bool enableRegrowth = true;
        public bool boostMood = true;
        public bool forcePsychopath = true;
        public float baseHealAmount = 0.5f;
        public int ticksBetweenHeals = 250;
        public float regrowSpeed = 0.01f;
        public int maxRegrowingParts = 2;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref enableHealing, "enableHealing", true);
            Scribe_Values.Look(ref enableRegrowth, "enableRegrowth", true);
            Scribe_Values.Look(ref boostMood, "boostMood", true);
            Scribe_Values.Look(ref forcePsychopath, "forcePsychopath", true);
            Scribe_Values.Look(ref baseHealAmount, "baseHealAmount", 0.5f);
            Scribe_Values.Look(ref ticksBetweenHeals, "ticksBetweenHeals", 250);
            Scribe_Values.Look(ref regrowSpeed, "regrowSpeed", 0.01f);
            Scribe_Values.Look(ref maxRegrowingParts, "maxRegrowingParts", 2);
            base.ExposeData();
        }
    }

    public class DeadpoolsHealingFactorMod : Mod
    {
        public static DeadpoolsHealingFactorSettings settings;

        public DeadpoolsHealingFactorMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<DeadpoolsHealingFactorSettings>();
        }

        public override string SettingsCategory() => "Deadpool's Healing Factor";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard list = new Listing_Standard();
            list.Begin(inRect);

            list.CheckboxLabeled("Enable Healing", ref settings.enableHealing);
            list.CheckboxLabeled("Enable Regrowth", ref settings.enableRegrowth);
            list.CheckboxLabeled("Boost Mood", ref settings.boostMood);
            list.CheckboxLabeled("Force Psychopath", ref settings.forcePsychopath);

            list.Label($"Ticks Between Heals: {settings.ticksBetweenHeals}");
            settings.ticksBetweenHeals = (int)list.Slider(settings.ticksBetweenHeals, 60, 600);

            list.Label($"Base Heal Amount: {settings.baseHealAmount:F2}");
            settings.baseHealAmount = list.Slider(settings.baseHealAmount, 0f, 2f);

            list.Label($"Regrow Speed: {settings.regrowSpeed:F2}");
            settings.regrowSpeed = list.Slider(settings.regrowSpeed, 0f, 0.1f);

            list.Label($"Max Regrowing Parts: {settings.maxRegrowingParts}");
            settings.maxRegrowingParts = (int)list.Slider(settings.maxRegrowingParts, 0, 5);

            list.End();
            settings.Write();
        }
    }
}
