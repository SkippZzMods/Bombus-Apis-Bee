namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class EaterOfHoneycombs : BeeDamageItem
    {
        public override bool SafeCanUseItem(Player player) => player.ownedProjectileCounts<EaterOfHoneycombsHoldout>() <= 0;
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Fires corrupted bees and mini eaters, before being thrown and seeking out targets\n'It looks... alive?'");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 13;

            Item.noMelee = true;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.sellPrice(0, 1, 50, 0);

            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<EaterOfHoneycombsHoldout>();
            Item.shootSpeed = 1f;
            Item.noUseGraphic = true;
            beeResourceCost = 4;
        }
    }
}