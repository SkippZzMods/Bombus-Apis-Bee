using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class BeeBubbleBlaster : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Beenopopper");
            Tooltip.SetDefault("'Aliens really like bubbles... that shoot bullets'");
        }


        public override void SafeSetDefaults()
        {
            Item.damage = 35;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1f;
            Item.value = Item.sellPrice(0, 10, 75, 25);
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BeeBubble>();
            Item.shootSpeed = 5;
            Item.UseSound = SoundID.Item95;
            Item.scale = 1;
            Item.crit = 4;
            honeyCost = 3;

        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(2, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int numberProjectiles = 4 + Main.rand.Next(2);
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(25));

                float scale = 1f - (Main.rand.NextFloat() * .3f);
                perturbedSpeed = perturbedSpeed * scale;
                Projectile.NewProjectile(source, position + Vector2.Normalize(velocity) * 30f, perturbedSpeed, type, damage, knockback, player.whoAmI);
            }
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(position + Vector2.Normalize(velocity) * 35f, ModContent.DustType<Dusts.HoneyDust>(), velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(3f), 0, default, 1.2f).noGravity = true;
                Dust.NewDustPerfect(position + Vector2.Normalize(velocity) * 35f, DustID.Honey2, velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(3.25f), 0, default, 1.2f).noGravity = true;
            }
            return false;
        }
    }
}