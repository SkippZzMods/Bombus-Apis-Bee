namespace BombusApisBee.Content.Crossmod.Calamity.Items.Accessories.Wulfrum
{
    public class GlisteningGear : CalamityItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Beekeeper().BeeResourceMax2 += 10;
            player.IncreaseBeeUseSpeed(0.07f);
        }
    }
}
