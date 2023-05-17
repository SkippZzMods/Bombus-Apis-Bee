using BombusApisBee.PrimitiveDrawing;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.Graphics.Effects;

namespace BombusApisBee.Projectiles
{
    public class CursedHoneycombThrowout : BeeProjectile
    {
        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;
        public Player owner => Main.player[Projectile.owner];

        public Vector2 randomizedVector = Vector2.Zero;
        public ref float RandomizePositionTimer => ref Projectile.ai[0];
        public ref float AttackDelay => ref Projectile.ai[1];
        public override string Texture => "BombusApisBee/Items/Weapons/BeeKeeperDamageClass/CursedHoneycomb";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Honeycomb");
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            if (owner.channel && !owner.noItems && !owner.CCed && !(owner.Hymenoptra().BeeResourceCurrent > owner.Hymenoptra().BeeResourceReserved))
                Projectile.timeLeft = 2;

            owner.ChangeDir(Projectile.Center.X < owner.Center.X ? -1 : 1);
            owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, owner.Center.DirectionTo(Projectile.Center).ToRotation() - MathHelper.PiOver2);
            owner.Hymenoptra().BeeResourceRegenTimer = -120;

            if (--RandomizePositionTimer <= 0)
            {
                randomizedVector = Main.rand.NextVector2CircularEdge(25f, 25f);
                RandomizePositionTimer = 10;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = (Projectile.velocity * 20f + Utils.SafeNormalize(Main.MouseWorld - Projectile.Center + randomizedVector, Vector2.UnitX) * (Projectile.Distance(Main.MouseWorld) > 100f ? 17f : 12f)) / 21f;
                if (--AttackDelay <= 0)
                {

                    if (owner.UseBeeResource(6))
                    {
                        for (int i = -1; i < 2; i++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.velocity, Projectile.DirectionTo(Main.MouseWorld).RotatedBy(0.3f * i) * 21f, ModContent.ProjectileType<CursedTooth>(), Projectile.damage, 3.5f, Projectile.owner);
                        }
                        for (int i = 0; i < 4; i++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.velocity, Projectile.DirectionTo(Main.MouseWorld).RotatedByRandom(0.35f) * Main.rand.NextFloat(15f, 18f), ModContent.ProjectileType<CursedEye>(), (int)(Projectile.damage * 0.66f), 1.5f, Projectile.owner);
                        }

                        for (int i = 0; i < 15; i++)
                        {
                            Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, DustID.CursedTorch, Projectile.DirectionTo(Main.MouseWorld).RotatedByRandom(0.35f) * Main.rand.NextFloat(10f, 15f), Scale: Main.rand.NextFloat(2f, 2.5f)).noGravity = true;
                            Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, ModContent.DustType<Dusts.GlowFastDecelerate>(), Projectile.DirectionTo(Main.MouseWorld).RotatedByRandom(0.35f) * Main.rand.NextFloat(5f, 10f), newColor: new Color(97, 130, 30), Scale: Main.rand.NextFloat(0.5f, 0.65f));
                        }
                        SoundEngine.PlaySound(SoundID.NPCDeath13, Projectile.position);
                        AttackDelay = 120;
                    }
                    else
                        Projectile.Kill();                
                }
            }

            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);

            for (int i = 1; i < 6; i++)
            {
                Gore.NewGorePerfect(Projectile.GetSource_Death(), Projectile.Center + Main.rand.NextVector2Circular(10f, 10f), Projectile.velocity, Mod.Find<ModGore>("CursedHoneycombGore" + i).Type).timeLeft = 60;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, 320);
            if (Main.rand.NextBool())
                Projectile.NewProjectile(Projectile.GetSource_OnHit(target), Projectile.Center, Vector2.One.RotatedByRandom(6.28f) * 2.5f, ModContent.ProjectileType<CursedBee>(), (int)(Projectile.damage * 0.66f), 0f, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrimitives();
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                Projectile.velocity.X = -oldVelocity.X;

            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                Projectile.velocity.Y = -oldVelocity.Y;
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
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 12, new TriangularTip(190), factor => factor * 19.5f, factor =>
            {
                return new Color(56, 61, 25) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center;

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 12, new TriangularTip(190), factor => factor * 17.5f, factor =>
            {
                return new Color(97, 130, 30) * factor.X;
            });

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = Projectile.Center;
        }

        public void DrawPrimitives()
        {
            Main.spriteBatch.End();
            Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["time"].SetValue(Main.GameUpdateCount * -0.025f);
            effect.Parameters["repeats"].SetValue(2f);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/FireTrail").Value);

            trail?.Render(effect);
            trail2?.Render(effect);

            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/ShadowTrail").Value);

            trail?.Render(effect);
            trail2?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
