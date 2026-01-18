namespace BossMod.Modules.Dawntrail.Trial.T07Doomtrain;

[SkipLocalsInit]
sealed class DoomtrainStates : StateMachineBuilder
{
    public DoomtrainStates(BossModule module) : base(module)
    {
        TrivialPhase();
    }
}