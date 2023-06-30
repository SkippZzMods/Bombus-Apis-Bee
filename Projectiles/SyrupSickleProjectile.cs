using Terraria;
namespace BombusApisBee.Projectiles
{
    public class SyrupSickleProjectile : BeeProjectile
    {
        public Vector2 startPos;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honeyed Sickle");
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
        }

        public override void SafeSetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 32;
            Projectile.timeLeft = 240;
            Projectile.penetrate = 4;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.Length() * 0.01f;
            if (Projectile.timeLeft > 30)
            {
                if (Projectile.alpha > 0)
                    Projectile.alpha -= 20;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }

            if (Projectile.timeLeft > 200)
                Projectile.velocity *= 1.03f;
            else
                Projectile.velocity *= 0.98f;

            if (Projectile.timeLeft > 30)
            {
                Dust.NewDustPerfect(Projectile.Center + ((Projectile.rotation + MathHelper.ToRadians(30f)).ToRotationVector2() * 10f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.Zero, Scale: 0.4f, newColor: new Color(255, 191, 93));

                Dust.NewDustPerfect(Projectile.Center + ((Projectile.rotation + MathHelper.ToRadians(30f)).ToRotationVector2() * -10f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.Zero, Scale: 0.4f, newColor: new Color(255, 191, 93));
            }
            else
            {
                Dust.NewDustPerfect(Projectile.Center + ((Projectile.rotation + MathHelper.ToRadians(30f)).ToRotationVector2() * 10f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.Zero, Scale: 0.4f, newColor: new Color(255, 191, 93) * MathHelper.Lerp(1f, 0f, 1f - Projectile.timeLeft / 30f));

                Dust.NewDustPerfect(Projectile.Center + ((Projectile.rotation + MathHelper.ToRadians(30f)).ToRotationVector2() * -10f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.Zero, Scale: 0.4f, newColor: new Color(255, 191, 93) * MathHelper.Lerp(1f, 0f, 1f - Projectile.timeLeft / 30f));
            }

            if (Projectile.timeLeft <= 30)
            {
                Projectile.alpha += 10;
                Projectile.velocity *= 0.97f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].AddBuff<HeartOfNectar>(240);
            if (Projectile.penetrate == 2)
            {
                Projectile.timeLeft = 30;
                Projectile.friendly = false;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, null, Projectile.GetAlpha(Color.White) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.75f, (i / (float)Projectile.oldPos.Length)), 0, 0);
            }
            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(new Color(255, 191, 93, 0)) * 0.65f, 0f, bloomTex.Size() / 2f, 0.6f, 0, 0);
            }
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0f, 0f);
            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(new Color(255, 191, 93, 0)), 0f, bloomTex.Size() / 2f, 0.4f, 0, 0);
            return false;
        }
    }
}
