using Microsoft.Xna.Framework.Graphics;

namespace BombusApisBee.Projectiles
{
    public class TheStingProj : BeeProjectile
    {
        public int HoneyTimer;
        public bool trailbee;
        public int stingprojtimer;
        int DashDelay;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Stinging Queen");
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SafeSetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 76;
            Projectile.height = 36;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            if (DashDelay > 0)
                DashDelay--;
            DrawOriginOffsetY = -30;
            Player player = Main.player[Projectile.owner];
            HoneyTimer++;
            var BeeDamagePlayer2 = player.Hymenoptra();
            if (HoneyTimer >= 60)
            {
                if (!player.UseBeeResource(2))
                    Projectile.Kill();

                BeeDamagePlayer2.BeeResourceRegenTimer = -240;
                HoneyTimer = 0;
            }
            if (player.dead && !player.active)
            {
                Projectile.Kill();
            }
            if (!player.channel)
            {
                Projectile.Kill();
            }
            stingprojtimer++;
            Projectile.ai[0] += 1f;
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                {
                    Projectile.frame = 0;
                }
            }
            if (Projectile.position.X > Main.MouseWorld.X)
            {
                Projectile.spriteDirection = 1;
            }
            else
            {
                Projectile.spriteDirection = -1;

            }
            if (Main.myPlayer == Projectile.owner && player.channel)
            {
                Projectile.netUpdate = true;
                if (stingprojtimer < 240)
                {
                    Projectile.localNPCHitCooldown = 100;
                    Projectile.velocity = (Projectile.velocity * 20f + Utils.SafeNormalize(Projectile.Center.DirectionTo(Main.MouseWorld + (Projectile.ai[1] / 16f).ToRotationVector2() * 95f), Vector2.UnitX) * 19f) / 21f;
                    Projectile.ai[1] += 1f;
                    float shootToX = Main.MouseWorld.X - Projectile.Center.X;
                    float shootToY = Main.MouseWorld.Y - Projectile.Center.Y;
                    float distance = (float)System.Math.Sqrt((double)(shootToX * shootToX + shootToY * shootToY));
                    if (distance < 800f)
                    {
                        if (Projectile.ai[0] > 10f)
                        {
                            distance = 3f / distance;
                            shootToX *= distance * 4;
                            shootToY *= distance * 4;
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, shootToX, shootToY, ModContent.ProjectileType<StingerFriendly>(), Projectile.damage * 1 / 2, Projectile.knockBack, Main.myPlayer);
                            Projectile.ai[0] = 0f;
                        }
                    }
                }
                else if (stingprojtimer < 480)
                {
                    Projectile.localNPCHitCooldown = 5;
                    Projectile.rotation = Projectile.velocity.X * 0.025f;
                    if (DashDelay <= 0)
                    {
                        DashDelay = 120;
                        Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * 18f;
                    }
                    else if (DashDelay <= 100)
                    {
                        Vector2 toCenter = Main.MouseWorld - Projectile.Center;
                        if (toCenter.Length() < 0.0001f)
                            toCenter = Vector2.Zero;

                        float distance = toCenter.Length();
                        float speed;
                        float inertia;
                        if (distance > 300)
                        {
                            speed = 18f;
                            inertia = 35f;
                        }
                        else
                        {
                            speed = 13f;
                            inertia = 50f;
                        }
                        if (distance > 25f)
                        {
                            toCenter.Normalize();
                            toCenter *= speed;
                            Projectile.velocity = (Projectile.velocity * (inertia - 1) + toCenter) / inertia;
                        }
                    }
                }
                else
                {
                    Projectile.rotation = Projectile.velocity.X * 0.025f;
                    trailbee = false;
                    if (stingprojtimer < 720)
                    {
                        Projectile.localNPCHitCooldown = 100;
                        float between = Vector2.Distance(Main.MouseWorld, Projectile.Center);
                        float distanceFromTarget = between;
                        if (distanceFromTarget > 20f)
                        {
                            Vector2 targetCenter = Main.MouseWorld;
                            float speed = 16f;
                            float inertia = 40f;
                            Vector2 direction = new Vector2(targetCenter.X - Projectile.Center.X, targetCenter.Y - 70 - Projectile.Center.Y);
                            direction.Normalize();
                            direction *= speed;
                            Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                            float shootToX = Main.MouseWorld.X - Projectile.Center.X;
                            float shootToY = Main.MouseWorld.Y - Projectile.Center.Y;
                            float distance = (float)System.Math.Sqrt((double)(shootToX * shootToX + shootToY * shootToY));
                            if (distance < 800f)
                            {
                                if (Projectile.ai[0] > 10f)
                                {
                                    distance = 3f / distance;
                                    shootToX *= distance * 4;
                                    shootToY *= distance * 4;
                                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X - 22, Projectile.Center.Y + 5, shootToX, shootToY, player.beeType(), player.beeDamage(Projectile.damage * 1 / 2), player.beeKB(Projectile.knockBack), Main.myPlayer);
                                    Main.projectile[proj].netUpdate = true;
                                    Projectile.ai[0] = -10f;
                                }
                            }
                        }
                    }
                    else
                    {
                        stingprojtimer = 0;
                    }
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            int startY = 194 / 3 * Projectile.frame;
            Rectangle trail = new Rectangle(0, startY, 76, 194 / 3);
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos + new Vector2(0, -30), trail, color, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0f); ;

            }
            return true;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 50; i++)
            {
                int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 31, 0f, 0f, 100, default(Color), 1.2f);
                Main.dust[dustIndex].velocity *= 1.4f;
            }
            for (int g = 0; g < 2; g++)
            {
                int goreIndex = Gore.NewGore(Projectile.GetSource_FromThis(), new Vector2(Projectile.position.X + (float)(Projectile.width / 2) - 24f, Projectile.position.Y + (float)(Projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
                Main.gore[goreIndex].scale = 1f;
                Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
                Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
                goreIndex = Gore.NewGore(Projectile.GetSource_FromThis(), new Vector2(Projectile.position.X + (float)(Projectile.width / 2) - 24f, Projectile.position.Y + (float)(Projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
                Main.gore[goreIndex].scale = 1f;
                Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X - 1.5f;
                Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
                goreIndex = Gore.NewGore(Projectile.GetSource_FromThis(), new Vector2(Projectile.position.X + (float)(Projectile.width / 2) - 24f, Projectile.position.Y + (float)(Projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
                Main.gore[goreIndex].scale = 1f;
                Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
                Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y - 1.5f;
                goreIndex = Gore.NewGore(Projectile.GetSource_FromThis(), new Vector2(Projectile.position.X + (float)(Projectile.width / 2) - 24f, Projectile.position.Y + (float)(Projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
                Main.gore[goreIndex].scale = 1f;
                Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X - 1.5f;
                Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y - 1.5f;
            }
        }
    }
}