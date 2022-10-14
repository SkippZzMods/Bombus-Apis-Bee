using Microsoft.Xna.Framework.Graphics;
using System;

namespace BombusApisBee.Projectiles
{
    public class CorruptMiniEater : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Corrupt Mini Eater");
            Main.projFrames[Projectile.type] = 2;

            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
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

            Projectile.extraUpdates = 1;
        }

        public override bool? CanDamage() => Projectile.timeLeft < 270;

        public override void AI()
        {
            if (Projectile.alpha > 0)
                Projectile.alpha -= 50;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            if (++Projectile.frameCounter % 6 == 0)
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            float findClosest = 10000f;
            float num262 = Projectile.position.X;
            float num263 = Projectile.position.Y;
            bool flag63 = false;
            Projectile.ai[0]++;
            if (Projectile.ai[0] > 30f)
            {
                Projectile.ai[0] = 30;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy() && (!npc.wet || npc.type == NPCID.DukeFishron))
                    {
                        float num267 = npc.position.X + (float)(npc.width / 2);
                        float num268 = npc.position.Y + (float)(npc.height / 2);
                        float dist = Vector2.Distance(Projectile.Center, npc.Center);
                        if (dist < 1000f && dist < findClosest && Collision.CanHit(Projectile.position, 1, 1, npc.position, 1, 1))
                        {
                            findClosest = dist;
                            num262 = num267;
                            num263 = num268;
                            flag63 = true;
                        }
                    }
                }
            }
            if (!flag63)
            {
                num262 = Projectile.position.X + (float)(Projectile.width / 2) + Projectile.velocity.X * 100f;
                num263 = Projectile.position.Y + (float)(Projectile.height / 2) + Projectile.velocity.Y * 100f;
            }
            float num270 = 10f;
            float num271 = 0.16f;

            Vector2 vector18 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
            float num272 = num262 - vector18.X;
            float num273 = num263 - vector18.Y;
            float num274 = (float)Math.Sqrt(num272 * num272 + num273 * num273);
            num274 = num270 / num274;
            num272 *= num274;
            num273 *= num274;

            if (Projectile.velocity.X < num272)
            {
                Projectile.velocity.X += num271;
                if (Projectile.velocity.X < 0f && num272 > 0f)
                    Projectile.velocity.X += num271 * 2f;
            }
            else if (Projectile.velocity.X > num272)
            {
                Projectile.velocity.X -= num271;
                if (Projectile.velocity.X > 0f && num272 < 0f)
                    Projectile.velocity.X -= num271 * 2f;
            }
            if (Projectile.velocity.Y < num273)
            {
                Projectile.velocity.Y += num271;
                if (Projectile.velocity.Y < 0f && num273 > 0f)
                    Projectile.velocity.Y += num271 * 2f;
            }
            else if (Projectile.velocity.Y > num273)
            {
                Projectile.velocity.Y -= num271;
                if (Projectile.velocity.Y > 0f && num273 < 0f)
                    Projectile.velocity.Y -= num271 * 2f;
            }
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.CorruptGibs, Main.rand.NextVector2Circular(2f, 2f), Main.rand.Next(100), default, 1.35f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 0, new Color(140, 169, 44), 0.4f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = tex.Frame(verticalFrames: 2, frameY: Projectile.frame);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, frame, lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, frame.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.5f, (i / (float)Projectile.oldPos.Length)), 0, 0);
            }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() / 2f, Projectile.scale, 0f, 0f);
            return false;
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

        private int bounce = 3;
    }
}
