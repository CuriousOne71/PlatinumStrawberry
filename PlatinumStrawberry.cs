using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.PlatinumStrawberry.Entities
{
    [CustomEntity("PlatinumStrawberry/PlatinumStrawberry")]
    [RegisterStrawberry(false, true)]
    [TrackedAs(typeof(Strawberry))]
    class PlatinumStrawberry : Entity, IStrawberry
    {
        public EntityID ID;
        public bool Golden = true;
        public bool ReturnHomeWhenLost = true;
        public static ParticleType P_Glow = Strawberry.P_Glow;
        public static ParticleType P_GhostGlow = Strawberry.P_GhostGlow;
        public Follower Follower;

        private Sprite sprite;
        private Wiggler wiggler;
        private Wiggler rotateWiggler;
        private BloomPoint bloom;
        private VertexLight light;
        private Tween lightTween;
        private Vector2 start;
        private float wobble = 0f;
        private float collectTimer = 0f;
        private bool collected;
        private bool isGhostBerry;
        private static SpriteBank spriteBank;

        public PlatinumStrawberry(EntityData data, Vector2 offset, EntityID gid)
        {
            ID = gid;
            Position = (start = data.Position + offset);
            isGhostBerry = SaveData.Instance.CheckStrawberry(ID);
            base.Depth = -100;
            base.Collider = new Hitbox(14f, 14f, -7f, -7f);
            Add(new PlayerCollider(OnPlayer));
            Add(new MirrorReflection());
            Add(Follower = new Follower(ID, null, OnLoseLeader));
            Follower.FollowDelay = 0.3f;
            sprite = spriteBank.Create("platinumberry");
            wiggler = Wiggler.Create(0.4f, 4f, delegate (float v) { sprite.Scale = Vector2.One * (1f + v * 0.35f); });
            rotateWiggler = Wiggler.Create(0.5f, 4f, delegate (float v) { sprite.Rotation = v * 30f * ((float)Math.PI / 180f); });
            bloom = new BloomPoint(0.5f, 12f);
            light = new VertexLight(Color.White, 1f, 16, 24);
            lightTween = light.CreatePulseTween();
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Add(sprite);
            if (!isGhostBerry) sprite.Play("normal");
            else sprite.Play("ghost");
            sprite.OnFrameChange = OnAnimate;
            Add(wiggler);
            Add(rotateWiggler);
            Add(bloom);
            Add(light);
            Add(lightTween);
        }

        public override void Update()
        {
            if (!collected)
            {
                wobble += Engine.DeltaTime * 4f;
                Sprite obj = sprite;
                BloomPoint bloomPoint = bloom;
                float num2 = (light.Y = (float)Math.Sin(wobble) * 2f);
                float num5 = (obj.Y = (bloomPoint.Y = num2));
                int followIndex = Follower.FollowIndex;
                if (Follower.Leader != null && Follower.DelayTimer <= 0f && StrawberryRegistry.IsFirstStrawberry(this))
                {
                    Player player = Follower.Leader.Entity as Player;
                    if (player != null && player.Scene != null && !player.StrawberriesBlocked)
                    {
                        if (player.CollideCheck<GoldBerryCollectTrigger>() || ((Level)base.Scene).Completed)
                        {
                            collectTimer += Engine.DeltaTime;
                            if (collectTimer > 0.15f) OnCollect();
                        }
                        else collectTimer = Math.Min(collectTimer, 0f);
                    }
                }
                else if (followIndex > 0) collectTimer = -0.15f;
            }
            base.Update();
            if (Follower.Leader != null && base.Scene.OnInterval(0.08f))
            {
                ParticleType type = (isGhostBerry ? P_GhostGlow : P_Glow );
                SceneAs<Level>().ParticlesFG.Emit(type, Position + Calc.Random.Range(-Vector2.One * 6f, Vector2.One * 6f));
            }
        }

        private void OnAnimate(string id)
        {
            if (sprite.CurrentAnimationFrame == 30)
            {
                lightTween.Start();
                if (!collected && (CollideCheck<FakeWall>() || CollideCheck<Solid>()))
                {
                    Audio.Play("event:/game/general/strawberry_pulse", Position);
                    SceneAs<Level>().Displacement.AddBurst(Position, 0.6f, 4f, 28f, 0.1f);
                }
                else
                {
                    Audio.Play("event:/game/general/strawberry_pulse", Position);
                    SceneAs<Level>().Displacement.AddBurst(Position, 0.6f, 4f, 28f, 0.2f);
                }
            }
        }

        public void OnPlayer(Player player)
        {
            ((Level)base.Scene).Session.GrabbedGolden = true;
            if (Follower.Leader != null || collected) return;
            ReturnHomeWhenLost = true;
            Audio.Play(isGhostBerry ? "event:/game/general/strawberry_blue_touch" : "event:/game/general/strawberry_touch", Position);
            player.Leader.GainFollower(Follower);
            wiggler.Start();
            base.Depth = -1000000;
        }

        public void OnCollect()
        {
            if (!collected)
            {
                int collectIndex = 0;
                collected = true;
                if (Follower.Leader != null)
                {
                    Player obj = Follower.Leader.Entity as Player;
                    collectIndex = obj.StrawberryCollectIndex;
                    obj.StrawberryCollectIndex++;
                    obj.StrawberryCollectResetTimer = 2.5f;
                    Follower.Leader.LoseFollower(Follower);
                }
                SaveData.Instance.AddStrawberry(ID, Golden);
                Session session = ((Level)base.Scene).Session;
                session.DoNotLoad.Add(ID);
                session.Strawberries.Add(ID);
                session.UpdateLevelStartDashes();
                Add(new Coroutine(CollectRoutine(collectIndex)));
            }
        }

        private void OnLoseLeader()
        {
            if (collected || !ReturnHomeWhenLost) return;
            Alarm.Set(this, 0.15f, delegate
            {
                Vector2 vector = (start - Position).SafeNormalize();
                float num = Vector2.Distance(Position, start);
                float num2 = Calc.ClampedMap(num, 16f, 120f, 16f, 96f);
                Vector2 control = start + vector * 16f + vector.Perpendicular() * num2 * Calc.Random.Choose(1, -1);
                SimpleCurve curve = new SimpleCurve(Position, start, control);
                Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineOut, MathHelper.Max(num / 100f, 0.4f), start: true);
                tween.OnUpdate = delegate (Tween f)
                {
                    Position = curve.GetPoint(f.Eased);
                };
                tween.OnComplete = delegate
                {
                    base.Depth = 0;
                };
                Add(tween);
            });
        }

        private IEnumerator CollectRoutine(int collectIndex)
        {
            _ = Scene;
            Tag = Tags.TransitionUpdate;
            Depth = -2000010;
            int num = 2;
            if (isGhostBerry) num = 1;
            Audio.Play("event:/game/general/strawberry_get", Position, "colour", num, "count", collectIndex);
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            sprite.Play("collect");
            while (sprite.Animating) yield return null;
            Scene.Add(new StrawberryPoints(Position, isGhostBerry, collectIndex, false));
            RemoveSelf();
        }
    }
}