using System;

namespace Celeste.Mod.Grabhelper;

public class GrabhelperModule : EverestModule {
    public static GrabhelperModule Instance { get; private set; }

    public override Type SettingsType => typeof(GrabhelperModuleSettings);
    public static GrabhelperModuleSettings Settings => (GrabhelperModuleSettings) Instance._Settings;

    public override Type SessionType => typeof(GrabhelperModuleSession);
    public static GrabhelperModuleSession Session => (GrabhelperModuleSession) Instance._Session;

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
    }

    public override void Unload() {
        // TODO: unapply any hooks applied in Load()
    }
}