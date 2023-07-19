using System;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ModLoader;

using ThoriumMod;
using ThoriumMod.Buffs.Healer;

namespace ModBridge.Items.LivingWeapon {
	public abstract class ScytheLivingWeaponProjectile : LivingWeaponProjectile<ScytheLivingWeapon, ScytheLivingWeaponLevelInfo, ScytheLivingWeaponEvolutionData> {

		public float RotationSpeed = 0.25f;
		public int ScytheCount = 2;
		public int DustCount = 1;
		public int DustType = -1;
		public int SoulEssenceBonus = 0;

		public Vector2 dustOffset = Vector2.Zero;

		public bool CanGiveScytheCharge {
			get {
				return base.Projectile.localAI[0] == 0f;
			}
			set {
				base.Projectile.localAI[0] = (value ? 0f : 1f);
			}
		}

		public bool FirstHit {
			get {
				return base.Projectile.localAI[1] == 0f;
			}
			set {
				base.Projectile.localAI[1] = (value ? 0f : 1f);
			}
		}

		public Vector2 DustCenterBase => new Vector2(Projectile.width * Projectile.scale, -Projectile.height * Projectile.scale) / 2f;
		public Vector2 DustCenter => DustCenterBase + dustOffset;


		public override void SetDefaults() {
			Projectile.aiStyle = 0;
			Projectile.light = 0.2f;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.ownerHitCheck = true;
			Projectile.ignoreWater = true;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 26;
			Projectile.usesIDStaticNPCImmunity = true;
			Projectile.idStaticNPCHitCooldown = 10;
			Projectile.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
			RotationSpeed = 0.25f;
			ScytheCount = 2;
			DustCount = 1;
			DustType = -1;
		}

		public override void ModifyDamageHitbox(ref Rectangle hitbox) {
			int width = (int) (((float) Projectile.width + 16f) * Projectile.scale);
			int height = (int) (((float) Projectile.height + 16f) * Projectile.scale);

			hitbox = new Rectangle((int) Projectile.Center.X - width/2, (int) Projectile.Center.Y - height/2, width, height);
		}

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) {
			Player player = Main.player[Projectile.owner];
			hitDirection = ((!(target.Center.X < player.Center.X)) ? 1 : (-1));
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) {
			base.OnHitNPC(target, damage, knockback, crit);

			Player player = Main.player[Projectile.owner];
			ThoriumPlayer thoriumPlayer = player.GetModPlayer<ThoriumPlayer>();

			int charge = SoulEssenceBonus;
			if (charge > 0 && CanGiveScytheCharge && target.IsHostile()) {
				CanGiveScytheCharge = false;
				player.AddBuff(ModContent.BuffType<SoulEssence>(), 1800);
				CombatText.NewText(target.Hitbox, new Color(100, 255, 200), charge, dramatic: false, dot: true);
				thoriumPlayer.soulEssence += charge;
			}
			if (FirstHit) {
				FirstHit = false;
			}
		}

		public override bool ShouldUpdatePosition() {
			return false;
		}

		public override void AI() {
			Player player = Main.player[Projectile.owner];

			if (player.dead) {
				Projectile.Kill();
				return;
			}

			Projectile.rotation += (float) player.direction * RotationSpeed;
			Projectile.spriteDirection = player.direction;
			player.heldProj = Projectile.whoAmI;
			//Projectile.position = player.Center - new Vector2(Projectile.width / 2f, Projectile.height / 2f) * Projectile.scale;
			Projectile.Center = player.Center;
			Projectile.gfxOffY = player.gfxOffY;
			SpawnDust();
		}

		private void SpawnDust() {
			int count = DustCount;
			int scythes = ScytheCount;
			int type = DustType;

			Vector2 dustCenter = DustCenter;
			if (scythes <= 0 || count <= 0 || type <= -1) {
				return;
			}

			for (int scytheIndex = 0; scytheIndex < scythes; scytheIndex++) {
				float offset = (float) scytheIndex * ((float) Math.PI * 2f / (float) scythes);
				float rot = Projectile.rotation;
				Vector2 rotationOffset = dustCenter;

				if (Projectile.spriteDirection < 0) {
					rotationOffset.X = 0f - rotationOffset.X;
				}

				rotationOffset = rotationOffset.RotatedBy(rot + offset);
				Vector2 rotationCenter = Projectile.Center + new Vector2(0f, Projectile.gfxOffY) + rotationOffset;
				for (int i = 0; i < count; i++) {
					Dust dust = Dust.NewDustPerfect(rotationCenter, type, Vector2.Zero);
					dust.noGravity = true;
					dust.noLight = true;
				}
			}
		}



	}
}
