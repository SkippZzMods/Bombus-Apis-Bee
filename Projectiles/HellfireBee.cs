using BombusApisBee.BeeHelperProj;

namespace BombusApisBee.Projectiles
{
    public class HellfireBee : BaseBeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hellfire Bee");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SafeOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 300);
        }

        public override bool SafePreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            Texture2D glowTex = ModContent.Request<Texture2D>(Giant ? "BombusApisBee/ExtraTextures/BeeGlowGiant" : "BombusApisBee/ExtraTextures/BeeGlowHM").Value;

            Rectangle frame = glowTex.Frame(verticalFrames: 4, frameY: Projectile.frame);

            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, frame, new Color(255, 100, 20, 0), Projectile.rotation, frame.Size() / 2f, Projectile.scale * 1.25f, Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            return true;
        }
    }
}