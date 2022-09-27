using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace BombusApisBee.Projectiles
{
    public class SkeletalHornetProjectile : ModProjectile
    {
        public Player player => Main.player[Projectile.owner];

        public int EnrageTransitionTimer;

        public int DashDelay;

        public ref float EnrageTimer => ref Projectile.ai[0];

        public ref float StingerDelay => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skeletal Hornet");
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = BeeUtils.BeeDamageClass();

            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.height = 40;
            Projectile.width = 50;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if (DashDelay > 0)
                DashDelay--;

            if (StingerDelay > 0)
                StingerDelay--;

            if (EnrageTimer > 0 && EnrageTransitionTimer <= 0)
                EnrageTimer--;

            if (++Projectile.frameCounter >= 7)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }

            if (player.Bombus().SkeletalSet)
                Projectile.timeLeft = 2;

            Projectile.rotation = Projectile.velocity.X * 0.02f;

            NPC target = Main.npc.Where(n => n.CanBeChasedBy(Projectile, false) && Vector2.Distance(n.Center, player.Center) < 1000f).
                OrderBy(n => Vector2.Distance(n.Center, player.Center)).FirstOrDefault();
            if (--EnrageTransitionTimer > 0)
            {
                Projectile.velocity *= 0.96f;
                return;
            }
            if (target != default)
            {
                if (EnrageTimer > 0)
                {
                    EnragedAttacking(target);
                }
                else
                {
                    NormalAttacking(target);
                }
            }
            else
            {
                IdleMovement();
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            SpriteEffects spriteEffects = Projectile.direction == 1 ? SpriteEffects.FlipHorizontally : 0;
            Rectangle sourceRect = tex.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);
            if (EnrageTransitionTimer > 0)
            {
                float sin = (float)Math.Sin((EnrageTransitionTimer / 60f) * 5.5f);

                for (int k = 0; k < 6; k++)
                {
                    Vector2 dir = Vector2.UnitX.RotatedBy(k / 6f * 6.28f) * (5.5f + sin * 3.2f);
                    var color = new Color(200, 45, 20) * (0.85f - sin * 0.1f) * MathHelper.Lerp(1f, 0.5f, 1f - (EnrageTransitionTimer / 60f));
                    color.A = 0;
                    Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition + dir, sourceRect, color, Projectile.rotation, sourceRect.Size() / 2f, Projectile.scale, spriteEffects, 0);
                }
            }
            else if (EnrageTimer > 0)
            {
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + (sourceRect.Size() / 2f) + new Vector2(0f, Projectile.gfxOffY);
                    Color color = new Color(200, 45, 20) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    color.A = 0;
                    if (EnrageTimer < 30)
                        color = color * MathHelper.Lerp(1f, 0f, 1f - (EnrageTimer / 30f));
                    Main.EntitySpriteDraw(tex, drawPos, sourceRect, color, Projectile.rotation, sourceRect.Size() / 2f, Projectile.scale, spriteEffects, 0);
                }
            }
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, sourceRect, lightColor, Projectile.rotation, sourceRect.Size() / 2f, Projectile.scale, spriteEffects, 0f);
            return false;
        }

        private void IdleMovement()
        {
            Vector2 idlePos = player.Center + new Vector2(-80 * player.direction, -50);
            Vector2 toIdlePos = idlePos - Projectile.Center;
            if (toIdlePos.Length() < 0.0001f)
                toIdlePos = Vector2.Zero;

            float distance = toIdlePos.Length();
            float speed;
            float inertia;
            if (distance > 450f)
            {
                speed = 18f;
                inertia = 40f;
            }
            else
            {
                speed = 12f;
                inertia = 55f;
            }

            if (distance > 25f)
            {
                toIdlePos.Normalize();
                toIdlePos *= speed;
                Projectile.velocity = (Projectile.velocity * (inertia - 1) + toIdlePos) / inertia;
            }
            else if (Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity.X = -0.25f;
                Projectile.velocity.Y = -0.1f;
            }

            if (Main.myPlayer == player.whoAmI && distance > 2000f)
            {
                Projectile.position = idlePos;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
        }
        private void EnragedAttacking(NPC target)
        {
            Vector2 targetCenter = target.Center;

            if (DashDelay <= 0)
            {
                DashDelay = 120;
                Projectile.velocity = Projectile.DirectionTo(targetCenter) * 12f;
                for (int i = 0; i < 3; i++)
                {
                    float rotation = 0;
                    if (i == 1)
                        rotation = 0.1f;
                    if (i == 2)
                        rotation = -0.1f;
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, (Projectile.DirectionTo(targetCenter) * 15f).RotatedBy(rotation), ModContent.ProjectileType<SkeletalStinger>(), (int)(Projectile.damage * 0.66f), 2f, player.whoAmI, 1f).tileCollide = false;
                }
                SoundEngine.PlaySound(SoundID.Item17, Projectile.Center);
                const int Repeats = 45;
                for (int i = 0; i < Repeats; ++i)
                {
                    float angle2 = 6.2831855f * (float)i / (float)Repeats;
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Utils.ToRotationVector2(angle2) * 3f, 25, new Color(200, 45, 20), 0.4f);
                }
            }
            else if (DashDelay <= 100)
            {
                Vector2 toCenter = targetCenter - Projectile.Center;
                toCenter.Y -= 80;
                if (toCenter.Length() < 0.0001f)
                    toCenter = Vector2.Zero;

                float distance = toCenter.Length();
                float speed;
                float inertia;
                if (distance > 300)
                {
                    speed = 15f;
                    inertia = 35f;
                }
                else
                {
                    speed = 10f;
                    inertia = 50f;
                }
                if (distance > 25f)
                {
                    toCenter.Normalize();
                    toCenter *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + toCenter) / inertia;
                }

                if (Vector2.Distance(targetCenter, Projectile.Center) < 750f)
                    if (StingerDelay <= 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item17, Projectile.Center);
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, (Projectile.DirectionTo(targetCenter + target.velocity) * 21f), ModContent.ProjectileType<SkeletalStinger>(), Projectile.damage, 2f, player.whoAmI, 1f).tileCollide = false;
                        StingerDelay = 25f;
                    }
            }
        }

        private void NormalAttacking(NPC target)
        {
            Vector2 targetCenter = target.Center;

            if (Vector2.Distance(targetCenter, player.Center) > 550f)
            {
                Vector2 toCenter = targetCenter - Projectile.Center;
                toCenter.Y -= 80;
                if (toCenter.Length() < 0.0001f)
                    toCenter = Vector2.Zero;

                float distance = toCenter.Length();
                float speed;
                float inertia;
                if (distance > 500)
                {
                    speed = 12f;
                    inertia = 50f;
                }
                else
                {
                    speed = 5f;
                    inertia = 65f;
                }
                if (distance > 25f)
                {
                    toCenter.Normalize();
                    toCenter *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + toCenter) / inertia;
                }
            }
            else
                IdleMovement();


            if (Vector2.Distance(targetCenter, Projectile.Center) < 550f && Collision.CanHitLine(Projectile.Center, 1, 1, targetCenter, 1, 1))
                if (StingerDelay <= 0)
                {
                    SoundEngine.PlaySound(SoundID.Item17, Projectile.Center);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.DirectionTo(targetCenter) * 15f, ModContent.ProjectileType<SkeletalStinger>(), Projectile.damage, 2f, player.whoAmI);
                    StingerDelay = 35f;
                }

        }
    }
}
