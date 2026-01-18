namespace BossMod.Modules.Dawntrail.Trial.T07Doomtrain;





[ModuleInfo(BossModuleInfo.Maturity.WIP,
StatesType = typeof(DoomtrainStates),
ConfigType = null, // replace null with typeof(DoomtrainConfig) if applicable
ObjectIDType = typeof(OID),
ActionIDType = typeof(AID), // replace null with typeof(AID) if applicable
StatusIDType = typeof(SID), // replace null with typeof(SID) if applicable
TetherIDType = null, // replace null with typeof(TetherID) if applicable
IconIDType = typeof(IconID), // replace null with typeof(IconID) if applicable
PrimaryActorOID = (uint)OID.Doomtrain,
Contributors = "",
Expansion = BossModuleInfo.Expansion.Dawntrail,
Category = BossModuleInfo.Category.Trial,
GroupType = BossModuleInfo.GroupType.CFC,
GroupID = 1076u,
NameID = 14284u,
SortOrder = 1,
PlanLevel = 0)]
[SkipLocalsInit]
public sealed class Doomtrain : BossModule
{
    public Doomtrain(WorldState ws, Actor primary) : this(ws, primary, BuildArena()) { }

    private Doomtrain(WorldState ws, Actor primary, (WPos center, ArenaBoundsCustom arena) a) : base(ws, primary, a.center, a.arena) { }

    private static (WPos center, ArenaBoundsCustom arena) BuildArena()
    {
        var arena = new ArenaBoundsCustom([new Rectangle(new(100f, 100f), 10.5f, 15f)],
        [new Square(new(102.5f, 97.5f), 2.01f), new Square(new(97.5f, 107.5f), 2.01f)], AdjustForHitboxInwards: true);
        return (arena.Center, arena);
    }
}
