using BombusApisBee.Assets;
using BombusApisBee.Content.Dusts.Pixelized;
using BombusApisBee.Core.Common.BeeProjectile;

namespace BombusApisBee.Content.Snow.Items.BorealApiary
{
    public class SnowBee : CommonBeeProjectile
    {
        public SnowBee() : base(name: "Snowy Bee", penetrate: 2, speed: 6f, giantSpeed: 7f) { }

        public override void SafeAI()
        {
            if (Main.rand.NextBool(20))
                Dust.NewDustPerfect(Projectile.Center, DustType<PixelatedGlow>(), -Projectile.velocity.RotatedByRandom(0.2f) * Main.rand.NextFloat(0.3f), 0, new Color(215, 216, 234, 0), 0.06f);        
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<PixelatedGlow>(), Main.rand.NextVector2CircularEdge(1.5f, 1.5f), 0, new Color(215, 216, 234, 0), 0.1f);
            }
        }

        public override void SafeOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 pos = target.Center + Main.rand.NextVector2Circular(target.width / 2, target.height / 2);

                Dust.NewDustPerfect(pos, DustType<PixelatedGlow>(), Main.rand.NextVector2CircularEdge(1f, 1f), 0, new Color(128, 180, 226, 0), 0.1f);
            }
        }

        public override bool SafePreDraw(ref Color lightColor)
        {
            Texture2D glow = Request<Texture2D>(Giant ? AssetDirectory.GiantBeeGlow : AssetDirectory.BeeGlow).Value;

            Rectangle frame = glow.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);

            SpriteEffects flip = Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, frame, new Color(215, 216, 234, 0) * 0.2f, Projectile.rotation, frame.Size() / 2f, Projectile.scale * 1.1f, flip, 0f);

            return true;
        }
    }
}
