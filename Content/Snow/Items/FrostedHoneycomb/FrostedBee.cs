using BombusApisBee.Core.Common.BeeProjectile;

namespace BombusApisBee.Content.Snow.Items.FrostedHoneycomb
{
    public class FrostedBee : CommonBeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("FrostedBee");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SafeOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff<Glacialstruck>(240);
        }

        public override void SafeAI()
        {
            if (Main.rand.NextBool(3))
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), Main.rand.NextBool() ? DustType<Dusts.FrostedDust>() : DustType<Dusts.GlowFastDecelerate>(), newColor: new Color(153, 212, 242, 150)).scale = 0.3f;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D glowTex = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(153, 212, 242, 0) * 0.35f, Projectile.rotation, glowTex.Size() / 2f, Giant ? 0.4f : 0.3f, 0, 0f);
        }
    }
}