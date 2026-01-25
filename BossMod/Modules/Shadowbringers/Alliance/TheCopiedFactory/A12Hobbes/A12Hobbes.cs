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
    private static float PlatformOuter => A12Hobbes.PlatformR;
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

// 3 stacks (one per alliance) typically.
sealed class LaserSight(BossModule module)
    : LineStack(module,
        iconID: (uint)IconID.Stackmarker,
        aidResolve: (uint)AID.LaserSight3,
        activationDelay: 0,          // because resolve is "no cast"; time off the boss cast instead if needed
        range: 65f,
        halfWidth: 4f,               // width 8
        minStackSize: 8,             // alliance stack; tune if needed
        maxStackSize: 8,
        maxCasts: 1,
        markerIsFinalTarget: false)  // IMPORTANT: LaserSight3 is helper->self, not helper->player
{
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {

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
    // Unknown cone angle. Start with 45° and adjust once I eyeball it in replay.
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
            // remove the matching AOE for this caster 
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
    private static readonly float EdgeDangerR = 18f; // platform radius 20, warn before lethal edge

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
        var c = NearestPlatformCenter(actor.Position);
        if ((actor.Position - c).Length() > EdgeDangerR)
            hints.Add("Conveyor active: EDGE IS LETHAL (move inward)!");
    }

    private WPos NearestPlatformCenter(WPos p)
    {
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

[ModuleInfo(BossModuleInfo.Maturity.WIP, Contributors = "The Combat Reborn Team, JoeSparkx", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 700, NameID = 9143)]
public class A12Hobbes(WorldState ws, Actor primary) : BossModule(ws, primary, _arena.center, _arena.arena)
{
    public static readonly WPos MapCenter = new(-804.308f, -240.519f);

    public static readonly WPos PlatBottom = new(-805.071f, -269.977f);
    public static readonly WPos PlatRight = new(-778.953f, -224.976f);
    public static readonly WPos PlatLeft = new(-831.119f, -225.306f);

    public const float PlatformR = 19.5f;

    private static readonly (WPos center, ArenaBoundsCustom arena) _arena = BuildArena();

    private static (WPos center, ArenaBoundsCustom arena) BuildArena()
    {
        var platforms = new Shape[]
        {
            new Circle(PlatBottom, PlatformR),
            new Circle(PlatRight,  PlatformR),
            new Circle(PlatLeft,   PlatformR),
        };

        var arena = new ArenaBoundsCustom(platforms, MapResolution: 0.25f);

        return (arena.Center, arena);
    }
}