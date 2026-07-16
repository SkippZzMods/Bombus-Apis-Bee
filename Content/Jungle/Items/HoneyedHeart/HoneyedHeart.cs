using BombusApisBee.Content.Forest.Items.Pollen;

namespace BombusApisBee.Content.Jungle.Items.HoneyedHeart
{
    public class HoneyedHeart : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {

            // TODO: Change this to "Restoring Honey has a 5% chance to heal you for the amount of Honey gained"

        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 4);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Bombus().HoneyedHeart = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<PollenItem>(), 25);
            recipe.AddIngredient(ItemID.LifeFruit, 3);
            recipe.AddIngredient(ItemID.SoulofFright, 5);
            recipe.AddIngredient(ItemID.SoulofSight, 5);
            recipe.AddIngredient(ItemID.SoulofMight, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
