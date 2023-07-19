using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using ThoriumMod;

namespace ModBridge.DamageClasses {
	public class ConvertedRadiantDamage : DamageClass {
		public static ConvertedRadiantDamage Instance => ModContent.GetInstance<ConvertedRadiantDamage>();

		public override StatInheritanceData GetModifierInheritance(DamageClass damageClass) {
			if (damageClass == DamageClass.Generic || damageClass == ThoriumDamageBase<HealerDamage>.Instance)
				return StatInheritanceData.Full;

			return new StatInheritanceData(
				damageInheritance: 0f,
				critChanceInheritance: 0f,
				attackSpeedInheritance: 0f,
				armorPenInheritance: 0f,
				knockbackInheritance: 0f
			);
		}

		public override bool GetEffectInheritance(DamageClass damageClass) {
			return damageClass == ThoriumDamageBase<HealerDamage>.Instance;
		}

		public override bool UseStandardCritCalcs => ThoriumDamageBase<HealerDamage>.Instance.UseStandardCritCalcs;

		public override void SetStaticDefaults() {
			base.ClassName.SetDefault(ThoriumDamageBase<HealerDamage>.Instance.ClassName.GetDefault());
		}
	}
}
