using BombusApisBee.BeeHelperProj;
using BombusApisBee.Content.Dusts.Pixelized;

namespace BombusApisBee.Content.Crossmod.Calamity.Items.Weapons.Corruption
{
    [JITWhenModsEnabled("CalamityMod")]
    class DarksentBee : BaseBeeProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Darksent Bee");
            Main.projFrames[Type] = 4;
        }

        public override void SafeAI()
        {
            if (Main.rand.NextBool(60))
            {
                Vector2 velocity = Main.rand.NextVector2Circular(2f, 2f);

                Color color = Main.rand.Next(new Color[] { new Color(152, 137, 255, 0), new Color(119, 89, 227, 0), new Color(210, 228, 255, 0) });

                Dust.NewDustPerfect(Projectile.Center, DustType<PixelatedGlowAlt>(),
                    velocity, 0, color, 0.5f);

                if (Main.rand.NextBool(2))
                {
                    color = Main.rand.Next(new Color[] { new Color(152, 137, 255, 0), new Color(119, 89, 227, 0), new Color(210, 228, 255, 0) });

                    Dust.NewDustPerfect(Projectile.Center, DustType<StarDustWhite>(),
                                       velocity, 0, color, 0.6f).customData = true;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(5f, 5f);

                Color color = Main.rand.Next(new Color[] { new Color(152, 137, 255, 0), new Color(119, 89, 227, 0), new Color(210, 228, 255, 0) });

                Dust.NewDustPerfect(Projectile.Center, DustType<PixelatedGlowAlt>(),
                    velocity, 0, color, 0.5f);

                if (Main.rand.NextBool(2))
                {
                    color = Main.rand.Next(new Color[] { new Color(152, 137, 255, 0), new Color(119, 89, 227, 0), new Color(210, 228, 255, 0) });

                    Dust.NewDustPerfect(Projectile.Center, DustType<StarDustWhite>(),
                                       velocity, 0, color, 0.6f).customData = true;
                }
            }
        }

        public override bool SafePreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texGlow = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/BeeGlowPREHM").Value;
            if (Giant)
                texGlow = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/BeeGlowGiant").Value;

            Texture2D giantTex = ModContent.Request<Texture2D>(Texture + "_Giant").Value;
            Texture2D drawTex = Giant ? giantTex : tex;
            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(152, 137, 255, 0), 0f, glowTex.Size() / 2f, 0.25f, 0f, 0f);

            Rectangle sourceRectangle = drawTex.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);
            Main.spriteBatch.Draw(drawTex, Projectile.Center - Main.screenPosition, sourceRectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, sourceRectangle.Size() / 2f,
                Projectile.scale, Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            
            sourceRectangle = texGlow.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);
            Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, sourceRectangle, new Color(152, 137, 255, 0), Projectile.rotation, sourceRectangle.Size() / 2f,
                Projectile.scale, Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

            return false;
        }
    }
}
