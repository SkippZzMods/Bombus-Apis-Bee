using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class TomeOfTheSun : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Creates a pulse of vibrant nectar energy at the mouse cursor, which splits into homing bolts");
        }


        public override void SafeSetDefaults()
        {
            Item.damage = 75;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 45;
            Item.useAnimation = 45;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2.5f;
            Item.value = Item.sellPrice(0, 7, 50);
            Item.rare = ItemRarityID.Lime;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<TomeOfTheSunProjectile>();

            Item.UseSound = SoundID.Item8;
            Item.shootSpeed = 1f;

            honeyCost = 5;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, Main.MouseWorld + Main.rand.NextVector2Circular(75f, 75f), Vector2.Zero, type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}