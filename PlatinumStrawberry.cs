using Celeste.Mod.Entities;
using Celeste.Mod.PlatinumStrawberry.Triggers;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.PlatinumStrawberry.Entities
{
    [CustomEntity("PlatinumStrawberry/PlatinumStrawberry")]
    [RegisterStrawberry(false, true)]
    [TrackedAs(typeof(Strawberry))]
    class PlatinumBerry : Entity, IStrawberry
    {
        public bool Collected;
        public bool Dead = false;
        public bool Golden = true;
        public bool ReturnHomeWhenLost = true;
        public EntityID ID;
        public Follower Follower;
        public static ParticleType P_Glow = Strawberry.P_Glow;
        public static ParticleType P_GhostGlow = Strawberry.P_GhostGlow;

        private float _wobble = 0f;
        private float _collectTimer = 0f;
        private bool _isGhostBerry;
        private bool _commandSpawned;
        private Vector2 _start;
        private Sprite _sprite;
        private Wiggler _wiggler;
        private Wiggler _rotateWiggler;
        private BloomPoint _bloom;
        private VertexLight _light;
        private Tween _lightTween;

        public PlatinumBerry(EntityData data, Vector2 offset, EntityID gid)
        {
            ID = gid;
            Position = (_start = data.Position + offset);
            _isGhostBerry = SaveData.Instance.CheckStrawberry(ID);
            base.Depth = -100;
            base.Collider = new Hitbox(14f, 14f, -7f, -7f);
            Add(new PlayerCollider(OnPlayer));
            Add(new MirrorReflection());
            Add(Follower = new Follower(ID, null, OnLoseLeader));
            Follower.FollowDelay = 0.3f;
            _sprite = GFX.SpriteBank.Create("platinumberry");
            _wiggler = Wiggler.Create(0.4f, 4f, delegate (float v) { _sprite.Scale = Vector2.One * (1f + v * 0.35f); });
            _rotateWiggler = Wiggler.Create(0.5f, 4f, delegate (float v) { _sprite.Rotation = v * 30f * ((float)Math.PI / 180f); });
            _bloom = new BloomPoint(0.5f, 12f);
            _light = new VertexLight(Color.White, 1f, 16, 24);
            _lightTween = _light.CreatePulseTween();
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Add(_sprite);
            if (!_isGhostBerry) _sprite.Play("idlePlat");
            else _sprite.Play("idleGhost");
            _sprite.OnFrameChange = OnAnimate;
            Add(_wiggler);
            Add(_rotateWiggler);
            Add(_bloom);
            Add(_light);
            Add(_lightTween);
        }

        public override void Update()
        {
            if (!Collected)
            {
                _wobble += Engine.DeltaTime * 4f;
                Sprite obj = _sprite;
                BloomPoint bloomPoint = _bloom;
                float num2 = (_light.Y = (float)Math.Sin(_wobble) * 2f);
                float num5 = (obj.Y = (bloomPoint.Y = num2));
                int followIndex = Follower.FollowIndex;
                if (Follower.Leader != null && Follower.DelayTimer <= 0f && StrawberryRegistry.IsFirstStrawberry(this))
                {
                    Player player = Follower.Leader.Entity as Player;
                    if (player != null && player.Scene != null && !player.StrawberriesBlocked)
                    {
                        if (player.CollideCheck<PlatBerryCollectTrigger>() || ((Level)base.Scene).Completed)
                        {
                            _collectTimer += Engine.DeltaTime;
                            if (_collectTimer > 0.15f) OnCollect();
                        }
                        else _collectTimer = Math.Min(_collectTimer, 0f);
                    }
                }
                else if (followIndex > 0) _collectTimer = -0.15f;
            }
            base.Update();
            if (Follower.Leader != null && base.Scene.OnInterval(0.08f))
            {
                ParticleType type = (_isGhostBerry ? P_GhostGlow : P_Glow);
                SceneAs<Level>().ParticlesFG.Emit(type, Position + Calc.Random.Range(-Vector2.One * 6f, Vector2.One * 6f));
            }
        }

        private void OnAnimate(string id)
        {
            if (_sprite.CurrentAnimationFrame == 30)
            {
                _lightTween.Start();
                if (!Collected && (CollideCheck<FakeWall>() || CollideCheck<Solid>()))
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
            if (Follower.Leader != null || Collected) return;
            ReturnHomeWhenLost = true;
            Audio.Play(_isGhostBerry ? "event:/game/general/strawberry_blue_touch" : "event:/game/general/strawberry_touch", Position);
            player.Leader.GainFollower(Follower);
            _wiggler.Start();
            base.Depth = -1000000;
        }

        public void OnCollect()
        {
            if (!Collected)
            {
                int collectIndex = 0;
                Collected = true;
                if (Follower.Leader != null)
                {
                    Player obj = Follower.Leader.Entity as Player;
                    collectIndex = obj.StrawberryCollectIndex;
                    obj.StrawberryCollectIndex++;
                    obj.StrawberryCollectResetTimer = 2.5f;
                    Follower.Leader.LoseFollower(Follower);
                }
                Session session = ((Level)base.Scene).Session;
                if (!_commandSpawned)
                {
                    SaveData.Instance.AddStrawberry(ID, Golden);
                    session.DoNotLoad.Add(ID);
                    session.Strawberries.Add(ID);
                }
                session.UpdateLevelStartDashes();
                Add(new Coroutine(CollectRoutine(collectIndex)));
            }
        }

        private void OnLoseLeader()
        {
            foreach (Player player in Scene.Entities.FindAll<Player>())
            {
                if (player.Dead)
                {
                    Audio.Play("event:/new_content/char/madeline/death_golden");
                    if (!_commandSpawned && PlatinumModule.Instance.Settings.DefaultPlatinumBerryRespawnBehavior) Dead = true;
                }
            }
            if (Collected || !ReturnHomeWhenLost) return;
            Alarm.Set(this, 0.15f, delegate
            {
                Vector2 vector = (_start - Position).SafeNormalize();
                float num = Vector2.Distance(Position, _start);
                float num2 = Calc.ClampedMap(num, 16f, 120f, 16f, 96f);
                Vector2 control = _start + vector * 16f + vector.Perpendicular() * num2 * Calc.Random.Choose(1, -1);
                SimpleCurve curve = new SimpleCurve(Position, _start, control);
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
            if (_isGhostBerry) num = 1;
            Audio.Play("event:/game/general/strawberry_get", Position, "colour", num, "count", 10);
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
            _sprite.Play("collect");
            while (_sprite.Animating) yield return null;
            Scene.Add(new PlatberryPoints(Position, _isGhostBerry));
            RemoveSelf();
        }

        [Command("give_plat", "gives you a platinum strawberry")]
        private static void cmdGivePlat()
        {
            if (Engine.Scene is Level level)
            {
                Player player = level.Tracker.GetEntity<Player>();
                if (player != null)
                {
                    EntityData entityData = new EntityData();
                    entityData.Position = player.Position + new Vector2(0f, -16f);
                    entityData.ID = Calc.Random.Next();
                    entityData.Name = "PlatinumStrawberry/PlatinumStrawberry";
                    PlatinumBerry platBerry = new PlatinumBerry(entityData, Vector2.Zero, new EntityID(level.Session.Level, entityData.ID));
                    platBerry._commandSpawned = true;
                    level.Add(platBerry);
                }
            }
        }
    }
}