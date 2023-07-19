using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

using ThoriumMod;
using ThoriumMod.Items.HealerItems;
using ThoriumMod.Projectiles.Scythe;

using ModBridge.Global;


namespace ModBridge.Items.LivingWeapon {

	public abstract class ScytheLivingWeaponLevelInfo : LivingWeaponLevelInfo {

		public abstract int Damage(int level);
		public abstract float Knockback(int level);
		public abstract float Scale(int level);
		public abstract float RotationSpeed(int level);
		public abstract int SoulEssenceBonus(int level);

	}

	public class ScytheLivingWeaponEvolutionData : LivingWeaponEvolutionData<ScytheLivingWeaponLevelInfo> {
		public int Damage;
		public float Knockback;
		public float RotationSpeed;
		public int SoulEssenceBonus;

		public int Projectile;

		public ScytheLivingWeaponEvolutionData(
				string name,
				int width,
				int height,
				float scale,
				string texture,
				int useTime,
				int holdStyle,
				bool holdRotate,
				Vector2 holdoutOffset,
				SoundStyle useSound,

				int damage,
				float knockback,
				float rotationSpeed,
				int soulEssenceBonus,
				int projectile,

				int levelRequirement,
				ScytheLivingWeaponLevelInfo levelInfo,

				bool isEvolutionConditionHidden,
				string customEvolutionConditionDescription = "",
				EvolutionCondition<ScytheLivingWeaponLevelInfo, LivingWeaponEvolutionData<ScytheLivingWeaponLevelInfo>, BaseLivingWeapon<ScytheLivingWeaponLevelInfo, LivingWeaponEvolutionData<ScytheLivingWeaponLevelInfo>>> customEvolutionCondition = null,
				bool isCustomEvolutionConditionHidden = true)
					: base(name, width, height, scale, texture, useTime, useTime, holdStyle, holdRotate, holdoutOffset, useSound, levelRequirement, levelInfo, isEvolutionConditionHidden, customEvolutionConditionDescription, customEvolutionCondition, isCustomEvolutionConditionHidden) {

			this.Damage = damage;
			this.Knockback = knockback;
			this.RotationSpeed = rotationSpeed;
			this.SoulEssenceBonus = soulEssenceBonus;
			this.Projectile = projectile;
		}
	}

	public abstract class ScytheLivingWeapon : BaseLivingWeapon<ScytheLivingWeaponLevelInfo, ScytheLivingWeaponEvolutionData> {

		protected override void ApplyStaticDefaults() {
			BossLootHandler.RegisterGuaranteedDrop(ItemID.EyeOfCthulhuBossBag, Item.type);
		}

		protected override void ApplyDefaults() {
			Item.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.autoReuse = true;
			Item.useTime = 22;
			Item.useAnimation = 22;
			Item.maxStack = 1;
			Item.knockBack = 6.5f;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.UseSound = SoundID.Item1;
			Item.shootSpeed = 0.1f;
		}

		protected override void ApplyEvolution() {
			base.ApplyEvolution();

			Item.damage = EvolutionData.Damage;
			Item.knockBack = EvolutionData.Knockback;
		}


		protected override void ApplyLevel() {
			base.ApplyLevel();

			Item.damage += LevelInfo.Damage(Level);
			Item.knockBack += LevelInfo.Knockback(Level);
			Item.scale *= LevelInfo.Scale(Level);
		}

		public override bool? UseItem(Player player) {
			base.UseItem(player);

			EntitySource_ItemUse_WithAmmo source = new EntitySource_ItemUse_WithAmmo(player, Item, AmmoID.None);
			int dmg = (int) player.GetDamage(Item.DamageType).ApplyTo(Item.damage);
			float knockback = player.GetKnockback(Item.DamageType).ApplyTo(Item.knockBack);

			Projectile projectile = Projectile.NewProjectileDirect(source, player.Center, player.velocity, EvolutionData.Projectile, dmg, knockback, player.whoAmI);
			projectile.scale = EvolutionData.Scale * LevelInfo.Scale(Level);
			ScytheLivingWeaponProjectile p = (ScytheLivingWeaponProjectile) projectile.ModProjectile;
			p.Weapon = this;
			p.Owner = player;
			p.RotationSpeed = EvolutionData.RotationSpeed * LevelInfo.RotationSpeed(Level);
			p.SoulEssenceBonus = EvolutionData.SoulEssenceBonus + LevelInfo.SoulEssenceBonus(Level);

			return true;
		}

		public override void ModifyTooltips(List<TooltipLine> lines) {
			base.ModifyTooltips(lines);

			ThoriumPlayer healer = Main.LocalPlayer.GetModPlayer<ThoriumPlayer>();
			int index = lines.FindIndex((TooltipLine tt) => tt.Mod.Equals("Terraria") && tt.Name.Equals("ItemName"));
			if (index != -1) {
				lines.Insert(index + 1, new TooltipLine(base.Mod, "HealerTag", "-Healer Class-") {
						OverrideColor = !healer.darkAura ? new Color(255, 255, 91) : new Color(178, 102, 255)
				});
			}

			int soulEssenceBonus = EvolutionData.SoulEssenceBonus + LevelInfo.SoulEssenceBonus(Level);
			if (soulEssenceBonus > 0) {
				index = lines.FindIndex((TooltipLine tt) => tt.Mod.Equals("Terraria") && tt.Name.Equals("Knockback"));
				if (index != -1) {
					lines.Insert(index + 1, new TooltipLine(Mod, "ScytheSoulCharge", "Grants " + soulEssenceBonus + " soul essence on direct hit"));
				}
			}
		}
	}

}
