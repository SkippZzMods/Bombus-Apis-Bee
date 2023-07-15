using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Body)]
    public class BeekeepersRobe : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beekeeper's Robe");
            Tooltip.SetDefault("5% increased hymenoptra damage\nLoyal Bees increased by 1");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(silver: 75);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
            player.IncreaseBeeDamage(0.05f);
            player.Hymenoptra().CurrentBees += 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient(ItemID.PlatinumBar, 20).
                AddIngredient(ItemID.Silk, 10).
                AddIngredient(ModContent.ItemType<Pollen>(), 15).
                AddTile(TileID.Loom).Register();

            CreateRecipe(1).
                AddIngredient(ItemID.GoldBar, 20).
                AddIngredient(ItemID.Silk, 10).
                AddIngredient(ModContent.ItemType<Pollen>(), 15).
                AddTile(TileID.Loom).Register();
        }
    }
}
