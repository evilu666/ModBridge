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

	public class DisparagerProjectile : ModProjectile {
		public override string Texture => "ThoriumMod/Empty";

		private static int baseTime = 240;
		private static int penetrateTiles = 8;

		public override void SetDefaults() {
			base.Projectile.width = 24;
			base.Projectile.height = 24;
			base.Projectile.aiStyle = -1;
			base.Projectile.friendly = true;
			base.Projectile.DamageType = DamageClass.Ranged;
			base.Projectile.penetrate = -1;
			base.Projectile.timeLeft = baseTime;
			base.Projectile.extraUpdates = 15;
			base.Projectile.tileCollide = false;
		}

		public override void AI() {
			if (base.Projectile.timeLeft == baseTime - 12) {
				float circleSegments = 31f;
				for (int circleSegment = 0; (float)circleSegment < circleSegments; circleSegment++) {
					Vector2 vector12 = Vector2.UnitX * 0f;
					vector12 += -Vector2.UnitY.RotatedBy((float)circleSegment * ((float)Math.PI * 2f / circleSegments)) * new Vector2(3f, 9f);
					vector12 = vector12.RotatedBy(base.Projectile.velocity.ToRotation());
					int dustIndex = Dust.NewDust(base.Projectile.Center, 0, 0, ModContent.DustType<RayParticle>(), 0f, 0f, 225, default(Color), 1.5f);
					Main.dust[dustIndex].shader = GameShaders.Armor.GetSecondaryShader(10, Main.LocalPlayer).UseColor(39, 204, 229);
					Main.dust[dustIndex].noGravity = true;
					Main.dust[dustIndex].position = base.Projectile.Center + vector12;
					Main.dust[dustIndex].velocity = base.Projectile.velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * 1f;
					Main.dust[dustIndex].scale = 0.8f;
				}
			} else if (base.Projectile.timeLeft < baseTime - 12) { 
				for (int num105 = 0; num105 < 4; num105++) {
					Vector2 vector13 = base.Projectile.Center;
					vector13 -= base.Projectile.velocity * ((float)num105 * 0.25f);
					int num106 = Dust.NewDust(vector13, 1, 1, ModContent.DustType<RayParticle>(), 0f, 0f, 225);
					Main.dust[num106].shader = GameShaders.Armor.GetSecondaryShader(10, Main.LocalPlayer).UseColor(39, 150, 229);
					Main.dust[num106].noGravity = true;
					Main.dust[num106].position = vector13;
					Main.dust[num106].scale = 1.5f;
					Dust obj = Main.dust[num106];
					obj.velocity *= 0.2f;
				}
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity) {
			if (base.Projectile.penetrate < -penetrateTiles * base.Projectile.extraUpdates) {
				SoundEngine.PlaySound(in SoundID.Item10, base.Projectile.position);
				for (int i = 0; i < 10; i++) {
					int DustID = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, 15, base.Projectile.velocity.X * 0.2f, base.Projectile.velocity.Y * 0.2f, 225, default(Color), 1.25f);
					Main.dust[DustID].shader = GameShaders.Armor.GetSecondaryShader(10, Main.LocalPlayer);
					Main.dust[DustID].noGravity = true;
				}
				return true;
			} else {
				base.Projectile.penetrate -= 1;
				return false;
			}
		}
	}
}

