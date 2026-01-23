namespace BossMod.Shadowbringers.Alliance.A12Hobbes;

class A12HobbesStates : StateMachineBuilder
{
    public A12HobbesStates(BossModule module) : base(module)
    {
        TrivialPhase()
        .ActivateOnEnter<LaserResistanceTest>()
        .ActivateOnEnter<RingLaser>()
        .ActivateOnEnter<LaserSight>()
        .ActivateOnEnter<ShortRangeMissile>();
    }
}
