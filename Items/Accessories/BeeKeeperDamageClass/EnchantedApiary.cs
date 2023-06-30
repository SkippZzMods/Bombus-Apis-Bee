namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    public class EnchantedApiary : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("'Its glistening!'\nDrops enchanted honey droplets when damaged");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(gold: 1);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Bombus().enchantedhoney = true;
        }
    }
}