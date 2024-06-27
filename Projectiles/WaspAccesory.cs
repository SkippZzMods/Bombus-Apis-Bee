namespace BombusApisBee.Projectiles
{

    public class WaspAccessory : BeeProjectile
    {
        public override string Texture => "BombusApisBee/ExtraTextures/WaspRetexture";
        public int gotoplayerwaspbeeTimer;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wasp");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 5;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle sourceRect = tex.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] + (sourceRect.Size() / 2f) - Main.screenPosition;
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.spriteBatch.Draw(tex, drawPos, sourceRect, color, Projectile.rotation, sourceRect.Size() / 2f, Projectile.scale, Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : 0, 0f);
            }
            return true;
        }


        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Bee);
                dust.noGravity = true;
                dust.scale = 0.5f;
                dust.alpha = 100;
            }

        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.Bombus().HoneyLocket)
                Projectile.timeLeft = 2;

            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.X * 0.085f;
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }

            if (Main.rand.Next(5) == 0) // only spawn 20% of the time
            {
                int choice = Main.rand.Next(2); // choose a random number: 0, 1, or 2
                if (choice == 0) // use that number to select dustID: 15, 57, or 58
                {
                    choice = DustID.Poisoned;
                }
                else if (choice == 1)
                {
                    choice = ModContent.DustType<HoneyDust>();
                }
                // Spawn the dust
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, choice, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 150, default(Color), 0.5f);
            }
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 10;
            }
            NPC target = Projectile.FindTargetWithinRange(500f);
            if (target != null)
            {
                Vector2 direction = target.Center - Projectile.Center;
                direction.Normalize();
                direction *= 10;
                Projectile.velocity = (Projectile.velocity * (35 - 1) + direction) / 35;
            }
            else
            {
                Vector2 idlePosition = player.Center;
                float speed = 40f;
                float inertia = 70f;
                Vector2 vectorToIdlePosition = idlePosition - Projectile.Center;
                float distanceToIdlePosition = vectorToIdlePosition.Length();
                if (distanceToIdlePosition > 600f)
                {
                    speed = 50f;
                    inertia = 100f;
                }
                else
                {
                    speed = 25f;
                    inertia = 110f;
                }
                if (distanceToIdlePosition > 20f)
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
            }
        }

    }
}
