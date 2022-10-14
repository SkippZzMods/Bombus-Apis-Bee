using BombusApisBee.BeeHelperProj;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombusApisBee.Projectiles
{
    public class CorruptedBee : BeeHelper
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Corrupted Bee");

            Main.projFrames[Projectile.type] = 4;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(Projectile.Center,DustID.CorruptGibs, Main.rand.NextVector2Circular(1.5f, 1.5f), Main.rand.Next(100), default, 1.35f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(2f, 2f), 0, new Color(140, 169, 44), 0.35f);
            }
        }

        public override bool SafePreDraw(ref Color lightColor)
        {
            Texture2D texGlow = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/BeeGlowPREHM").Value;
            if (Giant)
                glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/BeeGlowGiant").Value;

            Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, new Color(140, 169, 44, 0) * 0.65f, 0f, texGlow.Size() / 2f, Giant ? 0.45f : 0.35f, Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, glowTex.Frame(verticalFrames: 4, frameY: Projectile.frame), new Color(140, 169, 44, 0), Projectile.rotation, glowTex.Frame(verticalFrames: 4, frameY: Projectile.frame).Size() / 2f, Projectile.scale, Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

            return true;
        }
    }
}
