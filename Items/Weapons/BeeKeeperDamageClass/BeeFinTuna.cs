using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class BeeFinTuna : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Bee-Fin Tuna");
            Tooltip.SetDefault("Spews a spread of honey bubbles which burst into bees\n'The meat is said to be incredibly sweet and tender'");
            Item.staff[Item.type] = true;
        }
        public override void SafeSetDefaults()
        {
            Item.damage = 14;
            Item.width = 60;
            Item.height = 60;
            Item.useTime = 51;
            Item.useAnimation = 51;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item111;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BeeFishBubble>();
            Item.shootSpeed = 12f;
            beeResourceCost = 4;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(10, 10);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int numberProjectiles = 3 + Main.rand.Next(2);
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(25));

                float scale = 1f - (Main.rand.NextFloat() * .3f);
                perturbedSpeed = perturbedSpeed * scale;
                Projectile.NewProjectile(source, position + Vector2.Normalize(velocity) * 25f + new Vector2(0f, 5f), perturbedSpeed, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
    }
}