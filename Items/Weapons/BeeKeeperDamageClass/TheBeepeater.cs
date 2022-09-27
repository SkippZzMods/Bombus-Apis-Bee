using BombusApisBee.BeeDamageClass;
using BombusApisBee.Projectiles;
using Terraria.DataStructures;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class TheBeepeater : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            // DisplayName.SetDefault("Honeycomb"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("Rapidly fires bee arrows");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 35;
            Item.noMelee = true;
            Item.width = 50;
            Item.height = 18;
            Item.useTime = 19;
            Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4.5f;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ItemRarityID.LightRed;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.BeeArrow;
            Item.shootSpeed = 15.5f;
            Item.UseSound = SoundID.Item97;
            Item.scale = 1;
            Item.crit = 4;
            beeResourceCost = 1;
        }


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int numberProjectiles = 1 + Main.rand.Next(2); // 4 or 5 shots
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(5)); // 30 degree spread.
                                                                                            // If you want to randomize the speed to stagger the projectiles
                                                                                            // float scale = 1f - (Main.rand.NextFloat() * .3f);
                                                                                            // perturbedSpeed = perturbedSpeed * scale; 
                int proj = Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockback, player.whoAmI);
                Main.projectile[proj].GetGlobalProjectile<BombusApisBeeGlobalProjectile>().ForceBee = true;
            }
            return false;
        }

    }
}