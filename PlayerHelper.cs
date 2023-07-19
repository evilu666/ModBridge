using Terraria;
using Terraria.ModLoader;
using System;
using ThoriumMod;

namespace ModBridge {
  internal static class PlayerHelper {
    public static int HealMana(this Player player, int healAmount, bool healOverMax = true) {
      int real = 0;
      if ((!healOverMax && player.statMana >= player.statManaMax2) || healAmount <= 0) {
        return real;
      }
      real = healAmount;
      if (!healOverMax && player.statManaMax2 < healAmount + player.statMana) {
        real = player.statManaMax2 - player.statMana;
      }
      player.statMana += real;
      if (healOverMax && player.statMana > player.statManaMax2) {
        player.statMana = player.statManaMax2;
      }
      player.ManaEffect(real);
      return real;
    }

    public static int HealLife(this Player player, int healAmount, Player healer = null, bool healOverMax = true, bool statistics = true) {
      int real = 0;
      if (healer == null) {
        healer = player;
      }
      if (healer.whoAmI != Main.myPlayer && Main.netMode != 2) {
        return real;
      }
      if ((!healOverMax && player.statLife >= player.statLifeMax2) || healAmount <= 0) {
        return real;
      }
      real = healAmount;
      int healClamped = Math.Max(0, Math.Min(real, player.statLifeMax2 - player.statLife));
      if (!healOverMax && player.statLifeMax2 < healAmount + player.statLife) {
        real = healClamped;
      }
      player.statLife += real;
      if (statistics) {
        int healForStatistics = healClamped;
        ThoriumPlayer thoriumPlayer = healer.GetModPlayer<ThoriumPlayer>();
        thoriumPlayer.AddHPS(healForStatistics);
        thoriumPlayer.totalHealingDone += healForStatistics;
        thoriumPlayer.totalHealingDarkHeart += healForStatistics;
        thoriumPlayer.healStreak += healForStatistics;
        thoriumPlayer.healStreakTimer = 180;
        thoriumPlayer.healStreakDisplay = true;
      }
      if (healOverMax && player.statLife > player.statLifeMax2) {
        player.statLife = player.statLifeMax2;
      }
      if (healer.whoAmI == Main.myPlayer) {
        player.HealEffect(real);
      }
      if (player.whoAmI != Main.myPlayer) {
        NetMessage.SendData(66, -1, -1, null, player.whoAmI, real);
      }
      return real;
    }
  }
}

