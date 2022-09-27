using Terraria.DataStructures;

namespace BombusApisBee.Items.Other.Crafting
{
    public class Pollen : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pollen");
            Tooltip.SetDefault("'Careful if you're allergic'");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(7, 6));
            SacrificeTotal = 999;
        }
        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Blue;
            Item.value = 10;
            Item.width = Item.height = 16;
        }

        public override bool? CanBurnInLava()
        {
            return true;
        }
    }
}