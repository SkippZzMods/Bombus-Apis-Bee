namespace BombusApisBee.Projectiles
{
    public class ManaBolt : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("ManaBolt");
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 430;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 255;
        }


        public override void AI()
        {
            for (int i = 0; i < 4; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonWater, 0f, 0f, 25, default);
                dust.noGravity = true;
                dust.scale = 1.2f;
            }
            if (Projectile.timeLeft < 400)
            {
                float distanceRequired = 600f;
                float homingVelocity = 12f;
                float N = 20f;
                Vector2 destination = Projectile.Center;
                bool locatedTarget = false;
                for (int i = 0; i < 200; i++)
                {
                    float extraDistance = (float)(Main.npc[i].width / 2 + Main.npc[i].height / 2);
                    if (Main.npc[i].CanBeChasedBy(Projectile, false) && Projectile.WithinRange(Main.npc[i].Center, distanceRequired + extraDistance) && (Collision.CanHit(Projectile.Center, 1, 1, Main.npc[i].Center, 1, 1)))
                    {
                        destination = Main.npc[i].Center;
                        locatedTarget = true;
                        break;
                    }
                }
                if (locatedTarget)
                {
                    Vector2 homeDirection = Utils.SafeNormalize(destination - Projectile.Center, Vector2.UnitY);
                    Projectile.velocity = (Projectile.velocity * N + homeDirection * homingVelocity) / (N + 1f);
                    return;
                }
            }
        }
        public override bool? CanDamage()
        {
            return Projectile.timeLeft < 400;
        }
    }
}