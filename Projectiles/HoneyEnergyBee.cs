using BombusApisBee.BeeHelperProj;

namespace BombusApisBee.Projectiles
{
    public class HoneyEnergyBee : BeeHelper
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pure Honey Bee");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 3;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(1.5f, 1.5f), 0, new Color(255, 255, 204), 0.4f);

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 0, new Color(253, 232, 0), 0.3f);
            }
        }

        public override bool SafePreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Texture2D texGiant = ModContent.Request<Texture2D>(Texture + "_Giant").Value;
            Rectangle frame = tex.Frame(verticalFrames: 4, frameY: Projectile.frame);
            if (Giant)
            {
                frame = texGiant.Frame(verticalFrames: 4, frameY: Projectile.frame);
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Main.spriteBatch.Draw(texGiant, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, frame, lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                        Projectile.rotation, frame.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.5f, (i / (float)Projectile.oldPos.Length)), Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

                    Main.spriteBatch.Draw(glowTex, (Projectile.oldPos[i] + (Projectile.Size * 0.5f)) - Main.screenPosition, null, new Color(255, 255, 204, 0) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                       Projectile.rotation, glowTex.Size() / 2f, Projectile.scale * MathHelper.Lerp(0.35f, 0.05f, (i / (float)Projectile.oldPos.Length)), Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                }
            }
            else
            {
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, frame, lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                        Projectile.rotation, frame.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.5f, (i / (float)Projectile.oldPos.Length)), Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

                    Main.spriteBatch.Draw(glowTex, (Projectile.oldPos[i] + (Projectile.Size * 0.5f)) - Main.screenPosition, null, new Color(255, 255, 204, 0) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                        Projectile.rotation, glowTex.Size() / 2f, Projectile.scale * MathHelper.Lerp(0.25f, 0.05f, (i / (float)Projectile.oldPos.Length)), Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                }
            }

            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 204, 0) * 0.5f, 0f, bloomTex.Size() / 2f, Giant ? 0.35f : 0.2f, 0, 0);
        }
    }
}
