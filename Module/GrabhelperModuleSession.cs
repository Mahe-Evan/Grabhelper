using System.Collections.Generic;

namespace Celeste.Mod.Grabhelper;

public class GrabhelperModuleSession : EverestModuleSession {
    public int GrabCount { get; set; } = 0;

    public int GrabCountAtLevelStart { get; set; } = 0;
}