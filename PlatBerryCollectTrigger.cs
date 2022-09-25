using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.PlatinumStrawberry.Triggers
{
    [CustomEntity("PlatinumStrawberry/PlatBerryCollectTrigger")]
    [Tracked(false)]
    class PlatBerryCollectTrigger : Trigger
    {
        public PlatBerryCollectTrigger(EntityData data, Vector2 offset) : base(data, offset) { }
    }

}
