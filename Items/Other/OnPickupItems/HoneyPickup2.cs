using BombusApisBee.BeeDamageClass;
using System;

namespace BombusApisBee.Items.Other.OnPickupItems
{
    public class HoneyPickup2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Droplet");
            Tooltip.SetDefault("you shouldn't see this....");
        }
        public override void SetDefaults()
        {
            Item.width = Item.height = 20;
            Item.rare = ItemRarityID.Yellow;
        }
        public override bool ItemSpace(Player player)
        {
            return false;
        }
        public override bool OnPickup(Player player)
        {
            if (player.Hymenoptra().BeeResourceCurrent < player.Hymenoptra().BeeResourceMax2)
                player.GetModPlayer<BeeDamagePlayer>().BeeResourceCurrent += 12;
            if (player.Hymenoptra().BeeResourceCurrent > player.Hymenoptra().BeeResourceMax2)
                player.Hymenoptra().BeeResourceCurrent = player.Hymenoptra().BeeResourceMax2;

            CombatText.NewText(player.getRect(), BombusApisBee.honeyIncreaseColor, 12, false, false);

            return false;
        }
    }
}