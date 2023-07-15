namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Body)]
    public class BeeSniperArmor : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("25% increased chance to not consume honey\nLoyal Bees increased by 3");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 2, silver: 80);
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 7;
        }

        public override void UpdateEquip(Player player)
        {
            player.Hymenoptra().ResourceChanceAdd += 0.25f;
            player.Hymenoptra().CurrentBees += 3;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.Silk, 10).AddIngredient(ItemID.BeeWax, 17).AddIngredient(ItemID.TitaniumBar, 21).AddTile(TileID.MythrilAnvil).Register();
            CreateRecipe(1).AddIngredient(ItemID.Silk, 10).AddIngredient(ItemID.BeeWax, 17).AddIngredient(ItemID.AdamantiteBar, 21).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
