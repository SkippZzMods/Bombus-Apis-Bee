using BombusApisBee.BeeHelperProj;

namespace BombusApisBee.Projectiles
{
    public class IchorBee : BeeHelper
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ichor Bee");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SafeOnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 240, true);
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(1f, 1f), 0, new Color(253, 152, 0), 0.35f);

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(1.5f, 1.5f), 0, new Color(196, 102, 9), 0.3f);
            }

            SoundID.NPCHit13.PlayWith(Projectile.Center);
        }

        public override bool SafePreDraw(ref Color lightColor)
        {
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(196, 102, 9, 0), 0f, bloomTex.Size() / 2f, 0.35f, 0, 0);
            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(253, 152, 0, 0) * 0.5f, 0f, bloomTex.Size() / 2f, 0.45f, 0, 0);
        }
    }
}