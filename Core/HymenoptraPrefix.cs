using Terraria.ID;
using Terraria.Localization;

namespace BombusApisBee.Core
{
    public abstract class HymenoptraPrefix : ModPrefix
    {
        internal float damageMult = 1f;
        internal int critBonus = 0;
        internal float speedBonus = 0f;
        internal float chanceBonus = 0f;
        internal int resourceBonus = 0;
        internal string displayName;

        public override PrefixCategory Category => PrefixCategory.Custom;

        public HymenoptraPrefix() { }

        public HymenoptraPrefix(string displayName, float damageMult = 1f, int critBonus = 0, float speedBonus = 1f, float chanceBonus = 0f, int resourceBonus = 0)
        {
            this.displayName = displayName;
            this.damageMult = damageMult;
            this.critBonus = critBonus;
            this.speedBonus = speedBonus;
            this.chanceBonus = chanceBonus;
            this.resourceBonus = resourceBonus;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(displayName);
        }

        public override bool CanRoll(Item item)
        {
            return item.CountsAsClass<HymenoptraDamageClass>();
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult += chanceBonus + (resourceBonus / 10f);
        }

        public override void Apply(Item item)
        {
            var realItem = item.ModItem as BeeDamageItem;
            if (realItem is null)
                return;

            if (resourceBonus != 0)
                realItem.honeyCost -= resourceBonus;

            if (chanceBonus != 0)
                realItem.resourceChance += chanceBonus;
        }

        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            bool bad = resourceBonus < 0 ? true : false;

            string bonus = Math.Abs(resourceBonus).ToString();
            
            if (resourceBonus != 0)
            {
                yield return new TooltipLine(Mod, "BombusApisBee : PrefixBonusResource", (bad ? "+" : "-") + bonus + " honey cost")
                {
                    IsModifier = true,
                    IsModifierBad = bad,
                };
            }

            if (chanceBonus != 0)
            {
                yield return new TooltipLine(Mod, "BombusApisBee : PrefixBonusChance", "+" + (chanceBonus * 100f) + "% chance to not use honey")
                {
                    IsModifier = true,
                };
            }
        }

        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            damageMult = this.damageMult;
            critBonus = this.critBonus;
            useTimeMult = speedBonus;
        }
    }

    public class Rotten : HymenoptraPrefix { public Rotten() : base("Rotten", damageMult: 0.8f, resourceBonus: -1) { } }
    public class Moldy : HymenoptraPrefix { public Moldy() : base("Moldy", damageMult: 0.8f, speedBonus: 1.1f) { } }
    public class Snaillike : HymenoptraPrefix { public Snaillike() : base("Snail-like", speedBonus: 1.2f) { } }

    public class Punchy : HymenoptraPrefix { public Punchy() : base("Punchy", damageMult: 1.2f, resourceBonus: -1) { } }
    public class Critical : HymenoptraPrefix { public Critical() : base("Critical", damageMult: 0.9f, critBonus: 10) { } }

    public class Juicy : HymenoptraPrefix { public Juicy() : base("Juicy", damageMult: 1.1f) { } }
    public class Piquant : HymenoptraPrefix { public Piquant() : base("Piquant", critBonus: 5, chanceBonus: 0.1f) { } }
    public class Pungent : HymenoptraPrefix { public Pungent() : base("Pungent", damageMult: 1.15f, speedBonus: 0.9f) { } }
    public class Bland : HymenoptraPrefix { public Bland() : base("Bland", damageMult: 1.05f, critBonus: 5, speedBonus: 0.95f, chanceBonus: 0.05f) { } }

    public class Delicious : HymenoptraPrefix { public Delicious() : base("Delicious", damageMult: 1.15f, speedBonus: 0.85f) { } }
    public class Succulent : HymenoptraPrefix { public Succulent() : base("Succulent", damageMult: 1.2f) { } }
    public class Buzzing : HymenoptraPrefix { public Buzzing() : base("Buzzing", speedBonus: 0.75f, chanceBonus: 0.25f) { } }
    public class Delectable : HymenoptraPrefix { public Delectable() : base("Delectable", chanceBonus: 0.1f, resourceBonus: 1) { } }
    public abstract class HymenoptraAccessoryPrefix : ModPrefix
    {
        public override PrefixCategory Category => PrefixCategory.Accessory;

        internal int honeyBonus = 0;
        internal float chanceBonus = 0f;
        internal string displayName;

        public HymenoptraAccessoryPrefix() { }

        public HymenoptraAccessoryPrefix(string displayName, int honeyBonus, float chanceBonus)
        {
            this.honeyBonus = honeyBonus;
            this.chanceBonus = chanceBonus;
            this.displayName = displayName;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(displayName);
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 1f + (honeyBonus / 100f + chanceBonus);
        }

        public override void ApplyAccessoryEffects(Player player)
        {
            if (honeyBonus > 0)
                player.Hymenoptra().BeeResourceMax2 += honeyBonus;

            if (chanceBonus > 0f)
                player.Hymenoptra().ResourceChanceAdd += chanceBonus;
        }

        public override float RollChance(Item item)
        {
            if (item.vanity)
                return 0f;
            if (item.type == ItemID.GuideVoodooDoll || item.type == ItemID.MusicBox || item.type == ItemID.ClothierVoodooDoll)
                return 0f;
            if (item.type >= ItemID.MusicBoxOverworldDay && item.type <= ItemID.MusicBoxBoss3)
                return 0f;
            if (item.type >= ItemID.MusicBoxSnow && item.type <= ItemID.MusicBoxMushrooms)
                return 0f;

            return 1f;
        }

        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            if (honeyBonus > 0)
            {
                yield return new TooltipLine(Mod, "BombusApisBee : AccessoryBonusPrefixResource", "+" + honeyBonus + " honey")
                {
                    IsModifier = true,
                };
            }

            if (chanceBonus > 0)
            {
                yield return new TooltipLine(Mod, "BombusApisBee : AccessoryBonusPrefixResource", "+" + chanceBonus * 100 + "% chance to not use honey")
                {
                    IsModifier = true,
                };
            }
        }
    }
    public class Bittersweet : HymenoptraAccessoryPrefix
    {
        public Bittersweet() : base("Bittersweet", 4, 0f) { }
    }
    public class Sweetened : HymenoptraAccessoryPrefix
    {
        public Sweetened() : base("Sweetened", 8, 0f) { }
    }
    public class Frugal : HymenoptraAccessoryPrefix
    {
        public Frugal() : base("Frugal", 0, 0.03f) { }
    }
    public class Conserving : HymenoptraAccessoryPrefix
    {
        public Conserving() : base("Conserving", 0, 0.06f) { }
    }
}
