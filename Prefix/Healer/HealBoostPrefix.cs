using Terraria;
using Terraria.ModLoader;
using ThoriumMod;

namespace ModBridge.Prefix {
	public class HealBoostPrefix : HealerWeaponPrefix {
		public HealBoostPrefix(): base(1f, 1, 1.5f, 1.5f) {
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) {
			base.SetStats(ref damageMult, ref knockbackMult, ref useTimeMult, ref scaleMult, ref shootSpeedMult, ref manaMult, ref critBonus);

			useTimeMult *= 0.8f;
			damageMult *= 0.8f;
		}
	}
}
