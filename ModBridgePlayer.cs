using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.ID;
using ThoriumMod;
using ThoriumMod.Items.HealerItems;
using ThoriumMod.Projectiles.Scythe;
using ModBridge.Prefix;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using ModBridge.Items.LivingWeapon;

namespace ModBridge {
	public class ModBridgePlayer : ModPlayer {

		public bool canScytheDash = true;

		public bool skipDamageClassCheck = false;

		public bool collectSoulEssence = false;

		public int soulEssenceBoost = 0;

		public int soulEssenceRest = 0;

		private int healTickLength = 60;

		private int healMinTickLength = 20;

		public float healPerTick = 15.0f;

		private int tick = 0;

		private float healBuffer = 0f;
		private float manaBuffer = 0f;

		public override void ResetEffects() {
			canScytheDash = true;
		}

		public override void GetHealLife(Item item, bool quickHeal, ref int healValue) {
			if (item.prefix > 0) {
				ModPrefix p = PrefixLoader.GetPrefix(item.prefix);
				if (p is HealerWeaponPrefix prefix) {
					healValue = (int) (healValue * prefix.HealMultiplier);
				}
			}
		}

		public override void PreUpdate() {
			skipDamageClassCheck = false;
		}

		public override void PostUpdateEquips() {
				if (!skipDamageClassCheck) {
					foreach (Item item in Player.inventory) {
						if (item != null && item.DamageType == DamageClasses.ConvertedRadiantDamage.Instance) {
							item.SetDefaults(item.type);
						}
					}
				}

				if (!collectSoulEssence || (this.Player.statMana >= this.Player.statManaMax && this.Player.statLife >= this.Player.statLifeMax)) {
					soulEssenceRest = 0;
					manaBuffer = 0;
					healBuffer = 0;
					return;
				} else collectSoulEssence = false;

				ThoriumPlayer thoriumPlayer = this.Player.GetModPlayer<ThoriumPlayer>();
				float healSpeed = Player.GetAttackSpeed(ThoriumDamageBase<HealerTool>.Instance);

				if (thoriumPlayer.soulEssence > 5) {
					soulEssenceRest += thoriumPlayer.soulEssence - 5;
					thoriumPlayer.soulEssence = 5;
				}

				if (tick == 0 && (soulEssenceRest >= 5 || healBuffer > 0 | manaBuffer > 0)) {
					int times = soulEssenceRest / 5;
					soulEssenceRest = soulEssenceRest % 5;
					float maxHeal = healSpeed * healPerTick;
					float maxMana = healSpeed * healPerTick * 3;
					
					float heal = times * (1 + thoriumPlayer.healBonus);
					float mana = times * (3 + thoriumPlayer.healBonus * 3);

					if (heal > maxHeal) {
						healBuffer += heal - maxHeal;
						heal = maxHeal;
					} else if (heal < maxHeal && healBuffer > 0) {
						heal += healBuffer;
						if (heal > maxHeal) {
							healBuffer = heal - maxHeal;
							heal = maxHeal;
						}
					}

					if (mana > maxMana) {
						manaBuffer += mana - maxMana;
						mana = maxMana;
					} else if (mana < maxMana && manaBuffer > 0) {
						mana += manaBuffer;
						if (mana > maxMana) {
							manaBuffer = mana - maxMana;
							mana = maxMana;
						}
					}

					this.Player.HealLife((int) heal);
					this.Player.HealMana((int) mana);

					SoundEngine.PlaySound(in SoundID.NPCHit52, base.Player.Center);

					for (int i = 0; i < 15; i++) {
						int dustIndex = Dust.NewDust(base.Player.position, base.Player.width, base.Player.height, 229, 0f, 0f, 100, default(Color), 1.35f);
						Main.dust[dustIndex].noGravity = true;
						Main.dust[dustIndex].noLight = true;
						Dust dust = Main.dust[dustIndex];
						dust.velocity *= 0.75f;

						int num63 = Main.rand.Next(-50, 51);
						int num64 = Main.rand.Next(-50, 51);
						Dust dust7 = Main.dust[dustIndex];
						dust7.position.X = dust7.position.X + (float)num63;
						Dust dust8 = Main.dust[dustIndex];
						dust8.position.Y = dust8.position.Y + (float)num64;
						Main.dust[dustIndex].velocity.X = (0f - (float)num63) * 0.065f;
						Main.dust[dustIndex].velocity.Y = (0f - (float)num64) * 0.065f;
					}
				}

				float tickLenPercent = (healSpeed - 1f) * 0.66f;
				int tickLen = (int) (healTickLength - healTickLength * tickLenPercent);
				if (tickLen < healMinTickLength) tickLen = healMinTickLength;

				tick = (tick+1) % tickLen;

		}

		public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) {
			if (!target.IsHostile()) {
				return;
			}

			int soulBoost = 0;
			if (proj.ModProjectile is ScythePro projectile) {
				soulBoost = soulEssenceBoost;

				Item item = this.Player.inventory[this.Player.selectedItem];
				if (item.prefix > 0) {
					ModPrefix p = PrefixLoader.GetPrefix(item.prefix);
					if (p is HealerWeaponPrefix prefix) {
						soulBoost += prefix.SoulEssenceBonus;
					}
				}

			} else if (proj.ModProjectile is ScytheLivingWeaponProjectile pro) {
				soulBoost = pro.SoulEssenceBonus;
			}

			ThoriumPlayer thoriumPlayer = this.Player.GetModPlayer<ThoriumPlayer>();

			thoriumPlayer.soulEssence += soulBoost;
			if (thoriumPlayer.soulEssence > 5) {
				soulEssenceRest += thoriumPlayer.soulEssence - 5;
				thoriumPlayer.soulEssence = 5;
			}
		}

	}
}
