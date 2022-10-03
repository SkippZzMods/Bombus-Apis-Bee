using BombusApisBee.BeeHelperProj;
using BombusApisBee.PrimitiveDrawing;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace BombusApisBee.Projectiles
{
    public class ChlorophyteBee : BeeHelper
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chlorophyte Bee");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2Circular(12f, 12f), ModContent.ProjectileType<ChloroEnergy>(), Projectile.damage / 2, 1f, Projectile.owner);

            for (int i = 0; i < 6; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(1.5f, 1.5f), 0, new Color(117, 216, 19), 0.35F);
            }
        }

        public override void SafeAI()
        {
            if (Main.rand.NextBool(3))
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(2f, 2f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(1f, 1f), 0, new Color(117, 216, 19, 0), 0.3f);
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(117, 216, 19, 0) * 0.65f, 0f, bloomTex.Size() / 2f, 0.55f, 0, 0);
        }
    }
    public class ChloroEnergy : BeeProjectile, IDrawPrimitive_
    {
        private Vector2 targetCenter;
        private List<Vector2> cache;
        private Trail trail;
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";

        public override bool? CanDamage() => Projectile.timeLeft < 345;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chloro-energy");
        }

        public override void SafeSetDefaults()
        {
            Projectile.friendly = true;
            Projectile.timeLeft = 360;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;

            Projectile.width = Projectile.height = 8;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            bool foundTarget = false;
            float num = 1000f;
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
            if (foundTarget && Projectile.timeLeft < 345)
                Projectile.velocity = (Projectile.velocity * 20f + Utils.SafeNormalize(targetCenter - Projectile.Center, Vector2.UnitX) * 12f) / 21f;

            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.Glow>(), Vector2.Zero, 0,
                   new Color(117, 216, 19), 0.3f);

            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = 0; i < 3; i++)
            {
                Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
                Color color = new Color(117, 216, 19, 0);
                Main.spriteBatch.Draw(tex, (Projectile.Center - Projectile.velocity) - Main.screenPosition, null, color * 0.5f, Projectile.rotation, tex.Size() / 2f, 0.45f, 0f, 0f);
            }
            return false;
        }

        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < 12; i++)
                {
                    cache.Add(Projectile.Center);
                }
            }

            cache.Add(Projectile.Center);

            while (cache.Count > 12)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 12, new TriangularTip(160), factor => 10f * (factor), factor =>
            {
                return new Color(117, 216, 19) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center;
        }

        public void DrawPrimitives()
        {
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["time"].SetValue(Projectile.timeLeft * -0.01f);
            effect.Parameters["repeats"].SetValue(3f);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/EnergyTrail").Value);

            trail?.Render(effect);
        }
    }
}