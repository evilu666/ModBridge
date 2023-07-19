using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using System.Collections.Generic;

namespace ModBridge.Global {

	public delegate void NpcDeathListener(NPC npc);

	public class NpcDeathHandler : GlobalNPC {

		private static List<NpcDeathListener> listeners = new List<NpcDeathListener>();

		public static void RegisterListener(NpcDeathListener listener) {
			listeners.Add(listener);
		}

		public override void OnKill(NPC npc) {
			foreach (NpcDeathListener listener in listeners) {
				listener(npc);
			}
		}

		public override void HitEffect(NPC npc, int hitDirection, double dmg) {
			if (Main.netMode == NetmodeID.MultiplayerClient && npc.life <= 0) {
				foreach (NpcDeathListener listener in listeners) {
					listener(npc);
				}
			}
		}
	}
}
