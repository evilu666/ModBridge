using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using ThoriumMod.Projectiles.Scythe;

namespace ModBridge.Global {
	public class ScytheProjectileHitboxHandler : GlobalProjectile {

		public override void ModifyDamageHitbox(Projectile projectile, ref Rectangle hitBox) {
			if (projectile.ModProjectile is ScythePro pro) {
				int width = (int) (((float) projectile.width + 16f) * projectile.scale);
				int height = (int) (((float) projectile.height + 16f) * projectile.scale);

				hitBox = new Rectangle((int) projectile.Center.X - width/2, (int) projectile.Center.Y - height/2, width, height);
			}
		}
	}
}
