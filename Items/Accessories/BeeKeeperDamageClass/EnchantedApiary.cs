namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    public class EnchantedApiary : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("'Its glistening!'\nKilling an enemy has a chance to spawn up to two droplets of enchanted honey");
            SacrificeTotal = 1;
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
            var modPlayer2 = player.GetModPlayer<BombusApisBeePlayer>();
            modPlayer2.enchantedhoney = true;
        }
    }
}