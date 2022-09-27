using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Body)]
    public class LivingFlowerChestplate : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Flower Chestplate");
            Tooltip.SetDefault("3% increased hymenoptra damage");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(copper: 50);
            Item.rare = ItemRarityID.White;
            Item.defense = 2;
        }

        public override void UpdateEquip(Player player)
        {
            player.IncreaseBeeDamage(0.03f);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Wood, 25);
            recipe.AddIngredient(ItemID.Daybloom);
            recipe.AddIngredient(ModContent.ItemType<Pollen>(), 5);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}
