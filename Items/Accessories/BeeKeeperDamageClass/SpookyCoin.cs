
/*namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    public class SpookyCoin : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Increases hymenoptra damage and critical strike chance by 6% when above 35% honey\nMassively increases honey regeneration when below 35% honey");
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
            if (player.Hymenoptra().BeeResourceCurrent > player.Hymenoptra().BeeResourceMax2 * 0.35f)
            {
                player.IncreaseBeeDamage(0.06f);
                player.IncreaseBeeCrit(6);
            }
            else
            {
                //player.Hymenoptra().RegenRateStart -= 0.55f;
                //player.Hymenoptra().RegenRateLowerLimit -= 0.03f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.SpookyWood, 75).AddIngredient<Pollen>(35).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}*/
