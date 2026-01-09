namespace BossMod.Stormblood.Raid.O1NAlteRoite;

sealed class Roar(BossModule module) : Components.RaidwideCast(module, (uint)AID.Roar);
sealed class ThinIce(BossModule module) : Components.ThinIce(module, 6f, true, (uint)SID.ThinIce, true);
sealed class Burn(BossModule module) : Components.SimpleAOEs(module, (uint)AID.Burn, new AOEShapeCircle(8f));
sealed class Levinbolt1(BossModule module) : Components.SimpleAOEs(module, (uint)AID.Levinbolt1, new AOEShapeCircle(6f));
[SkipLocalsInit]
sealed class BreathWing(BossModule module) : Components.SimpleKnockbacks(module, (uint)AID.BreathWing, 10f, kind: Kind.DirForward, stopAtWall: true);
sealed class WyrmTail(BossModule module) : Components.SingleTargetCast(module, (uint)AID.WyrmTail);
sealed class TwinBolt(BossModule module) : Components.SingleTargetCast(module, (uint)AID.TwinBolt);
sealed class Blaze(BossModule module) : Components.StackTogether(module, (uint)AID.Blaze, activationDelay: 5.0f, radius: 6f);
[SkipLocalsInit]
sealed class Levinbolt(BossModule module) : Components.SpreadFromCastTargets(module, (uint)AID.Levinbolt, 5f)
{
    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (IsSpreadTarget(actor))
        {
            hints.Add("Spread!");
        }
    }
}
[SkipLocalsInit]

sealed class Clamp(BossModule module) : Components.SimpleKnockbacks(module, (uint)AID.Clamp, 10f, shape: new AOEShapeRect(10f, 9f), kind: Kind.DirForward, stopAtWall: true);
   
sealed class Flame(BossModule module) : Components.RaidwideCast(module, (uint)AID.Flame);
sealed class Charybdis(BossModule module) : Components.RaidwideCast(module, (uint)AID.Charybdis);
   
   




[ModuleInfo(BossModuleInfo.Maturity.WIP,
ObjectIDType = typeof(OID),
ActionIDType = typeof(AID), // replace null with typeof(AID) if applicable
StatusIDType = typeof(SID), // replace null with typeof(SID) if applicable
TetherIDType = typeof(TetherID), // replace null with typeof(TetherID) if applicable
IconIDType = null, // replace null with typeof(IconID) if applicable
Contributors = "JoeSparkx",
GroupType = BossModuleInfo.GroupType.CFC,
GroupID = 252,
NameID = 5629)]
public class O1NAlteRoite(WorldState ws, Actor primary) : BossModule(ws, primary, new(00, 00), new ArenaBoundsCircle(20));



