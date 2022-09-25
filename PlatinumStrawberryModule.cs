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
    public class PlatinumStrawberryModule : EverestModule
    {
        private static PlatinumStrawberryModule _instance = null;
        private bool ilTest0 = false;

        public PlatinumStrawberryModule()
        {
            if (_instance == null)
            {
                _instance = this;
                //Logger.Log(LogLevel.Error, "PlatinumStrawberry", "Initialize PlatinumStrawberryModule via Constructor");
            }

            ilTest0 = false;
            bool ilTest1 = true;
            Logger.Log(LogLevel.Error, "PlatinumStrawberry", ilTest0.ToString() + " " + ilTest1.ToString());
        }

        public static PlatinumStrawberryModule Instance 
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = new PlatinumStrawberryModule();
                    //Logger.Log(LogLevel.Error, "PlatinumStrawberry", "Initialize PlatinumStrawberryModule via Instance getter");
                }
                return _instance;
            }
        }

        public override void Load()
        {
            StrawberryILInjector.Load();
            //throw new NotImplementedException();
        }

        public override void Unload()
        {
            StrawberryILInjector.Unload();
            //throw new NotImplementedException();
        }

        // Optional, initialize anything after Celeste has initialized itself properly.
        public override void Initialize()
        {
        }

        // Optional, do anything requiring either the Celeste or mod content here.
        public override void LoadContent(bool firstLoad)
        {
        }
    }
}