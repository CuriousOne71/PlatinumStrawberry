using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.PlatinumStrawberry.Entities
{
    [CustomEntity("PlatinumStrawberry/PlatinumBlock")]
    [Tracked(false)]
    class PlatinumBlock : Solid
    {
        private float _startY;
        private float _yLerp;
        private float _sinkTimer;
        private float _renderLerp;
        private Image _berry;
        private MTexture[,] _nineSlice;

        public PlatinumBlock(Vector2 position, float width, float height) : base(position, width, height, safe: false)
        {
            _startY = base.Y;
            _berry = new Image(GFX.Game["SyrenyxPlatinumStrawberry/objects/platinumblock/icon"]);
            _berry.CenterOrigin();
            _berry.Position = new Vector2(width / 2f, height / 2f);
            MTexture mTexture = GFX.Game["SyrenyxPlatinumStrawberry/objects/platinumblock/block"];
            _nineSlice = new MTexture[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    _nineSlice[i, j] = mTexture.GetSubtexture(new Rectangle(i * 8, j * 8, 8, 8));
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
            DisableStaticMovers();
            Visible = false;
            Collidable = false;
            _renderLerp = 1f;
            bool platFollower = false;
            foreach (PlatinumBerry item in scene.Entities.FindAll<PlatinumBerry>())
            {
                if (item.Follower.Leader != null)
                {
                    platFollower = true;
                    break;
                }
            }
            if (!platFollower)
            {
                DestroyStaticMovers();
                RemoveSelf();
            }
        }

        public override void Update()
        {
            base.Update();
            if (!Visible)
            {
                Player entity = base.Scene.Tracker.GetEntity<Player>();
                if (entity != null && entity.X > base.X - 80f)
                {
                    Visible = true;
                    Collidable = true;
                    _renderLerp = 1f;
                }
            }

            if (Visible)
            {
                _renderLerp = Calc.Approach(_renderLerp, 0f, Engine.DeltaTime * 3f);
            }

            if (HasPlayerRider())
            {
                _sinkTimer = 0.1f;
            }
            else if (_sinkTimer > 0f)
            {
                _sinkTimer -= Engine.DeltaTime;
            }

            if (_sinkTimer > 0f)
            {
                _yLerp = Calc.Approach(_yLerp, 1f, 1f * Engine.DeltaTime);
            }
            else
            {
                _yLerp = Calc.Approach(_yLerp, 0f, 1f * Engine.DeltaTime);
            }

            float y = MathHelper.Lerp(_startY, _startY + 12f, Ease.SineInOut(_yLerp));
            MoveToY(y);
            if (_renderLerp == 0f)
            {
                EnableStaticMovers();
            }
            else
            {
                DisableStaticMovers();
            }
        }

        private void DrawBlock(Vector2 offset, Color color)
        {
            float num = base.Collider.Width / 8f - 1f;
            float num2 = base.Collider.Height / 8f - 1f;
            for (int i = 0; (float)i <= num; i++)
            {
                for (int j = 0; (float)j <= num2; j++)
                {
                    int num3 = (((float)i < num) ? Math.Min(i, 1) : 2);
                    int num4 = (((float)j < num2) ? Math.Min(j, 1) : 2);
                    _nineSlice[num3, num4].Draw(Position + offset + base.Shake + new Vector2(i * 8, j * 8), Vector2.Zero, color);
                }
            }
        }

        public override void Render()
        {
            Level level = Scene as Level;
            Vector2 vector = new Vector2(0f, ((float)level.Bounds.Bottom - _startY + 32f) * Ease.CubeIn(_renderLerp));
            Vector2 position = Position;
            Position += vector;
            DrawBlock(new Vector2(-1f, 0f), Color.Black);
            DrawBlock(new Vector2(1f, 0f), Color.Black);
            DrawBlock(new Vector2(0f, -1f), Color.Black);
            DrawBlock(new Vector2(0f, 1f), Color.Black);
            DrawBlock(Vector2.Zero, Color.White);
            _berry.Color = Color.White;
            _berry.RenderPosition = base.Center;
            _berry.Render();
            Position = position;
        }
    }
}
