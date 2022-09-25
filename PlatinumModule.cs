using Celeste.Mod.UI;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using Celeste;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.PlatinumStrawberry
{
    public class PlatinumModule : EverestModule
    {
        private static PlatinumModule? _instance = null;

        public PlatinumModule()
        {
            if (_instance == null)
            {
                _instance = this;
            }
        }

        public static PlatinumModule Instance 
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = new PlatinumModule();
                }
                return _instance;
            }
        }

        public override void Load()
        {
        }

        public override void Unload()
        {
        }

        public override void Initialize()
        {
        }

        public override void LoadContent(bool firstLoad)
        {
        }
    }
}