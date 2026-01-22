using BossMod.Components;

namespace BossMod.Shadowbringers.Alliance.A11CommandModel;

sealed class ClangingBlow(BossModule module) : SingleTargetCast(module, (uint)AID.ClangingBlow, hint: "Tankbuster", damageType: AIHints.PredictedDamageType.Tankbuster);
sealed class ForcefulImpact(BossModule module) : RaidwideCast(module, (uint)AID.ForcefulImpact, hint: "Raidwide");
sealed class EnergyAssault(BossModule module) : SimpleAOEGroups(module,
[(uint)AID.EnergyAssault1, (uint)AID.EnergyAssault2], new AOEShapeCone(60f, 30f.Degrees()));
sealed class EnergyBombardment(BossModule module) : SimpleAOEs(module, (uint)AID.EnergyBombardment2, new AOEShapeCircle(4f));
sealed class EnergyRing(BossModule module) : ConcentricAOEs(module,
[
    new AOEShapeCircle(12),
    new AOEShapeDonut(12, 24),
    new AOEShapeDonut(24, 36),
    new AOEShapeDonut(36, 48),
])
{
    private WPos _origin;
    private bool _haveSeq;

    private DateTime _t2, _t4, _t6, _t8;

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        // helper timings (corrected for NPC cast reporting)
        var finish = WorldState.CurrentTime.AddSeconds(spell.NPCRemainingTime);

        switch ((AID)spell.Action.ID)
        {
            case AID.EnergyRing2:
                _t2 = finish;
                _origin = caster.Position;
                EnsureSequence();
                // if sequence already exists, make sure the current (circle) activation is correct
                if (Sequences.Count > 0)
                {
                    var s = Sequences[0];
                    s.Origin = _origin;
                    s.NextActivation = _t2;
                    Sequences[0] = s;
                }
                break;

            case AID.EnergyRing4:
                _t4 = finish;
                _origin = caster.Position;
                EnsureSequence();
                break;

            case AID.EnergyRing6:
                _t6 = finish;
                _origin = caster.Position;
                EnsureSequence();
                break;

            case AID.EnergyRing8:
                _t8 = finish;
                _origin = caster.Position;
                EnsureSequence();
                break;
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.EnergyRing2:
                // circle just went off -> show donut 12-24 until its finish time
                AdvanceSequence(0, _origin, _t4 != default ? _t4 : WorldState.CurrentTime);
                break;

            case AID.EnergyRing4:
                // donut 12-24 went off -> show donut 24-36
                AdvanceSequence(1, _origin, _t6 != default ? _t6 : WorldState.CurrentTime);
                break;

            case AID.EnergyRing6:
                // donut 24-36 went off -> show donut 36-48
                AdvanceSequence(2, _origin, _t8 != default ? _t8 : WorldState.CurrentTime);
                break;

            case AID.EnergyRing8:
                // final donut went off -> remove sequence
                AdvanceSequence(3, _origin);
                _haveSeq = false;
                break;
        }
    }

    private void EnsureSequence()
    {
        if (_haveSeq)
            return;

        // If we haven't seen EnergyRing2 start yet, still create the sequence so later resolves can progress it.
        // We'll patch activation to _t2 when we see it.
        AddSequence(_origin, _t2 != default ? _t2 : default);
        _haveSeq = true;
    }
}

sealed class CentrifugalSpin(BossModule module) :
SimpleAOEs(module,
(uint)AID.CentrifugalSpin2,
shape: new AOEShapeRect(30f + module.PrimaryActor.HitboxRadius, 4f, 15f));

sealed class Shockwave(BossModule module) :
SimpleKnockbacks(module,
(uint)AID.Shockwave,
distance: 15f,
ignoreImmunes: true,
kind: Kind.AwayFromOrigin,
stopAtWall: false);

sealed class EnergyOrbs(BossModule module) : Voidzone(module, 1.5f, GetEnergyBombs, 2.5f)
{
    private static Actor[] GetEnergyBombs(BossModule module)
    {
        var enemies = module.Enemies((uint)OID.SJSM2);
        var count = enemies.Count;
        var index = 0;
        var SJSM2 = new Actor[count];
        for (var i = 0; i < count; i++)
        {
            var z = enemies[i];
            if (z.EventState != 7)
            {
                SJSM2[index++] = z;
            }
        }
        return SJSM2;

    }
}

sealed class SidestrikingSpin(BossModule module) : Components.SimpleAOEGroups(
    module,
    [(uint)AID.SidestrikingSpin2, (uint)AID.SidestrikingSpin3],
    new AOEShapeRect(6, 15, 6),   // total length 30, width 12
    expectedNumCasters: 2
);

[ModuleInfo(BossModuleInfo.Maturity.WIP, Contributors = "The Combat Reborn Team, JoeSparkx", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 700, NameID = 9141)] //Other service models 9142 and 9155, nonservice model 9923
public class A11CommandModel(WorldState ws, Actor primary) : BossModule(ws, primary, new(-500, -10), new ArenaBoundsSquare(20));