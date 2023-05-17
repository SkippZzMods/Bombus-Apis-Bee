using BombusApisBee.BeeDamageClass;
using BombusApisBee.Projectiles;

namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    public class HoneyLocket : BeeKeeperItem
    {
        public int timer;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Locket");
            Tooltip.SetDefault("'It seems to have a deep connection to the Hive'\nIncreases your amount of Loyal Bees by 3");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 3);
            Item.damage = 35;
            Item.DamageType = ModContent.GetInstance<HymenoptraDamageClass>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Hymenoptra().CurrentBees += 3;
        }
    }
}
