namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class BeeInyGun : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Honeycomb"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("'It costs four hundred thousand bottles of honey to fire this weapon... for twelve seconds'");
            DisplayName.SetDefault("Beenigun");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 55;
            Item.noMelee = true;
            Item.width = 120;
            Item.height = 40;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5f;
            Item.value = Item.sellPrice(gold: 10, silver: 75);
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BeeMinigunHoldout>();
            Item.shootSpeed = 20;
            Item.UseSound = SoundID.Item11;
            Item.scale = 1;
            Item.crit = 4;
            beeResourceCost = 1;
            ResourceChance = 0.33f;
            Item.channel = true;
            Item.noUseGraphic = true;
        }
        public override bool SafeCanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<BeeMinigunHoldout>()] <= 0;
        }
    }
}