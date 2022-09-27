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
            if (player.Hymenoptra().BeeResourceCurrent < player.Hymenoptra().BeeResourceMax2)
                player.GetModPlayer<BeeDamagePlayer>().BeeResourceCurrent += 8;
            if (player.Hymenoptra().BeeResourceCurrent > player.Hymenoptra().BeeResourceMax2)
                player.Hymenoptra().BeeResourceCurrent = player.Hymenoptra().BeeResourceMax2;
            if (player.whoAmI == Main.myPlayer)
            {
                secondtimer++;
                float Beefade = (float)((Math.Sin((double)(6.2831855f / secondtimer) * (double)Main.GlobalTimeWrappedHourly) + 1.0) * 0.5);
                CombatText.NewText(player.getRect(), Color.Lerp(Color.CornflowerBlue, Color.LightBlue, Beefade), 8, false, false);
            }
            return false;
        }
    }
}
