using BombusApisBee.BeeDamageClass;
using BombusApisBee.PrimitiveDrawing;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.Graphics.Effects;

namespace BombusApisBee.Projectiles
{
    public class IchorHoneycombHoldout : ModProjectile
    {
        public override string Texture => BombusApisBee.BeeWeapon + "CystComb";
        public Player owner => Main.player[Projectile.owner];
        public int MaxThrowTimer => (int)owner.ApplyHymenoptraSpeedTo(owner.HeldItem.useTime);
        public ref float throwTimer => ref Projectile.ai[1];
        public ref float boomerangTimer => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beemerang");
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 28;

            Projectile.friendly = false;
            Projectile.DamageType = BeeUtils.BeeDamageClass();
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.Bombus().HeldProj = true;
        }

        public float GetPullbackRotation()
        {
            return BeeUtils.PiecewiseAnimation((float)throwTimer / (float)MaxThrowTimer, new BeeUtils.CurveSegment[]
            {
                pullback,
                throwout
            });
        }

        public override void AI()
        {
            if (++throwTimer < MaxThrowTimer)
            {
                if (Main.myPlayer == owner.whoAmI)
                    owner.ChangeDir(Main.MouseWorld.X < owner.Center.X ? -1 : 1);
                float armRot = GetPullbackRotation() * owner.direction;
                owner.heldProj = Projectile.whoAmI;
                Projectile.Center = owner.MountedCenter + Vector2.UnitY.RotatedBy((double)(armRot * owner.gravDir), default) * -20f * owner.gravDir;
                Projectile.rotation = (-MathHelper.PiOver2 + armRot) * owner.gravDir;
                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, 3.1415927f + armRot);
                owner.Hymenoptra().BeeResourceRegenTimer = -60;
            }
            else
            {
                if (Main.myPlayer == owner.whoAmI && boomerangTimer == 0)
                {
                    SoundID.DD2_MonkStaffSwing.PlayWith(Projectile.Center, 0.15f);
                    owner.UseBeeResource(2);
                    Projectile.friendly = true;
                    Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * 25f;
                    Projectile.tileCollide = true;
                }

                boomerangTimer++;
                if (boomerangTimer > 30f)
                {
                    Projectile.tileCollide = false;
                    boomerangTimer = 30f;
                    float speed = 19f;
                    float inertia = 1f;
                    Vector2 pos = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                    float num = owner.position.X + (float)(owner.width / 2) - pos.X;
                    float num2 = owner.position.Y + (float)(owner.height / 2) - pos.Y;
                    float num3 = (float)Math.Sqrt(num * num + num2 * num2);
                    if (num3 > 3000f)
                        Projectile.Kill();
                    num3 = speed / num3;
                    num *= num3;
                    num2 *= num3;
                    if (Projectile.velocity.X < num)
                    {
                        Projectile.velocity.X += inertia;
                        if (Projectile.velocity.X < 0f && num > 0f)
                        {
                            Projectile.velocity.X += inertia;
                        }
                    }
                    else if (Projectile.velocity.X > num)
                    {
                        Projectile.velocity.X -= inertia;
                        if (Projectile.velocity.X > 0f && num < 0f)
                        {
                            Projectile.velocity.X -= inertia;
                        }
                    }
                    if (Projectile.velocity.Y < num2)
                    {
                        Projectile.velocity.Y += inertia;
                        if (Projectile.velocity.Y < 0f && num2 > 0f)
                        {
                            Projectile.velocity.Y += inertia;
                        }
                    }
                    else if (Projectile.velocity.Y > num2)
                    {
                        Projectile.velocity.Y -= inertia;
                        if (Projectile.velocity.Y > 0f && num2 < 0f)
                        {
                            Projectile.velocity.Y -= inertia;
                        }
                    }

                    if (Projectile.Distance(owner.Center) < 30f)
                        Projectile.Kill();
                }
                else
                    Projectile.velocity *= 0.97f;

                if (Projectile.soundDelay == 0)
                {
                    Projectile.soundDelay = 8;
                    SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
                }

                Projectile.rotation += 0.15f + Projectile.velocity.Length() * 0.015f;

                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, owner.Center.DirectionTo(Projectile.Center).ToRotation() - MathHelper.PiOver2);
                owner.ChangeDir(Projectile.Center.X < owner.Center.X ? -1 : 1);

                Dust.NewDustPerfect(Projectile.Center + (Projectile.rotation.ToRotationVector2() * 10f) + Projectile.velocity, ModContent.DustType<Dusts.Glow>(), Vector2.Zero, Scale: 0.5f, newColor: new Color(196, 102, 9));

                Dust.NewDustPerfect(Projectile.Center + (Projectile.rotation.ToRotationVector2() * -10f) + Projectile.velocity, ModContent.DustType<Dusts.Glow>(), Vector2.Zero, Scale: 0.5f, newColor: new Color(253, 152, 0));
            }

            owner.itemTime = 2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            SoundID.NPCDeath19.PlayWith(Projectile.Center);
            if (boomerangTimer < 30f)
            {
                boomerangTimer = 31f;
                Projectile.velocity *= -1f;
            }

            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<IchorExplosion>(), Projectile.damage * 2, 0f, Projectile.owner, 55);

            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center, Main.rand.NextVector2Circular(3f, 3f), ModContent.ProjectileType<IchorBee>(), Projectile.damage / 2, 0f, Projectile.owner);
            }

            for (int i = 0; i < 25; i++)
            {
                Dust.NewDustPerfect(target.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(4f, 4f), 0, new Color(253, 152, 0), 0.85f);

                Dust.NewDustPerfect(target.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(4.5f, 4.5f), 0, new Color(196, 102, 9), 0.7f);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (boomerangTimer < 30f)
            {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
                boomerangTimer = 31f;
                Projectile.velocity *= -1f;
            }
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Vector2 drawOrigin = new Vector2(Projectile.width * 0.5f, Projectile.height * 0.5f);

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(196, 102, 9, 0), 0f, bloomTex.Size() / 2f, 0.55f, 0, 0);

            if (throwTimer >= MaxThrowTimer)
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.oldRot[k], texture.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.5f, k / (float)Projectile.oldPos.Length), SpriteEffects.None, 0);
                }

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(253, 152, 0, 0) * 0.5f, 0f, bloomTex.Size() / 2f, 0.65f, 0, 0);
            return false;
        }

        public BeeUtils.CurveSegment pullback = new BeeUtils.CurveSegment(BeeUtils.EasingType.PolyOut, 0f, 0f, -1.35f, 2);

        public BeeUtils.CurveSegment throwout = new BeeUtils.CurveSegment(BeeUtils.EasingType.CircOut, 0.7f, -0.9424779f, 2.2132742f, 3);
    }
    class IchorExplosion : ModProjectile, IDrawPrimitive_
    {
        private List<Vector2> cache;

        private Trail trail;
        private Trail trail2;
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";
        private float Progress => 1 - (Projectile.timeLeft / 25f);

        private float Radius => Projectile.ai[0] * (float)Math.Sqrt(Math.Sqrt(Progress));

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.DamageType = BeeUtils.BeeDamageClass();
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 25;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ichor Explosion");
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
            }

            for (int k = 0; k < 6; k++)
            {
                float rot = Main.rand.NextFloat(0, 6.28f);

                Dust.NewDustPerfect(Projectile.Center + Vector2.One.RotatedBy(rot) * Radius, ModContent.DustType<Dusts.GlowFastDecelerate>(),
                    Vector2.One.RotatedBy(rot) * 0.5f, 0, Main.rand.NextBool() ? new Color(196, 102, 9) : new Color(253, 152, 0), Main.rand.NextFloat(0.35f, 0.4f));
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 line = targetHitbox.Center.ToVector2() - Projectile.Center;
            line.Normalize();
            line *= Radius;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + line);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 360);
        }

        private void ManageCaches()
        {
            if (cache is null)
            {
                cache = new List<Vector2>();

                for (int i = 0; i < 40; i++)
                {
                    cache.Add(Projectile.Center);
                }
            }

            for (int k = 0; k < 40; k++)
            {
                cache[k] = (Projectile.Center + Vector2.One.RotatedBy(k / 19f * 6.28f) * (Radius));
            }

            while (cache.Count > 40)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 40, new TriangularTip(1), factor => 30 * (1 - Progress), factor =>
            {
                return new Color(196, 102, 9);
            });

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 40, new TriangularTip(1), factor => 25 * (1 - Progress), factor =>
            {
                return new Color(255, 251, 166) * 0.5f;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = cache[39];

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = cache[39];
        }

        public void DrawPrimitives()
        {
            Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["time"].SetValue(Projectile.timeLeft * -0.01f);
            effect.Parameters["repeats"].SetValue(3f);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

            trail?.Render(effect);
            trail2?.Render(effect);

            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/LiquidTrail").Value);

            trail2?.Render(effect);
        }
    }
}
