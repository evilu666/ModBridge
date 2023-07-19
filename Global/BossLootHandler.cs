using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;

using System.Collections.Generic;
using System.Linq;


namespace ModBridge.Global {
	public class BossLootHandler : GlobalItem {

		private static Dictionary<int, HashSet<int>> guaranteedDrops = new Dictionary<int, HashSet<int>>();

		public static void RegisterGuaranteedDrop(int bagType, int itemType) {
			if (!guaranteedDrops.ContainsKey(bagType)) {
				guaranteedDrops[bagType] = new HashSet<int>();
			}

			guaranteedDrops[bagType].Add(itemType);
		}

		public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
			if(guaranteedDrops.ContainsKey(item.type)) {
				foreach (int lootId in guaranteedDrops[item.type]) {
					itemLoot.Add(new OneFromOptionsNotScaledWithLuckDropRule(1, 1, new int[] {lootId}));
				}
			}
		}

		public static List<int> GetGuaranteedDrops(int bagType) {
			return guaranteedDrops.ContainsKey(bagType) ? guaranteedDrops[bagType].ToList() : new List<int>();
		}


	}
}
