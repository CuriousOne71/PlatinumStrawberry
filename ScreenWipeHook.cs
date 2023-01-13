using Celeste.Mod.PlatinumStrawberry.Entities;
using Monocle;

namespace Celeste.Mod.PlatinumStrawberry.Hooks
{
    class ScreenWipeHook
    {
        internal static void Load() => On.Celeste.ScreenWipe.Update += platinumRestartCheck;

        internal static void Unload() => On.Celeste.ScreenWipe.Update -= platinumRestartCheck;

        private static void platinumRestartCheck(On.Celeste.ScreenWipe.orig_Update orig, ScreenWipe self, Scene scene)
        {
            orig(self, scene);
            if (scene.Entities.FindFirst<PlatinumBerry>() != null && PlatinumModule.Instance.Settings.DefaultPlatinumBerryRespawnBehavior)
            {
                void platinumRestart(PlatinumBerry plat)
                {
                    Level level = scene as Level;
                    Session session = level.Session;
                    Engine.Scene = new LevelExit(LevelExit.Mode.GoldenBerryRestart, session)
                    {
                        GoldenStrawberryEntryLevel = plat.ID.Level
                    };
                }

                foreach (PlatinumBerry berry in scene.Entities.FindAll<PlatinumBerry>())
                {
                    if (berry.Dead)
                    {
                        self.OnComplete = () => platinumRestart(berry);
                        break;
                    }
                }
            }
        }

        
    }
}
