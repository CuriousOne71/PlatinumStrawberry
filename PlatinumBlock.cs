using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.PlatinumStrawberry.Entities
{
    [CustomEntity("PlatinumStrawberry/PlatinumBlock")]
    [Tracked(false)]
    class PlatinumBlock : GoldenBlock
    {
        private float startY;
        private Image berry;
        private MTexture[,] nineSlice;

        public PlatinumBlock(Vector2 offset, float width, float height) : base(offset, width, height)
        {
            startY = base.Y;
            berry = new Image(GFX.Game["SyrenyxPlatinumStrawberry/objects/platinumblock/icon"]);
            berry.CenterOrigin();
            berry.Position = new Vector2(width / 2f, height / 2f);
            MTexture mTexture = GFX.Game["SyrenyxPlatinumStrawberry/objects/platinumblock/block"];
            nineSlice = new MTexture[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    nineSlice[i, j] = mTexture.GetSubtexture(new Rectangle(i * 8, j * 8, 8, 8));
                }
            }
            base.Depth = -10000;
            Add(new LightOcclude());
            Add(new MirrorSurface());
            SurfaceSoundIndex = 32;
        }

        public PlatinumBlock(EntityData data, Vector2 offset) : this(data.Position + offset, data.Width, data.Height) { }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            bool platFollower = false;
            foreach (Strawberry item in scene.Entities.FindAll<Strawberry>())
            {
                if (item is PlatinumStrawberry && item.Follower.Leader != null)
                {
                    platFollower = true;
                    break;
                }
            }
            if (!platFollower)
            {
                RemoveSelf();
            }
        }
    }
}
