using Microsoft.Xna.Framework;
using ModBridge.Items.LivingWeapon;

namespace ModBridge.Projectiles.Scythe {
	public class AncientLivingBatonProjectile : ScytheLivingWeaponProjectile {

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Ancient Living Baton");
		}

		public override void SetDefaults() {
			base.SetDefaults();

			Projectile.width = 64;
			Projectile.height = 64;
		}

	}
}
