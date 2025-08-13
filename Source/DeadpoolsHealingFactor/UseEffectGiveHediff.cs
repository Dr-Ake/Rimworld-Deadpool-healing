using System;
using RimWorld;
using Verse;

namespace DeadpoolsHealingFactor
{
    /// <summary>
    /// Comp properties for applying a specific hediff to the user when the item is used.
    /// </summary>
    public class CompProperties_UseEffectGiveHediffDP : CompProperties_UseEffect
    {
        public HediffDef hediffDef;
        public float severity = 1f;

        public CompProperties_UseEffectGiveHediffDP()
        {
            compClass = typeof(CompUseEffectGiveHediffDP);
        }
    }

    /// <summary>
    /// Applies the configured hediff to the user when the item is used via CompUsable.
    /// </summary>
    public class CompUseEffectGiveHediffDP : CompUseEffect
    {
        private CompProperties_UseEffectGiveHediffDP Props => (CompProperties_UseEffectGiveHediffDP)props;

        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);

            if (usedBy == null || usedBy.health == null || Props.hediffDef == null)
            {
                return;
            }

            // Only humanlikes by default; feels correct for a gene-like injector.
            if (usedBy.RaceProps?.Humanlike != true)
            {
                Messages.Message("Only humanlike pawns can use this injector.".Translate(), usedBy, MessageTypeDefOf.RejectInput, historical: false);
                return;
            }

            Hediff existing = usedBy.health.hediffSet.GetFirstHediffOfDef(Props.hediffDef);
            if (existing == null)
            {
                Hediff added = usedBy.health.AddHediff(Props.hediffDef);
                if (Props.severity > 0f)
                {
                    added.Severity = Math.Max(added.Severity, Props.severity);
                }
                Messages.Message("Injected: " + Props.hediffDef.LabelCap, usedBy, MessageTypeDefOf.PositiveEvent);
            }
            else
            {
                // Upgrade to at least desired severity; do not stack multiple separate hediffs
                if (Props.severity > 0f && existing.Severity < Props.severity)
                {
                    existing.Severity = Props.severity;
                }
                Messages.Message("Already has: " + Props.hediffDef.LabelCap, usedBy, MessageTypeDefOf.NeutralEvent);
            }
        }

        // Rely on DoEffect to perform eligibility checks to avoid API differences across game versions.
    }
}


