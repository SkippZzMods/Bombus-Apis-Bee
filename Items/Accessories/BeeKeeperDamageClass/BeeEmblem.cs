namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.HandsOff)]
    public class BeeEmblem : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beekeeper Emblem");
            Tooltip.SetDefault("15% increased hymenoptra damage");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.IncreaseBeeDamage(0.15f);
        }
    }
}