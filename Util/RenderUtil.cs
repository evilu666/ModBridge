using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace ModBridge.Util {
	public static class RenderUtil {
		public static void ShowCombatText(Player player, Color color, string text) {
			CombatText.NewText(new Rectangle((int) player.position.X, (int) player.position.Y, player.width, player.height), color, text);
		}

		public static void ShowTargetCombatText(NPC target, Color color, string text) {
			CombatText.NewText(new Rectangle((int) target.position.X, (int) target.position.Y, target.width, target.height), color, text);
		}
	}
}
