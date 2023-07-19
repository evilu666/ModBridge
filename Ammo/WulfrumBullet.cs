using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.Items.Darksteel;

namespace ModBridge.Ammo {
	public class WulfrumBullet : ModItem {
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("A bullet from ancient times, it is a wonder that its manufacturing process has not been lost");
		}

		public override void SetDefaults() {
			Item.width = 63; // The width of item hitbox
			Item.height = 32; // The height of item hitbox
			Item.scale = 0.5f;

			Item.damage = 0;
			Item.DamageType = DamageClass.Ranged;

			Item.maxStack = 9999;
			Item.consumable = true;
			Item.knockBack = 0f;
			Item.value = Item.sellPrice(0, 0, 2, 0);
			Item.rare = ItemRarityID.Orange;
			Item.shoot = ProjectileID.None;

			Item.ammo = Item.type;
		}

		public override void AddRecipes() {
			CreateRecipe(100)
				.AddIngredient<SmoothCoal>()
				.AddIngredient(ItemID.IronBar, 10)
				.AddTile(TileID.HeavyWorkBench)
				.Register();
		}
	}
}
