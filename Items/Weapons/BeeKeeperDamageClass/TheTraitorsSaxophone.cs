namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class TheTraitorsSaxophone : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("The Traitor's Saxophone");
            Tooltip.SetDefault("'Ya like jazz?'");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 14;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 60;

            Item.useTime = 16;
            Item.useAnimation = 16;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3f;
            Item.value = Item.sellPrice(gold: 4);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<TheTraitorsSaxophoneHoldout>();
            Item.shootSpeed = 1f;

            beeResourceCost = 2;
            ResourceChance = 0.25f;

            Item.noUseGraphic = true;
            Item.channel = true;
        }
    }
}