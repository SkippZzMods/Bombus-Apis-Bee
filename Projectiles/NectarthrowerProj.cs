using Terraria;
namespace BombusApisBee.Projectiles
{
    public class NectarthrowerProj : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Molten Nectar");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SafeSetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 100;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 255;
            Projectile.scale = 0.25f;
            Projectile.width = 100;
            Projectile.height = 100;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 9;

            Projectile.rotation = Main.rand.NextFloat(6.28f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.Zero;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/FlamethrowerTexture").Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Color color = Color.Lerp(new Color(214, 158, 79), new Color(163, 97, 66), 1f - Projectile.timeLeft / 100f);
            color.A = 0;

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(color), Projectile.rotation, bloomTex.Size() / 2f, Projectile.scale * 1.25f, 0, 0);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, null, Projectile.GetAlpha(color) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, Projectile.scale, (i / (float)Projectile.oldPos.Length)), 0, 0);
            }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(color), Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0f, 0f);

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(color) * 0.5f, Projectile.rotation, bloomTex.Size() / 2f, Projectile.scale, 0, 0);

            color = Color.Lerp(new Color(255, 218, 110), new Color(214, 158, 79), 1f - Projectile.timeLeft / 100f);
            color.A = 0;

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(color) * 0.75f, Projectile.rotation, bloomTex.Size() / 2f, Projectile.scale, 0, 0);
            return false;
        }
        public override void AI()
        {
            if (Projectile.timeLeft > 50)
            {
                Projectile.alpha -= 15;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;

                if (Projectile.scale < 0.8f)
                    Projectile.scale += 0.05f;
                if (Projectile.scale > 0.8f)
                    Projectile.scale = 0.8f;
            }
            else
            {
                Projectile.alpha += 15;

                Projectile.scale -= 0.015f;
                Projectile.velocity *= 0.975f;

                if (Projectile.alpha > 255 || Projectile.scale <= 0)
                    Projectile.Kill();
            }



            Projectile.rotation += 0.25f * Projectile.direction;

            Projectile.velocity *= 0.975f;

            if (Main.rand.NextBool(5))
            {
                float lerper = MathHelper.Lerp(65f, 5f, 1f - Projectile.timeLeft / 100f);
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(lerper, lerper), ModContent.DustType<Dusts.GlowFastDecelerate>(), null, Projectile.alpha, Color.Lerp(new Color(214, 158, 79), new Color(163, 97, 66), 1f - Projectile.timeLeft / 100f), 0.4f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff<NectarGlazed>(240);
        }
    }
}