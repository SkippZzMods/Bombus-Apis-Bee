namespace BombusApisBee.Core
{
    public abstract class HymenoptraPrefix : ModPrefix
    {
        internal float damageMult = 1f;
        internal int critBonus = 0;
        internal float speedBonus = 0f;
        internal string displayName;

        public override PrefixCategory Category => PrefixCategory.Custom;

        public HymenoptraPrefix() { }

        public HymenoptraPrefix(string displayName, float damageMult, int critBonus, float speedBonus)
        {
            this.displayName = displayName;
            this.damageMult = damageMult;
            this.critBonus = critBonus;
            this.speedBonus = speedBonus;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(displayName);
        }

        public override bool CanRoll(Item item)
        {
            return item.CountsAsClass<HymenoptraDamageClass>();
        }

        public override void Apply(Item item)
        {
            base.Apply(item);
        }

        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            damageMult = this.damageMult;
            critBonus = this.critBonus;
            useTimeMult = speedBonus;
        }
    }
    public class Godlike : HymenoptraPrefix
    {
        public Godlike() : base("Godlike", 1.2f, 6, 0.9f) { }
    }
    public class Embarrasing : HymenoptraPrefix
    {
        public Embarrasing() : base("Embarrasing", 0.85f, 0, 1.25f) { }
    }
    public class Hasty : HymenoptraPrefix
    {
        public Hasty() : base("Hasty", 1f, 0, 0.95f) { }
    }
    public class Offbalance : HymenoptraPrefix
    {
        public Offbalance() : base("Offbalance", 1.05f, 2, 1.1f) { }
    }
    public class Unwanted : HymenoptraPrefix
    {
        public Unwanted() : base("Unwanted", 0.8f, 0, 1.1f) { }
    }
    public class Jammed : HymenoptraPrefix
    {
        public Jammed() : base("Jammed", 1f, 0, 1.2f) { }
    }
    public class Accurate : HymenoptraPrefix
    {
        public Accurate() : base("Accurate", 1f, 5, 1f) { }
    }
    public class Acute : HymenoptraPrefix
    {
        public Acute() : base("Acute", 1f, 3, 1f) { }
    }
    public class Fast : HymenoptraPrefix
    {
        public Fast() : base("Fast", 1f, 0, 0.89f) { }
    }
    public class Effective : HymenoptraPrefix
    {
        public Effective() : base("Effective", 1.1f, 2, 1f) { }
    }
    public class Succulent : HymenoptraPrefix
    {
        public Succulent() : base("Succulent", 1.16f, 4, 0.87f) { }
    }
    public class Poor : HymenoptraPrefix
    {
        public Poor() : base("Poor", 0.95f, 0, 1.05f) { }
    }
    public class Destroyed : HymenoptraPrefix
    {
        public Destroyed() : base("Destroyed", 0.75f, 0, 1.1f) { }
    }
    public class Snailish : HymenoptraPrefix
    {
        public Snailish() : base("Snailish", 1f, 0, 1.15f) { }
    }
    public class Speedy : HymenoptraPrefix
    {
        public Speedy() : base("Speedy", 1f, 0, 0.79f) { }
    }
    public class Rotten : HymenoptraPrefix
    {
        public Rotten() : base("Rotten", 0.82f, 0, 1f) { }
    }

    public abstract class HymenoptraAccessoryPrefix : ModPrefix
    {
        public override PrefixCategory Category => PrefixCategory.Accessory;

        internal static List<int> HymenoptraAccessoryPrefixs;

        internal int honeyBonus = 0;
        internal float chanceBonus = 0f;

        public HymenoptraAccessoryPrefix() { }

        public HymenoptraAccessoryPrefix(int honeyBonus, float chanceBonus)
        {
            this.honeyBonus = honeyBonus;
            this.chanceBonus = chanceBonus;
        }
        public override void Load()
        {
            HymenoptraAccessoryPrefixs = new List<int>();
        }
        public override void Unload()
        {
            HymenoptraAccessoryPrefixs = null;
        }
        public override void SetStaticDefaults()
        {
            HymenoptraAccessoryPrefixs.Add(Type);
        }
        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 1f + (honeyBonus / 100f + chanceBonus);
        }
        public override void Apply(Item item)
        {
            item.GetGlobalItem<AccessoryGlobalItem>().chanceBonus = chanceBonus;
            item.GetGlobalItem<AccessoryGlobalItem>().honeyBonus = honeyBonus;
        }
    }
    public class Bittersweet : HymenoptraAccessoryPrefix
    {
        public Bittersweet() : base(4, 0f) { }
    }
    public class Sweetened : HymenoptraAccessoryPrefix
    {
        public Sweetened() : base(8, 0f) { }
    }
    public class Frugal : HymenoptraAccessoryPrefix
    {
        public Frugal() : base(0, 0.03f) { }
    }
    public class Conserving : HymenoptraAccessoryPrefix
    {
        public Conserving() : base(0, 0.06f) { }
    }
    public class AccessoryGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public float chanceBonus;
        public int honeyBonus;
        public AccessoryGlobalItem()
        {
            chanceBonus = 0f;
            honeyBonus = 0;
        }
        public override GlobalItem Clone(Item item, Item itemClone)
        {
            AccessoryGlobalItem GlobalItem = (AccessoryGlobalItem)base.Clone(item, itemClone);
            GlobalItem.chanceBonus = chanceBonus;
            GlobalItem.honeyBonus = honeyBonus;
            return GlobalItem;
        }
        public override void PreReforge(Item item)/* tModPorter Note: Use CanReforge instead for logic determining if a reforge can happen. */
        {
            chanceBonus = 0f;
            honeyBonus = 0;
        }
        private void BombusAccessoryTooltip(Item item, IList<TooltipLine> tooltips)
        {
            if (!item.accessory || item.social || item.prefix <= 0)
            {
                return;
            }
            float chanceBoost = item.GetGlobalItem<AccessoryGlobalItem>().chanceBonus;
            if (chanceBoost > 0f)
            {
                TooltipLine chanceTooltip = new TooltipLine(Mod, "chancePrefix", "+" + chanceBoost * 100 + "% chance to not consume honey")
                {
                    IsModifier = true
                };
                tooltips.Add(chanceTooltip);
            }
            int honeyBoost = item.GetGlobalItem<AccessoryGlobalItem>().honeyBonus;
            if (honeyBoost > 0)
            {
                TooltipLine honeyTooltip = new TooltipLine(Mod, "honeyPrefix", "+" + honeyBoost + " increased honey capacity")
                {
                    IsModifier = true
                };
                tooltips.Add(honeyTooltip);
            }
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            BombusAccessoryTooltip(item, tooltips);
        }
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (item.prefix > 0)
            {
                float chanceBoost = item.GetGlobalItem<AccessoryGlobalItem>().chanceBonus;
                if (chanceBoost > 0f)
                {
                    player.Hymenoptra().ResourceChanceAdd += chanceBoost;
                }
                int honeyBoost = item.GetGlobalItem<AccessoryGlobalItem>().honeyBonus;
                if (honeyBoost > 0f)
                {
                    player.Hymenoptra().BeeResourceMax2 += honeyBoost;
                }
            }
        }
    }
}
