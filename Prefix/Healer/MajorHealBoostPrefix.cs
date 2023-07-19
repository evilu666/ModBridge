using Terraria;
using Terraria.ModLoader;
using ThoriumMod;

namespace ModBridge.Prefix {
	public class MajorHealBoostPrefix : HealerWeaponPrefix {
		public MajorHealBoostPrefix(): base(0.5f, 1, 1.8f, 2.0f) {
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) {
			base.SetStats(ref damageMult, ref knockbackMult, ref useTimeMult, ref scaleMult, ref shootSpeedMult, ref manaMult, ref critBonus);

			useTimeMult *= 0.5f;
		}

		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
			DisplayName.SetDefault("Heal Boosting");
		}
	}
}
