namespace BossMod.Shadowbringers.Alliance.A11CommandModel;

class A11CommandModelStates : StateMachineBuilder
{
    public A11CommandModelStates(BossModule module) : base(module)
    {
        TrivialPhase()
        .ActivateOnEnter<ForcefulImpact>()
        .ActivateOnEnter<EnergyAssault>()
        .ActivateOnEnter<ClangingBlow>();
    }
}
