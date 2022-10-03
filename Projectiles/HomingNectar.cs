using BombusApisBee.PrimitiveDrawing;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace BombusApisBee.Projectiles
{
    class HomingNectar : BeeProjectile
    {
        private List<Vector2> cache;
        private Trail trail;

        public override bool? CanDamage() => Projectile.timeLeft < 225;
        public override string Texture => BombusApisBee.Invisible;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Homing Nectar");
        }

        public override void SafeSetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 12;

            Projectile.timeLeft = 240;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
            }

            Projectile.rotation += Projectile.velocity.Length() * 0.01f;

            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(2.5f, 2.5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), null, Scale: 0.35f, newColor: new Color(214, 158, 79));

            bool foundTarget = false;
            Vector2 targetCenter = Vector2.Zero;
            float num = 1500f;
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
            if (foundTarget && Projectile.timeLeft < 225)
            {
                Projectile.velocity = (Projectile.velocity * 20f + Utils.SafeNormalize(targetCenter - Projectile.Center, Vector2.UnitX) * 15f) / 21f;
            }
            else if (Projectile.timeLeft < 225)
            {
                Projectile.velocity *= 0.985f;
                if (Projectile.velocity.Length() < 1f)
                    Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.player[Projectile.owner].Heal(Projectile.ai[0] == 0 ? Main.rand.Next(1, 3) : (int)Projectile.ai[0]);

            for (int i = 0; i < 25; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 0, new Color(255, 191, 73), 0.3f);
            }
        }

        public override void PostDraw(Color lightColor)
        {
            DrawPrimitives();
            Texture2D bloom = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Texture2D star = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/StarTexture").Value;
            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw(bloom, (Projectile.Center - Projectile.velocity) - Main.screenPosition, null, new Color(214, 158, 79, 0), Projectile.rotation, bloom.Size() / 2f, 0.2f, 0f, 0f);
            }
            Main.spriteBatch.Draw(star, (Projectile.Center - Projectile.velocity) - Main.screenPosition, null, new Color(214, 158, 79, 0), Projectile.rotation, star.Size() / 2f, 0.3f, 0f, 0f);
        }

        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < 15; i++)
                {
                    cache.Add(Projectile.Center);
                }
            }

            cache.Add(Projectile.Center);

            while (cache.Count > 15)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 15, new TriangularTip(1f), factor => factor * 10f, factor =>
            {
                return new Color(214, 158, 79);
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center;
        }
        public void DrawPrimitives()
        {
            Main.spriteBatch.End();
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["time"].SetValue(Main.GameUpdateCount * -0.03f);
            effect.Parameters["repeats"].SetValue(1);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);
            trail?.Render(effect);
            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
