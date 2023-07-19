using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ModBridge.Projectiles;
using System.Collections.Generic;
using ThoriumMod.Items.RangedItems;

namespace ModBridge.Items.Ranger {
	public class LightCartridge : ModItem {


		public override void SetStaticDefaults() {
			base.DisplayName.SetDefault("Light Cartridge");
			base.Tooltip.SetDefault("Used to power weapons shooting coalesced light");
		}

		public override void SetDefaults() {
			Item.width = 7; // The width of item hitbox
			Item.height = 13; // The height of item hitbox

			Item.damage = 1; // The damage for projectiles isn't actually 8, it actually is the damage combined with the projectile and the item together
			Item.DamageType = DamageClass.Ranged; // What type of damage does this ammo affect?

			Item.maxStack = 9999; // The maximum number of items that can be contained within a single stack
			Item.consumable = true; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible
			Item.knockBack = 1f; // Sets the item's knockback. Ammunition's knockback added together with weapon and projectiles.
			Item.value = Item.sellPrice(0, 0, 50, 0); // Item price in copper coins (can be converted with Item.sellPrice/Item.buyPrice)
			Item.rare = ItemRarityID.Cyan;
			Item.shoot = ModContent.ProjectileType<ShortLaserProjectile>(); // The projectile that weapons fire when using this item as ammunition.

			Item.ammo = Item.type; // Important. The first item in an ammo class sets the AmmoID to its type
		}


		public override void AddRecipes() {
			Recipe.Create(Type, 50)
				.AddIngredient(ItemID.AdamantiteBar, 1)
				.AddIngredient(ItemID.SoulofLight, 1)
				.AddTile(TileID.AdamantiteForge)
				.Register();
		}
	}
}

