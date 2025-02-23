using System.Collections.Generic;

namespace Celeste.Mod.Grabhelper;

public class GrabhelperModuleSaveData : EverestModuleSaveData {

    public Dictionary<string, Dictionary<AreaMode, int>> GrabCountPerLevel { get; set; } = new();

}