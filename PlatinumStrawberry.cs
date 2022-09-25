using Celeste;
using MonoMod.Utils;
using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.PlatinumStrawberry
{
    internal class PlatinumStrawberry : Strawberry
    {

        public PlatinumStrawberry(EntityData data, Vector2 offset, EntityID id, bool restored) : base(data, offset, id)
        {
            new DynData<Strawberry>(this)["isGhostBerry"] = false;
        }
    }
}
