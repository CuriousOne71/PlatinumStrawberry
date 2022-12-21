using System;
using Celeste.Mod.PlatinumStrawberry.Hooks;
using Celeste.Mod.PlatinumStrawberry.Settings;

namespace Celeste.Mod.PlatinumStrawberry
{
    public class PlatinumModule : EverestModule
    {
        private static PlatinumModule? _instance = null;

        public PlatinumModule()
        {
            if (_instance == null) _instance = this;
        }

        public static PlatinumModule Instance 
        {
            get 
            {
                if (_instance == null) _instance = new PlatinumModule();
                return _instance;
            }
        }

        public override Type SettingsType => typeof(PlatinumSettings);
        public PlatinumSettings Settings => _Settings as PlatinumSettings;

        public override void Load()
        {
            TeleportHook.Load();
        }

        public override void Unload()
        {
            TeleportHook.Unload();
        }

        public override void Initialize()
        {
        }

        public override void LoadContent(bool firstLoad)
        {
        }
    }
}