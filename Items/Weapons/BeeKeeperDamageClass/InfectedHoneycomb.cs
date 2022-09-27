using BombusApisBee.BeeDamageClass;
using Terraria.DataStructures;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class InfectedHoneycomb : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            // DisplayName.SetDefault("Honeycomb"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("'Cursed, by the amalgam that is the corruption'\nRapidly fires bees, and periodically mini eaters");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 18;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 19;
            Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1f;
            Item.value = Item.sellPrice(0, 3, 50, 0);
            Item.rare = ItemRarityID.Expert;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Bee;
            Item.shootSpeed = 6f;
            Item.UseSound = SoundID.Item11;
            Item.scale = 1;
            Item.crit = 4;
            Item.expert = true;
            beeResourceCost = 1;
        }
        public int shoot;
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(10));
            type = player.beeType();
            damage = player.beeDamage(damage);
            knockback = player.beeKB(knockback);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            shoot += 1;
            if (shoot >= 3)
            {
                float numberProjectiles = 3;
                float rotation = MathHelper.ToRadians(15);
                position += Vector2.Normalize(velocity) * 55f;
                for (int i = 0; i < numberProjectiles; i++)
                {
                    Vector2 perturbedSpeed2 = new Vector2(velocity.X * 2, velocity.Y * 2).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f; // Watch out for dividing by 0 if there is only 1 projectile.
                    Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed2.X, perturbedSpeed2.Y, ProjectileID.TinyEater, damage * 1 / 2, knockback, player.whoAmI);
                }
                shoot = 0;
            }
            return true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-9, 0);
        }
    }
}