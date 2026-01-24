using BossMod.Components;

namespace BossMod.Shadowbringers.Alliance.A12Hobbes;

sealed class LaserResistanceTest(BossModule module) : RaidwideCast(module, (uint)AID.LaserResistanceTest1, hint: "Raidwide");
sealed class WhirlingAssault(BossModule module) : SimpleAOEs(module, (uint)AID.WhirlingAssault, new AOEShapeRect(40f, 4f));
sealed class BalancedEdge(BossModule module) : SimpleAOEs(module, (uint)AID.BalancedEdge, new AOEShapeCircle(5f));
sealed class ElectromagneticPulse(BossModule module) : SimpleAOEs(module, (uint)AID.ElectromagneticPulse, new AOEShapeRect(40f, 5f));
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

sealed class ShockingDischarge(BossModule module) : SimpleAOEs(module, (uint)AID.ShockingDischarge, new AOEShapeCircle(5f));
sealed class Impact(BossModule module) : SimpleAOEs(module, (uint)AID.Impact, new AOEShapeCircle(18f));

sealed class UnwillingCargo(BossModule module) : GenericAOEs(module)
{
    // 40 length, 7 width => centered rect: front 20, back 20, halfWidth 3.5
    private static readonly AOEShapeRect Shape = new(20f, 3.5f, 20f);
    private readonly List<AOEInstance> _aoes = [];

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor) => CollectionsMarshal.AsSpan(_aoes);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID == (uint)AID.UnwillingCargo)
            _aoes.Add(new(Shape, caster.Position, caster.Rotation, Module.CastFinishAt(spell)));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID == (uint)AID.UnwillingCargo)
            _aoes.RemoveAll(a => a.Origin.AlmostEqual(caster.Position, 0.5f));
    }
}

sealed class SmallExploderTethers(BossModule module) : GenericBaitAway(module)
{
    private static readonly AOEShapeCircle Boom = new(5f);

    public override void OnTethered(Actor source, in ActorTetherInfo tether)
    {
        if (tether.ID != (uint)TetherID.Tether84 || (OID)source.OID != OID.SmallExploder)
            return;

        if (WorldState.Actors.Find(tether.Target) is not Actor target)
            return;

        CurrentBaits.RemoveAll(b => b.Source == source); // handle retethers cleanly
        CurrentBaits.Add(new(source, target, Boom, WorldState.FutureTime(30))); // placeholder; overwritten on cast start
    }

    public override void OnUntethered(Actor source, in ActorTetherInfo tether)
    {
        if (tether.ID == (uint)TetherID.Tether84 && (OID)source.OID == OID.SmallExploder)
            CurrentBaits.RemoveAll(b => b.Source == source);
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((OID)caster.OID == OID.SmallExploder && spell.Action.ID == (uint)AID.ConvenientSelfDestruction)
        {
            var t = Module.CastFinishAt(spell);
            for (int i = 0; i < CurrentBaits.Count; ++i)
                if (CurrentBaits[i].Source == caster)
                    CurrentBaits.Ref(i).Activation = t;
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (spell.Action.ID == (uint)AID.ConvenientSelfDestruction)
            CurrentBaits.RemoveAll(b => b.Source == caster);
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        base.AddHints(slot, actor, hints);

        int my = 0;
        var baits = CollectionsMarshal.AsSpan(CurrentBaits);
        for (int i = 0; i < baits.Length; ++i)
            if (baits[i].Target == actor)
                ++my;

        if (my > 1)
            hints.Add("More than one tether!");
    }
}

sealed class VariableCombatTest(BossModule module) : GenericAOEs(module)
{
    // Unknown cone angle. Start with 45° and adjust once you eyeball it in replay.
    // (If it feels too narrow/wide, change this number.)
    private static readonly AOEShapeCone Cone = new(20f, 45f.Degrees());
    private static readonly AOEShapeDonut Donut = new(10f, 19f); // inner unknown, outer ~19 per enum comment
    private static readonly AOEShapeCircle Small = new(2f);

    private readonly List<AOEInstance> _aoes = [];

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor) => CollectionsMarshal.AsSpan(_aoes);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        AOEShape? shape = spell.Action.ID switch
        {
            (uint)AID.VariableCombatTest2 or (uint)AID.VariableCombatTest6 => Cone,  // cone (slow + fast)
            (uint)AID.VariableCombatTest3 or (uint)AID.VariableCombatTest7 => Donut, // donut (slow + fast)
            (uint)AID.VariableCombatTest4 or (uint)AID.VariableCombatTest8 => Small, // 2y circle (slow + fast)
            _ => null
        };

        if (shape != null)
            _aoes.Add(new(shape, caster.Position, caster.Rotation, Module.CastFinishAt(spell)));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID is (uint)AID.VariableCombatTest2 or (uint)AID.VariableCombatTest6
            or (uint)AID.VariableCombatTest3 or (uint)AID.VariableCombatTest7
            or (uint)AID.VariableCombatTest4 or (uint)AID.VariableCombatTest8)
        {
            // remove the matching AOE for this caster (good enough even with multiple platforms/helpers)
            _aoes.RemoveAll(a => a.Origin.AlmostEqual(caster.Position, 0.5f));
        }
    }
}
sealed class VariableCombatRotationHint(BossModule module) : BossComponent(module)
{
    private int _dir; // -1 CCW, +1 CW, 0 unknown

    public override void OnEventIcon(Actor actor, uint iconID, ulong targetID)
    {
        // icon appears on Hobbes3 (the arm actor)
        if ((OID)actor.OID != OID.Hobbes3)
            return;

        if (iconID == (uint)IconID.RotateCW)
            _dir = +1;
        else if (iconID == (uint)IconID.RotateCCW)
            _dir = -1;
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (_dir == +1)
            hints.Add("Right Arm: rotates CW");
        else if (_dir == -1)
            hints.Add("Right Arm: rotates CCW");
    }
}

sealed class ConveyorBelts(BossModule module) : BossComponent(module)
{
    private DateTime _activeUntil;

    // choose something reasonable; if belts last exactly the SRM window, 8-10s is fine
    private static readonly float EdgeDangerR = 18.5f; // platform radius 20, warn before lethal edge

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID == (uint)AID.ElectromagneticPulse)
        {
            // belts active; refresh timer
            _activeUntil = WorldState.FutureTime(8.0);
        }
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (WorldState.CurrentTime > _activeUntil)
            return;

        // pick nearest platform center (you already have these)
        var c = NearestPlatformCenter(actor.Position);
        if ((actor.Position - c).Length() > EdgeDangerR)
            hints.Add("Conveyor active: EDGE IS LETHAL (move inward)!");
    }

    private WPos NearestPlatformCenter(WPos p)
    {
        // uses the centres you defined in A12Hobbes.cs
        var dB = (p - A12Hobbes.PlatBottom).LengthSq();
        var dR = (p - A12Hobbes.PlatRight).LengthSq();
        var dL = (p - A12Hobbes.PlatLeft).LengthSq();
        return dB < dR ? (dB < dL ? A12Hobbes.PlatBottom : A12Hobbes.PlatLeft)
                      : (dR < dL ? A12Hobbes.PlatRight : A12Hobbes.PlatLeft);
    }
}

sealed class OilDebuff(BossModule module) : BossComponent(module)
{
    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (actor.FindStatus((uint)SID.Oil) != null)
            hints.Add("Oiled: fire damage becomes lethal!");
    }
}

//////////
/// Module info / Arena Bounds
//////////
[ModuleInfo(BossModuleInfo.Maturity.WIP, Contributors = "The Combat Reborn Team, JoeSparkx", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 700, NameID = 9143)]
public class A12Hobbes(WorldState ws, Actor primary) : BossModule(ws, primary, MapCenter, TriplePlatformBounds)
{
    // map centre
    public static readonly WPos MapCenter = new(-805f, -240f);

    // Platform centres
    public static readonly WPos PlatBottom = new(-805f, -269f);
    public static readonly WPos PlatRight = new(-779f, -225f);
    public static readonly WPos PlatLeft = new(-831f, -225f);

    // edge points -> derived radius
    private const float PlatformR = 20f;

    private static readonly Shape[] Platforms =
    [
        new Circle(PlatBottom, PlatformR),
        new Circle(PlatRight,  PlatformR),
        new Circle(PlatLeft,   PlatformR),
    ];

    public static readonly ArenaBoundsCustom TriplePlatformBounds = new(Platforms, MapResolution: 0.5f);
}
