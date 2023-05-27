namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    public class HimensApiary : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Himen's Apiary");
            Tooltip.SetDefault("'The apiary of one of the greatest hymenoptrian monster hunter's in history... he was said to be bested by a large plant-like creature'\nDrops honey droplets when damaged");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Expert;
            Item.value = Item.sellPrice(gold: 1);
            Item.expert = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Bombus().HimenApiary = true;
        }
    }
}