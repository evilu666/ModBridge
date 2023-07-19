using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ModBridge.Projectiles;
using System.Collections.Generic;
using ThoriumMod.Items.RangedItems;

namespace ModBridge.Items.Ranger {
	public class Disparager : ModItem {

		private static SoundStyle UseSound = new SoundStyle("ModBridge/Sounds/Items/Ranger/Disparager") with {
			MaxInstances = 2
		};

		public override void SetStaticDefaults() {
			base.DisplayName.SetDefault("Disparager");
			base.Tooltip.SetDefault("Shoots a medium ranged pulse of coalesced light that can pierce infinitely");
		}

		public override void ModifyTooltips(List<TooltipLine> lines) {
			base.ModifyTooltips(lines);
		}

		public override void SetDefaults() {
			base.Item.damage = 450;
			base.Item.DamageType = DamageClass.Ranged;
			base.Item.width = 30;
			base.Item.height = 30;
			base.Item.useTime = 120;
			base.Item.useAnimation = base.Item.useTime / 2;
			base.Item.useStyle = 5;
			base.Item.noMelee = true;
			base.Item.knockBack = 20f;
			base.Item.value = Item.sellPrice(0, 2, 40);
			base.Item.mana = 15;
			base.Item.UseSound = UseSound;
			base.Item.autoReuse = false;
			base.Item.shoot = ModContent.ProjectileType<DisparagerProjectile>();
			base.Item.shootSpeed = 8f;
			base.Item.expert = true;
			base.Item.expertOnly = true;
		}

		public override void AddRecipes() {
			Recipe.Create(Type, 1)
				.AddIngredient(ModContent.ItemType<Zapper>(), 1)
				.AddIngredient(ItemID.SoulofSight, 2)
				.AddIngredient(ItemID.SoulofLight, 2)
				.AddIngredient(ItemID.Lens, 2)
				.AddIngredient(ItemID.IllegalGunParts, 1)
				.AddTile(TileID.AdamantiteForge)
				.Register();
		}
	}
}

