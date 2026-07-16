namespace BombusApisBee.Content.Jungle.Items.NectarVial
{
    public class NectarVial : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {

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
