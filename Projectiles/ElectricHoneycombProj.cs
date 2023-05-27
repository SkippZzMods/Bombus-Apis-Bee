using BombusApisBee.BeeHelperProj;
using BombusApisBee.Buffs;
using BombusApisBee.PrimitiveDrawing;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.Graphics.Effects;

namespace BombusApisBee.Projectiles
{
    public class ElectricHoneycombProj : BeeProjectile
    {
        public override string Texture => BombusApisBee.BeeWeapon + "ElectricHoneycomb";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electric Honeycomh");
        }

        public override void SafeSetDefaults()
        {
            Projectile.friendly = true;
            Projectile.timeLeft = 120;
            Projectile.penetrate = -1;
            Projectile.height = Projectile.width = 28;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.97f;
            Projectile.rotation += Projectile.velocity.Length() * 0.05f;
            Projectile.ai[0] += 0.05f;
            Projectile.ai[1] += 0.1f;
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(1f, 1f) + (Projectile.ai[0].ToRotationVector2() * MathHelper.Lerp(55f, 5f, 1f - Projectile.timeLeft / 120f)), ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.Zero, Scale: 0.4f, newColor: new Color(233, 245, 255));

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(1f, 1f) + (Projectile.ai[0].ToRotationVector2() * -MathHelper.Lerp(55f, 5f, 1f - Projectile.timeLeft / 120f)), ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.Zero, Scale: 0.4f, newColor: new Color(233, 245, 255));

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(1f, 1f) + ((Projectile.ai[1] + MathHelper.ToRadians(180f)).ToRotationVector2() * MathHelper.Lerp(35f, 5f, 1f - Projectile.timeLeft / 120f)), ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.Zero, Scale: 0.4f, newColor: new Color(110, 220, 255));

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(1f, 1f) + ((Projectile.ai[1] + MathHelper.ToRadians(180f)).ToRotationVector2() * -MathHelper.Lerp(35f, 5f, 1f - Projectile.timeLeft / 120f)), ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.Zero, Scale: 0.4f, newColor: new Color(110, 220, 255));
            }

            if (Main.rand.NextBool(60) && Main.player[Projectile.owner].UseBeeResource(1))
            {
                new SoundStyle("BombusApisBee/Sounds/Item/LightningStrike").PlayWith(Projectile.position, 0, 0.1f, 0.85f);
                Main.player[Projectile.owner].Bombus().AddShake(10);
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2CircularEdge(3f, 3f), ModContent.ProjectileType<ElectricHoneycombLightning>(), Projectile.damage * 3 / 4, 0f, Projectile.owner);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(233, 245, 255, 0), 0f, bloomTex.Size() / 2f, 0.6f, 0, 0);
            }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0f, 0f);

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(233, 245, 255, 0), Projectile.rotation, bloomTex.Size() / 2f, 0.65f, 0f, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.player[Projectile.owner].Bombus().AddShake(12);

            if (Main.myPlayer == Projectile.owner)
            {
                for (int i = 0; i < Main.rand.Next(1, 4); i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2Circular(3f, 3f), ModContent.ProjectileType<ElectricBee>(), Projectile.damage * 2 / 3, 0f, Projectile.owner);
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center + new Vector2(Main.rand.Next(-200, 200), -900), Vector2.UnitY * 10f, ModContent.ProjectileType<ElectricHoneycombLightning>(), Projectile.damage * 3 / 4, 5f, Projectile.owner);
                }

                for (int i = 0; i < 2; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2Circular(3f, 3f), ModContent.ProjectileType<ElectricBee>(), Projectile.damage * 2 / 3, 0f, Projectile.owner);
                }
            }

            for (int i = 0; i < 25; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(3f, 3f), 0, new Color(233, 245, 255), 0.85f);

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.Glow>(), Main.rand.NextVector2Circular(3.5f, 3.5f), 0, new Color(110, 220, 255), 0.65f);

                for (int k = 0; k < 4; k++)
                {
                    var vel = Vector2.One.RotatedBy(Main.rand.NextFloat(6.28f)) * Main.rand.NextFloat(2f);
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.Gas>(), vel, 0, new Color(255, 255, 255) * 0.1f, Main.rand.NextFloat(4.5f, 6.5f));

                    vel = Vector2.One.RotatedBy(Main.rand.NextFloat(6.28f)) * Main.rand.NextFloat(2f);
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.Gas>(), vel, 0, new Color(233, 245, 255) * 0.1f, Main.rand.NextFloat(4.5f, 6.5f));
                }
            }

            new SoundStyle("BombusApisBee/Sounds/Item/LightningStrike").PlayWith(Projectile.position, 0, 0.1f, 1f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Electrocuted>(), Main.rand.Next(new int[] { 120, 240, 360 }));
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                Projectile.velocity.X = -oldVelocity.X;

            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }
    }

    class ElectricHoneycombLightning : BeeProjectile, IDrawPrimitive_
    {
        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;
        private bool HasHit;
        private bool collided;
        public override string Texture => BombusApisBee.Invisible;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning");
        }

        public override void SafeSetDefaults()
        {
            Projectile.friendly = true;
            Projectile.timeLeft = 150;
            Projectile.penetrate = -1;
            Projectile.height = Projectile.width = 8;
            Projectile.extraUpdates = 10;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 100;
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 150)
                for (int i = 0; i < 25; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(3f, 3f), 0, new Color(233, 245, 255), 0.55f);

                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(3.5f, 3.5f), 0, new Color(110, 220, 255), 0.45f);
                }

            if (!Main.dedServ && Projectile.timeLeft % 5 == 0)
            {
                if (Projectile.timeLeft % 2.5f == 0)
                    ManageCaches();

                ManageTrail();
            }

            if (Main.rand.NextBool(5) && !collided)
            {
                NPC target = Projectile.FindTargetWithinRange(1000f);
                if (target != null && Collision.CanHitLine(Projectile.Center, 1, 1, target.Center, 1, 1) && !HasHit)
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target.Center).RotatedByRandom(0.55f) * 10f, 0.55f);
                else
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.velocity.RotatedByRandom(0.55f), 0.55f);
            }

            if (Main.rand.NextBool())
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.Glow>(), (-Projectile.velocity * 0.45f).RotatedByRandom(0.45f), 50, new Color(233, 245, 255), 0.6f);
            else
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.Glow>(), (-Projectile.velocity * 0.45f).RotatedByRandom(0.45f), 50, new Color(110, 220, 255), 0.45f);

            if (Projectile.timeLeft == 1 && !collided)
            {
                for (int i = 0; i < 25; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Projectile.oldVelocity.RotatedByRandom(0.45f) * -Main.rand.NextFloat(0.55f), 0, new Color(233, 245, 255), 0.55f);

                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Projectile.oldVelocity.RotatedByRandom(0.45f) * -Main.rand.NextFloat(0.65f), 0, new Color(110, 220, 255), 0.45f);

                    for (int k = 0; k < 2; k++)
                    {
                        var vel = Projectile.oldVelocity.RotatedByRandom(0.45f) * -Main.rand.NextFloat(0.65f);
                        Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.Gas>(), vel, 0, new Color(255, 255, 255) * 0.1f, Main.rand.NextFloat(4.5f, 6.5f));

                        vel = Projectile.oldVelocity.RotatedByRandom(0.45f) * -Main.rand.NextFloat(0.55f);
                        Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.Gas>(), vel, 0, new Color(233, 245, 255) * 0.1f, Main.rand.NextFloat(4.5f, 6.5f));
                    }
                }
                for (int i = 0; i < 30; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(5f, 5f), 0, new Color(233, 245, 255), 0.55f);

                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(6f, 6f), 0, new Color(110, 220, 255), 0.45f);
                }
                collided = true;
                Projectile.timeLeft = 30;
                Projectile.extraUpdates = 0;
                Projectile.velocity = Vector2.Zero;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(233, 245, 255, 0) * MathHelper.Lerp(1f, 0f, 1f - Projectile.timeLeft / 150f), 0f, bloomTex.Size() / 2f, 0.85f, 0, 0);
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            HasHit = true;
            target.AddBuff(ModContent.BuffType<Electrocuted>(), Main.rand.Next(new int[] { 120, 240, 360 }));
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < 25; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), oldVelocity.RotatedByRandom(0.35f) * -Main.rand.NextFloat(0.35f), 0, new Color(233, 245, 255), 0.55f);

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), oldVelocity.RotatedByRandom(0.35f) * -Main.rand.NextFloat(0.45f), 0, new Color(110, 220, 255), 0.45f);

                for (int k = 0; k < 2; k++)
                {
                    var vel = Projectile.oldVelocity.RotatedByRandom(0.45f) * -Main.rand.NextFloat(0.65f);
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.Gas>(), vel, 0, new Color(255, 255, 255) * 0.1f, Main.rand.NextFloat(4.5f, 6.5f));

                    vel = Projectile.oldVelocity.RotatedByRandom(0.45f) * -Main.rand.NextFloat(0.55f);
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.Gas>(), vel, 0, new Color(233, 245, 255) * 0.1f, Main.rand.NextFloat(4.5f, 6.5f));
                }
            }
            for (int i = 0; i < 30; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(5f, 5f), 0, new Color(233, 245, 255), 0.55f);

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(6f, 6f), 0, new Color(110, 220, 255), 0.45f);
            }
            collided = true;
            Projectile.timeLeft = 30;
            Projectile.extraUpdates = 0;
            Projectile.velocity = Vector2.Zero;
            return false;
        }

        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < 50; i++)
                {
                    cache.Add(Projectile.Center);
                }
            }

            cache.Add(Projectile.Center);

            while (cache.Count > 50)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 50, new TriangularTip(190), factor => factor * 11.5f, factor =>
            {
                if (Projectile.timeLeft <= 45)
                    return new Color(233, 245, 255) * MathHelper.Lerp(1f, 0f, 1f - Projectile.timeLeft / 45f) * factor.X;

                return new Color(233, 245, 255) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center;

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 50, new TriangularTip(190), factor => factor * 22.5f, factor =>
            {
                if (Projectile.timeLeft <= 45)
                    return new Color(110, 220, 255) * MathHelper.Lerp(1f, 0f, 1f - Projectile.timeLeft / 45f) * factor.X;

                return new Color(110, 220, 255) * factor.X;
            });

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = Projectile.Center;
        }

        public void DrawPrimitives()
        {
            Effect effect = Filters.Scene["LightningTrail"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["time"].SetValue(Projectile.timeLeft * -0.025f);
            effect.Parameters["repeats"].SetValue(2f);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

            trail2?.Render(effect);
            trail?.Render(effect);
        }
    }
    public class ElectricBee : BeeHelper
    {
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electric Bee");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SafeOnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Electrocuted>(), Main.rand.Next(new int[] { 60, 120, 180 }));
        }
        public override void SafeAI()
        {
            if (Main.rand.NextBool(10))
                for (int i = 0; i < 15; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center + (Giant ? Main.rand.NextVector2CircularEdge(20.5f, 20.5f) : Main.rand.NextVector2CircularEdge(15.5f, 15.5f)), ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.Zero, 0, new Color(110, 220, 255), 0.2f);
                }
        }
    }
}
