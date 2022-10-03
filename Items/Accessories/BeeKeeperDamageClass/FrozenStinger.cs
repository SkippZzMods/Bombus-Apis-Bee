using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    public class FrozenStinger : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("8% increased hymenoptra critical strike chance\nHymenoptra critical strikes inflict Frostbroken\nEnemies who are Frostbroken have their defense reduced by 25, take 5% more damage, and take damage over time");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 4);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Bombus().FrozenStinger = true;
            player.IncreaseBeeCrit(8);
        }
    }
}
