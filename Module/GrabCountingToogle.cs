using Celeste.Mod.Grabhelper;

namespace Celeste.Mod.Grabhelper {
    public class GrabCountingToogle {

        static private bool istooglegrabbing = false;

        static private bool istoogleholding = false;

        static private bool isgrabactiveted = false;

        static private  bool hastoogleinput = false;

        static private bool hasClimbJumptoogle = false;

        static public void Load() {
            On.Celeste.Player.UpdateSprite += CountingToogle;
            On.Celeste.Player.UpdateSprite += ToogleClimbingCheck;
            On.Celeste.Player.WallJump += ToogleWallJump;
            On.Celeste.Player.ClimbJump += ToogleClimbingJump;
        }

        static public void Unload() {
            On.Celeste.Player.UpdateSprite -= CountingToogle;
            On.Celeste.Player.UpdateSprite -= ToogleClimbingCheck;
            On.Celeste.Player.WallJump -= ToogleWallJump;
            On.Celeste.Player.ClimbJump -= ToogleClimbingJump;
        }
        static public void ToogleWallJump(On.Celeste.Player.orig_WallJump orig, Player self, int dir) {
            if ((int)Settings.Instance.GrabMode == 2) {
                hasClimbJumptoogle = true;
            }
            orig(self, dir);
        }

        static public void ToogleClimbingJump(On.Celeste.Player.orig_ClimbJump orig, Player self) {
            if ((int)Settings.Instance.GrabMode == 2) {
                if (!self.OnGround())
                    hasClimbJumptoogle = true;
                orig(self);
            }
            orig(self);
        }

        static public void CountingToogle(On.Celeste.Player.orig_UpdateSprite orig, Player self) {
            if ((int)Settings.Instance.GrabMode == 2) {
                if (Input.Grab.Check && !hastoogleinput && !isgrabactiveted) {
                    hastoogleinput = true;
                    isgrabactiveted = true;
                }
                if (Input.Grab.Check && !hastoogleinput && isgrabactiveted) {
                    hastoogleinput = true;
                    isgrabactiveted = false;
                    GrabhelperModule.Instance.GrabCount++;
                    // Logger.Info("GrabHelper", GrabhelperModule.Instance.GrabCount.ToString());
                }
                if (!Input.Grab.Check) {
                    istoogleholding = true;
                }
                if (hastoogleinput && Input.Grab.Check && istoogleholding) {
                    istoogleholding = false;
                    hastoogleinput = false;
                }
            }
            orig(self);
        }
        static public void ToogleClimbingCheck(On.Celeste.Player.orig_UpdateSprite orig, Player self) {
            if ((int)Settings.Instance.GrabMode == 2) {
                if (self.StateMachine.State == 1 && !istooglegrabbing) {
                    if (!hasClimbJumptoogle) {
                    GrabhelperModule.Instance.GrabCount++;
                    hasClimbJumptoogle = false;
                    // Logger.Info("GrabHelper", GrabhelperModule.Instance.GrabCount.ToString());
                    }
                    istooglegrabbing = true;
                }
                if (hasClimbJumptoogle && istooglegrabbing && self.StateMachine.State == 1 ||
                    hasClimbJumptoogle && self.OnGround()) {
                    hasClimbJumptoogle = false;
                }
                if (self.StateMachine.State != 1) {
                    istooglegrabbing = false;
                }
                orig(self);
            }
            orig(self);
        }
    }
}