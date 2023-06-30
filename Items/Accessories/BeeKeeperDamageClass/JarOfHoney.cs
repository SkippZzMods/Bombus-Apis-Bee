using BombusApisBee.Items.Other.Crafting;
using Terraria;

namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    public class JarOfHoney : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Jar Of Honey");
            Tooltip.SetDefault("Increases hymenoptra damage by 6%\nIncreases maximum honey by 5\nIncreases life regeneration");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 24;
            Item.accessory = true;
            Item.value = Item.sellPrice(silver: 25);
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.lifeRegen += 1;
            player.IncreaseBeeDamage(0.06f);
            player.Hymenoptra().BeeResourceMax2 += 5;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient(ItemID.Bottle, 1).
                AddIngredient(ModContent.ItemType<Pollen>(), 20).
                AddCondition(Condition.NearHoney).
                Register();
        }
    }
}