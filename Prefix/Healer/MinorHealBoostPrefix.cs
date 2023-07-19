using Terraria;
using Terraria.ModLoader;
using ThoriumMod;

namespace ModBridge.Prefix {
	public class MinorHealBoostPrefix : HealerWeaponPrefix {
		public MinorHealBoostPrefix(): base(2f, 0, 1.3f, 1.3f) {
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) {
			base.SetStats(ref damageMult, ref knockbackMult, ref useTimeMult, ref scaleMult, ref shootSpeedMult, ref manaMult, ref critBonus);

			useTimeMult *= 0.9f;
			damageMult *= 0.9f;
		}

		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
			DisplayName.SetDefault("Healy");
		}
	}
}
