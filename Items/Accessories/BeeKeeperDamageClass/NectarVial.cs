namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    public class NectarVial : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Coats hymenoptra attacks in a sweet nectar, granting them lifesteal on critical strikes\nMaximum health increased by 20\n'Its a delicacy in most parts of the world'");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 20;
            Item.accessory = true;
            Item.rare = 2;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Bombus().NectarVial = true;
            player.statLifeMax2 += 20;
        }
    }
}
