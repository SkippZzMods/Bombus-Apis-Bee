using BombusApisBee.BeeDamageClass;
using BombusApisBee.Projectiles;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class CartilageCreator : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            // DisplayName.SetDefault("Honeycomb"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("'The power of the dungeon flows through this ancient honeycomb'\nFires a variety of skeletal bees, bones, and cursed skulls");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 24;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1f;
            Item.value = Item.sellPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SkeletalBee>();
            Item.shootSpeed = 6;
            Item.UseSound = SoundID.Item11;
            Item.scale = 1;
            Item.crit = 4;
            beeResourceCost = 2;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(8));
            type = Main.rand.Next(new int[] { type, ModContent.ProjectileType<SkeletalBee>(), ProjectileID.BoneGloveProj, ProjectileID.BookOfSkullsSkull, });
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }
    }
}