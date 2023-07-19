using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

using Microsoft.Xna.Framework;

using System.IO;
using System.Data;
using System.Collections.Generic;

using ModBridge.Util;
using ModBridge.Global;

namespace ModBridge.Items {


	public abstract class LivingWeaponLevelInfo {

		public abstract int UseTime(int level);
		public abstract int XpFromKill(Player player, int level, NPC killedTarget, int damageDealt);
		public abstract int XpFromDamage(Player player, int level, NPC damagedTarget, int damage, bool isCrit);
		public abstract int XpFromUse(Player player, int level);

		public virtual bool AltFunctionUse(Player player, int level, Item item) {
			return false;
		}
		
		public abstract int LevelUpXpRequirement(int level);
	}

	public delegate bool  EvolutionCondition<L, E, W>(W weapon)
			where L : LivingWeaponLevelInfo
			where E : LivingWeaponEvolutionData<L>
			where W : BaseLivingWeapon<L, E>;


	public class LivingWeaponEvolutionData<T> where T : LivingWeaponLevelInfo {
		public string Name;
		public int Width;
		public int Height;
		public float Scale;
		public string Texture;
		public int UseTime;
		public int UseAnimationTime;
		public int HoldStyle;
		public bool HoldRotate;
		public Vector2 HoldoutOffset;
		public SoundStyle UseSound;

		public int LevelRequirement;
		public T LevelInfo;

		public bool IsEvolutionConditionHidden;
		public string CustomEvolutionConditionDescription;
		public bool IsCustomEvolutionConditionHidden;
		public EvolutionCondition<T, LivingWeaponEvolutionData<T>, BaseLivingWeapon<T, LivingWeaponEvolutionData<T>>> CustomEvolutionCondition;

		public LivingWeaponEvolutionData(
				string name,
				int width,
				int height,
				float scale,
				string texture,
				int useTime,
				int useAnimationTime,
				int holdStyle,
				bool holdRotate,
				Vector2 holdoutOffset,
				SoundStyle useSound,
				int levelRequirement,
				T levelInfo,
				bool isEvolutionConditionHidden,
				string customEvolutionConditionDescription = "",
				EvolutionCondition<T, LivingWeaponEvolutionData<T>, BaseLivingWeapon<T, LivingWeaponEvolutionData<T>>> customEvolutionCondition = null,
				bool isCustomEvolutionConditionHidden = true) {

			this.Name = name;
			this.Width = width;
			this.Height = height;
			this.Scale = scale;
			this.Texture = texture;
			this.UseTime = useTime;
			this.UseAnimationTime = useAnimationTime;
			this.HoldStyle = holdStyle;
			this.HoldRotate = holdRotate;
			this.HoldoutOffset = holdoutOffset;
			this.UseSound = useSound;
			this.LevelRequirement = levelRequirement;
			this.LevelInfo = levelInfo;
			this.IsEvolutionConditionHidden = isEvolutionConditionHidden;
			this.CustomEvolutionConditionDescription = customEvolutionConditionDescription;
			this.IsCustomEvolutionConditionHidden = isCustomEvolutionConditionHidden;
		}

		public virtual bool IsCustomEvolutionConditionMet<L, E, W>(W weapon)
				where L : T
				where E : LivingWeaponEvolutionData<L>
				where W : BaseLivingWeapon<L, E> {
			return true;
		}

		public string EvolutionCondition => "Reach level " + LevelRequirement;

		public virtual bool AreEvolutionConditionsMet<L, E, W>(W weapon)
				where L : T
				where E : LivingWeaponEvolutionData<L>
				where W : BaseLivingWeapon<L, E> {
			if (weapon.Level >= LevelRequirement && IsCustomEvolutionConditionMet<L, E, W>(weapon)) {
				if (CustomEvolutionCondition is EvolutionCondition<L, E, W> condition && !condition(weapon)) {
					return false;
				}

				return true;
			} else {
				return false;
			}
		}

	}

	public abstract class BaseLivingWeapon<L, E> : ModItem 
			where L : LivingWeaponLevelInfo
			where E : LivingWeaponEvolutionData<L> {

		public static Color XpGainColor = new Color(72, 196, 68, 255);
		public static Color LevelUpColor = new Color(48, 87, 178, 255);
		public static Color EvolveColor = new Color(178, 48, 139, 255);
		public static Color AwakeColor = new Color(117, 31, 57, 255);

		
		private static SoundStyle LevelUpSound = new SoundStyle("ModBridge/Sounds/General/LevelUp") with {
			MaxInstances = 2
		};

		private static SoundStyle AwakeSound = new SoundStyle("ModBridge/Sounds/General/Awake") with {
			MaxInstances = 1
		};

		private static SoundStyle EvolveSound = new SoundStyle("ModBridge/Sounds/General/Evolve") with {
			MaxInstances = 1
		};

		public int Xp = 0;
		public int Level = 0;
		public int Evolution = 0;

		public bool IsAwoken = false;

		public Dictionary<int, int> NpcDamage = new Dictionary<int, int>();

		protected Player Owner;


		protected E[] evolutionData;

		public BaseLivingWeapon() {
			NpcDeathHandler.RegisterListener(npc => {
				if (NpcDamage.ContainsKey(npc.whoAmI)) {
					if (Owner != null) {
						OnKillNpc(Owner, npc, NpcDamage[npc.whoAmI]);
					}

					NpcDamage.Remove(npc.whoAmI);
				}
			});
		}

		public override void SetStaticDefaults() {
			DisplayName.SetDefault(EvolutionData.Name);


			ApplyStaticDefaults();
		}

		public override void SetDefaults() {
			Xp = 0;
			Level = 0;
			Evolution = 0;

			ApplyDefaults();
			ApplyEvolution();
			ApplyLevel();
		}

		public override string Texture {
			get => EvolutionData.Texture;
		}

		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool isCrit) {
			AddNpcDamage(target, damage);

			int xpGain = LevelInfo.XpFromDamage(player, Level, target, damage, isCrit);

			RenderUtil.ShowTargetCombatText(target, XpGainColor, xpGain + " xp");
			GainXp(player, xpGain);
		}

		public override bool? UseItem(Player player) {
			int xpGained = LevelInfo.XpFromUse(player, Level);


			if (xpGained > 0) {
				RenderUtil.ShowCombatText(player, XpGainColor, xpGained + " xp");
				GainXp(player, xpGained);
			}

			if (CanEvolve()) {
				Evolve(player);
			}

			return true;
		}

		public override bool AltFunctionUse(Player player) {
			return LevelInfo.AltFunctionUse(player, Level, Item);
		}

		public override void ModifyTooltips(List<TooltipLine> lines) {
			if (IsAwoken) {
				int i = 0;
				lines.Add(new TooltipLine(Mod, "Tooltip#" + i++, "This is a living weapon, it may gain xp under certain conditions and become stronger."));

				if (Main.masterMode || Main.expertMode) {
					lines.Add(new TooltipLine(Mod, "Tooltip#" + i++, "Some have even heard that these weapons can evolve into something much stronger under certian conditions..."));

					if (!EvolutionData.IsEvolutionConditionHidden) {
						lines.Add(new TooltipLine(Mod, "Tooltip#" + i++, "Evolution condition: " + EvolutionData.EvolutionCondition));

						if (!EvolutionData.IsCustomEvolutionConditionHidden) {
							lines.Add(new TooltipLine(Mod, "Tooltip#" + i++, "Additional condition: " + EvolutionData.CustomEvolutionConditionDescription));
						}
					}
				}

				lines.Add(new TooltipLine(Mod, "Tooltip#" + i++, "Level: " + Level));
				lines.Add(new TooltipLine(Mod, "Tooltip#" + i++, "Xp: " + Xp + "/" + LevelInfo.LevelUpXpRequirement(Level+1)));
			} else {
				lines.Add(new TooltipLine(Mod, "Tooltip#0", "This weapon may be more then it seems..."));
			}
		}

		public override void SaveData(TagCompound tag) {
			tag["Xp"] = Xp;
			tag["Level"] = Level;
			tag["Evolution"] = Evolution;
			tag["IsAwoken"] = IsAwoken;
		}

		public override void LoadData(TagCompound tag) {
			Xp = tag.Get<int>("Xp");
			Level = tag.Get<int>("Level");
			Evolution = tag.Get<int>("Evolution");
			IsAwoken = tag.Get<bool>("IsAwoken");

			ApplyEvolution();
			ApplyLevel();
		}

		public override ModItem Clone(Item item) {
			BaseLivingWeapon<L, E> clone = (BaseLivingWeapon<L, E>) base.Clone(item);
			clone.evolutionData = evolutionData;
			clone.Xp = Xp;
			clone.Level = Level;
			clone.Evolution = Evolution;

			clone.ApplyEvolution();
			clone.ApplyLevel();

			return clone;
		}

		public override void HoldStyle(Player player, Rectangle frame) {
			Owner = player;

			if (EvolutionData.HoldRotate) {
				Vector2 dir = (Main.MouseWorld - player.Center);
				dir.Normalize();
				player.itemRotation = (dir * player.direction).ToRotation();
			}
		}

		public override Vector2? HoldoutOffset() {
			return EvolutionData.HoldoutOffset;
		}

		public override void UpdateInventory(Player player) {
			Owner = player;
		}

		public override void Update(ref float gravity, ref float maxFallSpeed) {
			Owner = null;
		}

		public override void NetSend(BinaryWriter writer) {
			base.NetSend(writer);

			writer.Write(Xp);
			writer.Write(Level);
			writer.Write(Evolution);
			writer.Write(IsAwoken);
		}

		public override void NetReceive(BinaryReader reader) {
			base.NetReceive(reader);

			Xp = reader.ReadInt32();
			Level = reader.ReadInt32();
			Evolution = reader.ReadInt32();
			IsAwoken = reader.ReadBoolean();

			ApplyEvolution();
			ApplyLevel();
		}

		protected virtual E EvolutionData {
			get {
				if (evolutionData == null || Evolution >= evolutionData.Length) throw new ConstraintException("No evolution data set for evolution: " + Evolution);
				return evolutionData[Evolution];
			}
		}

		protected virtual L LevelInfo => EvolutionData.LevelInfo;

		protected virtual void ApplyStaticDefaults() {}
		protected virtual void ApplyDefaults() {}
		protected abstract bool AreAwakeningConditionsMet(Player player, NPC killedTarget);

		protected virtual void ApplyEvolution() {
			if (DisplayName != null) DisplayName.SetDefault(EvolutionData.Name);

			var prop = Item.GetType().GetField("_nameOverride", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			prop.SetValue(Item, EvolutionData.Name);

			// Render Settings
			Item.width = EvolutionData.Width;
			Item.height = EvolutionData.Height;
			Item.scale = EvolutionData.Scale;
			Item.useTime = EvolutionData.UseTime;
			Item.useAnimation = EvolutionData.UseAnimationTime;
			Item.holdStyle = EvolutionData.HoldStyle;

			// Effect settings
			Item.UseSound = EvolutionData.UseSound;
		}

		protected virtual void ApplyLevel() {
			Item.useTime -= LevelInfo.UseTime(Level);
		}

		protected void Awake(Player player) {
			RenderUtil.ShowCombatText(player, AwakeColor, EvolutionData.Name + " has awoken!");
			SoundEngine.PlaySound(AwakeSound, player.Center);
			//TODO: More fancy awake visual effects

			IsAwoken = true;
		}

		protected void LevelUp(Player player) {
			RenderUtil.ShowCombatText(player, LevelUpColor, "Level up!");
			SoundEngine.PlaySound(LevelUpSound, player.Center);
			//TODO: More fancy level up visual effects

			if (CanEvolve()) {
				Evolve(player);
				return;
			}

			ApplyEvolution();
			ApplyLevel();
		}

		protected void GainXp(Player player, int xpGained) {
			if (xpGained <= 0 || !IsAwoken) return;

			Xp += xpGained;
			
			int remainingXp = Xp - LevelInfo.LevelUpXpRequirement(Level+1);
			if (remainingXp > 0) {
				while (remainingXp > 0) {
					Level++;
					remainingXp = remainingXp - LevelInfo.LevelUpXpRequirement(Level+1);
				}

				Xp = remainingXp + LevelInfo.LevelUpXpRequirement(Level+1);

				LevelUp(player);
			}
		}

		protected virtual bool CanEvolve() {
			return Evolution < evolutionData.Length - 1 && EvolutionData.AreEvolutionConditionsMet<L, E, BaseLivingWeapon<L, E>>(this);
		}

		protected void Evolve(Player player) {
			string oldName = EvolutionData.Name;

			Evolution++;
			Level = 0;
			Xp = 0;

			

			ApplyEvolution();
			ApplyLevel();

			RenderUtil.ShowCombatText(player, EvolveColor, oldName + " evoloved into " + EvolutionData.Name + "!!!");
			SoundEngine.PlaySound(EvolveSound, player.Center);
			//TODO: More fancy evolution visual effects
		}

		protected void AddNpcDamage(NPC target, int damage) {
			if (!NpcDamage.ContainsKey(target.whoAmI)) {
				NpcDamage[target.whoAmI] = 0;
			}

			NpcDamage[target.whoAmI] += damage;
		}

		protected void OnKillNpc(Player player, NPC target, int damageDealt) {
			if (!IsAwoken && AreAwakeningConditionsMet(player, target)) {
				Awake(player);
				return;
			}

			int xpGain = LevelInfo.XpFromKill(player, Level, target, damageDealt);

			RenderUtil.ShowTargetCombatText(target, XpGainColor, xpGain + " xp");
			GainXp(player, xpGain);
		}


	}
}
