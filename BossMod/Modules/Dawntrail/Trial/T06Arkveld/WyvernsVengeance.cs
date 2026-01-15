namespace BossMod.Dawntrail.Trial.T06Arkveld;

sealed class WyvernsVengeance(BossModule module) : Components.Exaflare(module, 6f)
{
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID == (uint)AID.WyvernsVengeance)
        {
            // distance/time/count: these are the only “guessy” parts
            // distance: 8f matches the Extreme pattern; log shows steps roughly consistent with that
            // timeToMove: your log shows ~1.6s cadence between 43861 events
            Lines.Add(new(
                caster.Position,
                8f * spell.Rotation.Round(1f).ToDirection(),
                Module.CastFinishAt(spell),
                timeToMove: 1.6d,
                explosionsLeft: 6,       // adjust once you confirm exact count in Normal
                maxShownExplosions: 6
            ));
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (spell.Action.ID == (uint)AID.WyvernsVengeance1)
        {
            var count = Lines.Count;
            var loc = spell.TargetXZ;

            for (var i = 0; i < count; ++i)
            {
                var line = Lines[i];
                if (line.Next.AlmostEqual(loc, 1f))
                {
                    AdvanceLine(line, loc);
                    if (line.ExplosionsLeft == 0)
                        Lines.RemoveAt(i);
                    return;
                }
            }
        }
    }
}
