namespace BossMod.Shadowbringers.Alliance.A12Hobbes;

class A12HobbesStates : StateMachineBuilder
{
    public A12HobbesStates(BossModule module) : base(module)
    {
        TrivialPhase()
        .ActivateOnEnter<LaserResistanceTest>()
        .ActivateOnEnter<RingLaser>()
        .ActivateOnEnter<LaserSight>()
        .ActivateOnEnter<ShortRangeMissile>()
        .ActivateOnEnter<ShockingDischarge>()
        .ActivateOnEnter<Impact>()
        .ActivateOnEnter<VariableCombatTest>()
        .ActivateOnEnter<VariableCombatRotationHint>()
        .ActivateOnEnter<UnwillingCargo>()
        .ActivateOnEnter<SmallExploderTethers>()
        .ActivateOnEnter<ConveyorBelts>()
        .ActivateOnEnter<OilDebuff>()
        .ActivateOnEnter<WhirlingAssault>()
        .ActivateOnEnter<BalancedEdge>()
        .ActivateOnEnter<ElectromagneticPulse>();
    }
}
