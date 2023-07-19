using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using ThoriumMod;
using ThoriumMod.Items;
using System.Collections.Generic;
using ModBridge.DamageClasses;
using System;

namespace ModBridge.Items.Healer {

	public class RadiantFocus : ThoriumItem {

		public override void SetDefaults() {
			DisplayName.SetDefault("Radiant Focus");
			accessoryType = AccessoryType.Normal;
			isHealer = true;
			Item.width = 64;
			Item.height = 48;
			Item.accessory = true;
			Item.expert = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) {
			UpdateEquip(player);
		}

		public override void UpdateEquip(Player player) {
			foreach (Item item in player.inventory) {
				if (item != null) {
					if (item.DamageType != null && item.CountsAsClass(DamageClass.Magic)) {
						item.DamageType = ConvertedRadiantDamage.Instance;
						item.damage = (int) (item.damage * 0.9f);
						item.mana = (int) Math.Ceiling((float) item.mana * 1.3f);
					}
				}
			}

			player.GetModPlayer<ModBridgePlayer>().skipDamageClassCheck = true;
		}

		public override void ModifyTooltips(List<TooltipLine> lines) {
			base.ModifyTooltips(lines);

			lines.Add(new TooltipLine(Mod, "Tooltip#0", "Converts magic damage to radiant damage"));
			lines.Add(new TooltipLine(Mod, "Tooltip#1", "Converted damage is reduced by 10% (rounded down)"));
			lines.Add(new TooltipLine(Mod, "Tooltip#2", "Base mana cost is increased by 30% (rounded up)"));
		}

		public override void AddRecipes() {
			Recipe.Create(Type, 1)
				.AddIngredient(ItemID.SoulofFlight, 2)
				.AddIngredient(ItemID.SoulofFright, 2)
				.AddIngredient(ItemID.SoulofLight, 2)
				.AddIngredient(ItemID.SoulofMight, 2)
				.AddIngredient(ItemID.SoulofNight, 2)
				.AddIngredient(ItemID.SoulofSight, 2)
				.AddTile(TileID.SkyMill)
				.Register();
		}
	}
}
