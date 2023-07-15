using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class LaserbeemBlaster : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Fires pure concentrations of bouncing honey energy, which ricochet between enemies\nSpawns bees of pure honey energy on bounce");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 85;
            Item.noMelee = true;

            Item.rare = ItemRarityID.Yellow;
            Item.width = 50;
            Item.height = 15;
            Item.useTime = 7;
            Item.useAnimation = 21;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shootSpeed = 5f;

            Item.shoot = ModContent.ProjectileType<LaserbeemProjectile>();
            Item.value = Item.sellPrice(gold: 9, silver: 50);
            beeResourceCost = 2;

            ResourceChance = 0.33f;
            Item.autoReuse = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(3));
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 50f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                position += muzzleOffset;

            position += new Vector2(0, -3 * player.direction).RotatedBy(velocity.ToRotation());
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundID.Item91.PlayWith(position);
            player.UseBeeResource(beeResourceCost - 1);
            player.reuseDelay = 10;

            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.Glow>(), velocity.RotatedByRandom(0.25f) * Main.rand.NextFloat(1f), 0, new Color(255, 255, 204), 0.35f);

                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowFastDecelerate>(), velocity.RotatedByRandom(0.25f) * Main.rand.NextFloat(0.75f), 0, new Color(253, 232, 0), 0.45f);
            }
            return true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }
    }
}