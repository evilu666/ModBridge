using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using ThoriumMod;
using System;
using System.Collections.Generic;
using ModBridge.Global;
using ThoriumMod.Items.HealerItems;

namespace ModBridge.Prefix {

	public abstract class HealerWeaponPrefix : ModPrefix {

		private float rollChance = 2;
		private int soulEssenceBonus = 0;
		private float healMult = 1.0f;
		private float? scaleMult = null;

		public int SoulEssenceBonus {
			get => soulEssenceBonus;
		}

		public float HealMultiplier {
			get => healMult;
		}

		public static List<int> ALL = new List<int>();

		public HealerWeaponPrefix(float rollChance, int soulEssenceBonus, float healMult, float? scaleMult = null) {
			this.rollChance = rollChance;
			this.soulEssenceBonus = soulEssenceBonus;
			this.healMult = healMult;
			this.scaleMult = scaleMult;

			HealerWeaponPrefix.ALL.Add(this.Type);
		}

		public override PrefixCategory Category => PrefixCategory.AnyWeapon;

		public override bool CanRoll(Item item) {
			if (item.healLife > 0 && healMult != 1.0f) return true;
			if (item.ModItem is ScytheItem && scaleMult != null) return true;
			return soulEssenceBonus > 0 && item.DamageType != null && item.DamageType.CountsAsClass(ThoriumDamageBase<HealerDamage>.Instance);
		}

		public override float RollChance(Item item) {
			return rollChance;
		}

		public override void SetStaticDefaults() {
			GlobalItemPrefixTooltipHandler.RegisterHandler(this, this.ModifyTooltips);
			if (scaleMult is float mult) ScytheProjectileModficationHandler.RegisterHandler(this, projectile => projectile.scale *= mult);

			DisplayName.SetDefault("Healing");
		}

		public override void SetStats(ref float damageMult, ref float knockBackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) {
			if (this.scaleMult is float mult) {
				scaleMult *= mult;
			}
		}

		private void ModifyTooltips(Item item, List<TooltipLine> lines) {
			if (item.healLife > 0 && healMult != 1.0f) {
				TooltipLine line = new TooltipLine(Mod, "PrefixAccMoveSpeed", String.Format("{0:P2} increased healing", healMult));
				line.IsModifier = true;
				line.IsModifierBad = healMult < 1.0f;
				lines.Add(line);
			}

			if (soulEssenceBonus > 0 && item.DamageType != null && item.DamageType.CountsAsClass(ThoriumDamageBase<HealerDamage>.Instance)) {
				TooltipLine line = new TooltipLine(Mod, "PrefixAccMeleeSpeed", String.Format("{0:+0;-#} soul essence on hit", soulEssenceBonus));
				line.IsModifier = true;
				line.IsModifierBad = soulEssenceBonus < 0;
				lines.Add(line);
			}

			//if (scaleMult != null && item.ModItem is ScytheItem) {
			//	TooltipLine line = new TooltipLine(Mod, "PrefixSize", String.Format("{0:P2} size", soulEssenceBonus));
			//	line.IsModifier = true;
			//	line.IsModifierBad = soulEssenceBonus < 0;
			//	lines.Add(line);
			//}
		}

//				// This prefix doesn't affect any non-standard stats, so these additional tooltiplines aren't actually necessary, but this pattern can be followed for a prefix that does affect other stats.
//		public override IEnumerable<TooltipLine> GetTooltipLines(Item item) {
//			if (item.healLife > 0 && healMult != 1.0f) {
//				yield return new TooltipLine(Mod, "PrefixHealerWeaponHealMult", HealMultTooltip.Format(healMult - 1.0f)) {
//					IsModifier = true,
//				};
//			}
//
//			if (soulEssenceBonus > 0 && item.DamageType != null && item.DamageType.CountsAsClass(ThoriumDamageBase<HealerDamage>.Instance)) {
//				yield return new TooltipLine(Mod, "PrefixHealerWeaponSoulEssenceBonus", SoulEssenceBonusTooltip.Format(soulEssenceBonus)) {
//					IsModifier = true,
//				};
//			}
//		}
//
//		public static LocalizedText HealMultTooltip { get; private set; }
//		public static LocalizedText SoulEssenceBonusTooltip { get; private set; }
//
//		public override void SetStaticDefaults() {
//			// this.GetLocalization is not used here because we want to use a shared key
//			HealMultTooltip = Language.GetOrRegister(Mod.GetLocalizationKey($"{LocalizationCategory}.{nameof(HealMultTooltip)}"));
//			SoulEssenceBonusTooltip = Language.GetOrRegister(Mod.GetLocalizationKey($"{LocalizationCategory}.{nameof(SoulEssenceBonusTooltip)}"));
//		}
	}

}
