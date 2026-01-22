namespace BossMod.Shadowbringers.Alliance.A11CommandModel;

class A11CommandModelStates : StateMachineBuilder
{
    public A11CommandModelStates(BossModule module) : base(module)
    {
        TrivialPhase()
        .ActivateOnEnter<ForcefulImpact>()
        .ActivateOnEnter<EnergyAssault>()
        .ActivateOnEnter<ClangingBlow>()
        .ActivateOnEnter<EnergyRing>()
        .ActivateOnEnter<CentrifugalSpin>()
        .ActivateOnEnter<Shockwave>()
        .ActivateOnEnter<EnergyBombardment>()
;
    }
}
