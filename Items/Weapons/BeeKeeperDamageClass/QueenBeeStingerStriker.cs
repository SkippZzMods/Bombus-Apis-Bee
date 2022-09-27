using BombusApisBee.BeeDamageClass;
using Terraria.DataStructures;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class QueenBeeStingerStriker : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            // DisplayName.SetDefault("Honeycomb"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("'Its not the Bee Gun I swear'\nHas a chance to fire a tight spread of stingers");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 20;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1;
            Item.value = Item.sellPrice(gold: 3, silver: 75);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.HornetStinger;
            Item.shootSpeed = 6;
            Item.UseSound = SoundID.Item11;
            Item.scale = 1;
            Item.crit = 4;
            beeResourceCost = 1;
        }


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3, 0);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = player.beeType();
            damage = player.beeDamage(damage);
            knockback = player.beeKB(knockback);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.rand.NextBool(2))
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 perturbedSpeed2 = velocity.RotatedByRandom(MathHelper.ToRadians(8));
                    Projectile.NewProjectile(source, position, perturbedSpeed2 * 4, ProjectileID.HornetStinger, damage * 2 / 3, knockback, player.whoAmI);
                }
                return false;
            }
            return true;
        }
    }
}