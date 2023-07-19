using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using ThoriumMod.Items;
using System.Collections.Generic;

namespace ModBridge.Items.Healer {

	public class SoulCollector : ThoriumItem {

		public override void SetDefaults() {
			DisplayName.SetDefault("Soul Collector");
			accessoryType = AccessoryType.Normal;
			isHealer = true;
			Item.width = 32;
			Item.height = 32;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) {
			UpdateEquip(player);
		}

		public override void UpdateEquip(Player player) {
			ModBridgePlayer modPlayer = player.GetModPlayer<ModBridgePlayer>();
			modPlayer.collectSoulEssence = true;
		}

		public override void ModifyTooltips(List<TooltipLine> lines) {
			base.ModifyTooltips(lines);

			lines.Add(new TooltipLine(Mod, "Tooltip#0", "Collects overflowing soul essence"));
			lines.Add(new TooltipLine(Mod, "Tooltip#1", "Soul essence gained over 5 will not be lost until both your mana and health is full"));
			lines.Add(new TooltipLine(Mod, "Tooltip#2", "The amount of life and mana aswell as the speed at which it is gained depends on your healing speed"));
		}

		public override void AddRecipes() {
			Recipe.Create(Type, 1)
				.AddIngredient(ItemID.SoulofFlight, 1)
				.AddIngredient(ItemID.SoulofFright, 1)
				.AddIngredient(ItemID.SoulofLight, 1)
				.AddIngredient(ItemID.SoulofMight, 1)
				.AddIngredient(ItemID.SoulofNight, 1)
				.AddIngredient(ItemID.SoulofSight, 1)
				.AddIngredient(ItemID.Cloud, 6)
				.AddTile(TileID.SkyMill)
				.Register();
		}

	}
}
