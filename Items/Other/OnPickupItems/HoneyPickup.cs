using BombusApisBee.BeeDamageClass;
using System;

namespace BombusApisBee.Items.Other.OnPickupItems
{
    public class HoneyPickup : ModItem
    {
        public int secondtimer;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Enchanted Honey");
            Tooltip.SetDefault("you shouldn't see this....");
        }
        public override void SetDefaults()
        {
            Item.width = Item.height = 20;
            Item.rare = ItemRarityID.Blue;
        }
        public override bool ItemSpace(Player player)
        {
            return false;
        }
        public override bool OnPickup(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
                player.IncreaseBeeResource(8);

            return false;
        }
    }
}
