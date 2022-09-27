using BombusApisBee.BeeDamageClass;
using BombusApisBee.Projectiles;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class SpazmaCannon : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            // DisplayName.SetDefault("Honeycomb"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("'Retinatizer would be proud... maybe'\nFires Spazbees to spew cursed flames at enemies");
            DisplayName.SetDefault("Spazmacannon");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 52;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1f;
            Item.value = Item.sellPrice(0, 7, 50, 0);
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SpazBee>();
            Item.shootSpeed = 13;
            Item.UseSound = SoundID.Item11;
            Item.scale = 1;
            Item.crit = 4;
            beeResourceCost = 3;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(15.AsRadians());
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }

    }
}