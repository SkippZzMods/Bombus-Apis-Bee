using BombusApisBee.Content.Forest.Items.HoneyJar;
using BombusApisBee.Content.Forest.Items.Pollen;
using Terraria.DataStructures;

namespace BombusApisBee.Content.Forest.Items.GlassOfHoney
{
    public class GlassOfHoney : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("10% increased beekeeper damage\nIncreases maximum honey by 10\nGreatly increases life regeneration");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(9, 7));
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(gold: 4, silver: 25);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.IncreaseBeeDamage(0.1f);
            player.Beekeeper().BeeResourceMax2 += 10;
            player.lifeRegen += 4;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<PollenItem>(), 10);
            recipe.AddIngredient(ItemType<JarOfHoney>());
            recipe.AddIngredient(ItemID.OrichalcumBar, 8);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.Register();

            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemType<PollenItem>(), 10);
            recipe2.AddIngredient(ItemType<JarOfHoney>());
            recipe2.AddIngredient(ItemID.MythrilBar, 8);
            recipe2.AddTile(TileID.TinkerersWorkbench);
            recipe2.Register();
        }
    }
}
