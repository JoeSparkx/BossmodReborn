using BossMod.Components;

namespace BossMod.Shadowbringers.Alliance.A11CommandModel;

sealed class ClangingBlow(BossModule module) : SingleTargetCast(module, (uint)AID.ClangingBlow, hint: "Tankbuster", damageType: AIHints.PredictedDamageType.Tankbuster);
sealed class ForcefulImpact(BossModule module) : RaidwideCast(module, (uint)AID.ForcefulImpact, hint: "Raidwide");
sealed class EnergyAssault(BossModule module) : SimpleAOEGroups(module,
[(uint)AID.EnergyAssault1, (uint)AID.EnergyAssault2], new AOEShapeCone(60f, 40f.Degrees()));
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

// Systematic Siege projectiles (SJSM2): show hit circle + predicted travel capsule based on motion delta.
// Remove immediately on impact (EnergyBomb event cast) so circles don't linger.
sealed class EnergyOrbs(BossModule module) : GenericAOEs(module, default, "Avoid energy orbs!")
{
    private const float Radius = 1.5f;
    private const float PredictLen = 8.0f;      // tune as desired
    private const float MinMoveForDir = 0.35f;  // don't trust direction until it moved

    private readonly Dictionary<ulong, WPos> _prevPos = [];
    private readonly HashSet<ulong> _remove = []; // actor instanceIDs to stop showing (impact)
    private readonly AOEShapeCircle _circle = new(Radius);
    private readonly AOEShapeCapsule _capsule = new(Radius, PredictLen);

    private IEnumerable<Actor> Sources()
    {
        foreach (var a in Module.Enemies((uint)OID.SJSM2))
        {
            // If we've seen it impact, stop drawing it even if the actor lingers client-side
            if (_remove.Contains(a.InstanceID))
                continue;

            // Still keep normal cleanup (helps when actors truly vanish)
            if (a.IsDestroyed || a.IsDead || a.EventState == 7)
                continue;

            yield return a;
        }
    }

    public override void Update()
    {
        // Track positions for currently relevant sources and purge stale cached entries.
        var alive = new HashSet<ulong>();
        foreach (var s in Sources())
        {
            alive.Add(s.InstanceID);
            if (!_prevPos.ContainsKey(s.InstanceID))
                _prevPos[s.InstanceID] = s.Position;
        }

        if (_prevPos.Count > 0)
        {
            var toRemove = new List<ulong>();
            foreach (var id in _prevPos.Keys)
                if (!alive.Contains(id))
                    toRemove.Add(id);
            foreach (var id in toRemove)
                _prevPos.Remove(id);
        }

        // Optional: if the actor truly despawned, also forget "remove" flags over time.
        // (Not strictly necessary; instance IDs won't be reused in a fight.)
    }

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var aoes = new List<AOEInstance>();

        foreach (var s in Sources())
        {
            // Always show the true danger circle
            aoes.Add(new(_circle, s.Position, default));

            // Predict direction from movement delta (not Rotation)
            if (_prevPos.TryGetValue(s.InstanceID, out var prev))
            {
                var delta = s.Position - prev;
                if (delta.Length() >= MinMoveForDir)
                {
                    var rot = Angle.FromDirection(delta.Normalized());
                    aoes.Add(new(_capsule, s.Position, rot));
                }

                _prevPos[s.InstanceID] = s.Position;
            }
            else
            {
                _prevPos[s.InstanceID] = s.Position;
            }
        }

        return CollectionsMarshal.AsSpan(aoes);
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        foreach (var s in Sources())
        {
            hints.AddForbiddenZone(new SDCircle(s.Position, Radius), DateTime.MaxValue);

            if (_prevPos.TryGetValue(s.InstanceID, out var prev))
            {
                var delta = s.Position - prev;
                if (delta.Length() >= MinMoveForDir)
                {
                    var rot = Angle.FromDirection(delta.Normalized());
                    hints.AddForbiddenZone(new SDCapsule(s.Position, rot, PredictLen, Radius), DateTime.MaxValue);
                }
            }
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        // Orbs "dissolve on impact": when they deal damage, stop showing that orb immediately.
        if ((AID)spell.Action.ID == AID.EnergyBomb && caster.OID == (uint)OID.SJSM2)
        {
            _remove.Add(caster.InstanceID);
            _prevPos.Remove(caster.InstanceID);
        }
    }
}

sealed class SidestrikingSpin(BossModule module) : SimpleAOEGroups(
    module,
    [(uint)AID.SidestrikingSpin2, (uint)AID.SidestrikingSpin3],
    new AOEShapeRect(30, 6),   // range 30, width 12; origin at "start" of rect
    expectedNumCasters: 2
);

sealed class SystematicTargeting(BossModule module) : BaitAwayIcon(
    module,
    new AOEShapeRect(70f, 2f),
    (uint)IconID.Icon164,
    (uint)AID.HighPoweredLaser,
    activationDelay: 2.5f,
    centerAtTarget: false,
    source: null
)
{
    public override Actor? BaitSource(Actor target)
        => Module.Enemies((uint)OID.SJSM1).FirstOrDefault() ?? Module.PrimaryActor;
}
sealed class SystematicSuppression(BossModule module) : SimpleAOEGroups(module, [(uint)AID.HighCaliberLaser1, (uint)AID.HighCaliberLaser2],
new AOEShapeRect(70, 24));

sealed class SystematicAirstrike(BossModule module) : GenericAOEs(module, default, "GTFO from airstrike!")
{
    private static readonly AOEShapeCircle _shape = new(5);
    private readonly List<AOEInstance> _aoes = [];

    // tune this: how long you want the warning circle to remain visible / considered dangerous for AI
    private const float PersistSeconds = 0.5f;

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor)
        => CollectionsMarshal.AsSpan(_aoes);

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.AirToSurfaceEnergy)
        {
            // event is instant; place circle at caster position (or spell.TargetXZ if you find that’s more accurate in this fight)
            var pos = caster.Position;
            var expire = WorldState.FutureTime(PersistSeconds);
            _aoes.Add(new(_shape, pos, default, expire));
        }
    }

    public override void Update()
    {
        // remove expired circles so they don't linger
        var now = WorldState.CurrentTime;
        for (int i = _aoes.Count - 1; i >= 0; --i)
            if (_aoes[i].Activation <= now)
                _aoes.RemoveAt(i);
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        // Use forbidden zones until expiry so AI actively avoids being under the adds when they fire.
        // If you prefer softer behavior, use TemporaryObstacles instead.
        foreach (ref readonly var a in ActiveAOEs(slot, actor))
            hints.AddForbiddenZone(new SDCircle(a.Origin, 5), a.Activation);
    }
}

[ModuleInfo(BossModuleInfo.Maturity.WIP, Contributors = "The Combat Reborn Team, JoeSparkx", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 700, NameID = 9141)] //Other service models 9142 and 9155, nonservice model 9923
public class A11CommandModel(WorldState ws, Actor primary) : BossModule(ws, primary, new(-500, -10), new ArenaBoundsSquare(20));