using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Utilities;
using Terraria.DataStructures;
using Terraria.Audio;

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using ModBridge.Util;
using ModBridge.Render;

namespace ModBridge.Items.LivingWeapon {

	public enum ShootMode {
		SingleSemiAuto,
		SingleAuto,
		BurstSemiAuto,
		BurstAuto,
		Charge
	}

	public static class ShootModeCompanion {
		private static Dictionary<ShootMode, string> names = new Dictionary<ShootMode, string>() {
			[ShootMode.SingleSemiAuto] = "Semi-Automatic",
			[ShootMode.SingleAuto] = "Automatic",
			[ShootMode.BurstSemiAuto] = "Semi-Automatic Burst",
			[ShootMode.BurstAuto] = "Automatic Burst",
			[ShootMode.Charge] = "Charge",
		};

		public static string Name(this ShootMode mode) {
			return names[mode];
		}

		public static bool IsBurst(this ShootMode mode) {
			return mode == ShootMode.BurstAuto || mode == ShootMode.BurstSemiAuto;
		}

		public static bool IsSingle(this ShootMode mode) {
			return mode == ShootMode.SingleAuto || mode == ShootMode.SingleSemiAuto;
		}

		public static bool IsAuto(this ShootMode mode) {
			return mode == ShootMode.SingleAuto || mode == ShootMode.BurstAuto;
		}
	}

	public abstract class RangedLivingWeaponLevelInfo : LivingWeaponLevelInfo {

		public abstract int Damage(int level);
		public abstract float ShootSpeed(int level);
		public abstract float Knockback(int level);
		public abstract float Spread(int level);
		public abstract int AdditionalProjectileCount(int level);
		public abstract float AdditionalProjectileSpread(int level);

		public abstract int BurstProjectileCount(int level);
		public abstract int BurstProjectileDelay(int level);

		public virtual ShootMode[] AvailableShootModes(int level) {
			return new ShootMode[] {
				ShootMode.SingleSemiAuto
			};
		}

		public override bool AltFunctionUse(Player player, int level, Item item) {
			ShootMode[] modes = AvailableShootModes(level);
			RangedLivingWeapon weapon = (RangedLivingWeapon) item.ModItem;

			if (modes.Length > 1) {
				int index = Array.IndexOf(modes, weapon.shootMode);
				if (index < 0) {
					index = 0;
				} else {
					index++;
					index %= modes.Length;
				}

				ShootMode oldMode = weapon.shootMode;
				weapon.shootMode = modes[index];
				item.autoReuse = weapon.shootMode.IsAuto();

				if (oldMode != weapon.shootMode) {
					RenderUtil.ShowCombatText(player, RangedLivingWeapon.ModeSwitchColor, weapon.shootMode.Name() + " Mode");
					return true;
				}
			}

			return false;
		}

		public virtual bool ShowLaserPreview(int level) {
			return false;
		}

		public virtual Color LaserPreviewColor(int level) {
			return new Color(200, 0, 0, 255);
		}

		public virtual float LaserScanDepth(int level) {
			return 1600f;
		}

		public virtual int LaserScanProbeCount(int level) {
			return 4;
		}

	}

	public class RangedLivingWeaponEvolutionData : LivingWeaponEvolutionData<RangedLivingWeaponLevelInfo> {
		public int Damage;
		public float ShootSpeed;
		public float Spread;
		public float Knockback;
		public int Ammo;
		public int Projectile;
		public int AdditionalProjectileCount;
		public float AdditionalProjectileSpread;
		public int BurstProjectileCount;
		public int BurstProjectileDelay;
		public Vector2 ShootOffset;
		public float LaserScanWidth;

		public RangedLivingWeaponEvolutionData(
				string name,
				int width,
				int height,
				float scale,
				string texture,
				int useTime,
				int holdStyle,
				bool holdRotate,
				Vector2 holdoutOffset,
				SoundStyle useSound,

				int damage,
				float shootSpeed,
				float spread,
				float knockback,
				int ammo,
				int projectile,
				int additionalProjectileCount,
				float additionalProjectileSpread,
				int burstProjectileCount,
				int burstProjectileDelay,
				Vector2 shootOffset,
				float laserScanWidth,

				int levelRequirement,
				RangedLivingWeaponLevelInfo levelInfo,

				bool isEvolutionConditionHidden,
				string customEvolutionConditionDescription = "",
				EvolutionCondition<RangedLivingWeaponLevelInfo, LivingWeaponEvolutionData<RangedLivingWeaponLevelInfo>, BaseLivingWeapon<RangedLivingWeaponLevelInfo, LivingWeaponEvolutionData<RangedLivingWeaponLevelInfo>>> customEvolutionCondition = null,
				bool isCustomEvolutionConditionHidden = true)
					: base(name, width, height, scale, texture, useTime, useTime, holdStyle, holdRotate, holdoutOffset, useSound, levelRequirement, levelInfo, isEvolutionConditionHidden, customEvolutionConditionDescription, customEvolutionCondition, isCustomEvolutionConditionHidden) {

			this.Damage = damage;
			this.ShootSpeed = shootSpeed;
			this.Spread = spread;
			this.Knockback = knockback;
			this.Ammo = ammo;
			this.Projectile = projectile;
			this.AdditionalProjectileCount = additionalProjectileCount;
			this.AdditionalProjectileSpread  = additionalProjectileSpread;
			this.BurstProjectileCount = burstProjectileCount;
			this.BurstProjectileDelay = burstProjectileDelay;
			this.ShootOffset = shootOffset;
			this.LaserScanWidth = laserScanWidth;
		}
	}

	public abstract class RangedLivingWeapon : BaseLivingWeapon<RangedLivingWeaponLevelInfo, RangedLivingWeaponEvolutionData> {

		public static Color ModeSwitchColor = new Color(35, 27, 150, 255);

		public ShootMode shootMode = ShootMode.SingleSemiAuto;
		protected int shootTick = 0;
		private UnifiedRandom random = new UnifiedRandom();


		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
		}


		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			GainXp(player, LevelInfo.XpFromUse(player, Level));

			Vector2 dir = Vector2.Normalize(velocity);
			position += dir * EvolutionData.ShootOffset.X;
			position += dir.RotatedBy(player.direction * (Math.PI / 2f)) * EvolutionData.ShootOffset.Y;

			switch (shootMode) {
				case ShootMode.SingleSemiAuto:
				case ShootMode.SingleAuto:
					ShootSingle(player, source, position, velocity, type, damage, knockback);
					break;
				case ShootMode.BurstSemiAuto:
				case ShootMode.BurstAuto:
					ShootBurst(player, source, position, velocity, type, damage, knockback);
					break;
				case ShootMode.Charge:
					ShootCharge(player, source, position, velocity, type, damage, knockback);
					break;
				default:
					ShootSingle(player, source, position, velocity, type, damage, knockback);
					break;
			}


			return false;
		}

		public override bool? UseItem(Player player) {
			base.UseItem(player);

			int burstDelay = EvolutionData.BurstProjectileDelay - LevelInfo.BurstProjectileDelay(Level);
			int shotCount = EvolutionData.BurstProjectileCount + LevelInfo.BurstProjectileCount(Level);

			if (shootMode.IsBurst()) {
				Vector2 position = player.Center;
				Vector2 dir = Main.MouseWorld - position;
				dir = Vector2.Normalize(dir);
				player.itemRotation = (dir * player.direction).ToRotation();

				if (shootTick % burstDelay == 0) {
					int proj;
					float speed;
					int damage;
					float knockback;
					int ammoId;

					int shotsFired = (int) (shootTick / burstDelay);

					if (shotsFired < shotCount && player.PickAmmo(Item, out proj, out speed, out damage, out knockback, out ammoId)) {
						EntitySource_ItemUse_WithAmmo source = new EntitySource_ItemUse_WithAmmo(player, Item, ammoId);


						position += dir * EvolutionData.ShootOffset.X;
						position += dir.RotatedBy(player.direction * (Math.PI / 2f)) * EvolutionData.ShootOffset.Y;
						ApplySpread(ref dir);

						LivingWeaponProjectile<RangedLivingWeapon, RangedLivingWeaponLevelInfo, RangedLivingWeaponEvolutionData> projectile = (LivingWeaponProjectile<RangedLivingWeapon, RangedLivingWeaponLevelInfo, RangedLivingWeaponEvolutionData>) Projectile.NewProjectileDirect(source, position, dir * speed, proj, damage, knockback, player.whoAmI).ModProjectile;
						projectile.Weapon = this;
						projectile.Owner = player;
						projectile.Projectile.rotation = dir.ToRotation();

						SoundEngine.PlaySound(Item.UseSound, position);
					}
				}
			}

			shootTick++;

			return true;
		}

		public override void HoldStyle(Player player, Rectangle frame) {
			base.HoldStyle(player, frame);
			if (player.whoAmI != Main.myPlayer || !LevelInfo.ShowLaserPreview(Level)) return;

			Vector2 dir = (Main.MouseWorld - player.Center);
			dir.Normalize();

			Vector2 position = player.Center;
			position += dir * EvolutionData.ShootOffset.X;
			position += dir.RotatedBy(player.direction * (Math.PI / 2f)) * EvolutionData.ShootOffset.Y;

			float scanWidth = EvolutionData.LaserScanWidth;
			float scanDepth = LevelInfo.LaserScanDepth(Level);
			int probeCount = LevelInfo.LaserScanProbeCount(Level);
			Color previewColor = LevelInfo.LaserPreviewColor(Level) with {
				A = 70
			};

			Vector2 reflectPos = CollisionUtil.LaserScan(position, dir, scanWidth, scanDepth, probeCount);
			PostTilesRenderer.Instance.Render(new Line(position, reflectPos, previewColor));

			for (int i = 0; i < EvolutionData.AdditionalProjectileCount + LevelInfo.AdditionalProjectileCount(Level); i++) {
				float angle = Math.Clamp(EvolutionData.AdditionalProjectileSpread - LevelInfo.AdditionalProjectileSpread(Level), 0f, (float) Math.PI);
				angle *= (float) i+1;

				Vector2 dir1 = dir.RotatedBy(angle);
				Vector2 reflectPos1 = CollisionUtil.LaserScan(position, dir1, scanWidth, scanDepth, probeCount);
				PostTilesRenderer.Instance.Render(new Line(position, reflectPos1, previewColor));

				Vector2 dir2 = dir.RotatedBy(-angle);
				Vector2 reflectPos2 = CollisionUtil.LaserScan(position, dir2, scanWidth, scanDepth, probeCount);
				PostTilesRenderer.Instance.Render(new Line(position, reflectPos2, previewColor));
			}
		}

		public override void ModifyTooltips(List<TooltipLine> lines) {
			float accuracy = ((float) Math.PI - Math.Clamp(EvolutionData.Spread - LevelInfo.Spread(Level), 0f, (float) Math.PI)) / (float) Math.PI;
			lines.Add(new TooltipLine(Mod, "FishingPower", String.Format("{0:P2} accuracy", accuracy)));

			base.ModifyTooltips(lines);
		}

		public override void SetDefaults() {
			base.SetDefaults();

			Item.useStyle = ItemUseStyleID.Shoot;
			Item.DamageType = DamageClass.Ranged;
		}

		protected virtual void ShootSingle(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			Vector2 dir = Vector2.Normalize(velocity);

			Vector2 v = velocity;
			ApplySpread(ref v);
			LivingWeaponProjectile<RangedLivingWeapon, RangedLivingWeaponLevelInfo, RangedLivingWeaponEvolutionData> projectile = (LivingWeaponProjectile<RangedLivingWeapon, RangedLivingWeaponLevelInfo, RangedLivingWeaponEvolutionData>) Projectile.NewProjectileDirect(source, position, v, type, damage, knockback, player.whoAmI).ModProjectile;
			projectile.Weapon = this;
			projectile.Owner = player;
			projectile.Projectile.rotation = velocity.ToRotation();

			for (int i = 0; i < EvolutionData.AdditionalProjectileCount + LevelInfo.AdditionalProjectileCount(Level); i++) {
				float baseSpread = Math.Clamp((float) (EvolutionData.AdditionalProjectileSpread - LevelInfo.AdditionalProjectileSpread(Level)), 0f, (float) Math.PI);
				baseSpread *= (float) i+1;

				Vector2 velocity1 = velocity.RotatedBy(baseSpread);
				ApplySpread(ref velocity1);
				LivingWeaponProjectile<RangedLivingWeapon, RangedLivingWeaponLevelInfo, RangedLivingWeaponEvolutionData> projectile1 = (LivingWeaponProjectile<RangedLivingWeapon, RangedLivingWeaponLevelInfo, RangedLivingWeaponEvolutionData>) Projectile.NewProjectileDirect(source, position, velocity1, type, damage, knockback, player.whoAmI).ModProjectile;
				projectile1.Weapon = this;
				projectile.Owner = player;
				projectile1.Projectile.rotation = velocity1.ToRotation();

				Vector2 velocity2 = velocity.RotatedBy(-baseSpread);
				ApplySpread(ref velocity2);
				LivingWeaponProjectile<RangedLivingWeapon, RangedLivingWeaponLevelInfo, RangedLivingWeaponEvolutionData> projectile2 = (LivingWeaponProjectile<RangedLivingWeapon, RangedLivingWeaponLevelInfo, RangedLivingWeaponEvolutionData>) Projectile.NewProjectileDirect(source, position, velocity2, type, damage, knockback, player.whoAmI).ModProjectile;
				projectile2.Weapon = this;
				projectile.Owner = player;
				projectile2.Projectile.rotation = velocity2.ToRotation();
			}
		}

		protected virtual void ShootBurst(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			shootTick = 0;
		}

		protected virtual void ShootCharge(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			shootTick = 0;
		}

		protected override void ApplyEvolution() {
			base.ApplyEvolution();
			Item.damage = EvolutionData.Damage;
			Item.shootSpeed = EvolutionData.ShootSpeed;
			Item.knockBack = EvolutionData.Knockback;
			Item.useAmmo = EvolutionData.Ammo;
			Item.shoot = EvolutionData.Projectile;
			Item.autoReuse = false;
			Item.useAnimation = EvolutionData.UseTime;
		}

		protected override void ApplyLevel() {
			base.ApplyLevel();

			Item.damage += LevelInfo.Damage(Level);
			Item.shootSpeed += LevelInfo.ShootSpeed(Level);
			Item.knockBack += LevelInfo.Knockback(Level);
			Item.useAnimation = Item.useTime;

			if (!Array.Exists(LevelInfo.AvailableShootModes(Level), m => m == shootMode)) {
				shootMode = LevelInfo.AvailableShootModes(Level)[0];
			}

			Item.autoReuse = shootMode.IsAuto();
		}

		private void ApplySpread(ref Vector2 velocity) {
			double spread = EvolutionData.Spread - LevelInfo.Spread(Level);

			if (spread > float.Epsilon) {
				double spreadMax = random.NextBool() ? -spread : spread;
				velocity = velocity.RotatedBy(spreadMax * random.NextDouble());
			}
		}


	}
}
