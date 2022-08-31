using Celeste;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.PlatinumStrawberry.Entities
{
    [CustomEntity("PlatinumStrawberry/HiddenStrawberry")]
    [RegisterStrawberry(false, false)]
    class HiddenStrawberry : Strawberry
    {
        public HiddenStrawberry(EntityData data, Vector2 offset, EntityID gid) : base(data, offset, gid) { }
    }
}
