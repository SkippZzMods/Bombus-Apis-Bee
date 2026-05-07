namespace BombusApisBee.Content.Crossmod.Calamity.Items.Accessories.Wulfrum
{
    public class GlisteningGear : CalamityItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("7% increased hymenoptra attack speed\nIncreases maximum honey by 10\n'How in the world does making a gear sticky help anything?!'");
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
