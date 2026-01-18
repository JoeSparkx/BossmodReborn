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
    _Ability_ = 45662, // Aether->player, no cast, single-target
    _Ability_LightningBurst = 45660, // Doomtrain->self, 5.0s cast, single-target
    _Ability_LightningBurst1 = 45661, // Aether->player, 0.5s cast, range 5 circle
    _Ability_LightningExpress = 45618, // Doomtrain->self, 6.0s cast, range 70 width 70 rect
    _Ability_PlasmaBeam = 45619, // LevinSignal->self, 1.0s cast, range 30 width 5 rect
    _Ability_PlasmaBeam1 = 45620, // LevinSignal->self, 1.0s cast, range 30 width 5 rect
    _Ability_Windpipe = 45625, // Doomtrain->self, 6.0+1.0s cast, single-target
    _Ability_Windpipe1 = 45667, // Aether->self, 7.0s cast, range 30 width 20 rect
    _Ability_Blastpipe = 45626, // Doomtrain->self, no cast, single-target
    _Ability_Blastpipe1 = 45627, // Aether->self, 3.0s cast, range 10 width 20 rect
    _Ability_UnlimitedExpress = 45623, // Doomtrain->self, 5.0s cast, single-target
    _Ability_UnlimitedExpress1 = 45624, // Aether->self, 5.9s cast, range 70 width 70 rect
    _Ability_1 = 45641, // Doomtrain->location, no cast, single-target
    _Ability_TurretCrossing = 45628, // Doomtrain->self, 3.0s cast, single-target
    _Ability_Electray = 45629, // KinematicTurret->self, 5.0s cast, range 25 width 5 rect
    _Ability_Electray1 = 45633, // KinematicTurret->self, 5.0s cast, range 10 width 5 rect
    _Ability_Electray2 = 45632, // KinematicTurret->self, 5.0s cast, range 15 width 5 rect
    _Ability_PlasmaBeam2 = 45622, // LevinSignal->self, 1.0s cast, range 10 width 5 rect
    _Ability_HeadOnEmission = 45634, // Doomtrain->self, 6.0+1.0s cast, single-target
    _Ability_ThunderousBreath = 45635, // Aether->self, 7.0s cast, range 70 width 70 rect
    _Ability_HeadOnEmission1 = 45636, // Doomtrain->self, 6.0+1.0s cast, single-target
    _Ability_Headlight = 45637, // Doomtrain1->self, 7.0s cast, range 30 width 20 rect
    _Ability_RunawayTrain = 45638, // Doomtrain->self, 5.0s cast, single-target
    _Ability_Overdraught = 45639, // Aether1->self, no cast, single-target
    _Ability_AetherSurge = 45643, // Aether->self, 6.0s cast, range 30 45.000-degree cone
    _Ability_AetherSurge1 = 45642, // Aether1->self, 6.0s cast, single-target
    _Ability_AetherialRay = 45640, // Aether->self, no cast, range 50 ?-degree cone
    _Ability_RunawayTrain1 = 45644, // Doomtrain2->self, no cast, single-target
    _Ability_RunawayTrain2 = 45645, // Aether->self, no cast, range 20 circle
    _Ability_Shockwave = 45646, // Doomtrain->self, no cast, single-target
    _Ability_Shockwave1 = 45647, // Aether->self, no cast, range 50 circle
    _Ability_ArcaneRevelation = 47527, // Doomtrain->self, 2.0+1.0s cast, single-target
    _Ability_HailOfThunder = 45656, // Doomtrain->self, no cast, single-target
    _Ability_HailOfThunder1 = 45659, // Aether->location, 3.2s cast, range 16 circle
    _Ability_HailOfThunder2 = 45657, // Doomtrain->self, no cast, single-target
    _Ability_DerailmentSiege = 45648, // Doomtrain->self, 6.0+1.0s cast, single-target
    _Ability_DerailmentSiege1 = 45650, // Aether->self, no cast, range 5 circle
    _Ability_DerailmentSiege2 = 45651, // Aether->self, 0.5s cast, range 5 circle
    _Ability_DerailmentSiege3 = 45649, // Aether->self, 10.0s cast, range 5 circle
    _Ability_Derail = 45654, // Aether->self, 10.0s cast, range 30 width 20 rect
    _Ability_Derail1 = 45653, // Doomtrain->self, 10.1s cast, single-target
    _Ability_2 = 45655, // Doomtrain->location, no cast, single-target
    _Ability_Electray3 = 45631, // KinematicTurret->self, 5.0s cast, range 20 width 5 rect
    _Ability_Electray4 = 45630, // KinematicTurret->self, 5.0s cast, range 25 width 5 rect
    _Ability_BatteringArms = 47529, // Doomtrain->self, 6.0+1.0s cast, single-target
    _Ability_BatteringArms1 = 47236, // Aether->self, no cast, range 5 circle
    _Ability_BatteringArms2 = 47237, // Aether->self, 0.5s cast, range 5 circle
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
    _Gen_Icon_tank_lockonae_5m_5s_01k1 = 343, // player->self
    _Gen_Icon_x6fg_fan35_50_0k2 = 642, // GhostTrain->player
}

