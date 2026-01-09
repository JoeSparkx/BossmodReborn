namespace BossMod.Stormblood.Raid.O1NAlteRoite;
public enum OID : uint
{
    Boss = 0x1A6F,
    Helper = 0x233C,
    BallOfFire = 0x1A71, // R1.000, x0 (spawn during fight)
}
public enum AID : uint
{
    Attack = 872, // Boss->player, no cast, single-target
    WyrmTail = 9174, // Boss->player, no cast, single-target
    Flame = 9181, // Boss->self, no cast, ???
    Roar = 9180, // Boss->self, 4.0s cast, range 100 circle
    Burn = 9173, // 1A71->self, 1.0s cast, range 8 circle
    BreathWing = 9182, // Boss->self, 5.0s cast, single-target
    TwinBolt = 9175, // Boss->self, 5.0s cast, single-target
    Clamp = 9186, // Boss->self, 3.0s cast, range 9+R width 10 rect
    FlashFreeze = 9183, // Boss->self, no cast, single-target
    Levinbolt = 9177, // Boss->self, 5.0s cast, single-target
    Blaze = 9185, // Boss->players, 5.0s cast, range 6 circle
    TheClassicalElements = 9184, // Boss->self, 5.0s cast, single-target
    Downburst = 7896, // Boss->self, 5.0s cast, single-target
    Turbulence = 9603, // 18D6->self, no cast, range 5 circle
    Charybdis = 9179, // Boss->self, 5.0s cast, range 100 circle
    Levinbolt1 = 9178, // 18D6->self, no cast, range 6 circle
   
}

public enum SID : uint
{
    ThinIce = 911, // none->player, extra=0x96
}

public enum TetherID : uint
{
    Tether_chn_raikou1s = 21, // player->player
}
