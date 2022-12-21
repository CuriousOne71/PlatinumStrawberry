using Celeste.Mod.PlatinumStrawberry.Entities;
using IL.Monocle;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Monocle;

namespace Celeste.Mod.PlatinumStrawberry.Hooks
{
    class TeleportHook
    {
        internal static void Load() => On.Celeste.Level.TeleportTo += storePlatinumBerry;

        internal static void Unload() => On.Celeste.Level.TeleportTo -= storePlatinumBerry;

        private static void storePlatinumBerry(On.Celeste.Level.orig_TeleportTo orig, Level self, Player player, string nextLevel, Player.IntroTypes introType, Vector2? nearestSpawn = null)
        {
            var storedBerries = new List<PlatinumBerry>();
            var storedOffsets = new List<Vector2>();
            foreach (Follower follower in player.Leader.Followers)
            {
                if (follower.Entity is PlatinumBerry)
                {
                    storedBerries.Add(follower.Entity as PlatinumBerry);
                    storedOffsets.Add(follower.Entity.Position - player.Leader.Entity.Position);
                }
            }
            foreach (PlatinumBerry storedBerry in storedBerries)
            {
                player.Leader.Followers.Remove(storedBerry.Follower);
                storedBerry.Follower.Leader = null;
                storedBerry.AddTag(Tags.Global);
            }

            orig(self, player, nextLevel, introType, nearestSpawn);

            
            player = Monocle.Engine.Scene.Tracker.GetEntity<Player>();
            player.Leader.PastPoints.Clear();
            for (int i = 0; i < storedBerries.Count; i++)
            {
                PlatinumBerry strawberry = storedBerries[i];
                player.Leader.GainFollower(strawberry.Follower);
                strawberry.Position = player.Leader.Entity.Position + storedOffsets[i];
                strawberry.RemoveTag(Tags.Global);


            }
        }
    }
}
