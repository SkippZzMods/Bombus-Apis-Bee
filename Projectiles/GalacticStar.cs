using Microsoft.Xna.Framework.Graphics;
using System;

namespace BombusApisBee.Projectiles
{
    public class GalacticStar : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galactic Star");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SafeSetDefaults()
        {
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.alpha = 55;
            Projectile.scale = 0.5f;
        }
        public override void AI()
        {
            int dustType = Main.rand.Next(3);
            dustType = ((dustType == 0) ? 27 : ((dustType == 1) ? 59 : 58));
            Dust.NewDust(Projectile.Center, 14, 14, dustType, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 150, default(Color), 0.5f);
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Utils.NextBool(Main.rand, 5))
                {
                    SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
                }
            }
            if (Utils.NextBool(Main.rand, 55))
            {
                int starGore = Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f), 16, 1f);
                Main.gore[starGore].velocity *= 0.72f;
                Main.gore[starGore].velocity += Projectile.velocity * 0.35f;
            }
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;
            float distanceRequired = 800f;
            float homingVelocity = 18f;
            float N = 20f;
            Vector2 destination = Projectile.Center;
            bool locatedTarget = false;
            for (int i = 0; i < 200; i++)
            {
                float extraDistance = (float)(Main.npc[i].width / 2 + Main.npc[i].height / 2);
                if (Main.npc[i].CanBeChasedBy(Projectile, false) && Projectile.WithinRange(Main.npc[i].Center, distanceRequired + extraDistance))
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
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color drawColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, Projectile.alpha) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, drawColor * 0.9f, Projectile.oldRot[k], drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
    }
}
