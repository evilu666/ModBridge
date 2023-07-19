using System.Collections.Generic;
using ModBridge.Global;
using Terraria;
using Terraria.ModLoader;

namespace ModBridge.Global {
	public delegate void ItemPrefixTooltipHandler(Item item, List<TooltipLine> lines);

	public class GlobalItemPrefixTooltipHandler : GlobalItem {
		private static Dictionary<int, ItemPrefixTooltipHandler> handlers = new Dictionary<int, ItemPrefixTooltipHandler>();

		public static void RegisterHandler(ModPrefix prefix, ItemPrefixTooltipHandler handler) {
			handlers[prefix.Type] = handler;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> lines) {
			if (handlers.ContainsKey(item.prefix)) {
				handlers[item.prefix](item, lines);
			}
		}
	}
}

