
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;

using Microsoft.Xna.Framework;

using System.Collections.Generic;

using ThoriumMod.Items.HealerItems;

namespace ModBridge.Global {

	public delegate void ProjectileModifcationHandler(Projectile projectile);

	public class ScytheProjectileModficationHandler : GlobalItem {


		private static Dictionary<int, ProjectileModifcationHandler> handlers = new Dictionary<int, ProjectileModifcationHandler>();

		public static void RegisterHandler(ModPrefix prefix, ProjectileModifcationHandler handler) {
			handlers[prefix.Type] = handler;
		}
		
		public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			if (item.ModItem is ScytheItem && handlers.ContainsKey(item.prefix)) {
				Projectile projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
				handlers[item.prefix](projectile);
				return false;
			}

			return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
		}


	}
}
