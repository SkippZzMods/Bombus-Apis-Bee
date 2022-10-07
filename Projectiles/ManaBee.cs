using BombusApisBee.BeeHelperProj;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BombusApisBee.Projectiles
{
    public class ManaBee : BeeHelper
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mana Bee");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void Kill(int timeLeft)
        {
            float rot;
            if (Projectile.rotation < 0) { rot = Main.rand.NextFloat(0, (float)Math.PI * 2); }
            else { rot = Projectile.rotation; }

            for (float k = 0; k < 6.28f; k += 0.1f)
            {
                float rand = 0;

                float x = (float)Math.Cos(k + rand);
                float y = (float)Math.Sin(k + rand);
                float mult = ((Math.Abs(((k * (5 / 2)) % (float)Math.PI) - (float)Math.PI / 2)) * 1f) + 0.75f;
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), new Vector2(x, y).RotatedBy(rot) * mult * 1.2f, 0, new Color(18, 18, 255), 0.3f);
            }

            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2CircularEdge(12f, 12f), ModContent.ProjectileType<HomingManaStar>(), Projectile.damage, 3.5f, Projectile.owner);
        }

        public override void SafeAI()
        {
            if (Main.rand.NextBool(3))
                Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(1.5f, 1.5f), 55,
                    Main.rand.NextBool() ? new Color(18, 18, 255) : new Color(100, 96, 255), Giant ? 0.4f : 0.3f);
        }

        public override bool SafePreDraw(ref Color lightColor)
        {
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(18, 18, 255, 0), 0f, bloomTex.Size() / 2f, 0.35f, 0, 0);
            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            for (int i = 0; i < 3; i++)
            {
                Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(100, 96, 255, 0), 0f, bloomTex.Size() / 2f, 0.2f, 0, 0);
            }
        }
    }

    class HomingManaStar : BeeProjectile
    {
        public override bool? CanDamage() => Projectile.timeLeft < 400;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Homing Mana Star");

            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;

            Projectile.timeLeft = 430;

            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;

            Projectile.scale = 0.35f;
        }

        public override void AI()
        {
            if (Projectile.scale < 1f)
                Projectile.scale += 0.02f;
            if (Projectile.scale > 1f)
                Projectile.scale = 1f;

            bool foundTarget = false;
            Vector2 targetCenter = Vector2.Zero;
            float num = 750f;
            for (int i = 0; i < 200; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(this, false))
                {
                    float num2 = Projectile.Distance(npc.Center);
                    if (num > num2)
                    {
                        num = num2;
                        targetCenter = npc.Center;
                        foundTarget = true;
                    }
                }
            }

            if (foundTarget && Projectile.timeLeft < 400)
                Projectile.velocity = (Projectile.velocity * 35f + Utils.SafeNormalize(targetCenter - Projectile.Center, Vector2.UnitX) * 14f) / 36f;
            else
            {
                Projectile.velocity *= 0.96f;

                if (Projectile.velocity.Length() < 1f)
                    Projectile.Kill();
            }

            Projectile.rotation += 0.35f * Projectile.direction;

            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 20 + Main.rand.Next(40);
                SoundEngine.PlaySound(in SoundID.Item9, Projectile.position);
            }

            Dust.NewDustPerfect(Projectile.Center + (Projectile.rotation.ToRotationVector2() * 12f) + Projectile.velocity, ModContent.DustType<Dusts.Glow>(), Vector2.Zero, Scale: 0.35f, newColor: new Color(18, 18, 255));

            Dust.NewDustPerfect(Projectile.Center + (Projectile.rotation.ToRotationVector2() * -12f) + Projectile.velocity, ModContent.DustType<Dusts.Glow>(), Vector2.Zero, Scale: 0.35f, newColor: new Color(100, 96, 255));

            if (Main.rand.NextBool(10))
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.ManaStardust>(), Main.rand.NextVector2Circular(3.5f, 3.5f));
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10 with { Pitch = 0.1f }, Projectile.position);

            for (int i = 0; i < 25; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 0, new Color(18, 18, 255), 0.45f);

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 0, new Color(100, 96, 255), 0.45f);

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.Glow>(), Main.rand.NextVector2Circular(2f, 2f), 0, new Color(100, 96, 255), 0.55f);
            }

            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.ManaStardust>(), Main.rand.NextVector2Circular(3.5f, 3.5f), Scale: 1.25f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texGlow = ModContent.Request<Texture2D>("BombusApisBee/Projectiles/AstralStar_Glow").Value;
            Texture2D starTex = TextureAssets.Extra[91].Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, null, Color.White * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.oldRot[i], tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.5f, (i / (float)Projectile.oldPos.Length)), 0, 0);

                Main.spriteBatch.Draw(bloomTex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, null, new Color(18, 18, 255, 0) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, bloomTex.Size() / 2f, Projectile.scale * MathHelper.Lerp(0.5f, 0.15f, (i / (float)Projectile.oldPos.Length)), 0, 0);
            }

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(18, 18, 255, 0), 0f, bloomTex.Size() / 2f, Projectile.scale * 0.45f, 0, 0);

            Main.spriteBatch.Draw(starTex, Projectile.Center - Main.screenPosition - Projectile.velocity, null, new Color(18, 18, 255, 0) * 0.65f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, starTex.Size() / 2f, Projectile.scale * 0.6f, 0, 0);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0f, 0f);

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(18, 18, 255, 0), 0f, bloomTex.Size() / 2f, Projectile.scale * 0.3f, 0, 0);

            Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, new Color(100, 96, 255, 0), Projectile.rotation, texGlow.Size() / 2f, Projectile.scale, 0f, 0f);

            return false;
        }
    }
}