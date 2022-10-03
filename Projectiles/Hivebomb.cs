using BombusApisBee.Dusts;
using BombusApisBee.PrimitiveDrawing;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace BombusApisBee.Projectiles
{
    public class Hivebomb : BeeProjectile
    {
        private List<Vector2> cache;
        private Trail trail;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hivebomb");
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;

            Projectile.timeLeft = 360;
        }

        public override void AI()
        {
            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
            }
            bool foundTarget = false;
            Vector2 targetCenter = Vector2.Zero;
            float num = 1000f;
            for (int i = 0; i < 200; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(this, false) && Collision.CanHitLine(Projectile.Center, 1, 1, npc.Center, 1, 1))
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
                
            if (foundTarget)
                Projectile.velocity = (Projectile.velocity * 20f + Utils.SafeNormalize(targetCenter - Projectile.Center, Vector2.UnitX) * 12f) / 21f;

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void Kill(int timeLeft)
        {
            Main.player[Projectile.owner].Bombus().shakeTimer += 4;
            if (Main.myPlayer == Projectile.owner)
            {
                for (int i = 0; i < Main.rand.Next(4, 7); i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2Circular(3.5f, 3.5f), ModContent.ProjectileType<HoneySmoke>(), Projectile.damage, 0f, Projectile.owner);
                }

                for (int i = 0; i < 3; i++)
                {
                    Projectile.SpawnBee(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2Circular(3.5f, 3.5f), Projectile.damage * 0.65f).DamageType = BeeUtils.BeeDamageClass();
                }
            }

            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<HoneyDust>(), Main.rand.NextVector2Circular(2f, 2f), Main.rand.Next(50, 150), default, 1.25f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, DustID.Honey2, Main.rand.NextVector2Circular(3f, 3f), Main.rand.Next(50, 100), default, 1.25f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<HoneyDust>(), Main.rand.NextVector2Circular(6f, 6f), Main.rand.Next(50, 150), default, 1.25f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, DustID.Honey2, Main.rand.NextVector2Circular(7f, 7f), Main.rand.Next(50, 100), default, 1.25f).noGravity = true;
            }

            SoundID.DD2_GoblinBomb.PlayWith(Projectile.Center, -0.1f, 0.25f, 1.25f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrimitives();
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0, 0f);

            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(163, 97, 55, 0), Projectile.rotation, glowTex.Size() / 2f, 0.35f, 0, 0f);
            return false;
        }

        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < 13; i++)
                {
                    cache.Add(Projectile.Center);
                }
            }

            cache.Add(Projectile.Center);

            while (cache.Count > 13)
            {
                cache.RemoveAt(0);
            }

        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 13, new TriangularTip(8), factor => 3.5f, factor =>
            {
                return Color.Lerp(new Color(214, 157, 79), new Color(163, 97, 66), factor.X) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.position + Projectile.velocity;
        }
        public void DrawPrimitives()
        {
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["time"].SetValue(Projectile.timeLeft * -0.04f);
            effect.Parameters["repeats"].SetValue(1);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/EnergyTrail").Value);

            trail?.Render(effect);

            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/FireTrail").Value);

            trail?.Render(effect);
        }
    }
}
