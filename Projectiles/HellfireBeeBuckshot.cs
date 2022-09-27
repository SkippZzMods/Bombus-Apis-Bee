using BombusApisBee.PrimitiveDrawing;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.Graphics.Effects;

namespace BombusApisBee.Projectiles
{
    public class HellfireBeeBuckshot : BeeProjectile, IDrawPrimitive_
    {
        private Vector2 initVelo;
        private List<Vector2> cache;
        private Trail trail;

        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";

        public override void SafeSetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.extraUpdates = 4;
            Projectile.alpha = 255;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hellfire Bee Buckshot");
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 180)
            {
                Projectile.velocity *= Main.rand.NextFloat(1.75f, 2.25f);
                initVelo = Projectile.velocity;
            }

            Projectile.velocity *= 0.985f;

            if (Main.rand.NextBool(3))
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), default, 55,
                    Main.rand.NextBool() ? new Color(195, 40, 20) : new Color(255, 225, 45), Main.rand.NextFloat(0.3f, 0.5f));

            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire3, 300);
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            if (Main.myPlayer == Projectile.owner)
            {
                int damage = player.beeDamage((int)(Projectile.damage * 0.85f));
                float knockBack = player.beeKB(Projectile.knockBack);
                Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, initVelo * 0.5f, ModContent.ProjectileType<HellfireBee>(), damage, knockBack, Projectile.owner);
            }
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
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 12, new TriangularTip(190), factor => factor * 6f, factor =>
            {
                return Color.Lerp(new Color(255, 225, 45), new Color(195, 40, 20), 1 - (Projectile.timeLeft / 100f));
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center;
        }

        public void DrawPrimitives()
        {
            Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
            effect.Parameters["repeats"].SetValue(2f);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

            trail?.Render(effect);
        }
    }
}
