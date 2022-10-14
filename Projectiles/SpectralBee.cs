using BombusApisBee.BeeHelperProj;
using BombusApisBee.Dusts;
using Microsoft.Xna.Framework.Graphics;

namespace BombusApisBee.Projectiles
{
    public class SpectralBee : BeeHelper
    {
        public override int FrameTimer => 4;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spectral Bee");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
        }

        public override void SafeSetDefaults()
        {
            Projectile.penetrate = 4;
            Projectile.tileCollide = false;
            Projectile.light = 0.1f;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(1.5f, 1.5f), 0, new Color(130, 153, 208), 0.35f);

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 0, new Color(189, 240, 224), 0.3f);
            }
        }

        public override bool SafePreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            Texture2D tex = ModContent.Request<Texture2D>(Texture + (Giant ? "_Giant" : "")).Value;
            Texture2D glowTex = ModContent.Request<Texture2D>(Texture + (Giant ? "_Giant_Glowy" : "_Glowy")).Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Rectangle frame = tex.Frame(verticalFrames: 4, frameY: Projectile.frame);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, frame, lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, frame.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.5f, (i / (float)Projectile.oldPos.Length)), Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

                Main.spriteBatch.Draw(glowTex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, glowTex.Frame(verticalFrames: 4, frameY: Projectile.frame), new Color(130, 153, 208, 0) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, glowTex.Frame(verticalFrames: 4, frameY: Projectile.frame).Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.5f, (i / (float)Projectile.oldPos.Length)), Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D glowTex = ModContent.Request<Texture2D>(Texture + (Giant ? "_Giant_Glowy" : "_Glowy")).Value;
            Rectangle frame = glowTex.Frame(verticalFrames: 4, frameY: Projectile.frame);
            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, frame, new Color(130, 153, 208, 0), Projectile.rotation, frame.Size() / 2f, Projectile.scale, Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        }
    }
}