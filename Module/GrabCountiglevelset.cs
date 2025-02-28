using Celeste.Mod.Grabhelper;

namespace Celeste.Mod.Grabhelper {
    public static class GrabCountingLevelSet {
        
        static public string level = null;

        static public AreaMode side = new AreaMode();

        static public bool checkpoint = false;
    
        static public void Load() {
            On.Celeste.Level.Begin += onLevelBegin;
            On.Celeste.Level.End += onLevelEnd;
        }

        static public void Unload() {
            On.Celeste.Level.Begin -= onLevelBegin;
            On.Celeste.Level.End -= onLevelEnd;
        }

        private static void onLevelBegin(On.Celeste.Level.orig_Begin orig, Level self) {
            GrabhelperModule.Instance.GrabCount = 0;
            level = self.Session.Area.GetSID();
            side = self.Session.Area.Mode;
            checkpoint = self.Session.StartedFromBeginning;
            self.Add(new CountDisplayInLevel());
            // Logger.Info("GrabHelper", "Level: " + level + " Side: " + side.ToString());
            // Logger.Info("GrabHelper", "Checkpoint: " + checkpoint.ToString());
            orig(self);
        }
    
        private static void onLevelEnd(On.Celeste.Level.orig_End orig, Level self) {
            if (self.Completed && checkpoint) {
                if (!GrabhelperModule.SaveData.GrabCountPerLevel.ContainsKey(level)) {
                    GrabhelperModule.SaveData.GrabCountPerLevel[level] = new();
                }
                if (!GrabhelperModule.SaveData.GrabCountPerLevel[level].ContainsKey(side)) {
                    GrabhelperModule.SaveData.GrabCountPerLevel[level][side] = GrabhelperModule.Instance.GrabCount;
                }
                if (GrabhelperModule.Instance.GrabCount < GrabhelperModule.SaveData.GrabCountPerLevel[level][side]) {
                    GrabhelperModule.SaveData.GrabCountPerLevel[level][side] = GrabhelperModule.Instance.GrabCount;
                }
                Logger.Info("GrabHelper", "GrabCount: " + GrabhelperModule.Instance.GrabCount.ToString());
            }
            orig(self);
        }

    }
}