using BombusApisBee.BeeDamageClass;
using BombusApisBee.Projectiles;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class GelatinousHoneycomb : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            // DisplayName.SetDefault("Honeycomb"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("'A Honeycomb fit for a king'\nReleases a bee which spawns gel from the sky on death");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 9;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1f;
            Item.value = Item.sellPrice(gold: 2);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<GelBee>();
            Item.shootSpeed = 6;
            Item.UseSound = SoundID.Item11;
            Item.scale = 1;
            Item.crit = 4;
            beeResourceCost = 1;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(5));
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }
    }
}