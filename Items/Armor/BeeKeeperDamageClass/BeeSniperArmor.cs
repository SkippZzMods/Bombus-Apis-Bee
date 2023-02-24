namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Body)]
    public class BeeSniperArmor : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("15% increased chance to not consume honey\nIncreases your amount of Bees by 1");
            SacrificeTotal = 1;
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
            player.Hymenoptra().ResourceChanceAdd += 0.15f;
            player.Hymenoptra().CurrentBees += 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.Silk, 10).AddIngredient(ItemID.BeeWax, 17).AddIngredient(ItemID.TitaniumBar, 21).AddTile(TileID.MythrilAnvil).Register();
            CreateRecipe(1).AddIngredient(ItemID.Silk, 10).AddIngredient(ItemID.BeeWax, 17).AddIngredient(ItemID.AdamantiteBar, 21).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
