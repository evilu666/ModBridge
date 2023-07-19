using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using ModBridge.Dusts;

namespace ModBridge.Projectiles {

	public class ShortLaserProjectile : ModProjectile {
		public override string Texture => "ThoriumMod/Empty";

		private static SoundStyle HitSound = new SoundStyle("ModBridge/Sounds/Projectiles/Zap_", 2) with {
			MaxInstances = 8
		};

		private static SoundStyle ReflectSound = new SoundStyle("ModBridge/Sounds/Projectiles/LaserReflect_1") with {
			MaxInstances = 2
		};

		private static int baseTime = 360;
		private static int penetrateTiles = 8;
		public static Color[] colors = new Color[] {
			new Color(22, 206, 34),
			new Color(22, 206, 120),
			new Color(22, 206, 188),
			new Color(22, 169, 206),
			new Color(22, 44, 206),
			new Color(87, 22, 206),
			new Color(145, 22, 206),
			new Color(145, 22, 206),
			new Color(182, 22, 206),
			new Color(206, 22, 145),
			new Color(206, 22, 71),
		};

		public override void SetDefaults() {
			base.Projectile.width = 8;
			base.Projectile.height = 8;
			base.Projectile.aiStyle = -1;
			base.Projectile.friendly = true;
			base.Projectile.DamageType = DamageClass.Ranged;
			base.Projectile.penetrate = 8;
			base.Projectile.timeLeft = baseTime;
			base.Projectile.tileCollide = true;
			base.Projectile.extraUpdates = 7;
			base.Projectile.ai[0] = 1.30f;
			base.Projectile.ai[1] = 0.0f;
		}

		public override void AI() {
			if (base.Projectile.timeLeft == baseTime - 4) {
				float circleSegments = 15f;
				for (int circleSegment = 0; (float)circleSegment < circleSegments; circleSegment++) {
					Vector2 vector12 = Vector2.UnitX * 0f;
					vector12 += -Vector2.UnitY.RotatedBy((float)circleSegment * ((float)Math.PI * 2f / circleSegments)) * new Vector2(2f, 6f);
					vector12 = vector12.RotatedBy(base.Projectile.velocity.ToRotation());
					int dustIndex = Dust.NewDust(base.Projectile.Center, 0, 0, ModContent.DustType<RayParticle>(), 0f, 0f, 225, default(Color), 1.5f);
					Main.dust[dustIndex].shader = GameShaders.Armor.GetSecondaryShader(10, Main.LocalPlayer).UseColor(40, 226, 53);
					Main.dust[dustIndex].noGravity = true;
					Main.dust[dustIndex].position = base.Projectile.Center + vector12;
					Main.dust[dustIndex].velocity = base.Projectile.velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * 1f;
					Main.dust[dustIndex].scale = 0.7f;
				}
			} else if (base.Projectile.timeLeft < baseTime - 4) { 
				int colorIndex = 8 - Projectile.penetrate;
				if (colorIndex >= colors.Length) colorIndex = colors.Length - 1;
				Color c = colors[colorIndex];

				for (int num105 = 0; num105 < 4; num105++) {
					Vector2 vector13 = base.Projectile.Center;
					vector13 -= base.Projectile.velocity * ((float)num105 * 0.25f);
					int num106 = Dust.NewDust(vector13, 1, 1, ModContent.DustType<RayParticle>(), 0f, 0f, 225);
					Main.dust[num106].shader = GameShaders.Armor.GetSecondaryShader(10, Main.LocalPlayer).UseColor(c.R, c.G, c.B);
					Main.dust[num106].noGravity = true;
					Main.dust[num106].position = vector13;
					Main.dust[num106].scale = 0.5f;
					Dust obj = Main.dust[num106];
					obj.velocity *= 0.2f;
				}
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity) {

			if (Projectile.penetrate <= 0) {
				Projectile.Kill();
			} else {
				Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
				SoundEngine.PlaySound(ReflectSound, Projectile.position);

				// If the projectile hits the left or right side of the tile, reverse the X velocity
				if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon) {
					Projectile.velocity.X = -oldVelocity.X;
				}

				// If the projectile hits the top or bottom side of the tile, reverse the Y velocity
				if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon) {
					Projectile.velocity.Y = -oldVelocity.Y;
				}

				Projectile.penetrate--;
			}

			return false;
		}

		public override void OnHitNPC(NPC target, int damage, float knockBack, bool crit) {
			damage = (int) ((float) damage * (Math.Pow(Projectile.ai[0], 8 - Projectile.penetrate)));

			base.OnHitNPC(target, damage, knockBack, crit);
			SoundEngine.PlaySound(HitSound, Projectile.position);
		}
	}
}

