using BossMod.Components;

namespace BossMod.Shadowbringers.Alliance.A12Hobbes;

sealed class LaserResistanceTest(BossModule module) : RaidwideCast(module, (uint)AID.LaserResistanceTest1, hint: "Raidwide");

sealed class RingLaser(BossModule module) : GenericAOEs(module)
{
    // Tune these once you confirm exact radii in-game.
    // Based on txt: rings start at platform edge and step inward.
    // With your measured platform radius ~19.49, the first pulse is basically "15 -> edge".
    private const float PlatformOuter = 19.49f; // your measured platform edge
    private static readonly Dictionary<uint, AOEShape> Shapes = new()
    {
        // outer ring (edge unsafe)
        [(uint)AID.RingLaser3] = new AOEShapeDonut(15f, PlatformOuter),

        // next ring inward
        [(uint)AID.RingLaser5] = new AOEShapeDonut(10f, 15f),

        // next ring inward (leaves only the very centre safe if this is correct)
        [(uint)AID.RingLaser6] = new AOEShapeDonut(5f, 10f),
    };

    private readonly List<AOEInstance> _aoes = [];

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor) => CollectionsMarshal.AsSpan(_aoes);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (Shapes.TryGetValue(spell.Action.ID, out var shape))
            _aoes.Add(new(shape, caster.Position, caster.Rotation, Module.CastFinishAt(spell)));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (Shapes.ContainsKey(spell.Action.ID))
            _aoes.RemoveAll(a => a.Origin.AlmostEqual(caster.Position, 0.5f));
    }
}

// 3 stacks (one per alliance) typically, but you can leave maxCasts wide open.
sealed class LaserSight(BossModule module)
    : LineStack(module,
        iconID: (uint)IconID.Stackmarker,
        aidResolve: (uint)AID.LaserSight3,
        activationDelay: 0,          // because resolve is "no cast"; we’ll time off the boss cast instead if needed
        range: 65f,
        halfWidth: 4f,               // width 8
        minStackSize: 8,             // alliance stack; tune if needed
        maxStackSize: 8,
        maxCasts: 1,
        markerIsFinalTarget: false)  // IMPORTANT: LaserSight3 is helper->self, not helper->player
{
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        // If you want the warning timing to align with the 8s boss cast:
        // when LaserSight2 starts, shift all existing baits to resolve at its finish time.
        if (spell.Action.ID == (uint)AID.LaserSight2 && CurrentBaits.Count > 0)
        {
            var t = Module.CastFinishAt(spell);
            var baits = CollectionsMarshal.AsSpan(CurrentBaits);
            for (int i = 0; i < baits.Length; ++i)
                baits[i].Activation = t;
        }
    }
}

sealed class ShortRangeMissile(BossModule module)
    : SimpleAOEs(module, (uint)AID.ShortRangeMissile2, new AOEShapeCircle(8f));

[ModuleInfo(BossModuleInfo.Maturity.WIP, Contributors = "The Combat Reborn Team, JoeSparkx", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 700, NameID = 9143)]
public class A12Hobbes(WorldState ws, Actor primary) : BossModule(ws, primary, MapCenter, TriplePlatformBounds)
{
    // Holy Circle (map centre)
    public static readonly WPos MapCenter = new(-805f, -240f);

    // Prominence (platform centres)
    public static readonly WPos PlatBottom = new(-805f, -269f);
    public static readonly WPos PlatRight = new(-779f, -225f);
    public static readonly WPos PlatLeft = new(-831f, -225f);

    // Total Eclipse (edge points) -> derived radius
    private const float PlatformR = 20f;

    private static readonly Shape[] Platforms =
    [
        new Circle(PlatBottom, PlatformR),
        new Circle(PlatRight,  PlatformR),
        new Circle(PlatLeft,   PlatformR),
    ];

    public static readonly ArenaBoundsCustom TriplePlatformBounds = new(Platforms, MapResolution: 0.5f);
}
