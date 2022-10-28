using BombusApisBee.Dusts;
using BombusApisBee.PrimitiveDrawing;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BombusApisBee.Projectiles
{
    public class BeezookaRocket : BeeProjectile
    {
        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;

        public bool exploding;
        public int explodingTimer;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bee Rocket");
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;

            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 360;
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
            }
            if (exploding)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

                Projectile.velocity.Y += 0.1f;
                if (Projectile.velocity.Y > 0)
                {
                    if (Projectile.velocity.Y < 13f)
                        Projectile.velocity.Y *= 1.035f;
                    else
                        Projectile.velocity.Y *= 1.005f;
                }
                if (Projectile.velocity.Y > 16f)
                    Projectile.velocity.Y = 16f;

                Projectile.velocity *= 0.95f;

                if (++explodingTimer >= 45)
                    Projectile.Kill();

                float lerper = MathHelper.Lerp(50f, 5f, explodingTimer / 45f);
                Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(lerper, lerper);
                Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.GlowFastDecelerate>(), pos.DirectionTo(Projectile.Center) * 2.5f, 0, new Color(214, 158, 79), 0.35f);
                pos = Projectile.Center + Main.rand.NextVector2CircularEdge(lerper, lerper);
                Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.GlowFastDecelerate>(), pos.DirectionTo(Projectile.Center) * 2.5f, 0, new Color(255, 218, 110), 0.35f);
            }
            else
            {
                Vector2 targetCenter = Projectile.Center;
                bool foundTarget = false;
                float num = 1500f;
                for (int i = 0; i < 200; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy())
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
                    Projectile.velocity = (Projectile.velocity * 30f + Utils.SafeNormalize(targetCenter - Projectile.Center, Vector2.UnitX) * 25f) / 31f;

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (!exploding)
            {
                for (int i = 0; i < Main.rand.Next(3, 6); i++)
                {
                    Projectile.SpawnBee(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.One.RotatedBy(Projectile.velocity.ToRotation() - MathHelper.PiOver4).RotatedByRandom(0.4f) * 6f, Projectile.damage, 0f);
                }

                for (int i = 0; i < 15; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Projectile.velocity.RotatedByRandom(0.45f) * Main.rand.NextFloat(), 0, new Color(214, 158, 79), 0.45f);

                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.HoneyDustSolid>(), Projectile.velocity.RotatedByRandom(0.65f) * Main.rand.NextFloat(0.25f), 50 + Main.rand.Next(100), default, 1.45f).noGravity = true;
                }

                SoundID.NPCHit4.PlayWith(Projectile.Center, -0.1f, 0.2f);
                Gore.NewGorePerfect(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * 0.5f, Mod.Find<ModGore>("BeezookaRocket_Gore1").Type);
                exploding = true;
                Projectile.velocity *= -1;
                Projectile.friendly = false;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!exploding)
            {
                for (int i = 0; i < Main.rand.Next(3, 6); i++)
                {
                    Projectile.SpawnBee(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.One.RotatedBy(Projectile.velocity.ToRotation() - MathHelper.PiOver4).RotatedByRandom(0.4f) * 6f, Projectile.damage, 0f);
                }

                for (int i = 0; i < 15; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Projectile.velocity.RotatedByRandom(0.45f) * Main.rand.NextFloat(), 0, new Color(214, 158, 79), 0.45f);

                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.HoneyDustSolid>(), Projectile.velocity.RotatedByRandom(0.65f) * Main.rand.NextFloat(0.25f), 50 + Main.rand.Next(100), default, 1.45f).noGravity = true;
                }

                SoundID.NPCHit4.PlayWith(Projectile.Center, -0.1f, 0.2f);
                Gore.NewGorePerfect(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * 0.5f, Mod.Find<ModGore>("BeezookaRocket_Gore1").Type);
                exploding = true;
                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                    Projectile.velocity.X = -oldVelocity.X;

                if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                    Projectile.velocity.Y = -oldVelocity.Y;
                Projectile.friendly = false;
            }
            else
            {
                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                    Projectile.velocity.X = -oldVelocity.X;

                if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                    Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }
            
        public override void Kill(int timeLeft)
        {       
            if (exploding)
            {
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.ExplosionDust>(), Main.rand.NextVector2Circular(5f, 5f), 50 + Main.rand.Next(100), default, 0.9f).rotation = Main.rand.NextFloat(6.28f);

                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.ExplosionDustTwo>(), Main.rand.NextVector2Circular(6f, 6f), 80 + Main.rand.Next(100), default, 0.6f).rotation = Main.rand.NextFloat(6.28f);

                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.ExplosionDustThree>(), Main.rand.NextVector2Circular(6f, 6f), 80 + Main.rand.Next(100), default, 0.9f).rotation = Main.rand.NextFloat(6.28f);

                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.ExplosionDustFour>(), Main.rand.NextVector2Circular(5f, 5f), 50 + Main.rand.Next(100), default, 0.6f).rotation = Main.rand.NextFloat(6.28f);

                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(8f, 8f), 0, new Color(214, 158, 79), 0.55f);

                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(8f, 8f), 0, new Color(255, 218, 110), 0.45f);

                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2().RotatedByRandom(0.45f) * Main.rand.NextFloat(10f, 20f), 0, new Color(214, 158, 79), 0.45f);
                }

                new SoundStyle("BombusApisBee/Sounds/Item/FireHit").PlayWith(Projectile.Center, -0.1f, 0.2f, 1.1f);

                Main.player[Projectile.owner].Bombus().shakeTimer += 4;

                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BasicExplosion>(), Projectile.damage, 2f, Projectile.owner);

                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2().RotatedByRandom(0.45f) * Main.rand.NextFloat(10f, 20f), ModContent.ProjectileType<BeezookaRocketSmall>(), (int)(Projectile.damage * 0.65f), 3f, Projectile.owner);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrimitives();
            string alt = exploding ? "Alt" : "";
            Texture2D tex = ModContent.Request<Texture2D>(Texture + alt).Value;
            Texture2D texGlow = ModContent.Request<Texture2D>(Texture + alt + "_Glow").Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, new Color(214, 158, 79, 0), Projectile.rotation, texGlow.Size() / 2f, Projectile.scale, 0f, 0f);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0f, 0f);

            if (exploding)
            {
                Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.Transparent, new Color(214, 158, 79, 0), explodingTimer / 45f), Projectile.rotation, texGlow.Size() / 2f, Projectile.scale, 0f, 0f);

                Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.Transparent, new Color(214, 158, 79, 0), explodingTimer / 45f), 0f, bloomTex.Size() / 2f, 0.6f, 0, 0);
            }
                
            return false;
        }
        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < 18; i++)
                {
                    cache.Add(Projectile.Center + Projectile.velocity);
                }
            }

            cache.Add(Projectile.Center + Projectile.velocity);

            while (cache.Count > 18)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 18, new TriangularTip(240), factor => 4f, factor =>
            {
                if (exploding)
                    return new Color(214, 158, 79) * factor.X * (1f - (explodingTimer / 45f));

                return new Color(214, 158, 79) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;

            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 18, new TriangularTip(240), factor => 1f, factor =>
            {
                if (exploding)
                    return Color.White * factor.X * (1f - (explodingTimer / 45f));

                return Color.White * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;
        }

        public void DrawPrimitives()
        {
            Main.spriteBatch.End();
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
            effect.Parameters["repeats"].SetValue(2f);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

            trail?.Render(effect);
            trail2?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
        }
    }

    public class BeezookaRocketSmall : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mini-Bee Rocket");
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;

            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                NPC target = Main.npc.Where(n => n.CanBeChasedBy() && Main.MouseWorld.Distance(n.Center) < 450f).OrderBy(n => Vector2.Distance(n.Center, Main.MouseWorld)).FirstOrDefault();
                if (target != default)
                    Projectile.velocity = (Projectile.velocity * 22f + Utils.SafeNormalize(target.Center - Projectile.Center, Vector2.UnitX) * 30f) / 23f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowFastDecelerate>(), Vector2.Zero, 0, new Color(214, 158, 79), 0.3f);
        }

        public override void Kill(int timeLeft)
        {
            new SoundStyle("BombusApisBee/Sounds/Item/LightGunshot").PlayWith(Projectile.Center, pitchVariance: 0.1f);

            new SoundStyle("BombusApisBee/Sounds/Item/ScrapExplode").PlayWith(Projectile.Center, 0.4f, 0.35f);

            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.75f, 2f), ModContent.ProjectileType<BeezookaShrapnel>(), (int)(Projectile.damage * 0.35f), 3.5f, Projectile.owner);
            }

            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Projectile.velocity.RotatedByRandom(0.45f) * Main.rand.NextFloat(), 0, new Color(214, 158, 79), 0.35f);

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 0, new Color(214, 158, 79), 0.35f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texGlow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;

            Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, new Color(214, 158, 79, 0), Projectile.rotation, texGlow.Size() / 2f, Projectile.scale, 0f, 0f);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0f, 0f);

            return false;
        }
    }

    public class BeezookaShrapnel : BeeProjectile, IDrawPrimitive_
    {
        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";

        public override void SafeSetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 100;
            Projectile.extraUpdates = 4;
            Projectile.alpha = 255;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beezooka Shrapnel");
        }

        public override void AI()
        {
            Projectile.velocity *= 0.965f;

            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
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
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 12, new TriangularTip(175), factor => factor * 6.5f, factor =>
            {
                return new Color(214, 158, 79) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center;

            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 12, new TriangularTip(175), factor => factor * 3.5f, factor =>
            {
                return Color.White * factor.X;
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

            effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
            effect.Parameters["repeats"].SetValue(2f);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

            trail?.Render(effect);
            trail2?.Render(effect);
        }
    }
}
