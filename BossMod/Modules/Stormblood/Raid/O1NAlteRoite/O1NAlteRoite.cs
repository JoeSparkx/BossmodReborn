using BossMod.Components;

namespace BossMod.Stormblood.Raid.O1NAlteRoite;

// Working
sealed class Roar(BossModule module) : Components.RaidwideCast(module, (uint)AID.Roar);
sealed class ThinIce(BossModule module) : Components.ThinIce(module, 6f, true, (uint)SID.ThinIce, true);
sealed class Charybdis(BossModule module) : Components.RaidwideCast(module, (uint)AID.Charybdis);
sealed class Burn(BossModule module) : Components.SimpleAOEs(
    module,
    (uint)AID.Burn1,
    new AOEShapeCircle(8f));
// Twinbolt Stuff
sealed class TwinBoltTetheredBuster(BossModule module)
    : Components.BaitAwayTethers(
        module,
        shape: new AOEShapeCircle(6),          // TODO radius
        tetherID: (uint)TetherID.TwinBolt,     // TODO correct tether id
        aid: (uint)AID.TwinBolt1                // TODO correct action id
      )
{ }
// Fire Orbs Burn
sealed class FireOrbBurnFast(BossModule module)
    : Components.SimpleAOEs(module, (uint)AID.Burn, new AOEShapeCircle(8f));

sealed class FireOrbBurnSlow(BossModule module)
    : Components.SimpleAOEs(module, (uint)AID.Burn1, new AOEShapeCircle(8f));

[ModuleInfo(BossModuleInfo.Maturity.WIP,
ObjectIDType = typeof(OID),
ActionIDType = typeof(AID), // replace null with typeof(AID) if applicable
StatusIDType = typeof(SID), // replace null with typeof(SID) if applicable
TetherIDType = typeof(TetherID), // replace null with typeof(TetherID) if applicable
IconIDType = typeof(IconID), // replace null with typeof(IconID) if applicable
Contributors = "JoeSparkx",
GroupType = BossModuleInfo.GroupType.CFC,
GroupID = 252,
NameID = 5629)]
public class O1NAlteRoite(WorldState ws, Actor primary) : BossModule(ws, primary, new(00, 00), new ArenaBoundsCircle(20));
