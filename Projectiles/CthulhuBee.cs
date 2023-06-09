namespace BombusApisBee.Projectiles
{
    public class CthulhuBee : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cthulhu Bee");
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 3;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return new bool?(Projectile.timeLeft < 270 && target.CanBeChasedBy(Projectile, false));
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 50;
            }
            else
            {
                Projectile.extraUpdates = 1;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 1)
            {
                Projectile.frame = 0;
            }
            for (int num369 = 0; num369 < 1; num369++)
            {
                int dustType = Utils.NextBool(Main.rand, 3) ? DustID.WhiteTorch : DustID.RedTorch;
                float num370 = Projectile.velocity.X / 3f * (float)num369;
                float num371 = Projectile.velocity.Y / 3f * (float)num369;
                int num372 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 0, default(Color), 1f);
                Dust dust = Main.dust[num372];
                dust.position.X = Projectile.Center.X - num370;
                dust.position.Y = Projectile.Center.Y - num371;
                dust.velocity *= 0f;
                dust.scale = 0.6f;
            }
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) - 1.5707964f;
            float num373 = Projectile.position.X;
            float num374 = Projectile.position.Y;
            float num375 = 100000f;
            bool flag10 = false;
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 30f)
            {
                Projectile.ai[0] = 30f;
                for (int num376 = 0; num376 < 200; num376++)
                {
                    if (Main.npc[num376].CanBeChasedBy(Projectile, false))
                    {
                        float num377 = Main.npc[num376].position.X + (float)(Main.npc[num376].width / 2);
                        float num378 = Main.npc[num376].position.Y + (float)(Main.npc[num376].height / 2);
                        float num379 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num377) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num378);
                        if (num379 < 800f && num379 < num375 && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Main.npc[num376].position, Main.npc[num376].width, Main.npc[num376].height))
                        {
                            num375 = num379;
                            num373 = num377;
                            num374 = num378;
                            flag10 = true;
                        }
                    }
                }
            }
            if (!flag10)
            {
                num373 = Projectile.position.X + (float)(Projectile.width / 2) + Projectile.velocity.X * 100f;
                num374 = Projectile.position.Y + (float)(Projectile.height / 2) + Projectile.velocity.Y * 100f;
            }
            float num384 = 10f;
            float num380 = 0.16f;
            Vector2 vector30 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
            float num381 = num373 - vector30.X;
            float num382 = num374 - vector30.Y;
            float num383 = (float)Math.Sqrt((double)(num381 * num381 + num382 * num382));
            num383 = num384 / num383;
            num381 *= num383;
            num382 *= num383;
            if (Projectile.velocity.X < num381)
            {
                Projectile.velocity.X = Projectile.velocity.X + num380;
                if (Projectile.velocity.X < 0f && num381 > 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X + num380 * 2f;
                }
            }
            else if (Projectile.velocity.X > num381)
            {
                Projectile.velocity.X = Projectile.velocity.X - num380;
                if (Projectile.velocity.X > 0f && num381 < 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X - num380 * 2f;
                }
            }
            if (Projectile.velocity.Y < num382)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + num380;
                if (Projectile.velocity.Y < 0f && num382 > 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + num380 * 2f;
                    return;
                }
            }
            else if (Projectile.velocity.Y > num382)
            {
                Projectile.velocity.Y = Projectile.velocity.Y - num380;
                if (Projectile.velocity.Y > 0f && num382 < 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - num380 * 2f;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            this.bounce--;
            if (this.bounce <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
            }
            return false;
        }
        private int bounce = 2;
    }
}
