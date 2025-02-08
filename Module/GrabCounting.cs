using Celeste.Mod.Grabhelper;

namespace Celeste.Mod.Grabhelper {
    public class GrabCounting {

        static private int grabcount = 0;

        static private bool isGrabbing = false;

        static public int FGrabCounting(On.Celeste.Player.orig_NormalUpdate orig, Player self) {
            if (self.Holding == null && Input.Grab.Check && !isGrabbing) {
                grabcount++;
                Logger.Info("GrabHelper", grabcount.ToString());
                isGrabbing = true;
            }
            if (!Input.Grab.Check) {
                isGrabbing = false;
            }
            return orig(self);
        }

    //     public static void ResetGrab() {
    //         GrabhelperModule.Session.Grabcount = 0;
    //         GrabhelperModule.Session.Grabcountlevelstart = 0;
    //     }

    //     public static void SetGrabLevelStart() {
    //         GrabhelperModule.Session.Grabcountlevelstart = GrabhelperModule.Session.Grabcount;
    //     }
    }
}