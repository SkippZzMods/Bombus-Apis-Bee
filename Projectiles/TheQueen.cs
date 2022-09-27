using Microsoft.Xna.Framework.Graphics;
using System;

namespace BombusApisBee.Projectiles
{

    public class TheQueen : BeeProjectile
    {
        public int beequeenshoot;
        public int gotoplayerqueenbeeTimer;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Queen");

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.aiStyle = 36;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 350;
            Projectile.timeLeft = 18000;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 3;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            int startY = 612 / 4 * Projectile.frame;
            Rectangle trail = new Rectangle(0, startY, 164, 612 / 4);
            //Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, trail, color, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0f);

            }
            return true;
        }


        public override void Kill(int timeLeft)
        {
            // This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            int numberDust = 15 + Main.rand.Next(2); // 4 or 5 shots
            for (int i = 0; i < numberDust; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
                dust.noGravity = true;
                dust.scale = 7f;
            }

        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.dead && !player.active)
            {
                Projectile.Kill();
            }
            if (player.ownedProjectileCounts[ModContent.ProjectileType<TheQueen>()] > 1)
            {
                Projectile.Kill();
            }
            gotoplayerqueenbeeTimer++;
            Projectile.ai[0] += 1f;
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }
            for (int i = 0; i < 200; i++)
            {
                NPC target = Main.npc[i];
                float shootToX = target.position.X + (float)target.width * 0.5f - Projectile.Center.X;
                float shootToY = target.position.Y - Projectile.Center.Y;
                float distance = (float)System.Math.Sqrt((double)(shootToX * shootToX + shootToY * shootToY));
                if (distance < 800f && !target.friendly && target.active)
                {
                    if (Projectile.ai[0] > 10f)
                    {
                        {
                            {
                                if (Main.myPlayer == Projectile.owner)
                                {
                                    distance = 3f / distance;
                                    shootToX *= distance * 4;
                                    shootToY *= distance * 4;
                                    beequeenshoot = Main.rand.Next(new int[] { ProjectileID.GiantBee, ProjectileID.Bee, ProjectileID.Wasp, ProjectileID.HornetStinger });
                                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, shootToX, shootToY, beequeenshoot, Projectile.damage * 1, Projectile.knockBack, Main.myPlayer);
                                    Main.projectile[proj].DamageType = BeeUtils.BeeDamageClass();
                                    Projectile.ai[0] = -15f;
                                }
                            }


                        }

                    }

                }
            }
            float num132 = (float)Math.Sqrt((double)(Projectile.velocity.X * Projectile.velocity.X + Projectile.velocity.Y * Projectile.velocity.Y));
            float num133 = Projectile.localAI[0];
            if (num133 == 0f)
            {
                Projectile.localAI[0] = num132;
                num133 = num132;
            }
            float num134 = Projectile.position.X;
            float num135 = Projectile.position.Y;
            float num136 = 1200f;
            bool flag3 = false;
            int num137 = 0;
            if (Projectile.ai[1] == 0f)
            {
                for (int num138 = 0; num138 < 200; num138++)
                {
                    if (Main.npc[num138].CanBeChasedBy(this, false) && (Projectile.ai[1] == 0f || Projectile.ai[1] == (float)(num138 + 1)))
                    {
                        float num139 = Main.npc[num138].position.X + (float)(Main.npc[num138].width / 2);
                        float num140 = Main.npc[num138].position.Y + (float)(Main.npc[num138].height / 2);
                        float num141 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num139) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num140);
                        if (num141 < num136 && Collision.CanHit(new Vector2(Projectile.position.X + (float)(Projectile.width / 2), Projectile.position.Y + (float)(Projectile.height / 2)), 1, 1, Main.npc[num138].position, Main.npc[num138].width, Main.npc[num138].height))
                        {
                            num136 = num141;
                            num134 = num139;
                            num135 = num140;
                            flag3 = true;
                            num137 = num138;
                        }
                    }
                }
                if (flag3)
                {
                    Projectile.ai[1] = (float)(num137 + 1);
                }
                flag3 = false;
            }
            if (Projectile.ai[1] > 0f)
            {
                int num142 = (int)(Projectile.ai[1] - 1f);
                if (Main.npc[num142].active && Main.npc[num142].CanBeChasedBy(this, true) && !Main.npc[num142].dontTakeDamage)
                {
                    float num143 = Main.npc[num142].position.X + (float)(Main.npc[num142].width / 2);
                    float num144 = Main.npc[num142].position.Y + (float)(Main.npc[num142].height / 2);
                    if (Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num143) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num144) < 1000f)
                    {
                        flag3 = true;
                        num134 = Main.npc[num142].position.X + (float)(Main.npc[num142].width / 2);
                        num135 = Main.npc[num142].position.Y + (float)(Main.npc[num142].height / 2);
                    }
                }
                else
                {
                    Projectile.ai[1] = 0f;
                }
            }
            if (!Projectile.friendly)
            {
                flag3 = false;
            }
            if (flag3)
            {
                float num145 = num133;
                Vector2 vector10 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                float num146 = num134 - vector10.X;
                float num147 = num135 - vector10.Y;
                float num148 = (float)Math.Sqrt((double)(num146 * num146 + num147 * num147));
                num148 = num145 / num148;
                num146 *= num148;
                num147 *= num148;
                int num149 = 8;
                Projectile.velocity.X = (Projectile.velocity.X * (float)(num149) + num146) / (float)num149;
                Projectile.velocity.Y = (Projectile.velocity.Y * (float)(num149) + num147) / (float)num149;
            }
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 10;
            }
            if (gotoplayerqueenbeeTimer >= 50)
            {
                Vector2 idlePosition = player.Center;
                float speed = 40f;
                float inertia = 70f;
                Vector2 vectorToIdlePosition = idlePosition - Projectile.Center;
                float distanceToIdlePosition = vectorToIdlePosition.Length();
                if (distanceToIdlePosition > 600f)
                {
                    // Speed up the minion if it's away from the player
                    speed = 50f;
                    inertia = 100f;
                }
                else
                {
                    // Slow down the minion if closer to the player
                    speed = 25f;
                    inertia = 110f;
                }
                if (distanceToIdlePosition > 20f)
                {
                    // The immediate range around the player (when it passively floats about)

                    // This is a simple movement formula using the two parameters and its desired direction to create a "homing" movement
                    vectorToIdlePosition.Normalize();
                    vectorToIdlePosition *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
                }
                else if (Projectile.velocity == Vector2.Zero)
                {
                    // If there is a case where it's not moving at all, give it a little "poke"
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
            }
        }

    }
}











