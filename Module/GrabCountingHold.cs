using Celeste.Mod.Grabhelper;

namespace Celeste.Mod.Grabhelper {
    public class GrabCountingHold {

        static public bool isGrabbing = false;
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
                //Logger.Info("GrabHelper", "GrabMode 0");
                if (result && !isGrabbing && !GrabhelperModule.GrabhelperSettings.CheckHold) {
                    //Logger.Info("GrabHelper", "Pickup successful");
                    PickupCheck = true;
                    GrabhelperModule.Instance.GrabCount++;
                    // Logger.Info("GrabHelper", GrabhelperModule.Instance.GrabCount.ToString());
                }
            }
            return result;
        }

        static public int HoldGrabCounting(On.Celeste.Player.orig_NormalUpdate orig, Player self) {
            if ((int)Settings.Instance.GrabMode == 0) {
                if (self.Holding == null && Input.Grab.Check && !isGrabbing && !isHolding) {
                    if (!GrabhelperModule.GrabhelperSettings.CheckHold && !PickupCheck) {
                        GrabhelperModule.Instance.GrabCount++;
                    }
                    // Logger.Info("GrabHelper", GrabhelperModule.Instance.GrabCount.ToString());
                    isGrabbing = true;
                }
                if (self.Holding != null && Input.Grab.Check && isGrabbing && !isHolding) {
                    // Logger.Info("GrabHelper", GrabhelperModule.Instance.GrabCount.ToString());
                    isHolding = true;
                }
                if (!Input.Grab.Check) {
                    if (isGrabbing && !isHolding && GrabhelperModule.GrabhelperSettings.CheckHold && !PickupCheck) {
                        GrabhelperModule.Instance.GrabCount++;
                        // Logger.Info("GrabHelper", GrabhelperModule.Instance.GrabCount.ToString());
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
                if (!GrabhelperModule.GrabhelperSettings.CheckHold && !PickupCheck) {
                    GrabhelperModule.Instance.GrabCount++;
                }
                // Logger.Info("GrabHelper", GrabhelperModule.Instance.GrabCount.ToString());
                isGrabbing = false;
                isHolding = false;
                PickupCheck = false;
                orig(self);
            }
        }
    }
}