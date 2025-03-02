using Celeste.Mod.Grabhelper;
using Monocle;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.Grabhelper {
    
    public class CountDisplayInLevel : Entity {

        protected GrabhelperModuleSaveData ModSaveData {
            get {
                return (GrabhelperModuleSaveData) GrabhelperModule.Instance._SaveData;
            }
        }

        protected readonly MTexture icon = GFX.Gui["hand"];
        protected readonly MTexture x = GFX.Gui["x"];
        protected readonly MTexture bg = GFX.Gui["background"];

        private SpeedrunTimerDisplay speedrunTimer;

        private TotalStrawberriesDisplay berryCounter;

        private int positionY = 200;
        private Level level;

        public CountDisplayInLevel() {
            Tag = Tags.HUD | Tags.Global | Tags.PauseUpdate | Tags.TransitionUpdate;
            Position = new(0, -1000);
        }

        public override void Awake(Scene scene) {
            base.Awake(scene);

            level = SceneAs<Level>();
            speedrunTimer = Scene.Entities.FindFirst<SpeedrunTimerDisplay>();
            berryCounter = Scene.Entities.FindFirst<TotalStrawberriesDisplay>();
        }

        public override void Render() {
            base.Render();

            if (level == null || level.Session == null) {
                Logger.Log("Grabhelper", "I lost the game :3");
                return;
            }

            string levelSID = level.Session.Area.GetSID();
            AreaMode side = level.Session.Area.Mode;

            string str = string.Empty;
            if (ModSaveData.GrabCountPerLevel.TryGetValue(levelSID, out Dictionary<AreaMode, int> grabCounts)) {
                if (grabCounts.TryGetValue(side, out int grabCount) && GrabhelperModule.Settings.PrintGrabCount) {
                    //Logger.Info("Grabhelper", $"Grab count: {grabCount}");
                    str = "( " + ModSaveData.GrabCountPerLevel[levelSID][side].ToString() + " ) " + GrabhelperModule.Instance.GrabCount.ToString();
                
                } else {
                    str = "( " + " ) " + GrabhelperModule.Instance.GrabCount.ToString();
                }
            } else {
                str = "( " + " ) " + GrabhelperModule.Instance.GrabCount.ToString();
            }
            if (GrabhelperModule.Settings.PrintGrabCount) {
                if (speedrunTimer.DrawLerp > 0f) {
                    positionY = 200;
                } else {
                    positionY = 100;
                }
                if (berryCounter.DrawLerp > 0f) {
                    positionY += 78;
                } else {
                    positionY += 0;
                }
                Vector2 position = new(20, positionY);
                Vector2 iconSize = new(1.5f, 1.5f);
                Vector2 xSize = new(8, 8);
                Vector2 bgSize = new(str.Length * 0.05f, 1.5f);

                bg.Draw(new Vector2(position.X, position.Y), Vector2.Zero, Color.Black, bgSize);
                icon.Draw(position + new Vector2(0, 0), Vector2.Zero, Color.White, iconSize);
                ActiveFont.DrawOutline(str , position + new Vector2(50, -10), new(0, 0), Vector2.One, Color.White, 2, Color.Black);
            }
        }

    }
}