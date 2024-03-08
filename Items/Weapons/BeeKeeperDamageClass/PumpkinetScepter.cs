namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class PumpkinetScepter : BeeDamageItem
    {
        public override bool SafeCanUseItem(Player player) => player.ownedProjectileCounts<PumpkinetScepterHoldout>() <= 0;
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Conjures a pumpkinet to attack enemies near the mouse cursor");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 70;
            Item.noMelee = true;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5f;
            Item.value = Item.sellPrice(0, 12, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<PumpkinetScepterHoldout>();
            honeyCost = 5;
            Item.noUseGraphic = true;
            Item.channel = true;
        }
    }
}
