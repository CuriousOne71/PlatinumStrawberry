using MonoMod.ModInterop;

namespace Celeste.Mod.PlatinumStrawberry
{
    public static class ModExports
    {
        [ModExportName("PlatinumStrawberry.Setting")]
        public static class Settings
        {
            #region RespawnBehavior

            public static bool RespawnBehavior() => PlatinumModule.Instance.Settings.DefaultPlatinumBerryRespawnBehavior;

            #endregion
        }
    }
}
