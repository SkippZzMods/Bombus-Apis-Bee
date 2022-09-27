using BombusApisBee.Items.Other.Crafting;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    public class GlassOfHoney : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("10% increased hymenoptra damage\nIncreases maximum honey by 10\nGreatly increases life regeneration");
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
            player.Hymenoptra().BeeResourceMax2 += 10;
            player.lifeRegen += 4;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<Pollen>(), 10);
            recipe.AddIngredient(ModContent.ItemType<JarOfHoney>());
            recipe.AddIngredient(ItemID.OrichalcumBar, 8);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.Register();

            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ModContent.ItemType<Pollen>(), 10);
            recipe2.AddIngredient(ModContent.ItemType<JarOfHoney>());
            recipe2.AddIngredient(ItemID.MythrilBar, 8);
            recipe2.AddTile(TileID.TinkerersWorkbench);
            recipe2.Register();
        }
    }
}
