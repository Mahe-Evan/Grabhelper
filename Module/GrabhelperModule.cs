using System;
using Celeste;

namespace Celeste.Mod.Grabhelper {
    public class GrabhelperModule : EverestModule {
        public static GrabhelperModule Instance;

        public int GrabCount { get; set; } = 0;

        public static bool isGrabbing {
            get {
                switch ((int)Settings.Instance.GrabMode) {
                    case 0:
                        return GrabCountingHold.isGrabbing;
                    case 1:
                        return GrabCountingInvert.isClimbing;
                    case 2:
                        return GrabCountingToogle.isGrabbing;
                    default:
                        return false;
                }
            }
        }

        public override Type SettingsType => typeof(GrabhelperModuleSettings);
        public static GrabhelperModuleSettings GrabhelperSettings => (GrabhelperModuleSettings) Instance._Settings;

        // public override Type SessionType => typeof(GrabhelperModuleSession);
        // public static GrabhelperModuleSession Session => (GrabhelperModuleSession) Instance._Session;

        public override Type SaveDataType => typeof(GrabhelperModuleSaveData);
        public static GrabhelperModuleSaveData SaveData => (GrabhelperModuleSaveData) Instance._SaveData;

        public GrabhelperModule() {
            Instance = this;
    #if DEBUG
            // debug builds use verbose logging
            Logger.SetLogLevel(nameof(GrabhelperModule), LogLevel.Verbose);
    #else
            // release builds use info logging to reduce spam in log files
            Logger.SetLogLevel(nameof(GrabhelperModule), LogLevel.Info);
    #endif
        }

        public override void Load() {
            // TODO: apply any hooks that should always be active
            GrabCountingHold.Load();
            GrabCountingInvert.Load();
            GrabCountingToogle.Load();
            GrabCountingLevelSet.Load();
            GrabCountJournalPage.Load();
        }

        public override void Unload() {
            // TODO: unapply any hooks applied in Load()
            GrabCountingHold.Unload();
            GrabCountingInvert.Unload();
            GrabCountingToogle.Unload();
            GrabCountingLevelSet.Unload();
            GrabCountJournalPage.Unload();
        }
    }
    
}