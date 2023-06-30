namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    public class RetinaReleaser : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Retina Releaser");
            Tooltip.SetDefault("'I think they're looking at you'\nTaking damage releases a flurry of Cthulhubees and increases hymenoptra damage by 12% for a short time");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 75);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<BombusApisBeePlayer>().RetinaReleaser = true;
        }
    }
}