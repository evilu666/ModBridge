using Terraria;
using Terraria.ModLoader;

namespace ModBridge.Items.LivingWeapon {
	public abstract class LivingWeaponProjectile<T, L, E> : ModProjectile 
			where L : LivingWeaponLevelInfo
			where E : LivingWeaponEvolutionData<L>
			where T : BaseLivingWeapon<L, E> {

		public T Weapon;
		public Player Owner;

		public override void OnHitNPC(NPC target, int damage, float knockBack, bool crit) {
			Weapon.OnHitNPC(Owner, target, damage, knockBack, crit);
		}
	}
}
