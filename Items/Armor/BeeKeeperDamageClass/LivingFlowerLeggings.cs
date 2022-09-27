using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Legs)]
    public class LivingFlowerLeggings : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("2% increased hymenoptra damage");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(copper: 65);
            Item.rare = ItemRarityID.White;
            Item.defense = 1;
        }

        public override void UpdateEquip(Player player)
        {
            player.IncreaseBeeDamage(0.02f);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Wood, 20);
            recipe.AddIngredient(ItemID.Daybloom);
            recipe.AddIngredient(ModContent.ItemType<Pollen>(), 4);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}