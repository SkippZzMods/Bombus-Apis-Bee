using BombusApisBee.PrimitiveDrawing;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.Graphics.Effects;


namespace BombusApisBee.Projectiles
{
    public class HomingStinger : ModProjectile
    {
        private List<Vector2> cache;
        private Trail trail;
        public override bool? CanDamage() => Projectile.timeLeft < 465;
        public override string Texture => "BombusApisBee/ExtraTextures/StingerRetexture";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Homing Stinger");
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = BeeUtils.BeeDamageClass();

            Projectile.timeLeft = 480;
            Projectile.width = Projectile.height = 10;
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
            }

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
            if (foundTarget && Projectile.timeLeft < 465)
            {
                Projectile.velocity = (Projectile.velocity * 20f + Utils.SafeNormalize(targetCenter - Projectile.Center, Vector2.UnitX) * 15f) / 21f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Main.rand.NextBool())
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptGibs);
                dust.scale = 0.9f;
                dust.velocity *= 0.5f;
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrimitives();
            return true;
        }
        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < 10; i++)
                {
                    cache.Add(Projectile.Center);
                }
            }

            cache.Add(Projectile.Center);

            while (cache.Count > 10)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 10, new TriangularTip(190), factor => 6.5f, factor =>
            {
                return new Color(160, 191, 38) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center;
        }

        public void DrawPrimitives()
        {
            Main.spriteBatch.End();
            Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["time"].SetValue(Projectile.timeLeft * -0.025f);
            effect.Parameters["repeats"].SetValue(2f);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/EnergyTrail").Value);

            trail?.Render(effect);
            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
