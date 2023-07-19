using Terraria;
using Terraria.ModLoader;

using ModBridge.Items.LivingWeapon;

namespace ModBridge.Projectiles {
	public class WulfrumScattergunProjectile : LivingWeaponProjectile<RangedLivingWeapon, RangedLivingWeaponLevelInfo, RangedLivingWeaponEvolutionData> {
		public override void SetDefaults() {
			Projectile.width = 44;
			Projectile.height = 12;
			Projectile.scale = 0.5f;
			Projectile.aiStyle = 0;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = 16;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = true;
		}
	}
}
