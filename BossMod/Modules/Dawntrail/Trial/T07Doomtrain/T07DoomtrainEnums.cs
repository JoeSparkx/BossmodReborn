namespace BossMod.Modules.Dawntrail.Trial.T07Doomtrain;

public enum OID : uint

{
    _Gen_ = 0xF845D, // R0.500, x?, EventNpc type
    Doomtrain = 0x4A30, // R19.040, x?
    Doomtrain1 = 0x4A33, // R1.000, x?, Helper type
    Actor1ea1a1 = 0x1EA1A1, // R2.000, x?, EventObj type
    Aether = 0x233C, // R0.500, x?, Helper type
    // Boss = 0x0, // R340282346638528859811704183484516925440.000--340282346638528859811704183484516925440.000, x?, None type
    LevinSignal = 0x4A31, // R1.000, x?
    KinematicTurret = 0x4A32, // R1.200, x?
    Exit = 0x1E850B, // R0.500, x?, EventObj type
    Aether1 = 0x4A34, // R1.500, x?
    GhostTrain = 0x4B80, // R2.720, x?
    Doomtrain2 = 0x4B7E, // R19.040, x?
    _Gen_1 = 0x4A36, // R1.000, x?
}

public enum AID : uint
{
    autoattack = 45662, // Aether->player, no cast, single-target
    LightningBurst = 45660, // Doomtrain->self, 5.0s cast, single-target
    LightningBurst1 = 45661, // Aether->player, 0.5s cast, range 5 circle
    LightningExpress = 45618, // Doomtrain->self, 6.0s cast, range 70 width 70 rect
    PlasmaBeam = 45619, // LevinSignal->self, 1.0s cast, range 30 width 5 rect
    PlasmaBeam1 = 45620, // LevinSignal->self, 1.0s cast, range 30 width 5 rect
    Windpipe = 45625, // Doomtrain->self, 6.0+1.0s cast, single-target
    Windpipe1 = 45667, // Aether->self, 7.0s cast, range 30 width 20 rect
    Blastpipe = 45626, // Doomtrain->self, no cast, single-target
    Blastpipe1 = 45627, // Aether->self, 3.0s cast, range 10 width 20 rect
    UnlimitedExpress = 45623, // Doomtrain->self, 5.0s cast, single-target
    UnlimitedExpress1 = 45624, // Aether->self, 5.9s cast, range 70 width 70 rect
    Teleport1 = 45641, // Doomtrain->location, no cast, single-target
    TurretCrossing = 45628, // Doomtrain->self, 3.0s cast, single-target
    Electray = 45629, // KinematicTurret->self, 5.0s cast, range 25 width 5 rect
    Electray1 = 45633, // KinematicTurret->self, 5.0s cast, range 10 width 5 rect
    Electray2 = 45632, // KinematicTurret->self, 5.0s cast, range 15 width 5 rect
    PlasmaBeam2 = 45622, // LevinSignal->self, 1.0s cast, range 10 width 5 rect
    HeadOnEmission = 45634, // Doomtrain->self, 6.0+1.0s cast, single-target
    ThunderousBreath = 45635, // Aether->self, 7.0s cast, range 70 width 70 rect
    HeadOnEmission1 = 45636, // Doomtrain->self, 6.0+1.0s cast, single-target
    Headlight = 45637, // Doomtrain1->self, 7.0s cast, range 30 width 20 rect
    RunawayTrain = 45638, // Doomtrain->self, 5.0s cast, single-target
    Overdraught = 45639, // Aether1->self, no cast, single-target
    AetherSurge = 45643, // Aether->self, 6.0s cast, range 30 45.000-degree cone
    AetherSurge1 = 45642, // Aether1->self, 6.0s cast, single-target
    AetherialRay = 45640, // Aether->self, no cast, range 50 ?-degree cone
    RunawayTrain1 = 45644, // Doomtrain2->self, no cast, single-target
    RunawayTrain2 = 45645, // Aether->self, no cast, range 20 circle
    Shockwave = 45646, // Doomtrain->self, no cast, single-target
    Shockwave1 = 45647, // Aether->self, no cast, range 50 circle
    ArcaneRevelation = 47527, // Doomtrain->self, 2.0+1.0s cast, single-target
    HailOfThunder = 45656, // Doomtrain->self, no cast, single-target
    HailOfThunder1 = 45659, // Aether->location, 3.2s cast, range 16 circle
    HailOfThunder2 = 45657, // Doomtrain->self, no cast, single-target
    DerailmentSiege = 45648, // Doomtrain->self, 6.0+1.0s cast, single-target
    DerailmentSiege1 = 45650, // Aether->self, no cast, range 5 circle
    DerailmentSiege2 = 45651, // Aether->self, 0.5s cast, range 5 circle
    DerailmentSiege3 = 45649, // Aether->self, 10.0s cast, range 5 circle
    Derail = 45654, // Aether->self, 10.0s cast, range 30 width 20 rect
    Derail1 = 45653, // Doomtrain->self, 10.1s cast, single-target
    Teleport2 = 45655, // Doomtrain->location, no cast, single-target
    Electray3 = 45631, // KinematicTurret->self, 5.0s cast, range 20 width 5 rect
    Electray4 = 45630, // KinematicTurret->self, 5.0s cast, range 25 width 5 rect
    BatteringArms = 47529, // Doomtrain->self, 6.0+1.0s cast, single-target
    BatteringArms1 = 47236, // Aether->self, no cast, range 5 circle
    BatteringArms2 = 47237, // Aether->self, 0.5s cast, range 5 circle
}

public enum SID : uint
{
    _Gen_VulnerabilityUp = 1789, // Aether/KinematicTurret->player, extra=0x1/0x2/0x3
    _Gen_SystemLock = 2578, // none->player, extra=0x0
    _Gen_ = 4721, // none->player, extra=0x0
    _Gen_2 = 4541, // none->GhostTrain, extra=0x578
    _Gen_3 = 2056, // none->Doomtrain2, extra=0x400
    _Gen_4 = 2552, // none->GhostTrain, extra=0x42B
    _Gen_5 = 4176, // none->GhostTrain, extra=0x0

}

public enum IconID : uint
{
    LightningBurst = 343, // player->self
    _Gen_Icon_x6fg_fan35_50_0k2 = 642, // GhostTrain->player
}

