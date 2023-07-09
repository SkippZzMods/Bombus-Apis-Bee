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
            Item.ResearchUnlockCount = 1;
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
                int index = tooltips.FindIndex(tt => tt.Mod.Equals("Terraria") && tt.Name.Equals("Knockback"));

                if (index != -1)
                    tooltips.Insert(index + 1, new TooltipLine(Mod, "Honey Cost", $"Uses {beeResourceCost} honey"));
            }

            if (ResourceChance > 0 || Main.LocalPlayer.GetModPlayer<BeeDamagePlayer>().ResourceChanceAdd > 0f)
            {
                int index = tooltips.FindIndex(tt => tt.Mod.Equals("Terraria") && tt.Name.Equals("Knockback"));

                if (index != -1)
                    tooltips.Insert(index + 2, new TooltipLine(Mod, "Chance to consume honey", $"{Math.Round(Utils.Clamp((ResourceChance + Main.LocalPlayer.GetModPlayer<BeeDamagePlayer>().ResourceChanceAdd) * 100, 0, 50))}% chance to not consume honey"));
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
            int[] prefixes = new int[] {
                PrefixType<Rotten>(),
                PrefixType<Moldy>(),
                PrefixType<Snaillike>(),
                PrefixType<Punchy>(),
                PrefixType<Critical>(),
                PrefixType<Juicy>(),
                PrefixType<Piquant>(),
                PrefixType<Pungent>(),
                PrefixType<Bland>(),
                PrefixType<Succulent>(),
                PrefixType<Buzzing>(),
                PrefixType<Delectable>(),
            };

            int hymenoptraPrefixe = Main.rand.Next(prefixes);
            if (hymenoptraPrefixe == PrefixType<Delectable>() && beeResourceCost < 4)
            {
                prefixes = new int[] {
                    PrefixType<Rotten>(),
                    PrefixType<Moldy>(),
                    PrefixType<Snaillike>(),
                    PrefixType<Punchy>(),
                    PrefixType<Critical>(),
                    PrefixType<Juicy>(),
                    PrefixType<Piquant>(),
                    PrefixType<Pungent>(),
                    PrefixType<Bland>(),
                    PrefixType<Succulent>(),
                    PrefixType<Buzzing>(),
                };

                hymenoptraPrefixe = Main.rand.Next(prefixes);
            }

            return hymenoptraPrefixe;
        }
    }
}
