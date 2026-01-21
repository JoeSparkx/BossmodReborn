namespace BossMod.Shadowbringers.Alliance.A11CommandModel;

sealed class ClangingBlow(BossModule module) : Components.SingleTargetCast(module, (uint)AID.ClangingBlow, hint: "Tankbuster", damageType: AIHints.PredictedDamageType.Tankbuster);
sealed class ForcefulImpact(BossModule module) : Components.RaidwideCast(module, (uint)AID.ForcefulImpact, hint: "Raidwide");
sealed class EnergyAssault(BossModule module) : Components.SimpleAOEGroups(module, 
[(uint)AID.EnergyAssault1, (uint)AID.EnergyAssault2], new AOEShapeCone(60f, 30f.Degrees()));

[ModuleInfo(BossModuleInfo.Maturity.WIP, Contributors = "The Combat Reborn Team, JoeSparkx", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 700, NameID = 9141)] //Other service models 9142 and 9155, nonservice model 9923
public class A11CommandModel(WorldState ws, Actor primary) : BossModule(ws, primary, new(-500, -10), new ArenaBoundsSquare(20));