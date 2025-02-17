using Celeste.Mod.Grabhelper;

namespace Celeste.Mod.Grabhelper {
    public class GrabCountingInvert {

        static private int grabcount = 0;

        static private bool HoldableCheck = false;
        static private bool isGrabbing = false;
        static private bool isHolding = false;

        static private bool PickupCheck = false;

        static private bool isClimbing = false;

        static private bool hasClimbJump = false;
    
        static public void InvertWallJump(On.Celeste.Player.orig_WallJump orig, Player self, int dir) {
            if ((int)Settings.Instance.GrabMode == 1) {
                hasClimbJump = true;
            }
            orig(self, dir);
        }
        static public void InvertClimbingJump(On.Celeste.Player.orig_ClimbJump orig, Player self) {
            if ((int)Settings.Instance.GrabMode == 1) {
                if (!self.OnGround())
                    hasClimbJump = true;
                orig(self);
            }
            orig(self);
        }
        static public void InvertClimbingCheck(On.Celeste.Player.orig_UpdateSprite orig, Player self) {
            if ((int)Settings.Instance.GrabMode == 1) {
                if (self.StateMachine.State == 1 && !isClimbing) {
                    if (!hasClimbJump) {
                        grabcount++;
                        hasClimbJump = false;
                        Logger.Info("GrabHelper", grabcount.ToString());
                    }
                    isClimbing = true;
                }
                if (hasClimbJump && isClimbing && self.StateMachine.State == 1 ||
                    hasClimbJump && self.OnGround()) {
                    hasClimbJump = false;
                }
                if (self.StateMachine.State != 1) {
                    isClimbing = false;
                }
                orig(self);
            }
            orig(self);
        }
        static public bool InvertPickup(On.Celeste.Holdable.orig_Pickup orig, Holdable self, Player player) {
            bool result = orig(self, player);
            if ((int)Settings.Instance.GrabMode == 1) {
                Logger.Info("GrabHelper", "GrabMode 0");
                if (result && !isGrabbing && !HoldableCheck) {
                    //Logger.Info("GrabHelper", "Pickup successful");
                    PickupCheck = true;
                    grabcount++;
                    Logger.Info("GrabHelper", grabcount.ToString());
                }
            }
            return result;
        }

        static public int InvertGrabCounting(On.Celeste.Player.orig_NormalUpdate orig, Player self) {
            if ((int)Settings.Instance.GrabMode == 1) {
                if (self.Holding == null && !Input.Grab.Check && !isGrabbing && !isHolding) {
                    if (!HoldableCheck && !PickupCheck) {
                        grabcount++;
                    }
                    Logger.Info("GrabHelper", grabcount.ToString());
                    isGrabbing = true;
                }
                if (self.Holding != null && !Input.Grab.Check && isGrabbing && !isHolding) {
                    Logger.Info("GrabHelper", grabcount.ToString());
                    isHolding = true;
                }
                if (Input.Grab.Check) {
                    if (isGrabbing && !isHolding && HoldableCheck && !PickupCheck) {
                        grabcount++;
                        Logger.Info("GrabHelper", grabcount.ToString());
                    }
                    isGrabbing = false;
                    isHolding = false;
                    PickupCheck = false;
                }
            }
            return orig(self);
        }

        static public void InvertChecklastgrab(On.Celeste.Level.orig_End orig, Level self) {
            // if ((int)Settings.Instance.GrabMode == 1) {
            //     if (!HoldableCheck && !PickupCheck) {
            //         grabcount++;
            //     }
            //     Logger.Info("GrabHelper", grabcount.ToString());
            //     isGrabbing = false;
            //     isHolding = false;
            //     PickupCheck = false;
            //     orig(self);
            // }
        }

    }
}