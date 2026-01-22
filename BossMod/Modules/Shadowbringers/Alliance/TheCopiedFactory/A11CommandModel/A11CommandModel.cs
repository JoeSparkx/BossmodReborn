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
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        var activation = WorldState.CurrentTime.AddSeconds(spell.NPCRemainingTime);

        switch ((AID)spell.Action.ID)
        {
            case AID.EnergyRing2:
                AddSequence(caster.Position, activation);
                break;

            case AID.EnergyRing4:
                AdvanceSequence(0, caster.Position, activation);
                break;

            case AID.EnergyRing6:
                AdvanceSequence(1, caster.Position, activation);
                break;

            case AID.EnergyRing8:
                AdvanceSequence(2, caster.Position, activation);
                break;
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.EnergyRing8)
            AdvanceSequence(3, caster.Position);
    }
}
sealed class CentrifugalSpin(BossModule module) :
SimpleKnockbacks(module,
(uint)AID.CentrifugalSpin2,
distance: 5f,
ignoreImmunes: true,
shape: new AOEShapeRect(30f + module.PrimaryActor.HitboxRadius, 8f),
kind: Kind.AwayFromOrigin);
sealed class Shockwave(BossModule module) :
SimpleKnockbacks(module,
(uint)AID.Shockwave,
distance: 20f,
ignoreImmunes: true,
kind: Kind.AwayFromOrigin);

[ModuleInfo(BossModuleInfo.Maturity.WIP, Contributors = "The Combat Reborn Team, JoeSparkx", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 700, NameID = 9141)] //Other service models 9142 and 9155, nonservice model 9923
public class A11CommandModel(WorldState ws, Actor primary) : BossModule(ws, primary, new(-500, -10), new ArenaBoundsSquare(20));