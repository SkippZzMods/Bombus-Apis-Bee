using BombusApisBee.Dusts;
using Microsoft.Xna.Framework.Graphics;

namespace BombusApisBee.Projectiles
{
    public class BeeFishBubble : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Bubble");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] > 2f)
            {
                Projectile.alpha -= 10;
                if (Projectile.alpha < 150)
                {
                    Projectile.alpha = 150;
                }
            }
            else
            {
                Projectile.localAI[0] += 1f;
            }
            if (Projectile.ai[0] > 30f)
            {
                if (Projectile.velocity.Y > -8f)
                {
                    Projectile.velocity.Y -= 0.05f;
                }
                Projectile.velocity.X *= 0.98f;
            }
            else
            {
                Projectile.ai[0] += 1f;
            }
            Projectile.rotation = Projectile.velocity.X * 0.1f;

            if (Projectile.wet)
            {
                if (Projectile.velocity.Y > 0f)
                    Projectile.velocity.Y *= 0.98f;
                if (Projectile.velocity.Y > -8f)
                    Projectile.velocity.Y -= 0.2f;
                Projectile.velocity.X *= 0.94f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, null, lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.5f, (i / (float)Projectile.oldPos.Length)), 0, 0);
            }
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0f, 0f);
            return false; ;
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            if (Main.myPlayer == Projectile.owner)
                for (int i = 0; i < 2 + Main.rand.Next(3); i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.position, Main.rand.NextVector2Circular(2f, 2f), player.beeType(), player.beeDamage(Projectile.damage / 2), 0f, Projectile.owner);
                }

            SoundEngine.PlaySound(SoundID.Item54, Projectile.position);
            for (int i = 0; i < 25; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<HoneyDust>(), Main.rand.NextVector2Circular(2f, 2f), Main.rand.Next(80), Scale: Main.rand.NextFloat(0.8f, 1.25f)).noGravity = true;
            }
        }
    }
}
