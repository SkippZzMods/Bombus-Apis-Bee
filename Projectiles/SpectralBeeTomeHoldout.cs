using BombusApisBee.BeeDamageClass;
using BombusApisBee.PrimitiveDrawing;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace BombusApisBee.Projectiles
{
    public class SpectralBeeTomeHoldout : BeeProjectile
    {
        public Player Owner
        {
            get
            {
                return Main.player[Projectile.owner];
            }
        }
        public ref float Delay
        {
            get
            {
                return ref Projectile.ai[0];
            }
        }
        public override string Texture
        {
            get
            {
                return "BombusApisBee/Items/Weapons/BeeKeeperDamageClass/SpectralBeeTome";
            }
        }

        public int DelayTillNextShot;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spectral Bee Tome");
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.Bombus().HeldProj = true;
        }
        public override void AI()
        {
            Projectile.Center = Owner.Center + Vector2.UnitX * Owner.direction * 14f;
            if (++Projectile.frameCounter >= 10)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
            if (!Owner.channel || Owner.noItems || Owner.CCed)
            {
                Projectile.Kill();
                return;
            }

            if (DelayTillNextShot == 0)
                DelayTillNextShot = (int)Owner.ApplyHymenoptraSpeedTo(Owner.HeldItem.useAnimation);

            Delay += 1f;
            if (Delay > DelayTillNextShot)
            {
                ShootBees();
                Delay = 0;
            }

            AdjustPlayerValues();
            Projectile.timeLeft = 2;
            Owner.GetModPlayer<BeeDamagePlayer>().BeeResourceRegenTimer = -60;

        }
        public void ShootBees()
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            if (Owner.GetModPlayer<BeeDamagePlayer>().BeeResourceCurrent == 0)
            {
                Projectile.Kill();
                return;
            }

            SoundEngine.PlaySound(SoundID.NPCDeath52, Projectile.Center);
            for (int i = 0; i < 2; i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, -Vector2.UnitY.RotatedByRandom(0.35f) * Main.rand.NextFloat(5f, 10f), ModContent.ProjectileType<SpectralBee>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, -Vector2.UnitY.RotatedByRandom(0.45f) * Main.rand.NextFloat(10f, 15f), ModContent.ProjectileType<SpectralSoul>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

            for (float k = 0; k < 6.28f; k += 0.1f)
            {
                float x = (float)Math.Cos(k) * 50;
                float y = (float)Math.Sin(k) * 25;

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), new Vector2(x, y) * 0.035f, 0, new Color(6, 106, 255), 0.4f);
            }

            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(2f, 2f), 0, new Color(6, 106, 255), 0.45f);
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), -Vector2.UnitY.RotatedByRandom(0.35f) * Main.rand.NextFloat(2.5f, 5f), 0, new Color(130, 153, 208), 0.35f);
            }

            Projectile.netUpdate = true;
            Owner.UseBeeResource(3);
        }

        public void AdjustPlayerValues()
        {
            if (Main.myPlayer == Projectile.owner)
                Owner.ChangeDir(Main.MouseWorld.X < Owner.Center.X ? -1 : 1);

            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = Utils.ToRotation((float)Projectile.direction * Projectile.velocity);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D bookTexture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Rectangle frame = bookTexture.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.Draw(bloomTex, drawPosition, null, new Color(2, 41, 69, 0) * 0.75f, 0f, bloomTex.Size() / 2f, 0.75f, 0f, 0f);

            Main.spriteBatch.Draw(bookTexture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, Owner.direction == -1 ? SpriteEffects.FlipHorizontally : 0, 0f);

            Main.spriteBatch.Draw(bloomTex, drawPosition, null, new Color(6, 106, 255, 0), 0f, bloomTex.Size() / 2f, 0.5f, 0f, 0f);
            return false;
        }

        public override bool? CanDamage() => false;
    }

    class SpectralSoul : BeeProjectile, IDrawPrimitive_
    {
        public float opacity => Projectile.timeLeft / 240f;

        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;

        public override string Texture => BombusApisBee.Invisible;

        public override bool? CanDamage() => Projectile.penetrate > 1;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spectral Soul");
        }

        public override void SafeSetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 16;

            Projectile.timeLeft = 240;
            Projectile.penetrate = 3;
            Projectile.tileCollide = false;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
            }

            if (Main.rand.NextBool())
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(1.5f, 1.5f), 0, (Main.rand.NextBool() ? new Color(149, 238, 238) : new Color(209, 246, 239)) * opacity, 0.45f);

            if (Projectile.penetrate <= 1)
            {
                Projectile.velocity = (Projectile.velocity * 25f + Utils.SafeNormalize(Main.player[Projectile.owner].Center - Projectile.Center, Vector2.UnitX) * 25f) / 26f;
                if (Projectile.Hitbox.Intersects(Main.player[Projectile.owner].Hitbox))
                {
                    Main.player[Projectile.owner].Heal(Main.rand.Next(1, 3));
                    Projectile.Kill();
                }
                return;
            }
            if (Main.myPlayer == Projectile.owner)
            {
                if (Projectile.timeLeft < 220 && Projectile.Distance(Main.MouseWorld) > 50f)
                    Projectile.velocity = (Projectile.velocity * 25f + Utils.SafeNormalize(Main.MouseWorld - Projectile.Center, Vector2.UnitX) * 25f) / 26f;
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(1.5f, 1.5f), 0, new Color(149, 238, 238) * 0.5f, 0.55f);

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(1f, 1f), 0, new Color(209, 246, 239) * 0.5f, 0.5f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Vector2 drawPos = Projectile.Center - Projectile.velocity - Main.screenPosition;

            Main.spriteBatch.Draw(glowTex, drawPos, null, new Color(149, 238, 238, 0) * opacity, 0f, glowTex.Size() / 2f, 0.45f, 0f, 0f);

            Main.spriteBatch.Draw(glowTex, drawPos, null, new Color(209, 246, 239, 0) * opacity, 0f, glowTex.Size() / 2f, 0.35f, 0f, 0f);
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
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 15, new TriangularTip(120), factor => factor * 15.5f, factor =>
            {
                return new Color(149, 238, 238) * factor.X * opacity;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center;

            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 15, new TriangularTip(120), factor => factor * 12.5f, factor =>
            {
                return new Color(209, 246, 239) * factor.X * opacity;
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
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/FireTrail").Value);
            
            trail?.Render(effect);
            trail2?.Render(effect);

            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/EnergyTrail").Value);
            effect.Parameters["time"].SetValue(Projectile.timeLeft * -0.015f);
            trail?.Render(effect);
            trail2?.Render(effect);

            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/ShadowTrail").Value);
            effect.Parameters["time"].SetValue(Projectile.timeLeft * 0.025f);
            trail2?.Render(effect);
        }
    }
}
