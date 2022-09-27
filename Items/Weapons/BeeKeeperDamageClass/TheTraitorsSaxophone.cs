using BombusApisBee.BeeDamageClass;
using BombusApisBee.Projectiles;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class TheTraitorsSaxophone : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            // DisplayName.SetDefault("Honeycomb"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("'Ya like jazz?'");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 25;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 9;
            Item.useAnimation = 9;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1;
            Item.value = Item.sellPrice(gold: 4);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Bee;
            Item.shootSpeed = 12;
            Item.scale = 1;
            Item.crit = 4;
            beeResourceCost = 1;
            ResourceChance = 0.33f;
        }


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-9, 18);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            type = player.beeType();
            damage = player.beeDamage(damage);
            knockback = player.beeKB(knockback);
            type = Main.rand.Next(new int[] { type, ModContent.ProjectileType<HoneyEighthNote>(), ModContent.ProjectileType<HoneyQuarterNote>(), ModContent.ProjectileType<HoneyTiedEighthNote>() });
            if (type == ModContent.ProjectileType<HoneyEighthNote>() || type == ModContent.ProjectileType<HoneyQuarterNote>() || type == ModContent.ProjectileType<HoneyTiedEighthNote>())
            {
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
                SoundEngine.PlaySound(SoundID.Item26);
                return false;
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                    Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockback, player.whoAmI);
                }
                SoundEngine.PlaySound(SoundID.Item11);
                return false;
            }
        }


    }
}