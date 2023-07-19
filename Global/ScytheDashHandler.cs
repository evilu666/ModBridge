using System;

using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using ReLogic.Utilities;

using ThoriumMod.Items.HealerItems;
using ThoriumMod.Projectiles.Scythe;

using ModBridge.Util;

namespace ModBridge.Global {
	public class ScytheDashHandler : GlobalItem {

		private static SoundStyle DashSound = new SoundStyle("ModBridge/Sounds/General/ScytheDash") with {
			MaxInstances = 1
		};

		public static int manaTickSpan = 5;
		public static float manaPerTickBase = 1.13f;

		public static float minScaleMult = 2f;
		public static float maxScaleMult = 7f;

		public static float minSpeedMult = 1.3f;
		public static float maxSpeedMult = 3f;

		public static float minSpeed = 8f;
		public static float maxSpeed = 24f;

		public static float minKnockbackMult = 2f;
		public static float maxKnockbackMult = 8f;

		public static int projectileTickSpan = 22;

		public static float speed = 8f;

		public static int cooldownMult = 8;

		private int currentDashTick = 0;
		private bool isDashActive;
		private SlotId? audioSlot;

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(Item item, bool isLateInstantiation) {
			return item.ModItem is ScytheItem;
		}

		public override void HoldStyle(Item item, Player player, Rectangle frame) {
			if (isDashActive) {
				DashTick(player, item);
			}
		}


		public override bool AltFunctionUse(Item item, Player player) {
			if (base.AltFunctionUse(item, player)) return true;

			if (player.GetModPlayer<ModBridgePlayer>().canScytheDash && player.dashDelay == 0) {
				StartDash(player);
				DashTick(player, item);
				return true;
			} else if (isDashActive) {
				StopDash(player);
				return true;
			}

			return false;
		}

		private void StartDash(Player player) {
			currentDashTick = 0;
			isDashActive = true;

			audioSlot = SoundEngine.PlaySound(DashSound, player.Center);
		}

		private void StopDash(Player player) {
			currentDashTick = 0;
			isDashActive = false;

			CurrentDashSound?.Stop();
		}

		private void DashTick(Player player, Item item) {
			if (currentDashTick % manaTickSpan == 0) {
				int mana = (int) Math.Pow(manaPerTickBase, (float) (currentDashTick / manaTickSpan));

				if (!player.CheckMana(mana, true)) {
					StopDash(player);
					return;
				}

				player.AddImmuneTime(ImmunityCooldownID.General, manaTickSpan);
			}

			if (currentDashTick % projectileTickSpan == 0) {
				EntitySource_ItemUse_WithAmmo source = new EntitySource_ItemUse_WithAmmo(player, item, AmmoID.None);
				int dmg = (int) player.GetDamage(item.DamageType).ApplyTo(item.damage);
				float knockback = player.GetKnockback(item.DamageType).ApplyTo(item.knockBack * KnockbackMult);

				Projectile projectile = Projectile.NewProjectileDirect(source, player.Center, player.velocity, item.shoot, dmg, knockback, player.whoAmI);
				projectile.netUpdate = true;
				projectile.scale *= ScaleMult;
				ScythePro pro = (ScythePro) projectile.ModProjectile;
				pro.rotationSpeed *= SpeedMult;

			}

			Vector2 dir = (Main.MouseWorld - player.Center);
			dir.Normalize();
			player.velocity = dir * Speed;

			if (CurrentDashSound is ActiveSound sound) {
				sound.Position = player.Center;
			}

			currentDashTick++;
			player.dashDelay = currentDashTick * cooldownMult;
		}

		private float ScaleMult => minScaleMult + Math.Clamp((float) currentDashTick / 180f, 0f, 1f) * (maxScaleMult - minScaleMult);
		private float SpeedMult => minSpeedMult + Math.Clamp((float) currentDashTick / 180f, 0f, 1f) * (maxSpeedMult - minSpeedMult);
		private float KnockbackMult => minKnockbackMult + Math.Clamp((float) currentDashTick / 300f, 0f, 1f) * (maxKnockbackMult - minKnockbackMult);
		private float Speed => minSpeed + Math.Clamp((float) currentDashTick / 300f, 0f, 1f) * (maxSpeed - minSpeed);
		private ActiveSound? CurrentDashSound {
			get {
				if (audioSlot is SlotId slot) {
					ActiveSound? sound;
					SoundEngine.TryGetActiveSound(slot, out sound);
					return sound;
				}

				return null;
			}
		}

	}
}
