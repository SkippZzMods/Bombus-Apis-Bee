namespace BombusApisBee.Projectiles
{
    public class SquireBee : ModProjectile
    {
        public int Squirebeeshoot;
        public int gotoplayerSquirebeeTimer;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("SquireBee");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 0;
            Projectile.alpha = 255;
        }

        public override void Kill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            int numberDust = 15 + Main.rand.Next(2);
            for (int i = 0; i < numberDust; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
                dust.noGravity = true;
                dust.scale = 1f;
            }
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.damage = player.ApplyHymenoptraDamageTo(Projectile.originalDamage);

            if (!player.GetModPlayer<BombusApisBeePlayer>().squire)
                Projectile.Kill();

            if (player.dead || !player.active)
                player.ClearBuff(ModContent.BuffType<SquireBuff>());

            if (player.HasBuff(ModContent.BuffType<SquireBuff>()))
                Projectile.timeLeft = 2;

            Projectile.spriteDirection = Projectile.direction;
            Projectile.ai[0] += 1f;
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }

            NPC target = Projectile.FindTargetWithinRange(600f);

            if (target != null && Collision.CanHit(Projectile.position, 1, 1, target.position, 1, 1))
            {
                Vector2 velocity = Projectile.DirectionTo(target.Center) * 15f;
                if (Projectile.ai[0] > 10f && Main.myPlayer == Projectile.owner)
                {
                    int type = Main.rand.Next(new int[] { ProjectileID.WoodenArrowFriendly, ProjectileID.FireArrow, ProjectileID.HolyArrow, ProjectileID.HellfireArrow, ProjectileID.JestersArrow, ProjectileID.IchorArrow, ProjectileID.FrostburnArrow });
                    int squire = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, velocity, type, Projectile.damage, Projectile.knockBack, Main.myPlayer);
                    Projectile.ai[0] = -45f;
                    Main.projectile[squire].noDropItem = true;
                    Main.projectile[squire].DamageType = BeeUtils.BeeDamageClass();
                }
            }
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<HoneyDust>(), Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 150, default(Color), 0.7f);
            }
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 10;
            }
            Vector2 idlePosition = player.Center;
            idlePosition.Y -= 90f;
            Vector2 vectorToIdlePosition = idlePosition - Projectile.Center;
            float distanceToIdlePosition = vectorToIdlePosition.Length();
            float speed = 8f;
            float inertia = 20f;
            if (distanceToIdlePosition > 600f)
            {
                speed = 16f;
                inertia = 40f;
            }
            else
            {
                speed = 10f;
                inertia = 30f;
            }
            if (distanceToIdlePosition > 30f)
            {
                vectorToIdlePosition.Normalize();
                vectorToIdlePosition *= speed;
                Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
            }
            else if (Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity.X = -0.15f;
                Projectile.velocity.Y = -0.05f;
            }
            Projectile.rotation = Projectile.velocity.X * 0.05f;
        }
    }
}
