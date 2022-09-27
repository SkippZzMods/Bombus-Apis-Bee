using Microsoft.Xna.Framework.Graphics;
using System;

namespace BombusApisBee.Projectiles
{
    public class AstralStar : ModProjectile
    {
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Star");
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = BeeUtils.BeeDamageClass();
            Projectile.penetrate = -1;

            Projectile.width = Projectile.height = 16;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;

            Projectile.timeLeft = 240;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            Projectile.rotation += 0.35f * Projectile.direction;

            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 20 + Main.rand.Next(40);
                Terraria.Audio.SoundEngine.PlaySound(in SoundID.Item9, Projectile.position);
            }

            //dust
            if (++Projectile.localAI[0] % 15 == 0)
            {
                for (float k = 0; k < 6.28f; k += 0.1f)
                {
                    float x = (float)Math.Cos(k) * 30;
                    float y = (float)Math.Sin(k) * 10;

                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.MagicDust>(), new Vector2(x, y).RotatedBy(Projectile.velocity.ToRotation() + MathHelper.PiOver2) * 0.03f, 0, new Color(107, 172, 255, 100), 1.35f).noGravity = true;
                    Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, ModContent.DustType<Dusts.MagicDust>(), new Vector2(x, y).RotatedBy(Projectile.velocity.ToRotation() + MathHelper.PiOver2) * 0.045f, 0, new Color(157, 127, 207, 100), 1.35f).noGravity = true;
                }
            }

            Dust.NewDustPerfect(Projectile.Center + (Projectile.rotation.ToRotationVector2() * 10f) + Projectile.velocity, ModContent.DustType<Dusts.MagicDust>(), Vector2.Zero, Scale: 1.2f, newColor: new Color(157, 127, 207, 100)).noGravity = true;

            Dust.NewDustPerfect(Projectile.Center + (Projectile.rotation.ToRotationVector2() * -10f) + Projectile.velocity, ModContent.DustType<Dusts.MagicDust>(), Vector2.Zero, Scale: 1.2f, newColor: new Color(107, 172, 255, 100)).noGravity = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texGlow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D starTex = TextureAssets.Extra[91].Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, null, Color.White * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.5f, (i / (float)Projectile.oldPos.Length)), 0, 0);
            }
            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(107, 172, 255, 0), 0f, bloomTex.Size() / 2f, 0.55f, 0, 0);
            }

            Main.spriteBatch.Draw(starTex, Projectile.Center - Main.screenPosition - Projectile.velocity, null, new Color(107, 172, 255, 0) * 0.75f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, starTex.Size() / 2f, 0.55f, 0, 0);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0f, 0f);

            Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, new Color(107, 172, 255, 0), Projectile.rotation, texGlow.Size() / 2f, Projectile.scale, 0f, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            float rot;
            if (Projectile.rotation < 0) { rot = Main.rand.NextFloat(0, (float)Math.PI * 2); }
            else { rot = Projectile.rotation; }

            float density = 1 / 1f * 0.1f;

            for (float k = 0; k < 6.28f; k += density)
            {
                float rand = 0;

                float x = (float)Math.Cos(k + rand);
                float y = (float)Math.Sin(k + rand);
                float mult = ((Math.Abs(((k * (5 / 2)) % (float)Math.PI) - (float)Math.PI / 2)) * 0.6f) + 0.5f;
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.MagicDust>(), new Vector2(x, y).RotatedBy(rot) * mult * 1.2f, 0, new Color(107, 172, 255, 100), 1.5f);
            }

            for (float k = 0; k < 6.28f; k += density)
            {
                float rand = 0;

                float x = (float)Math.Cos(k + rand);
                float y = (float)Math.Sin(k + rand);
                float mult = ((Math.Abs(((k * (6 / 2)) % (float)Math.PI) - (float)Math.PI / 2)) * 0.5f) + 0.5f;
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.MagicDust>(), new Vector2(x, y).RotatedBy(rot) * mult * 0.85f, 0, new Color(157, 127, 207, 100), 1.5f);
            }

            for (int i = 0; i < 25; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4f, 4f), ModContent.DustType<Dusts.MagicDust>(), Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(1.5f, 2f), 0, new Color(157, 127, 207, 100), 1.2f);

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4f, 4f), ModContent.DustType<Dusts.MagicDust>(), Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(1f, 1.5f), 0, new Color(107, 172, 255, 100), 1.2f);

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(3f, 3f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(1f, 2.5f), 0, new Color(157, 127, 207, 100), 0.35f);

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(3f, 3f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(1f, 2.5f), 0, new Color(107, 172, 255, 100), 0.35f);
            }

            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(3f, 3f), ModContent.DustType<Dusts.Stardust>(), Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(1f, 2.5f));
            }
        }
    }
}
