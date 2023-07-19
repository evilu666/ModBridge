using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ModBridge.Dusts {
	public class RayParticle : ModDust {

		private static float speed = 8f;

		public override void OnSpawn(Dust dust) {
			dust.noGravity = true; // Makes the dust have no gravity.
			dust.noLight = false; // Makes the dust emit light.
			dust.noLightEmittence = false;
			dust.color = new Color(255, 255, 255, 255);
			dust.velocity = new Vector2(0, 0);
			dust.firstFrame = true;
			dust.scale = 1.0f;

		}

		public override bool Update(Dust dust) { // Calls every frame the dust is active
			if (dust.customData is Vector2 targetPos) {
				Vector2 dir = (targetPos - dust.position);
				Vector2.Normalize(dir);
				dust.velocity = dir * speed;

				if ((targetPos - dust.position).Length() < 1) {
					dust.active = false;
				} else {
					dust.position += dust.velocity;
					Lighting.AddLight(dust.position, 1f, 1f, 1f);
				}
			} else {
				dust.scale -= 0.01f;
				
				Lighting.AddLight(dust.position, dust.scale, dust.scale, dust.scale);
				if (dust.scale < 0.01f) {
					dust.active = false;
				}
			}



			return false;
		}
	}
}
