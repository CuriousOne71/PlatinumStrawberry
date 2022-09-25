using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.PlatinumStrawberry.Entities
{
    [CustomEntity("PlatinumStrawberry/HiddenStrawberry")]
    [RegisterStrawberry(false, false)]
    class HiddenStrawberry : Strawberry
    {
        public HiddenStrawberry(EntityData data, Vector2 offset, EntityID gid) : base(data, offset, gid) { }
    }
}
