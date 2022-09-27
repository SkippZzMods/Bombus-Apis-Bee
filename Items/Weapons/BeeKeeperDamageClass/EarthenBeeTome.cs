using BombusApisBee.BeeDamageClass;
using BombusApisBee.Dusts;
using BombusApisBee.Projectiles;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class EarthenBeeTome : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            // DisplayName.SetDefault("Honeycomb"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("'Finally, a good golem weapon!\nThe projectiles will be created at your cursor.");
        }


        public override void SafeSetDefaults()
        {
            Item.damage = 77;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1f;
            Item.value = Item.sellPrice(0, 10, 50, 25);
            Item.rare = ItemRarityID.Lime;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Bee;
            Item.shootSpeed = 6;
            Item.UseSound = SoundID.Item13;
            Item.scale = 1;
            beeResourceCost = 1;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }
        private int ShootLaser;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 vel = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
            while (vel.X == 0f && vel.Y == 0f)
            {
                vel = new Vector2(Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
            }
            vel.Normalize();
            vel *= (float)Main.rand.Next(70, 101) * 0.1f;
            type = player.beeType();
            damage = player.beeDamage(damage);
            knockback = player.beeKB(knockback);
            Vector2 Mouse = Main.MouseWorld;
            Projectile.NewProjectile(source, Main.MouseWorld, vel, type, damage, knockback, player.whoAmI);
            ShootLaser += 1;
            if (ShootLaser >= 5)
            {
                Vector2 speed = new Vector2(40f, 40f);
                float numberProjectiles = 8;
                float rotation = MathHelper.ToRadians(360);
                position += Vector2.Normalize(new Vector2(speed.X, speed.Y)) * 360f;
                for (int i = 0; i < numberProjectiles; i++)
                {
                    Vector2 perturbedSpeed = new Vector2(speed.X, speed.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f;
                    Projectile.NewProjectile(source, Mouse.X, Mouse.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<GolemLaser>(), (int)(damage * 0.8), 1f, player.whoAmI);
                }
                player.UseBeeResource(2);
                ShootLaser = 0;
            }
            const int Repeats = 75;
            for (int i = 0; i < Repeats; ++i)
            {
                float angle2 = 6.2831855f * (float)i / (float)Repeats;
                Dust dust3 = Dust.NewDustPerfect(Mouse, ModContent.DustType<HoneyDust>(), null, 0, default(Color), 1.1f);
                dust3.velocity = Utils.ToRotationVector2(angle2) * 4f;
                dust3.noGravity = true;
            }
            return false;
        }
    }
}