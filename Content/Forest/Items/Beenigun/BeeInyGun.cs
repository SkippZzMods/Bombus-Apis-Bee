namespace BombusApisBee.Content.Forest.Items.Beenigun
{
    public class BeeInyGun : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
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
            Item.shoot = ProjectileType<BeeMinigunHoldout>();
            Item.shootSpeed = 20;
            Item.UseSound = SoundID.Item11;
            Item.scale = 1;
            Item.crit = 4;
            honeyCost = 1;
            resourceChance = 0.33f;
            Item.channel = true;
            Item.noUseGraphic = true;
        }
        public override bool SafeCanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ProjectileType<BeeMinigunHoldout>()] <= 0;
        }
    }
}