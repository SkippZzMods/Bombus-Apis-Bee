using Terraria.Utilities;

namespace BombusApisBee.BeeDamageClass
{
    public abstract class BeeDamageItem : ModItem
    {
        public int honeyCost = 0;
        public int altHoneyCost = 0;
        public int critAdd = 0;

        public float resourceChance = 0f;        

        public virtual void SafeSetStaticDefaults() { }
        public sealed override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            SafeSetStaticDefaults();
        }

        public virtual void SafeSetDefaults() { }       
        public sealed override void SetDefaults()
        {
            SafeSetDefaults();
            Item.DamageType = ModContent.GetInstance<HymenoptraDamageClass>();
            Item.crit = critAdd;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (honeyCost > 0)
            {
                int index = tooltips.FindIndex(tt => tt.Mod.Equals("Terraria") && tt.Name.Equals("Knockback"));

                if (index != -1)
                    tooltips.Insert(index + 1, new TooltipLine(Mod, "Honey Cost", $"Uses [c/FFBC00:{honeyCost}]{(altHoneyCost > 0 ? " [c/FFA200:|] [c/FF9100:" + altHoneyCost + "]" : string.Empty)} honey"));
            }

            if (resourceChance > 0 || Main.LocalPlayer.GetModPlayer<BeeDamagePlayer>().ResourceChanceAdd > 0f)
            {
                int index = tooltips.FindIndex(tt => tt.Mod.Equals("Terraria") && tt.Name.Equals("Knockback"));

                if (index != -1)
                    tooltips.Insert(index + 2, new TooltipLine(Mod, "Chance to consume honey", $"{Math.Round(Utils.Clamp((resourceChance + Main.LocalPlayer.GetModPlayer<BeeDamagePlayer>().ResourceChanceAdd) * 100, 0, 50))}% chance to not consume honey"));
            }

            if (altHoneyCost > 0)
            {
                if (!Main.LocalPlayer.controlUp)
                    tooltips.Add(new TooltipLine(Mod, "KeywordInfo", "[c/646464:Press UP for more info]"));
                else
                {
                    int index = tooltips.FindIndex(tt => tt.Mod.Equals("BombusApisBee") && tt.Name.Equals("Honey Cost"));

                    if (index != -1)
                        tooltips.Insert(index + 1, new TooltipLine(Mod, "HoneyCostDescription", "Uses [c/FFBC00:{Honey Cost On Left Click}] [c/FFA200:|] [c/FF9100:{Honey Cost On Right Click}] honey"));
                }
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

            int honeyCost = this.honeyCost;
            if (player.altFunctionUse == 2)
                honeyCost = altHoneyCost;

            if (BeeDamagePlayer.BeeResourceCurrent >= honeyCost + BeeDamagePlayer.BeeResourceReserved && SafeCanUseItem(player))
            {
                if (UseHoney(player, player.TrueResourceChance()))
                    BeeDamagePlayer.BeeResourceCurrent -= honeyCost;

                BeeDamagePlayer.BeeResourceRegenTimer = Utils.Clamp(-Item.useAnimation * 3, -180, -20);
                
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
            if (hymenoptraPrefixe == PrefixType<Delectable>() && honeyCost < 4)
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
