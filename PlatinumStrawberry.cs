using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;

namespace Celeste.Mod.PlatinumStrawberry.Entities
{
    [CustomEntity("PlatinumStrawberry/PlatinumStrawberry")]
    [RegisterStrawberry(false, true)]
    class PlatinumStrawberry : Strawberry
    {
		public PlatinumStrawberry(EntityData data, Vector2 offset, EntityID gid) : base(data, offset, gid)
		{
		}
	}
}