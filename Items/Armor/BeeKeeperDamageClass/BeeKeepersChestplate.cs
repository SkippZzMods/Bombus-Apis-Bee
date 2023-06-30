using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Body)]
    public class BeeKeepersChestplate : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bee Keeper's Shirt");
            Tooltip.SetDefault("4% increased hymenoptra damage\nIncreases your amount of Loyal Bees by 1");
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
            player.IncreaseBeeDamage(0.06f);
            player.Hymenoptra().CurrentBees += 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.Silk, 10).AddIngredient(ModContent.ItemType<Pollen>(), 15).AddTile(TileID.Loom).Register();
        }
    }
}
