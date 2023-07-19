using System;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using ModBridge.Global;
using ModBridge.Items.LivingWeapon;
using ModBridge.Projectiles.Scythe;

namespace ModBridge.Items.Healer {

	public class AncientLivingScytheLevelInfo : ScytheLivingWeaponLevelInfo {

		public override int UseTime(int level) {
			return 0;
		}

		public override int XpFromKill(Player player, int level, NPC killedTarget, int damageDealt) {
			float dmgRatio = (float) damageDealt / (float) killedTarget.lifeMax;

			if (dmgRatio > 0.95f) {
				return (int) ((float) killedTarget.lifeMax * 1.3f);
			} else if (dmgRatio > 0.5f) {
				return killedTarget.lifeMax;
			} else if (dmgRatio > 0.2f) {
				return killedTarget.lifeMax / 2;
			} else {
				return damageDealt;
			}
		}

		public override int XpFromDamage(Player player, int level, NPC damagedTarget, int damage, bool isCrit) {
			return damage / 4;
		}

		public override int XpFromUse(Player player, int level) {
			if (level < 10) {
				return  2;
			} else if (level < 20) {
				return 1;
			}

			return 0;
		}

		public override int LevelUpXpRequirement(int level) {
			return (int) (700d * Math.Pow(1.8d, (float) level * 0.09d));
		}

		public override int Damage(int level) {
			return (int) (Math.Pow(1.34d, ((double) level + 78d) * 0.08d) - 5d);
		}

		public override float Knockback(int level) {
			return 35f * ((float) level / 100f);
		}

		public override float Scale(int level) {
			return 1f + (5f * ((float) level / 100f));
		}

		public override float RotationSpeed(int level) {
			return 1f + (7f * ((float) level / 100f));
		}

		public override int SoulEssenceBonus(int level) {
			if (level > 75) {
				return 3;
			} else if (level > 50) {
				return 2;
			}

			return 1;
		}
	}


	public class AncientLivingScythe : ScytheLivingWeapon {

		private static ScytheLivingWeaponEvolutionData AncientLivingBatonEvolutionData = new ScytheLivingWeaponEvolutionData(
				"Ancient Living Baton",
				64,
				64,
				1.3f,
				"ModBridge/Projectiles/Scythe/AncientLivingBatonProjectile",
				22,
				ItemHoldStyleID.None,
				false,
				new Vector2(0, 0),
				SoundID.Item1,

				4,
				4f,
				0.125f,
				0,
				ModContent.ProjectileType<AncientLivingBatonProjectile>(),

				0,
				new AncientLivingScytheLevelInfo(),
				true
		);



		protected override bool AreAwakeningConditionsMet(Player player, NPC killedTarget) {
			return true;
		}

		protected override void ApplyDefaults() {
			base.ApplyDefaults();

			BossLootHandler.RegisterGuaranteedDrop(ItemID.EyeOfCthulhuBossBag, Item.type);

			evolutionData = new ScytheLivingWeaponEvolutionData[] {
				AncientLivingBatonEvolutionData
			};
		}

	}
}
