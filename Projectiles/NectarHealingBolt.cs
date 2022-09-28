using BombusApisBee.PrimitiveDrawing;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;
using System.Collections.Generic;

namespace BombusApisBee.Projectiles
{
    class NectarHealingBolt : BeeProjectile, IDrawPrimitive_
    {
        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;
        public override string Texture => BombusApisBee.Invisible;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nectar Healing Bolt");
        }

        public override void SafeSetDefaults()
        {
            Projectile.friendly = false;
            Projectile.width = Projectile.height = 16;

            Projectile.timeLeft = 360;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (!Main.player[Projectile.owner].active)
                Projectile.Kill();

            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
            }

            Projectile.rotation += Projectile.velocity.Length() * 0.035f;

            Dust.NewDustPerfect(Projectile.Center + (Projectile.rotation.ToRotationVector2() * 3f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.Zero, Scale: 0.35f, newColor: new Color(255, 255, 150));

            Projectile.velocity = (Projectile.velocity * 20f + Utils.SafeNormalize(Main.player[Projectile.owner].Center - Projectile.Center, Vector2.UnitX) * 16.5f) / 21f;

            if (Projectile.Hitbox.Intersects(Main.player[Projectile.owner].Hitbox))
            {
                Main.player[Projectile.owner].Heal(Main.rand.Next(3, 6));
                Projectile.Kill();
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(3f, 3f), 0, new Color(255, 191, 73), 0.3f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D bloom = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Texture2D star = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/StarTexture").Value;
            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw(bloom, (Projectile.Center - Projectile.velocity) - Main.screenPosition, null, new Color(206, 116, 59, 0), Projectile.rotation, bloom.Size() / 2f, 0.2f, 0f, 0f);
            }
            Main.spriteBatch.Draw(star, (Projectile.Center - Projectile.velocity) - Main.screenPosition, null, new Color(206, 116, 59, 0), Projectile.rotation, star.Size() / 2f, 0.2f, 0f, 0f);
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
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 12, new TriangularTip(190), factor => factor * 9.5f, factor =>
            {
                return new Color(206, 116, 59) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center;

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 12, new TriangularTip(190), factor => factor * 7f, factor =>
            {
                return new Color(255, 191, 73) * factor.X;
            });

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = Projectile.Center;
        }

        public void DrawPrimitives()
        {
            Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["time"].SetValue(Projectile.timeLeft * -0.025f);
            effect.Parameters["repeats"].SetValue(2f);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

            trail?.Render(effect);
            trail2?.Render(effect);
        }
    }
}
