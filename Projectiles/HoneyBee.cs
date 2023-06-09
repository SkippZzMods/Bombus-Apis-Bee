using BombusApisBee.BeeHelperProj;

namespace BombusApisBee.Projectiles
{
    public class HoneyBee : BeeHelper
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honeybee");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.HoneyDustSolid>(), Main.rand.NextVector2Circular(1.5f, 1.5f), Main.rand.Next(100), default, 1.35f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(2f, 2f), 0, Main.rand.NextBool() ? new Color(180, 90, 0) : new Color(251, 172, 17), 0.6f);
            }
        }

        public override bool SafePreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture + (Giant ? "_Giant" : "")).Value;
            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/BeeGlowHM").Value;
            if (Giant)
                glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/BeeGlowGiant").Value;

            Rectangle frame = tex.Frame(verticalFrames: 4, frameY: Projectile.frame);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {

                Main.spriteBatch.Draw(glowTex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, glowTex.Frame(verticalFrames: 4, frameY: Projectile.frame), new Color(229, 114, 0, 0) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, glowTex.Frame(verticalFrames: 4, frameY: Projectile.frame).Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.5f, (i / (float)Projectile.oldPos.Length)), Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, frame, lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, frame.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.5f, (i / (float)Projectile.oldPos.Length)), Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }

            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, glowTex.Frame(verticalFrames: 4, frameY: Projectile.frame), new Color(229, 114, 0, 0), Projectile.rotation, glowTex.Frame(verticalFrames: 4, frameY: Projectile.frame).Size() / 2f, Projectile.scale, Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

            return true;
        }
    }
}
