using Celeste.Mod.Entities;
using Celeste.Mod.PlatinumStrawberry.Cutscenes;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Diagnostics;

namespace Celeste.Mod.PlatinumStrawberry.Entities
{
    [CustomEntity("PlatinumStrawberry/PlatinumBadelineBoost")]
    [TrackedAs(typeof(BadelineBoost))]
    class PlatinumBadelineBoost : Entity
    {
        public FMOD.Studio.EventInstance Ch9FinalBoostSfx;

        private static ParticleType P_Ambience = new ParticleType(BadelineBoost.P_Ambience) { };

        private Sprite _sprite;
        private Wiggler _wiggler;
        private VertexLight _light;
        private BloomPoint _bloom;
        private Player _holding;

        public PlatinumBadelineBoost(Vector2 position) : base(position)
        {
            base.Depth = -1000000;
            base.Collider = new Circle(16f);
            Add(new PlayerCollider(OnPlayer));
            Add(_sprite = GFX.SpriteBank.Create("badelineBoost"));
            Add(_light = new VertexLight(Color.White, 0.7f, 12, 20));
            Add(_bloom = new BloomPoint(0.5f, 12f));
            Add(_wiggler = Wiggler.Create(0.4f, 3f, delegate
            {
                _sprite.Scale = Vector2.One * (1f + _wiggler.Value * 0.4f);
            }));
            Add(new CameraLocker(Level.CameraLockModes.BoostSequence, 0f, 160f));
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            if (CollideCheck<FakeWall>())
            {
                base.Depth = -12500;
            }
        }

        private void OnPlayer(Player player)
        {
            Add(new Coroutine(BoostRoutine(player)));
        }

        private IEnumerator BoostRoutine(Player player)
        {
            _holding = player;
            _sprite.Visible = false;
            _sprite.Position = Vector2.Zero;
            Collidable = false;
            Level level = Scene as Level;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Audio.Play("event:/new_content/char/badeline/booster_finalfinal_part1", Position);
            if (player.Holding != null)
            {
                player.Drop();
            }
            player.StateMachine.State = 11;
            player.DummyAutoAnimate = false;
            player.DummyGravity = false;
            if (player.Inventory.Dashes > 1)
            {
                player.Dashes = 1;
            }
            else
            {
                player.RefillDash();
            }
            player.RefillStamina();
            player.Speed = Vector2.Zero;
            int num = Math.Sign(player.X - X);
            if (num == 0)
            {
                num = -1;
            }
            BadelineDummy badeline = new BadelineDummy(Position);
            Scene.Add(badeline);
            player.Facing = (Facings)(-num);
            badeline.Sprite.Scale.X = num;
            Vector2 playerFrom = player.Position;
            Vector2 playerTo = Position + new Vector2(num * 4, -3f);
            Vector2 badelineFrom = badeline.Position;
            Vector2 badelineTo = Position + new Vector2(-num * 4, 3f);
            for (float p = 0f; p < 1f; p += Engine.DeltaTime / 0.2f)
            {
                Vector2 vector = Vector2.Lerp(playerFrom, playerTo, p);
                if (player.Scene != null)
                {
                    player.MoveToX(vector.X);
                }
                if (player.Scene != null)
                {
                    player.MoveToY(vector.Y);
                }
                badeline.Position = Vector2.Lerp(badelineFrom, badelineTo, p);
                yield return null;
            }
            Vector2 screenSpaceFocusPoint = new Vector2(Calc.Clamp(player.X - level.Camera.X, 120f, 200f), Calc.Clamp(player.Y - level.Camera.Y, 60f, 120f));
            Add(new Coroutine(level.ZoomTo(screenSpaceFocusPoint, 1.5f, 0.18f)));
            Engine.TimeRate = 0.5f;
            badeline.Sprite.Play("boost");
            yield return 0.1f;
            if (!player.Dead)
            {
                player.MoveV(5f);
            }
            yield return 0.1f;
                Scene.Add(new CS9D_FinalLaunch(player, this));
                player.Active = false;
                badeline.Active = false;
                Active = false;
                yield return null;
                player.Active = true;
                badeline.Active = true;
            Add(Alarm.Create(Alarm.AlarmMode.Oneshot, delegate
            {
                if (player.Dashes < player.Inventory.Dashes)
                {
                    player.Dashes++;
                }
                Scene.Remove(badeline);
                (Scene as Level).Displacement.AddBurst(badeline.Position, 0.25f, 8f, 32f, 0.5f);
            }, 0.15f, start: true));
            (Scene as Level).Shake();
            _holding = null;
                Ch9FinalBoostSfx = Audio.Play("event:/new_content/char/badeline/booster_finalfinal_part2", Position);
                Engine.FreezeTimer = 0.1f;
                yield return null;
                Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
                level.Flash(Color.White * 0.5f, drawPlayerOver: true);
                level.DirectionalShake(-Vector2.UnitY, 0.6f);
                level.Displacement.AddBurst(Center, 0.6f, 8f, 64f, 0.5f);
                level.ResetZoom();
                player.SummitLaunch(X);
                Engine.TimeRate = 1f;
                Finish();
        }

        public void Wiggle()
        {
            _wiggler.Start();
            (base.Scene as Level).Displacement.AddBurst(Position, 0.3f, 4f, 16f, 0.25f);
            Audio.Play("event:/game/general/crystalheart_pulse", Position);
        }

        public override void Update()
        {
            if (_sprite.Visible && base.Scene.OnInterval(0.05f))
            {
                SceneAs<Level>().ParticlesBG.Emit(P_Ambience, 1, base.Center, Vector2.One * 3f);
            }
            if (_holding != null)
            {
                _holding.Speed = Vector2.Zero;
            }
            Player entity = base.Scene.Tracker.GetEntity<Player>();
            if (entity != null)
            {
                float num = Calc.ClampedMap((entity.Center - Position).Length(), 16f, 64f, 12f, 0f);
                Vector2 vector = (entity.Center - Position).SafeNormalize();
                _sprite.Position = Calc.Approach(_sprite.Position, vector * num, 32f * Engine.DeltaTime);
            }
            _light.Visible = (_bloom.Visible = _sprite.Visible);
            base.Update();
        }

        private void Finish()
        {
            SceneAs<Level>().Displacement.AddBurst(base.Center, 0.5f, 24f, 96f, 0.4f);
            SceneAs<Level>().Particles.Emit(BadelineOldsite.P_Vanish, 12, base.Center, Vector2.One * 6f);
            SceneAs<Level>().CameraLockMode = Level.CameraLockModes.None;
            SceneAs<Level>().CameraOffset = new Vector2(0f, -16f);
            RemoveSelf();
        }
    }
}
