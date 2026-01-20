namespace BossMod.Modules.Dawntrail.Trial.T07Doomtrain;

[SkipLocalsInit]
sealed class DoomtrainStates : StateMachineBuilder
{
    public DoomtrainStates(BossModule module) : base(module)
    {
        DeathPhase(default, SinglePhase)
            .ActivateOnEnter<ArenaChanges>()
            .Raw.Update = () => Module.Enemies((uint)OID.Aether).Count != 0;
    }

    private void SinglePhase(uint id)
    {
        Car1(id, 8.2f);
        Car2(id + 0x10000u, 2.2f);
        Car3p1(id + 0x20000u, 5.8f);
        Intermission(id + 0x30000u, 10f);
        // Car3p2(id + 0x40000u, 2.2f);
        // Car4(id + 0x50000u, 2.2f);
        // Car5(id + 0x60000u, 2.2f);
        // Car6(id + 0x70000u, 2.2f);
        SimpleState(id + 0xFF0000u, 10000f, "???");
    }
    private void Car1(uint id, float delay)
    {
    }
    private void Car2(uint id, float delay)
    {
    }
    private void Car3p1(uint id, float delay)
    {
    }
    private void Intermission(uint id, float delay)
    {
    }
}