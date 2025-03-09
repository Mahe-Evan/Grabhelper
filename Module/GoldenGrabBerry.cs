using System;
using System.Collections.Immutable;
using System.Collections;
using Celeste.Mod.Entities;
using Celeste.Mod.Grabhelper;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Grabhelper {

    [CustomEntity("Grabhelper/GoldenGrabBerry")]
    [Tracked]
	[RegisterStrawberry(false, true)]
    public class GoldenGrabBerry : Entity, IStrawberry {


		public GoldenGrabBerry(EntityData data, Vector2 offset, EntityID gid) : base(data.Position + offset)
		{
			ReturnHomeWhenLost = true;
			ID = gid;
			this.start = this.Position;
			this.isfollowing = false;
			Golden = true;
			isGhostBerry = SaveData.Instance.CheckStrawberry(ID);
			Logger.Info("GoldenGrabBerry in Constructor: ", isGhostBerry.ToString());
			base.Depth = -100;
			base.Collider = new Hitbox(14f, 14f, -7f, -7f);
			base.Add(new PlayerCollider(new Action<Player>(OnPlayer), null, null));
			base.Add(new MirrorReflection());
			base.Add(Follower = new Follower(ID, null, new Action(OnLoseLeader)));
			Follower.FollowDelay = 0.3f;
		}

		public override void Added(Scene scene)
		{
			base.Added(scene);
			if (SaveData.Instance.CheckStrawberry(ID))
			{
				Logger.Info("GoldenGrabBerry in Added: ", isGhostBerry.ToString());
				sprite = GFX.SpriteBank.Create("goldghostberry");
				sprite.Color = Color.White * 0.8f;
			}
			else
			{
				sprite = GFX.SpriteBank.Create("goldberry");
			}
			base.Add(sprite);
			// if (this.Winged)
			// {
			// 	sprite.Play("flap", false, false);
			// }
			sprite.OnFrameChange = new Action<string>(OnAnimate);
			base.Add(wiggler = Wiggler.Create(0.4f, 4f, delegate(float v)
			{
				sprite.Scale = Vector2.One * (1f + v * 0.35f);
			}, false, false));
			base.Add(rotateWiggler = Wiggler.Create(0.5f, 4f, delegate(float v)
			{
				sprite.Rotation = v * 30f * 0.017453292f;
			}, false, false));
			base.Add(bloom = new BloomPoint((Golden || isGhostBerry) ? 0.5f : 1f, 12f));
			base.Add(light = new VertexLight(Color.White, 1f, 16, 24));
			base.Add(lightTween = light.CreatePulseTween());
			if ((scene as Level).Session.BloomBaseAdd > 0.1f)
			{
				bloom.Alpha *= 0.5f;
			}
		}

		public override void Update()
		{
			if (!collected)
			{
				if (GrabhelperModule.isGrabbing)
				{
					OnGrab();
				}
				wobble += Engine.DeltaTime * 4f; //possible bug here
				sprite.Y = (bloom.Y = (light.Y = (float)Math.Sin((double)wobble) * 2f));
				int followIndex = Follower.FollowIndex;
				if (Follower.Leader != null && Follower.DelayTimer <= 0f && StrawberryRegistry.IsFirstStrawberry(this) && !GrabhelperModule.isGrabbing)
				{
					Player player = Follower.Leader.Entity as Player;
					bool flag = false;
					if (player != null && player.Scene != null && !player.StrawberriesBlocked)
					{
						if (player.CollideCheck<GoldBerryCollectTrigger>() || (base.Scene as Level).Completed)
						{
							flag = true;
						}
					}
					if (flag)
					{
						collectTimer += Engine.DeltaTime;
						if (collectTimer > 0.15f)
						{
							OnCollect();
						}
					}
					else
					{
						collectTimer = Math.Min(collectTimer, 0f);
					}
				}
				else
				{
					if (followIndex > 0)
					{
						collectTimer = -0.15f;
					}
					if (!isfollowing) {
						base.Y += flapSpeed * Engine.DeltaTime;
						if (flyingAway)
						{
							if (base.Y < (float)(base.SceneAs<Level>().Bounds.Top - 16))
							{
								base.RemoveSelf();
							}
						}
						else
						{
							flapSpeed = Calc.Approach(flapSpeed, 20f, 170f * Engine.DeltaTime);
							if (base.Y < start.Y - 5f)
							{
								base.Y = start.Y - 5f;
							}
							else if (base.Y > start.Y + 5f)
							{
								base.Y = start.Y + 5f;
							}
						}
					}
				}
			}

			base.Update();
			if (Follower.Leader != null && base.Scene.OnInterval(0.08f))
			{
				ParticleType particleType;
				if (isGhostBerry)
				{
					particleType = Strawberry.P_GhostGlow;
				}
				else if (Golden)
				{
					particleType = Strawberry.P_GoldGlow;
				}
				else
				{
					particleType = Strawberry.P_Glow;
				}
				base.SceneAs<Level>().ParticlesFG.Emit(particleType, Position + Calc.Random.Range(-Vector2.One * 6f, Vector2.One * 6f));
			}
		}

		private void OnGrab()
		{
			if (!flyingAway)
			{
				base.Depth = -1000000;
				base.Add(new Coroutine(FlyAwayRoutine(), true));
				flyingAway = true;
			}
		}

		private bool IsFirstStrawberry
		{
			get
			{
				for (int i = Follower.FollowIndex - 1; i >= 0; i--)
				{
					Strawberry strawberry = Follower.Leader.Followers[i].Entity as Strawberry;
					if (strawberry != null && !strawberry.Golden)
					{
						return false;
					}
				}
				return true;
			}
		}

		private void OnAnimate(string id)
		{
			if (!flyingAway && id == "flap" && sprite.CurrentAnimationFrame % 9 == 4)
			{
				Audio.Play("event:/game/general/strawberry_wingflap", Position);
				flapSpeed = -50f;
			}
			int num;
			if (id == "flap")
			{
				num = 25;
			}
			else if (Golden)
			{
				num = 30;
			}
			else
			{
				num = 35;
			}
			if (sprite.CurrentAnimationFrame == num)
			{
				lightTween.Start();
				if (!collected && (base.CollideCheck<FakeWall>() || base.CollideCheck<Solid>()))
				{
					Audio.Play("event:/game/general/strawberry_pulse", Position);
					base.SceneAs<Level>().Displacement.AddBurst(Position, 0.6f, 4f, 28f, 0.1f, null, null);
					return;
				}
				Audio.Play("event:/game/general/strawberry_pulse", Position);
				base.SceneAs<Level>().Displacement.AddBurst(Position, 0.6f, 4f, 28f, 0.2f, null, null);
			}
		}

		public void OnPlayer(Player player)
		{
			if (Follower.Leader == null && !collected)
			{
				ReturnHomeWhenLost = true;
				if (!isfollowing) {
					Level level = base.SceneAs<Level>();

					sprite.Rate = 0f;
					Alarm.Set(this, Follower.FollowDelay, delegate
					{
						sprite.Rate = 1f;
						sprite.Play("idle", false, false);
						level.Particles.Emit(Strawberry.P_WingsBurst, 8, Position + new Vector2(8f, 0f), new Vector2(4f, 2f));
						level.Particles.Emit(Strawberry.P_WingsBurst, 8, Position - new Vector2(8f, 0f), new Vector2(4f, 2f));
					}, Alarm.AlarmMode.Oneshot);
					isfollowing = true;
				}
				(base.Scene as Level).Session.GrabbedGolden = true;
				Audio.Play(isGhostBerry ? "event:/game/general/strawberry_blue_touch" : "event:/game/general/strawberry_touch", Position);
				player.Leader.GainFollower(Follower);
				wiggler.Start();
				base.Depth = -1000000;
			}
		}

		public void OnCollect()
		{
			if (collected)
			{
				return;
			}
			int num = 0;
			collected = true;
			if (Follower.Leader != null)
			{
				Player player = Follower.Leader.Entity as Player;
				num = player.StrawberryCollectIndex;
				player.StrawberryCollectIndex++;
				player.StrawberryCollectResetTimer = 2.5f;
				Follower.Leader.LoseFollower(Follower);
			}
			SaveData.Instance.AddStrawberry(ID, Golden);
			Session session = (base.Scene as Level).Session;
			session.DoNotLoad.Add(ID);
			session.Strawberries.Add(ID);
			session.UpdateLevelStartDashes();
			base.Add(new Coroutine(CollectRoutine(num), true));
			Level level = base.Scene as Level;
		}

		private IEnumerator FlyAwayRoutine()
		{
			rotateWiggler.Start();
			flapSpeed = -200f;
			isfollowing = false;
			Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 0.25f, true);
			tween.OnUpdate = delegate(Tween t)
			{
				flapSpeed = MathHelper.Lerp(-200f, 0f, t.Eased);
			};
			Add(tween);
			yield return 0.1f;
			Audio.Play("event:/game/general/strawberry_laugh", Position);
			yield return 0.2f;
			if (!Follower.HasLeader)
			{
				Audio.Play("event:/game/general/strawberry_flyaway", Position);
			}
			tween = Tween.Create(Tween.TweenMode.Oneshot, null, 0.5f, true);
			tween.OnUpdate = delegate(Tween t)
			{
				flapSpeed = MathHelper.Lerp(0f, -200f, t.Eased);
			};
			Add(tween);
			yield break;
		}

		private IEnumerator CollectRoutine(int collectIndex)
		{
			Scene scene = Scene;
			Tag = Tags.TransitionUpdate;
			Depth = -2000010;
			int num = 0;
			if (isGhostBerry)
			{
				num = 1;
			}
			else if (Golden)
			{
				num = 2;
			}
			Audio.Play("event:/game/general/strawberry_get", Position, "colour", (float)num, "count", (float)collectIndex);
			Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
			sprite.Play("collect", false, false);
			while (sprite.Animating)
			{
				yield return null;
			}
			Scene.Add(new StrawberryPoints(Position, isGhostBerry, collectIndex, Golden));
			RemoveSelf();
			yield break;
		}

		private void OnLoseLeader()
		{
			if (!collected && ReturnHomeWhenLost)
			{
				Alarm.Set(this, 0.15f, delegate
				{
					Vector2 vector = (start - Position).SafeNormalize();
					float num = Vector2.Distance(Position, start);
					float num2 = Calc.ClampedMap(num, 16f, 120f, 16f, 96f);
					Vector2 vector2 = start + vector * 16f + vector.Perpendicular() * num2 * (float)Calc.Random.Choose(1, -1);
					SimpleCurve curve = new SimpleCurve(Position, start, vector2);
					Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineOut, MathHelper.Max(num / 100f, 0.4f), true);
					tween.OnUpdate = delegate(Tween f)
					{
						Position = curve.GetPoint(f.Eased);
					};
					tween.OnComplete = delegate(Tween f)
					{
						base.Depth = 0;
					};
					base.Add(tween);
				}, Alarm.AlarmMode.Oneshot);
			}
		}

		public bool isfollowing;


		public static ParticleType P_Glow;

		public static ParticleType P_GhostGlow;

		public static ParticleType P_GoldGlow;

		public static ParticleType P_MoonGlow;

		public static ParticleType P_WingsBurst;

		public EntityID ID;

		private Sprite sprite;

		public Follower Follower;

		private Wiggler wiggler;

		private Wiggler rotateWiggler;

		private BloomPoint bloom;

		private VertexLight light;

		private Tween lightTween;

		private float wobble;

		private Vector2 start;

		private float collectTimer;

		private bool collected;

		public bool Golden { get; private set; }

		private bool isGhostBerry;

		private bool flyingAway;

		private float flapSpeed;

		public bool ReturnHomeWhenLost;

    }
}