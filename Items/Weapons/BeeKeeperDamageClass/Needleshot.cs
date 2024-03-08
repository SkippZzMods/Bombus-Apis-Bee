using Terraria.DataStructures;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class Needleshot : BeeDamageItem
    {
        public int shot;
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Fires a burst of high velocity stingers and hivebombs");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 20;
            Item.noMelee = true;
            Item.width = 60;
            Item.height = 20;
            Item.useTime = 7;
            Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(gold: 3, silver: 75);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<HighVelocityStinger>();
            Item.shootSpeed = 25f;

            honeyCost = 1;
            resourceChance = 0.25f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(4.AsRadians());

            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 62f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                position += muzzleOffset;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.UseBeeResource(honeyCost);
            SoundID.Item11.PlayWith(position, -0.05f, 0.1f);
            for (int i = 0; i < 7; i++)
            {
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.StingerDust>(), velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(0.25f), Main.rand.Next(50, 125)).noGravity = true;
                Dust.NewDustPerfect(position, DustID.Poisoned, velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(0.25f), Main.rand.Next(75)).noGravity = true;
            }
            if (++shot >= 5)
            {
                Vector2 pos = position + new Vector2(-15, 5 * player.direction).RotatedBy(velocity.ToRotation());
                Projectile.NewProjectile(source, pos, velocity * 0.75f, ModContent.ProjectileType<Hivebomb>(), (int)(damage * 0.65f), knockback, player.whoAmI);
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.HoneyDust>(), velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(0.25f), Main.rand.Next(50, 125)).noGravity = true;
                    Dust.NewDustPerfect(pos, DustID.Honey2, velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(0.25f), Main.rand.Next(75)).noGravity = true;
                }
                SoundID.Item61.PlayWith(position, 0.1f);
                player.reuseDelay = 30;
                shot = 0;
            }

            return true;
        }
    }
}