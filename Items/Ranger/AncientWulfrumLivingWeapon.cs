using System;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

using ModBridge.Items.LivingWeapon;
using ModBridge.Projectiles;
using ModBridge.Global;
using ModBridge.Ammo;

namespace ModBridge.Items.Ranger {

	public class AncientWulfrumShotgunLevelInfo : RangedLivingWeaponLevelInfo {

		public override int UseTime(int level) {
			float mult = (float) level / 75f;
			int useTimeBonus = (int) (100f * mult);
			if (useTimeBonus > 100) return 100;

			return useTimeBonus;
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
				return  8;
			} else if (level < 20) {
				return 4;
			} else if (level < 30) {
				return 2;
			}

			return 0;
		}

		public override int LevelUpXpRequirement(int level) {
			return (int) (500f * Math.Pow(2f, 0.09f * (float) level));
		}

		public override int Damage(int level) {
			if (level < 1) return 0;

			return (int) (Math.Pow(1.35d, ((double) level + 98.5d) * 0.08d) - 8.5d);
		}

		public override float ShootSpeed(int level) {
			return (float) level / 9f;
		}

		public override float Knockback(int level) {
			return (float) level / 8f;
		}

		public override float Spread(int level) {
			return Math.Clamp((float) level / 100f, 0f, 1.0f) * (float) Math.PI / 8f;
		}

		public override int AdditionalProjectileCount(int level) {
			return level / 10;
		}

		public override float AdditionalProjectileSpread(int level) {
			return ((float) Math.PI / 8f) * Math.Clamp((float) level / 70f, 0f, 1f) - (float) Math.PI / 256f;
		}

		public override int BurstProjectileCount(int level) {
			return 0;
		}

		public override int BurstProjectileDelay(int level) {
			return 0;
		}

		public override ShootMode[] AvailableShootModes(int level) {
			return new ShootMode[] {
				level >= 15 ? ShootMode.SingleAuto : ShootMode.SingleSemiAuto
			};
		}

		public override bool ShowLaserPreview(int level) {
			return level >= 25;
		}

		public override Color LaserPreviewColor(int level) {
			return new Color(175, 158, 28, 255);
		}

		public override float LaserScanDepth(int level) {
			return 40f + Math.Clamp((float) level - 25  / 75f, 0f, 1.0f) * 280f;
		}

	}

	public class AncientWulfrumScattergunLevelInfo : RangedLivingWeaponLevelInfo {

		public override int UseTime(int level) {
			float mult = (float) level / 75f;
			int useTimeBonus = (int) (70f * mult);
			if (useTimeBonus > 70) return 70;

			return useTimeBonus;
		}

		public override int XpFromKill(Player player, int level, NPC killedTarget, int damageDealt) {
			float dmgRatio = (float) damageDealt / (float) killedTarget.lifeMax;

			if (dmgRatio > 0.95f) {
				return (int) ((float) killedTarget.lifeMax * 1.6f);
			} else if (dmgRatio > 0.5f) {
				return killedTarget.lifeMax;
			} else if (dmgRatio > 0.2f) {
				return killedTarget.lifeMax / 2;
			} else {
				return damageDealt;
			}
		}

		public override int XpFromDamage(Player player, int level, NPC damagedTarget, int damage, bool isCrit) {
			return damage / 3;
		}

		public override int XpFromUse(Player player, int level) {
			if (level < 10) {
				return  6;
			} else if (level < 20) {
				return 3;
			} else if (level < 30) {
				return 1;
			}

			return 0;
		}

		public override int LevelUpXpRequirement(int level) {
			return (int) (1000f * Math.Pow(2.1f, 0.09f * (float) level));
		}

		public override int Damage(int level) {
			if (level < 1) return 0;

			return (int) (Math.Pow(1.37d, ((double) level + 106.5d) * 0.08d) - 11.5d);
		}

		public override float ShootSpeed(int level) {
			return (float) level / 5f;
		}

		public override float Knockback(int level) {
			return (float) level / 12f;
		}

		public override float Spread(int level) {
			return Math.Clamp((float) level / 100f, 0f, 1.0f) * (float) Math.PI / 10f;
		}

		public override int AdditionalProjectileCount(int level) {
			return 0;
		}

		public override float AdditionalProjectileSpread(int level) {
			return 0f;
		}

		public override int BurstProjectileCount(int level) {
			return (int) (10f * Math.Clamp((float) level / 100f, 0f, 1f));
		}

		public override int BurstProjectileDelay(int level) {
			return (int) (10f * Math.Clamp((float) level / 100f, 0f, 1f));
		}

		public override ShootMode[] AvailableShootModes(int level) {
			return new ShootMode[] {
				level >= 35 ? ShootMode.BurstAuto : ShootMode.BurstSemiAuto
			};
		}

		public override bool ShowLaserPreview(int level) {
			return level >= 25;
		}

		public override Color LaserPreviewColor(int level) {
			return new Color(28, 175, 158, 255);
		}

		public override float LaserScanDepth(int level) {
			return 80f + Math.Clamp((float) level - 25  / 75f, 0f, 1.0f) * 380f;
		}

	}


	public class AncientWulfrumLivingWeapon : RangedLivingWeapon {

		public static RangedLivingWeaponEvolutionData AncientWulfrumShotgunEvolutionData = new RangedLivingWeaponEvolutionData(
			name: "Ancient Wulfrum Shotgun",
			width: 240,
			height: 96,
			scale: 0.3f,
			texture: "ModBridge/Items/Ranger/AncientWulfrumShotgun",
			useTime: 120,
			holdStyle: ItemHoldStyleID.HoldHeavy,
			holdRotate: true,
			holdoutOffset: new Vector2(-80, 0),
			useSound: SoundID.Item38,

			damage: 16,
			shootSpeed: 3f,
			spread: (float) Math.PI / 8f,
			knockback: 2.0f,
			ammo: ModContent.ItemType<WulfrumBullet>(),
			projectile: ModContent.ProjectileType<WulfrumShotgunProjectile>(),
			additionalProjectileCount: 1,
			additionalProjectileSpread: (float) Math.PI / 8f,
			burstProjectileCount: 0,
			burstProjectileDelay: 0,
			shootOffset: new Vector2(50, -5),
			laserScanWidth: 8f,

			levelRequirement: 50,
			levelInfo: new AncientWulfrumShotgunLevelInfo(),
			isEvolutionConditionHidden: false,
			isCustomEvolutionConditionHidden: false,
			customEvolutionConditionDescription: "Reach hardmode",
			customEvolutionCondition: w => Main.hardMode
		);

		public static RangedLivingWeaponEvolutionData AncientWulfrumScattergunEvolutionData = new RangedLivingWeaponEvolutionData(
			name: "Ancient Wulfrum Scattergun",
			width: 336,
			height: 120,
			scale: 0.3f,
			texture: "ModBridge/Items/Ranger/AncientWulfrumScattergun",
			useTime: 90,
			holdStyle: ItemHoldStyleID.HoldHeavy,
			holdRotate: true,
			holdoutOffset: new Vector2(-80, 0),
			useSound: SoundID.Item38,

			damage: 40,
			shootSpeed: 6f,
			spread: (float) Math.PI / 10f,
			knockback: 1.0f,
			ammo: ModContent.ItemType<WulfrumBullet>(),
			projectile: ModContent.ProjectileType<WulfrumScattergunProjectile>(),
			additionalProjectileCount: 0,
			additionalProjectileSpread: 0f,
			burstProjectileCount: 2,
			burstProjectileDelay: 15,
			shootOffset: new Vector2(45, 0),
			laserScanWidth: 8f,

			levelRequirement: 50,
			levelInfo: new AncientWulfrumScattergunLevelInfo(),
			isEvolutionConditionHidden: true
		);

		protected override void ApplyDefaults() {
			base.ApplyDefaults();

			BossLootHandler.RegisterGuaranteedDrop(ItemID.EyeOfCthulhuBossBag, Item.type);

			evolutionData = new RangedLivingWeaponEvolutionData[] {
				AncientWulfrumShotgunEvolutionData,
				AncientWulfrumScattergunEvolutionData
			};

		}

		protected override bool AreAwakeningConditionsMet(Player player, NPC killedTarget) {
			return killedTarget.type == NPCID.EyeofCthulhu;
		}
	}
}
