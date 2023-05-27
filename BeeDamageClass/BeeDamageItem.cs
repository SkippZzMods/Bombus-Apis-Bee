using System;
using System.Collections.Generic;
using Terraria.Utilities;

namespace BombusApisBee.BeeDamageClass
{
    public abstract class BeeDamageItem : ModItem
    {
        public int beeResourceCost = 0;
        public float ResourceChance = 0f;

        public int critAdd = 0;

        public virtual void SafeSetStaticDefaults() { }
        public sealed override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            SafeSetStaticDefaults();
        }
        public virtual void SafeSetDefaults()
        {
        }
        public sealed override void SetDefaults()
        {
            SafeSetDefaults();
            Item.DamageType = ModContent.GetInstance<HymenoptraDamageClass>();
            Item.crit = critAdd;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (beeResourceCost > 0)
            {
                tooltips.Add(new TooltipLine(Mod, "Honey Cost", $"Uses {beeResourceCost} honey"));
            }
            if (ResourceChance > 0 || Main.LocalPlayer.GetModPlayer<BeeDamagePlayer>().ResourceChanceAdd > 0f)
            {
                tooltips.Add(new TooltipLine(Mod, "Chance to consume honey", $"{Math.Round(Utils.Clamp((ResourceChance + Main.LocalPlayer.GetModPlayer<BeeDamagePlayer>().ResourceChanceAdd) * 100, 0, 50))}% chance to not consume honey"));
            }
        }
        public virtual bool UseHoney(Player player, float Chance)
        {
            return Main.rand.NextFloat() > Chance;
        }
        public virtual bool SafeCanUseItem(Player player)
        {
            return true;
        }
        public override bool CanUseItem(Player player)
        {
            bool bee = false;
            var BeeDamagePlayer = player.GetModPlayer<BeeDamagePlayer>();

            if (BeeDamagePlayer.BeeResourceCurrent >= beeResourceCost + BeeDamagePlayer.BeeResourceReserved && SafeCanUseItem(player))
            {
                if (UseHoney(player, player.TrueResourceChance()))
                {
                    BeeDamagePlayer.BeeResourceCurrent -= beeResourceCost;
                }
                BeeDamagePlayer.BeeResourceRegenTimer = Utils.Clamp(-Item.useAnimation * 3, -180, -20);
                //BeeDamagePlayer.BeeResourceRegenRate = 1f;
                bee = true;
            }
            return SafeCanUseItem(player) && bee;
        }

        public sealed override int ChoosePrefix(UnifiedRandom rand)
        {
            int hymenoptraPrefixes = Main.rand.Next(new int[] {
                ModContent.PrefixType<Godlike>(),
                ModContent.PrefixType<Embarrasing>(),
                ModContent.PrefixType<Hasty>(),
                ModContent.PrefixType<Offbalance>(),
                ModContent.PrefixType<Jammed>(),
                ModContent.PrefixType<Unwanted>(),
                ModContent.PrefixType<Accurate>(),
                ModContent.PrefixType<Acute>(),
                ModContent.PrefixType<Fast>(),
                ModContent.PrefixType<Effective>(),
                ModContent.PrefixType<Succulent>(),
                ModContent.PrefixType<Poor>(),
                ModContent.PrefixType<Destroyed>(),
                ModContent.PrefixType<Snailish>(),
                ModContent.PrefixType<Speedy>(),
                ModContent.PrefixType<Rotten>()
            });
            return hymenoptraPrefixes;
        }
    }
}
