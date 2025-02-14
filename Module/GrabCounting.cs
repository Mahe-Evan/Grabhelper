using Celeste.Mod.Grabhelper;

namespace Celeste.Mod.Grabhelper {
    public class GrabCounting {

static private int grabcount = 0;

        static private bool HoldableCheck = true;
        static private bool isGrabbing = false;
        static private bool isHolding = false;

        static private bool PickupCheck = false;
        static public bool Pickup(On.Celeste.Holdable.orig_Pickup orig, Holdable self, Player player) {
            bool result = orig(self, player);
            if (result && !isGrabbing && !HoldableCheck) {
                //Logger.Info("GrabHelper", "Pickup successful");
                PickupCheck = true;
                grabcount++;
                Logger.Info("GrabHelper", grabcount.ToString());

            }
            return result;
        }

        static public int FGrabCounting(On.Celeste.Player.orig_NormalUpdate orig, Player self) {
            if (self.Holding == null && Input.Grab.Check && !isGrabbing && !isHolding) {
                if (!HoldableCheck && !PickupCheck) {
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
                if (isGrabbing && !isHolding && HoldableCheck && !PickupCheck) {
                    grabcount++;
                    Logger.Info("GrabHelper", grabcount.ToString());
                }
                isGrabbing = false;
                isHolding = false;
                PickupCheck = false;
            }
            return orig(self);
        }

        static public void Checklastgrab(On.Celeste.Level.orig_End orig, Level self) {
            if (isGrabbing && !isHolding && !PickupCheck) {
                grabcount++;
                Logger.Info("GrabHelper", grabcount.ToString());
            }
            isGrabbing = false;
            isHolding = false;
            PickupCheck = false;
            orig(self);
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