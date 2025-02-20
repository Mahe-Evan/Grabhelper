using Celeste.Mod.Grabhelper;

namespace Celeste.Mod.Grabhelper {
    public class GrabCountingHold {

        static private int grabcount = 0;
        static private bool isGrabbing = false;
        static private bool isHolding = false;

        static private bool PickupCheck = false;

        static public void Load() {
            On.Celeste.Player.NormalUpdate += HoldGrabCounting;
            On.Celeste.Holdable.Pickup += HoldPickup;
            On.Celeste.Level.End += HoldChecklastgrab;
        }

        static public void Unload() {
            On.Celeste.Player.NormalUpdate -= HoldGrabCounting;
            On.Celeste.Holdable.Pickup -= HoldPickup;
            On.Celeste.Level.End -= HoldChecklastgrab;
        }

        static public bool HoldPickup(On.Celeste.Holdable.orig_Pickup orig, Holdable self, Player player) {
            bool result = orig(self, player);
            if ((int)Settings.Instance.GrabMode == 0) {
                Logger.Info("GrabHelper", "GrabMode 0");
                if (result && !isGrabbing && !GrabhelperModule.Settings.CheckHold) {
                    //Logger.Info("GrabHelper", "Pickup successful");
                    PickupCheck = true;
                    grabcount++;
                    Logger.Info("GrabHelper", grabcount.ToString());
                }
            }
            return result;
        }

        static public int HoldGrabCounting(On.Celeste.Player.orig_NormalUpdate orig, Player self) {
            if ((int)Settings.Instance.GrabMode == 0) {
                if (self.Holding == null && Input.Grab.Check && !isGrabbing && !isHolding) {
                    if (!GrabhelperModule.Settings.CheckHold && !PickupCheck) {
                        grabcount++;
                    }
                    Logger.Info("GrabHelper", grabcount.ToString());
                    isGrabbing = true;
                }
                if (self.Holding != null && Input.Grab.Check && isGrabbing && !isHolding) {
                    Logger.Info("GrabHelper", grabcount.ToString());
                    isHolding = true;
                }
                if (!Input.Grab.Check) {
                    if (isGrabbing && !isHolding && GrabhelperModule.Settings.CheckHold && !PickupCheck) {
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

        static public void HoldChecklastgrab(On.Celeste.Level.orig_End orig, Level self) {
            if ((int)Settings.Instance.GrabMode == 0) {
                if (!GrabhelperModule.Settings.CheckHold && !PickupCheck) {
                    grabcount++;
                }
                Logger.Info("GrabHelper", grabcount.ToString());
                isGrabbing = false;
                isHolding = false;
                PickupCheck = false;
                orig(self);
            }
        }

        // static public void MyGrabCounting(On.Celeste.Actor.orig_MoveVExact orig, Player self, int moveV) {
        //     if (moveV != 0) {
        //         grabcount += 1;
        //         Logger.Info("GrabHelper", grabcount.ToString());
        //         isGrabbing = true;
        //     }
    //     public static void ResetGrab() {
    //         GrabhelperModule.Session.Grabcount = 0;
    //         GrabhelperModule.Session.Grabcountlevelstart = 0;
    //     }

    //     public static void SetGrabLevelStart() {
    //         GrabhelperModule.Session.Grabcountlevelstart = GrabhelperModule.Session.Grabcount;
    //     }
    }
}