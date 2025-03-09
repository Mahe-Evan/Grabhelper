using Celeste.Mod.Grabhelper;
using static Celeste.Mod.Grabhelper.GrabhelperModuleSettings;

namespace Celeste.Mod.Grabhelper {
    public class GrabCountingInvert {

        static private bool isGrabbing = false;
        static private bool isHolding = false;

        static protected bool PickupCheck = false;
    
        static public bool isClimbing = false;

        static private bool hasClimbJump = false;

        static public void Load() {
            On.Celeste.Player.NormalUpdate += InvertGrabCounting;
            On.Celeste.Holdable.Pickup += InvertPickup;
            On.Celeste.Player.UpdateSprite += InvertClimbingCheck;
            On.Celeste.Player.WallJump += InvertWallJump;
            On.Celeste.Player.ClimbJump += InvertClimbingJump;
        }

        static public void Unload() {
            On.Celeste.Player.NormalUpdate -= InvertGrabCounting;
            On.Celeste.Holdable.Pickup -= InvertPickup;
            On.Celeste.Player.UpdateSprite -= InvertClimbingCheck;
            On.Celeste.Player.WallJump -= InvertWallJump;
            On.Celeste.Player.ClimbJump -= InvertClimbingJump;
        }
    
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
                        GrabhelperModule.Instance.GrabCount++;
                        hasClimbJump = false;
                  //      Logger.Info("GrabHelper", GrabhelperModule.Instance.GrabCount.ToString());
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
                if (result && !isGrabbing && GrabhelperModule.GrabhelperSettings.CheckHold) {
                    //Logger.Info("GrabHelper", "Pickup successful");
                    PickupCheck = true;
                    GrabhelperModule.Instance.GrabCount++;
                 //   Logger.Info("GrabHelper", GrabhelperModule.Instance.GrabCount.ToString());
                }
            }
            return result;
        }

        static public int InvertGrabCounting(On.Celeste.Player.orig_NormalUpdate orig, Player self) {
            if ((int)Settings.Instance.GrabMode == 1) {
                if (self.Holding == null && !Input.Grab.Check && !isGrabbing && !isHolding) {
                    if (!GrabhelperModule.GrabhelperSettings.CheckHold && !PickupCheck) {
                        GrabhelperModule.Instance.GrabCount++;
                    }
                //    Logger.Info("GrabHelper", GrabhelperModule.Instance.GrabCount.ToString());
                    isGrabbing = true;
                }
                if (self.Holding != null && !Input.Grab.Check && isGrabbing && !isHolding) {
                 //   Logger.Info("GrabHelper", GrabhelperModule.Instance.GrabCount.ToString());
                    isHolding = true;
                }
                if (Input.Grab.Check) {
                    if (isGrabbing && !isHolding && GrabhelperModule.GrabhelperSettings.CheckHold && !PickupCheck) {
                        GrabhelperModule.Instance.GrabCount++;
                   //     Logger.Info("GrabHelper", GrabhelperModule.Instance.GrabCount.ToString());
                    }
                    isGrabbing = false;
                    isHolding = false;
                    PickupCheck = false;
                }
            }
            return orig(self);
        }

    }
}