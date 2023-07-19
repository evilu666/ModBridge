using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using ThoriumMod.Items.HealerItems;
using ModBridge.Global;

namespace ModBridge {


	public class ModBridge : Mod {

		public static Dictionary<int, ScytheItem> ScytheProjectileItemMapping = null;

		public static void PostSetupContent() {
			DoBossChecklistIntegration();

			ScytheProjectileItemMapping = new Dictionary<int, ScytheItem>();

			for (int i = 0; i < ItemLoader.ItemCount; i++) {
				try {
					Item item = new Item();
					item.SetDefaults(i, noMatCheck: true);
					int shoot = item.shoot;
					ModItem mItem = ItemLoader.GetItem(i);
					if (shoot > 0 && mItem is ScytheItem scytheItem) {
						ModContent.GetInstance<ModBridge>().Logger.Info("Mapped scythe type " + shoot + " to " + mItem.GetType());
						ScytheProjectileItemMapping.Add(shoot, scytheItem);
					}
				} catch {}
			}
		}

		private static void DoBossChecklistIntegration() {
			// The mods homepage links to its own wiki where the calls are explained: https://github.com/JavidPack/BossChecklist/wiki/Support-using-Mod-Call
			// If we navigate the wiki, we can find the "AddBoss" method, which we want in this case

			if (!ModLoader.TryGetMod("BossChecklist", out Mod bossChecklistMod)) {
				return;
			}


			bossChecklistMod.Call("AddToBossLoot", "Terraria EyeofCthulhu", BossLootHandler.GetGuaranteedDrops(ItemID.EyeOfCthulhuBossBag));

			// Other bosses or additional Mod.Call can be made here.
		}
	}
}
