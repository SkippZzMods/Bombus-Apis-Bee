using BombusApisBee.BeeDamageClass;
using BombusApisBee.Projectiles;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class HoneyBeeSkySlasher : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            // DisplayName.SetDefault("Honeycomb"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("Rains a sword projection from the heavens\nSpawns bees on impact");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 12;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 23;
            Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5f;
            Item.value = Item.sellPrice(gold: 3);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BeeSwordProjectile>();
            Item.shootSpeed = 20f;
            Item.UseSound = SoundID.Item1;
            Item.scale = 1.25f;
            Item.crit = 4;
            beeResourceCost = 1;
        }


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 target = Main.screenPosition + new Vector2((float)Main.mouseX, (float)Main.mouseY);
            float ceilingLimit = target.Y;
            if (ceilingLimit > player.Center.Y - 200f)
            {
                ceilingLimit = player.Center.Y - 200f;
            }
            for (int i = 0; i < 2; i++)
            {
                position = player.Center + new Vector2((-(float)Main.rand.Next(0, 401) * player.direction), -600f);
                position.Y -= (100 * i);
                Vector2 heading = target - position;
                if (heading.Y < 0f)
                {
                    heading.Y *= -1f;
                }
                if (heading.Y < 20f)
                {
                    heading.Y = 20f;
                }
                heading.Normalize();
                heading *= velocity.Length();
                velocity.X = heading.X;
                velocity.Y = heading.Y + Main.rand.Next(-40, 41) * 0.02f;
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage * 2, knockback, player.whoAmI, 0f, ceilingLimit);
            }
            return false;
        }
    }
}