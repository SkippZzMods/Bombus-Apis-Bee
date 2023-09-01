using BombusApisBee.Items.Other.Crafting;
using Terraria.Localization;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Legs)]
    public class LivingFlowerLeggings : BeeKeeperItem
    {
        public static int IncreaseCrit = 3;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(IncreaseCrit);
        public override void SetStaticDefaults()
        {
            //Tooltip.SetDefault("3% increased hymenoptra critical strike chance");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(copper: 65);
            Item.rare = ItemRarityID.White;
            Item.defense = 2;
        }

        public override void UpdateEquip(Player player)
        {
            player.IncreaseBeeCrit(3);
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