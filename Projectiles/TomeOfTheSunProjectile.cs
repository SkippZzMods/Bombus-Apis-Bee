using BombusApisBee.PrimitiveDrawing;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombusApisBee.Projectiles
{
    public class TomeOfTheSunProjectile : BeeProjectile
    {
        public float Progress => 1f - Projectile.timeLeft / 120f;
        public override string Texture => BombusApisBee.Invisible;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vibrant Honey Energy");
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = Projectile.height = 30;

            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.timeLeft = 120;

            Projectile.penetrate = -1;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
        }

        public override void AI()
        {
            for (int i = 0; i < 2; i++)
            {
                float lerper = MathHelper.Lerp(105f, 1f, Progress);
                Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(lerper, lerper);
                Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.GlowFastDecelerate>(), pos.DirectionTo(Projectile.Center) * 4.25f, 0, new Color(245, 245, 149), 0.35f);
            }

            if (Projectile.timeLeft == 60)
                new SoundStyle("BombusApisBee/Sounds/Item/FireCast").PlayWith(Projectile.Center);
        }

        public override void Kill(int timeLeft)
        {
            Main.player[Projectile.owner].Bombus().AddShake(7);
            for (int i = 0; i < 30; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(4.5f, 4.5f), 0, new Color(245, 245, 149), 0.85f);

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(4f, 4f), 0, new Color(222, 173, 40), 0.9f);
            }
            new SoundStyle("BombusApisBee/Sounds/Item/FireHit").PlayWith(Projectile.Center);

            for (int i = 0; i < Main.rand.Next(2, 4); i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2CircularEdge(18f, 18f) * Main.rand.NextFloat(0.5f, 1f), ModContent.ProjectileType<TomeOfTheSunProjectileHoming>(), Projectile.damage, 3.5f, Projectile.owner);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 line = targetHitbox.Center.ToVector2() - Projectile.Center;
            line.Normalize();
            line *= MathHelper.Lerp(60f, 5f, Progress);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + line);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];
            player.Heal(1);
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public override void PostDraw(Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/BloomFlare").Value;
            Texture2D starTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/StarTexture").Value;

            Color yellow = new Color(222, 173, 40, 0);
            Color lightYellow = new Color(245, 245, 149, 0);

            Main.spriteBatch.Draw(starTex, Projectile.Center - Main.screenPosition, null, lightYellow * 0.25f, 0.78f + Projectile.timeLeft * 0.055f, starTex.Size() / 2f, MathHelper.Lerp(1.75f, 0.05f, Progress), 0f, 0f);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, new Color(190, 115, 39, 0) * 0.35f, Projectile.timeLeft * 0.05f, tex.Size() / 2f, MathHelper.Lerp(1.75f, 0.15f, Progress), 0f, 0f);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, yellow * 0.35f, Projectile.timeLeft * -0.065f, tex.Size() / 2f, MathHelper.Lerp(1.35f, 0.1f, Progress), 0f, 0f);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightYellow * 0.35f, 1.57f + Projectile.timeLeft * -0.045f, tex.Size() / 2f, MathHelper.Lerp(0.95f, 0.05f, Progress), 0f, 0f);

            Main.spriteBatch.Draw(starTex, Projectile.Center - Main.screenPosition, null, lightYellow * 0.75f, 0.78f + Projectile.timeLeft * 0.025f, starTex.Size() / 2f, MathHelper.Lerp(1.5f, 0.05f, Progress), 0f, 0f);
        }
    }

    class TomeOfTheSunProjectileHoming : BeeProjectile, IDrawPrimitive_
    {
        private List<Vector2> cache;
        private Trail trail;
        private int hitDelay;
        public override string Texture => BombusApisBee.Invisible;

        public override bool? CanDamage() => Projectile.timeLeft < 460 && hitDelay <= 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sun Energy Bolt");
        }

        public override void SafeSetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 30;

            Projectile.timeLeft = 480;
            Projectile.penetrate = 3;
            Projectile.tileCollide = false;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            if (hitDelay > 0)
                hitDelay--;

            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
            }

            Projectile.rotation += 0.15f + Projectile.velocity.Length() * 0.005f;

            Dust.NewDustPerfect(Projectile.Center - Projectile.velocity + Main.rand.NextVector2Circular(15f, 15f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.Zero, 0, new Color(222, 173, 40), 0.5f);

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
            if (foundTarget && Projectile.timeLeft < 460 && hitDelay <= 0)
            {
                Projectile.velocity = (Projectile.velocity * 25f + Utils.SafeNormalize(targetCenter - Projectile.Center, Vector2.UnitX) * 35f) / 26f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];
            if (Main.rand.NextBool())
                player.Heal(1);

            hitDelay = 15;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 0, new Color(245, 245, 149), 0.55f);

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(2f, 2f), 0, new Color(222, 173, 40), 0.5f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/BloomFlare").Value;
            Texture2D starTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/StarTexture").Value;

            Color yellow = new Color(222, 173, 40, 0);
            Color lightYellow = new Color(245, 245, 149, 0);

            Vector2 drawPos = Projectile.Center - Projectile.velocity - Main.screenPosition;

            Main.spriteBatch.Draw(starTex, drawPos, null, lightYellow * 0.25f, 1.57f + Projectile.rotation, starTex.Size() / 2f, 0.45f, 0f, 0f);

            Main.spriteBatch.Draw(tex, drawPos, null, yellow * 0.55f, -Projectile.rotation * 0.5f, tex.Size() / 2f, 0.4f, 0f, 0f);

            Main.spriteBatch.Draw(tex, drawPos, null, lightYellow * 0.55f, 0.78f + Projectile.rotation, tex.Size() / 2f, 0.4f, 0f, 0f);

            Main.spriteBatch.Draw(starTex, drawPos, null, lightYellow * 0.55f, -Projectile.rotation * 0.5f, starTex.Size() / 2f, 0.55f, 0f, 0f);
            return false;
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
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 15, new TriangularTip(190), factor => factor * 15.5f, factor =>
            {
                return new Color(245, 245, 149) * factor.X;
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

            effect.Parameters["time"].SetValue(Projectile.timeLeft * -0.025f);
            effect.Parameters["repeats"].SetValue(2f);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

            trail?.Render(effect);

            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/FireTrail").Value);

            trail?.Render(effect);
        }
    }
}
