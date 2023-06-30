namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Legs)]
    public class BeeSniperLeggings : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("7% increased hymenoptra damage and critical strike chance\nIncreases your amount of Loyal Bees by 1");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 2, silver: 15);
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 6;
        }

        public override void UpdateEquip(Player player)
        {
            player.IncreaseBeeDamage(0.05f);
            player.IncreaseBeeCrit(5);
            player.Hymenoptra().CurrentBees += 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.Silk, 9).AddIngredient(ItemID.BeeWax, 15).AddIngredient(ItemID.TitaniumBar, 17).AddTile(TileID.MythrilAnvil).Register();
            CreateRecipe(1).AddIngredient(ItemID.Silk, 9).AddIngredient(ItemID.BeeWax, 15).AddIngredient(ItemID.AdamantiteBar, 17).AddTile(TileID.MythrilAnvil).Register();
        }

    }
}