namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class HoneyShot : BeeDamageItem
    {
        public override bool SafeCanUseItem(Player player) => player.ownedProjectileCounts<HoneyShotHoldout>() <= 0;
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Honeyshot");
            Tooltip.SetDefault("Fires a volley of honey arrows");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 43;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.width = 20;
            Item.height = 50;

            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.knockBack = 3.5f;
            Item.value = Item.sellPrice(gold: 3);

            Item.rare = ItemRarityID.LightRed;

            Item.shoot = ModContent.ProjectileType<HoneyShotHoldout>();

            Item.shootSpeed = 17.5f;
            Item.autoReuse = true;

            beeResourceCost = 1;
            Item.UseSound = new SoundStyle("BombusApisBee/Sounds/Item/BowPull");
        }
    }
}