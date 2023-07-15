using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Legs)]
    public class BeekeepersPants : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("5% increased hymenoptra critical strike chance");
            DisplayName.SetDefault("Beekeeper's Pants");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(silver: 50);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 2;
        }

        public override void UpdateEquip(Player player)
        {
            player.IncreaseBeeCrit(5);
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient(ItemID.PlatinumBar, 15).
                AddIngredient(ItemID.Silk, 8).
                AddIngredient(ModContent.ItemType<Pollen>(), 12).
                AddTile(TileID.Loom).Register();

            CreateRecipe(1).
                AddIngredient(ItemID.GoldBar, 15).
                AddIngredient(ItemID.Silk, 8).
                AddIngredient(ModContent.ItemType<Pollen>(), 12).
                AddTile(TileID.Loom).Register();
        }
    }
}
