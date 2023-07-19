using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ModBridge.Projectiles;
using ModBridge.Render;
using ModBridge.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ModBridge.Items.Ranger {
	public class TacticalLaserRifle : ModItem {

		private static SoundStyle UseSound = new SoundStyle("ModBridge/Sounds/Items/Ranger/ShortLaserBlast") with {
			MaxInstances = 8
		};

		private static int autoDamage = 120;
		private static int semiDamage = 330;

		private static int autoUseTime = 30;
		private static int semiUseTime = 20;

		public override void SetStaticDefaults() {
			base.DisplayName.SetDefault("Tactical Laser Rifle");
			base.Tooltip.SetDefault("Shoots a short pulses of coalesced light that can pierce infinitely\nWill reflect up to 8 times\nEach reflection increases the damge by 40%\nCan switch between automatic and semi automatic mode with right click");
		}

		public override void ModifyTooltips(List<TooltipLine> lines) {
			base.ModifyTooltips(lines);
		}

		public override void SetDefaults() {
			base.Item.damage = autoDamage;
			base.Item.DamageType = DamageClass.Ranged;
			base.Item.width = 444;
			base.Item.height = 130;
			base.Item.useTime = 40;
			base.Item.scale = 0.14f;
			base.Item.useAnimation = 40;
			base.Item.useStyle = 5;
			base.Item.noMelee = true;
			base.Item.knockBack = 2f;
			base.Item.value = Item.sellPrice(0, 2, 40);
			base.Item.UseSound = UseSound;
			base.Item.autoReuse = true;
			base.Item.shoot = ModContent.ProjectileType<ShortLaserProjectile>();
			base.Item.shootSpeed = 16f;
			base.Item.useAmmo = ModContent.ItemType<LightCartridge>();
			base.Item.master = true;
			base.Item.masterOnly = true;
			base.Item.holdStyle = ItemHoldStyleID.HoldHeavy;
		}

		public override bool AltFunctionUse(Player player) {
			Item.autoReuse = !Item.autoReuse;
			Item.damage = Item.autoReuse ? autoDamage : semiDamage;
			Item.useTime = Item.autoReuse ? autoUseTime : semiUseTime;
			Item.useAnimation = Item.useTime;

			RenderUtil.ShowCombatText(player, new Color(34, 206, 229, 255), Item.autoReuse ? "Automatic mode" : "Semi automatic mode");

			return false;
		}

		public override bool PreDrawInWorld(SpriteBatch batch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
			scale = Item.scale;
			return true;
		}

		public override void HoldStyle(Player player, Rectangle frame) {
			Vector2 dir = (Main.MouseWorld - player.Center);
			dir.Normalize();
			player.itemRotation = (dir * player.direction).ToRotation();

			if (player.whoAmI != Main.myPlayer) return;

			if (!Item.autoReuse) {

				float scanWidth = 11.3f;

				Vector2 reflectPos = CollisionUtil.LaserScan(player.Center, dir, scanWidth) - dir * scanWidth;
				PostTilesRenderer.Instance.Render(new Line(player.Center, reflectPos, ShortLaserProjectile.colors[0] with {
							A = 70
				}));

				int colorIndex = 1;
				for (int i = 0; i < 4; i++) {
					Vector2 startPos = reflectPos;

					Vector2 oldDir = dir;
					int collisionDir;
					if (Collision.FindCollisionDirection(out collisionDir, startPos, 16, 16)) {
						// If the projectile hits the left or right side of the tile, reverse the X velocity
						if (collisionDir < 2) {
							dir.X = -dir.X;
						}

						// If the projectile hits the top or bottom side of the tile, reverse the Y velocity
						if (collisionDir > 1) {
							dir.Y = -dir.Y;
						}
					} else {
						reflectPos = CollisionUtil.LaserScan(startPos, dir, scanWidth) - dir * scanWidth * 0.5f;
						continue;
					}

					reflectPos = CollisionUtil.LaserScan(startPos, dir, scanWidth) - oldDir * scanWidth;
					PostTilesRenderer.Instance.Render(new Line(startPos, reflectPos, ShortLaserProjectile.colors[colorIndex] with {
								A = 70
					}));
					colorIndex++;
				}
			}
		}

		public override void AddRecipes() {
			Recipe.Create(Type, 1)
				.AddIngredient(ModContent.ItemType<Disparager>(), 1)
				.AddIngredient(ItemID.TacticalShotgun, 1)
				.AddIngredient(ItemID.SoulofSight, 5)
				.AddIngredient(ItemID.Lens, 2)
				.AddIngredient(ItemID.IllegalGunParts, 1)
				.AddTile(TileID.AdamantiteForge)
				.Register();
		}
	}
}

