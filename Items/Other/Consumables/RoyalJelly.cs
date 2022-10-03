using Terraria.ID;

namespace BombusApisBee.Items.Other.Consumables
{
    public class RoyalJelly : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Permanently increases maximum honey by 15\n'A delicacy that's highly sought after for its refined taste and healing properties'");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 24;

            Item.consumable = true;

            Item.rare = ItemRarityID.Orange;

            Item.useStyle = ItemUseStyleID.EatFood;

            Item.value = Item.sellPrice(gold: 2);

            Item.useTime = Item.useAnimation = 20;

            Item.UseSound = SoundID.Item3;
        }

        public override bool? UseItem(Player player)
        {
            if (player.Bombus().RoyalJelly)
                return null;

            player.Bombus().RoyalJelly = true;

            CombatText.NewText(player.getRect(), BombusApisBee.honeyIncreaseColor, 15);

            return true;
        }
    }
}
