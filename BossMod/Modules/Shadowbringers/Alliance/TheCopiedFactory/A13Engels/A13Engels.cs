namespace BossMod.Shadowbringers.Alliance.A13Engels;

////////////////////////////////////////////
/// Arena Changes
///////////////////////////////////////////
class DemolishStructureArenaChange(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeRect square = new(5f, 5f, 5f, invertForbiddenZone: true);
    private AOEInstance[] _aoe = [];

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor) => _aoe;

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID == (uint)AID.DemolishStructure2 && Arena.Bounds == A13MarxEngels.StartingBounds)
        {
            _aoe = [new(square, A13MarxEngels.TransitionSpot, color: Colors.SafeFromAOE)];
        }
    }

    public override void OnMapEffect(byte index, uint state)
    {
        if (index == 0x0B && state == 0x00020001u)
        {
            Arena.Center = A13MarxEngels.SecondArenaCenter;
            Arena.Bounds = A13MarxEngels.StartingBounds;
            _aoe = [];
        }
    }
}
////////////////////////////////////////////
/// MarxSmash + MarxCrush
///////////////////////////////////////////

// Replaces MarxSmash1..7 + MarxCrush SimpleAOEs.
// Shows telegraphs from boss "visual" casts (earlier), then clears when the real smash/crush event happens.
sealed class MarxSmashAndCrushTelegraphs(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeRect Smash12 = new(60f, 15f); // width 30
    private static readonly AOEShapeRect Smash3 = new(60f, 15f); // width 30
    private static readonly AOEShapeRect Smash4 = new(30f, 30f); // width 60
    private static readonly AOEShapeRect Smash5 = new(35f, 30f); // width 60
    private static readonly AOEShapeRect Smash67 = new(60f, 10f); // width 20
    private static readonly AOEShapeRect Crush = new(15f, 15f); // width 30

    private readonly List<AOEInstance> _aoes = [];

    // We "tag" instances via ActorID so we can update/remove them reliably:
    // upper 32 bits = real AID, lower 32 bits = index (0/1 for multi-aoes)
    private static ulong Tag(uint realAID, uint index = 0) => ((ulong)realAID << 32) | index;

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor) => CollectionsMarshal.AsSpan(_aoes);

    private void AddOrUpdate(uint realAID, uint index, AOEShapeRect shape, WPos origin, Angle rot, DateTime activation)
    {
        var tag = Tag(realAID, index);
        for (int i = 0; i < _aoes.Count; ++i)
        {
            if (_aoes[i].ActorID == tag)
            {
                _aoes[i] = new(shape, origin, rot, activation, actorID: tag, shapeDistance: shape.Distance(origin, rot));
                return;
            }
        }
        _aoes.Add(new(shape, origin, rot, activation, actorID: tag, shapeDistance: shape.Distance(origin, rot)));
    }

    private void RemoveAll(uint realAID)
    {
        var hi = (ulong)realAID << 32;
        _aoes.RemoveAll(a => (a.ActorID & 0xFFFFFFFF00000000UL) == hi);
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        // Early warning: boss visual casts (these are the ones that start ~6s earlier in your log).
        var activation = Module.CastFinishAt(spell);

        switch ((AID)spell.Action.ID)
        {
            case AID.MarxSmashVisual1:
                AddOrUpdate((uint)AID.MarxSmash1, 0, Smash12, caster.Position, caster.Rotation, activation);
                break;

            case AID.MarxSmashVisual2:
                AddOrUpdate((uint)AID.MarxSmash2, 0, Smash12, caster.Position, caster.Rotation, activation);
                break;

            case AID.MarxSmashVisual3:
                // visual3 corresponds to smash4 in your enums/log flow
                AddOrUpdate((uint)AID.MarxSmash4, 0, Smash4, caster.Position, caster.Rotation, activation);
                break;

            case AID.MarxSmashVisual5:
                AddOrUpdate((uint)AID.MarxSmash5, 0, Smash5, caster.Position, caster.Rotation, activation);
                break;

            case AID.MarxCrushVisual:
                {
                    var c = Arena.Center;
                    AddOrUpdate((uint)AID.MarxCrush, 0, Crush, new WPos(c.X - 30f, c.Z), caster.Rotation, activation);
                    AddOrUpdate((uint)AID.MarxCrush, 1, Crush, new WPos(c.X + 30f, c.Z), caster.Rotation, activation);
                    break;
                }

            case AID.MarxSmash1:
                AddOrUpdate((uint)AID.MarxSmash1, 0, Smash12, caster.Position, caster.Rotation, activation);
                break;
            case AID.MarxSmash2:
                AddOrUpdate((uint)AID.MarxSmash2, 0, Smash12, caster.Position, caster.Rotation, activation);
                break;
            case AID.MarxSmash3:
                AddOrUpdate((uint)AID.MarxSmash3, 0, Smash3, caster.Position, caster.Rotation, activation);
                break;
            case AID.MarxSmash4:
                AddOrUpdate((uint)AID.MarxSmash4, 0, Smash4, caster.Position, caster.Rotation, activation);
                break;
            case AID.MarxSmash5:
                AddOrUpdate((uint)AID.MarxSmash5, 0, Smash5, caster.Position, caster.Rotation, activation);
                break;
            case AID.MarxSmash6:
                AddOrUpdate((uint)AID.MarxSmash6, 0, Smash67, caster.Position, caster.Rotation, activation);
                break;
            case AID.MarxSmash7:
                AddOrUpdate((uint)AID.MarxSmash7, 0, Smash67, caster.Position, caster.Rotation, activation);
                break;
            case AID.MarxCrush:
                AddOrUpdate((uint)AID.MarxCrush, 0, Crush, caster.Position, caster.Rotation, activation);
                break;
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.MarxSmashVisual4)
        {
            var t = WorldState.FutureTime(0.5f);
            AddOrUpdate((uint)AID.MarxSmash3, 0, Smash3, caster.Position, caster.Rotation, t);
            return;
        }

        if ((AID)spell.Action.ID == AID.MarxSmashVisual6)
        {
            var c = Arena.Center;
            var t = WorldState.FutureTime(0.5f);
            AddOrUpdate((uint)AID.MarxSmash6, 0, Smash67, new WPos(c.X - 10f, c.Z), caster.Rotation, t);
            AddOrUpdate((uint)AID.MarxSmash7, 1, Smash67, new WPos(c.X + 10f, c.Z), caster.Rotation, t);
            return;
        }

        // Clear once the REAL damage event happens.
        switch ((AID)spell.Action.ID)
        {
            case AID.MarxSmash1:
            case AID.MarxSmash2:
            case AID.MarxSmash3:
            case AID.MarxSmash4:
            case AID.MarxSmash5:
            case AID.MarxSmash6:
            case AID.MarxSmash7:
            case AID.MarxCrush:
                RemoveAll(spell.Action.ID);
                break;
        }
    }
}
////////////////////////////////////////////
/// Rest of stuff
///////////////////////////////////////////

class PrecisionGuidedMissile2(BossModule module) : Components.SpreadFromCastTargets(module, (uint)AID.PrecisionGuidedMissile2, 6);
class LaserSight1(BossModule module) : Components.SimpleAOEs(module, (uint)AID.LaserSight1, new AOEShapeRect(100, 10));
class GuidedMissile2(BossModule module) : Components.SimpleAOEs(module, (uint)AID.GuidedMissile2, 6);
class IncendiaryBombing2(BossModule module) : Components.SimpleAOEs(module, (uint)AID.IncendiaryBombing2, 8);
class IncendiaryBombing1(BossModule module) : Components.SpreadFromIcon(module, (uint)IconID.Spreadmarker, (uint)AID.IncendiaryBombing1, 8, 5);
class DiffuseLaser(BossModule module) : Components.RaidwideCast(module, (uint)AID.DiffuseLaser);
class SurfaceMissile2(BossModule module) : Components.SimpleAOEs(module, (uint)AID.SurfaceMissile2, 6);

class GuidedMissile(BossModule module) : Components.StandardChasingAOEs(module, 6f, (uint)AID.GuidedMissile2, (uint)AID.GuidedMissile3, 5.5f, 1d, 4, true, (uint)IconID.GuidedMissile);

[ModuleInfo(BossModuleInfo.Maturity.WIP, Contributors = "The Combat Reborn Team, JoeSparkx", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 700, NameID = 9147)]
public class A13MarxEngels(WorldState ws, Actor primary) : BossModule(ws, primary, StartingArenaCenter, StartingBounds)
{
    public static readonly WPos TransitionSpot = new(900, 697);
    public static readonly WPos StartingArenaCenter = new(900, 670);
    public static readonly WPos SecondArenaCenter = new(900, 785);
    public static readonly ArenaBoundsSquare StartingBounds = new(30);
}
