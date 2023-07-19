using Terraria;
using Terraria.ModLoader;

namespace ModBridge {
	internal static class NPCHelper {

		public static bool IsHostile(this NPC npc, object attacker = null, bool ignoreDontTakeDamage = false) {
			if (!npc.friendly && npc.lifeMax > 5 && npc.chaseable && (!npc.dontTakeDamage || ignoreDontTakeDamage)) {
				return !npc.immortal;
			}

			return false;
		}
	}
}
