namespace BombusApisBee.Projectiles
{
    public class NectarSlasherHoldout : BeeProjectile
    {
        private List<Vector2> cache;
        private List<Vector2> cache2;
        private Trail trail;
        private Trail trail2;

        private List<Vector2> throwcache;
        private List<Vector2> throwcache2;
        private Trail throwtrail;
        private Trail throwtrail2;
        public override string Texture => BombusApisBee.BeeWeapon + "NectarSlasher";
        private Vector2 direction;
        private bool initialized;
        private float maxTimeLeft;

        private float throwTimer;
        private float maxThrowTimer = 20;
        private float boomerangTimer;
        public float SwingDirection => Projectile.ai[0] * Math.Sign(direction.X);
        public float Combo => Projectile.ai[1];
        public Player owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nectar Slasher");
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SafeSetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.Size = new Vector2(40);
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.Bombus().HeldProj = true;
        }

        internal float ThrowRatio()
        {
            return BeeUtils.PiecewiseAnimation(1f - throwTimer / maxThrowTimer, new BeeUtils.CurveSegment[]
            {
                pullback,
                throwout
            });
        }

        internal float SwingRatio()
        {
            return BeeUtils.PiecewiseAnimation(1f - Projectile.timeLeft / maxTimeLeft, new BeeUtils.CurveSegment[]
            {
                swing,
            });
        }

        public override void AI()
        {
            owner.Hymenoptra().BeeResourceRegenTimer = -60;
            if (Combo >= 2)
            {
                owner.itemTime = 2;
                Projectile.timeLeft = 2;
                if (++throwTimer < maxThrowTimer)
                {
                    if (Main.myPlayer == owner.whoAmI)
                        owner.ChangeDir(Main.MouseWorld.X < owner.Center.X ? -1 : 1);

                    float armRot = MathHelper.Lerp(-2f, 1f, ThrowRatio()) * owner.direction;
                    Projectile.friendly = false;
                    owner.heldProj = Projectile.whoAmI;
                    Projectile.Center = owner.MountedCenter + Vector2.UnitY.RotatedBy((double)(armRot * owner.gravDir), default) * -40f * owner.gravDir;
                    Projectile.rotation = (-MathHelper.PiOver4 + armRot) * owner.gravDir + (owner.direction == -1 ? MathHelper.PiOver2 : 0);
                    owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, 3.14f + armRot);

                    Vector2 swordLength = Vector2.Lerp(owner.Center.RotatedBy(Projectile.rotation) * 15f, owner.Center.RotatedBy(Projectile.rotation) * 50f, Main.rand.NextFloat());
                    Dust.NewDustPerfect(swordLength, ModContent.DustType<Dusts.GlowFastDecelerate>(), null, 0, new Color(214, 158, 79, 50), 0.4f);
                }
                else
                {
                    if (Main.myPlayer == owner.whoAmI && boomerangTimer == 0)
                    {
                        owner.UseBeeResource(3);
                        Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * 20f;
                        Projectile.tileCollide = true;
                        Projectile.friendly = true;
                        SoundID.DD2_MonkStaffSwing.PlayWith(Projectile.Center);
                    }

                    if (++boomerangTimer > 30f)
                    {
                        Projectile.tileCollide = false;

                        Vector2 playerCenter = owner.Center;
                        Vector2 pos = Projectile.Center;

                        float betweenX = playerCenter.X - pos.X;
                        float betweenY = playerCenter.Y - pos.Y;

                        float distance = (float)Math.Sqrt(betweenX * betweenX + betweenY * betweenY);
                        float speed = 17;
                        float adjust = 1f;

                        distance = speed / distance;
                        betweenX *= distance;
                        betweenY *= distance;

                        if (Projectile.velocity.X < betweenX)
                        {
                            Projectile.velocity.X += adjust;
                            if (Projectile.velocity.X < 0f && betweenX > 0f)
                                Projectile.velocity.X += adjust;
                        }
                        else if (Projectile.velocity.X > betweenX)
                        {
                            Projectile.velocity.X -= adjust;
                            if (Projectile.velocity.X > 0f && betweenX < 0f)
                                Projectile.velocity.X -= adjust;
                        }
                        if (Projectile.velocity.Y < betweenY)
                        {
                            Projectile.velocity.Y += adjust;
                            if (Projectile.velocity.Y < 0f && betweenY > 0f)
                                Projectile.velocity.Y += adjust;
                        }
                        else if (Projectile.velocity.Y > betweenY)
                        {
                            Projectile.velocity.Y -= adjust;
                            if (Projectile.velocity.Y > 0f && betweenY < 0f)
                                Projectile.velocity.Y -= adjust;
                        }
                        if (Projectile.Distance(owner.Center) < 25f)
                            Projectile.Kill();
                    }

                    if (!Main.dedServ)
                    {
                        ManageCaches();
                        ManageTrail();
                    }

                    Projectile.rotation += 0.1f + Projectile.velocity.Length() * 0.02f;

                    owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, owner.DirectionTo(Projectile.Center).ToRotation() - 1.57f);

                    owner.ChangeDir(Projectile.Center.X < owner.Center.X ? -1 : 1);

                    Dust.NewDustPerfect(Projectile.Center + (Projectile.rotation.ToRotationVector2() * 20f) + Projectile.velocity, ModContent.DustType<Dusts.Glow>(), Vector2.Zero, Scale: 0.45f, newColor: new Color(214, 158, 79, 50));

                    Dust.NewDustPerfect(Projectile.Center + (Projectile.rotation.ToRotationVector2() * -20f) + Projectile.velocity, ModContent.DustType<Dusts.Glow>(), Vector2.Zero, Scale: 0.45f, newColor: new Color(214, 158, 79, 50));
                }
                return;
            }

            if (!initialized)
            {
                initialized = true;
                Projectile.timeLeft = (int)owner.ApplyHymenoptraSpeedTo(owner.HeldItem.useAnimation);
                maxTimeLeft = Projectile.timeLeft;
                direction = Projectile.velocity;
                direction.Normalize();
                Projectile.rotation = Utils.ToRotation(direction);
                Projectile.netUpdate = true;
                SoundID.DD2_MonkStaffSwing.PlayWith(Projectile.Center);
                Projectile.direction = Main.MouseWorld.X < owner.Center.X ? -1 : 1;
            }

            if (!Main.dedServ && Projectile.timeLeft < maxTimeLeft)
            {
                ManageCaches();
                ManageTrail();
            }

            Projectile.Center = owner.Center + Projectile.rotation.ToRotationVector2() * 25f;

            float rot = direction.X < 0 ? MathHelper.PiOver2 : 0f;
            if (SwingDirection == 1)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Lerp(2.5f, -1f, SwingRatio()) - rot;
            else if (SwingDirection == -1)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Lerp(-1f, 2.5f, SwingRatio()) - rot;

            owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(owner.direction == -1 ? 90f : 70f));

            Projectile.scale = 1f + (float)Math.Sin(SwingRatio() * 3.1415927f) * 0.4f * 0.4f;

            owner.heldProj = Projectile.whoAmI;

            owner.ChangeDir(Projectile.direction);

            if (Main.rand.NextBool())
            {
                Vector2 swordLength = Vector2.Lerp(owner.Center + Projectile.rotation.ToRotationVector2() * (35f * Projectile.scale), owner.Center + Projectile.rotation.ToRotationVector2() * (50f * Projectile.scale), Main.rand.NextFloat());
                Dust.NewDustPerfect(swordLength, ModContent.DustType<Dusts.GlowFastDecelerate>(), null, 0, new Color(214, 158, 79, 50), 0.35f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            owner.Bombus().AddShake(3);

            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(3.5f, 3.5f), 0, new Color(255, 191, 73), 0.3f);
            }

            Vector2 pos = target.Center + Main.rand.NextVector2CircularEdge(150f, 150f);
            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), pos, pos.DirectionTo(target.Center) * 20f, ModContent.ProjectileType<Nectarslash>(), Projectile.damage, 2f, Projectile.owner);

            if (Combo < 2)
                owner.Heal(2);
            else if (Main.rand.NextBool())
                owner.Heal(1);
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

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Combo >= 2)
                return null;

            float collisionPoint = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), owner.Center, owner.Center + Projectile.rotation.ToRotationVector2() * (50f * Projectile.scale), 20, ref collisionPoint))
                return true;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrimitives();
            Texture2D sword = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D swordGlow = ModContent.Request<Texture2D>(Texture + "_Glowy").Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            SpriteEffects flip = owner.direction == -1 ? SpriteEffects.FlipHorizontally : 0;
            float rotation = Projectile.rotation + MathHelper.PiOver4 + (owner.direction == -1 ? MathHelper.PiOver2 : 0f);
            Vector2 drawPos = owner.Center + Projectile.rotation.ToRotationVector2() * 35f - Main.screenPosition;
            if (Combo != 2)
            {
                Main.spriteBatch.Draw(sword, drawPos, null, lightColor, rotation, sword.Size() / 2f, Projectile.scale, flip, 0f);

                Main.spriteBatch.Draw(swordGlow, drawPos, null, new Color(214, 158, 79, 0) * (float)Math.Sin(1f - Projectile.timeLeft / maxTimeLeft), rotation, swordGlow.Size() / 2f, Projectile.scale, flip, 0f);
            }
            else
            {
                if (throwTimer >= maxThrowTimer)
                    flip = 0;
                Color color = Color.Lerp(Color.Transparent, new Color(214, 158, 79, 0), throwTimer / maxThrowTimer);
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Color color2 = color * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(swordGlow, Projectile.oldPos[k] - Main.screenPosition + (Projectile.Size / 2f), null, color2, Projectile.oldRot[k], sword.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.5f, k / (float)Projectile.oldPos.Length), flip, 0);
                }

                Main.spriteBatch.Draw(sword, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, sword.Size() / 2f, Projectile.scale, flip, 0f);

                Main.spriteBatch.Draw(swordGlow, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, swordGlow.Size() / 2f, Projectile.scale, flip, 0f);

                if (throwTimer >= maxThrowTimer)
                    for (int i = 0; i < 2; i++)
                    {
                        Main.spriteBatch.Draw(bloomTex, Projectile.Center + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 20f - Main.screenPosition, null, new Color(214, 158, 79, 0), 0f, bloomTex.Size() / 2f, 0.55f, 0, 0);

                        Main.spriteBatch.Draw(bloomTex, Projectile.Center - (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 20f - Main.screenPosition, null, new Color(214, 158, 79, 0), 0f, bloomTex.Size() / 2f, 0.55f, 0, 0);
                    }
            }

            return false;
        }

        public BeeUtils.CurveSegment swing => new BeeUtils.CurveSegment(BeeUtils.EasingType.SineOut, 0f, 0f, 1.25f, 3);


        public BeeUtils.CurveSegment pullback = new BeeUtils.CurveSegment(BeeUtils.EasingType.PolyOut, 0f, 1f, -1.05f, 2);

        public BeeUtils.CurveSegment throwout = new BeeUtils.CurveSegment(BeeUtils.EasingType.PolyOut, 1f, 0f, 2.2132742f, 2);

        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < 20; i++)
                {
                    cache.Add(owner.Center + (Projectile.rotation - 0.25f * SwingDirection).ToRotationVector2() * (40f * Projectile.scale));
                }
            }

            cache.Add(owner.Center + (Projectile.rotation - 0.25f * SwingDirection).ToRotationVector2() * (40f * Projectile.scale));

            while (cache.Count > 20)
            {
                cache.RemoveAt(0);
            }

            if (cache2 == null)
            {
                cache2 = new List<Vector2>();
                for (int i = 0; i < 15; i++)
                {
                    cache2.Add(owner.Center + (Projectile.rotation - 0.25f * SwingDirection).ToRotationVector2() * (30f * Projectile.scale));
                }
            }

            cache2.Add(owner.Center + (Projectile.rotation - 0.25f * SwingDirection).ToRotationVector2() * (30f * Projectile.scale));

            while (cache2.Count > 15)
            {
                cache2.RemoveAt(0);
            }

            if (throwcache == null)
            {
                throwcache = new List<Vector2>();
                for (int i = 0; i < 20; i++)
                {
                    throwcache.Add(Projectile.Center + Projectile.velocity + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 20f);
                }
            }

            throwcache.Add(Projectile.Center + Projectile.velocity + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 20f);

            while (throwcache.Count > 20)
            {
                throwcache.RemoveAt(0);
            }

            if (throwcache2 == null)
            {
                throwcache2 = new List<Vector2>();
                for (int i = 0; i < 20; i++)
                {
                    throwcache2.Add(Projectile.Center + Projectile.velocity - (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 20f);
                }
            }

            throwcache2.Add(Projectile.Center + Projectile.velocity - (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 20f);

            while (throwcache2.Count > 20)
            {
                throwcache2.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 20, new TriangularTip(190), factor => (40f * MathHelper.Lerp(0f, 1f, 1f - Projectile.timeLeft / maxTimeLeft)) * factor * factor * factor, factor =>
            {
                if (Projectile.timeLeft <= 5)
                    return new Color(214, 158, 79) * MathHelper.Lerp(0.5f, 0, 1f - Projectile.timeLeft / 5f);

                return new Color(214, 158, 79) * MathHelper.Lerp(0, 0.5f, 1f - Projectile.timeLeft / maxTimeLeft) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = owner.Center + (Projectile.rotation - 0.25f * SwingDirection).ToRotationVector2() * (40f * Projectile.scale);

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 15, new TriangularTip(190), factor => (50f * MathHelper.Lerp(0f, 1f, 1f - Projectile.timeLeft / maxTimeLeft)) * factor * factor * factor, factor =>
            {
                if (Projectile.timeLeft <= 5)
                    return new Color(214, 158, 79) * MathHelper.Lerp(0.5f, 0, 1f - Projectile.timeLeft / 5f);

                return new Color(214, 158, 79) * MathHelper.Lerp(0, 0.5f, 1f - Projectile.timeLeft / maxTimeLeft) * factor.X;
            });

            trail2.Positions = cache2.ToArray();
            trail2.NextPosition = owner.Center + (Projectile.rotation - 0.25f * SwingDirection).ToRotationVector2() * (30f * Projectile.scale);


            throwtrail = throwtrail ?? new Trail(Main.instance.GraphicsDevice, 20, new TriangularTip(190), factor => factor * 9f, factor =>
            {
                return new Color(214, 158, 79, 150);
            });

            throwtrail.Positions = throwcache.ToArray();
            throwtrail.NextPosition = Projectile.Center + Projectile.velocity + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 20f;

            throwtrail2 = throwtrail2 ?? new Trail(Main.instance.GraphicsDevice, 20, new TriangularTip(190), factor => factor * 9f, factor =>
            {
                return new Color(214, 158, 79, 150);
            });

            throwtrail2.Positions = throwcache2.ToArray();
            throwtrail2.NextPosition = Projectile.Center + Projectile.velocity - (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 20f;
        }

        public void DrawPrimitives()
        {
            Main.spriteBatch.End();
            Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["time"].SetValue(Main.GameUpdateCount * -0.025f);
            effect.Parameters["repeats"].SetValue(1f);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

            if (Combo >= 2)
            {
                throwtrail?.Render(effect);
                throwtrail2?.Render(effect);
            }
            else
            {
                trail?.Render(effect);
                trail2?.Render(effect);
            }

            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/FireTrail").Value);

            if (Combo >= 2)
            {
                throwtrail?.Render(effect);
                throwtrail2?.Render(effect);
            }
            else
            {
                trail?.Render(effect);
                trail2?.Render(effect);
            }

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
        }
    }

    class Nectarslash : BeeProjectile
    {
        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;

        public Vector2 startPos;
        public override string Texture => BombusApisBee.Invisible;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nectarslash");
        }

        public override void SafeSetDefaults()
        {
            Projectile.DamageType = BeeUtils.BeeDamageClass();

            Projectile.timeLeft = 30;
            Projectile.extraUpdates = 1;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;

            Projectile.width = Projectile.height = 24;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 30)
                startPos = Projectile.Center;

            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.player[Projectile.owner].Heal(1);

            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(3.5f, 3.5f), 0, new Color(255, 191, 73), 0.3f);
            }
        }

        public override void PostDraw(Color lightColor)
        {
            DrawPrimitives();
            Texture2D bloom = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Texture2D star = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/StarTexture").Value;
            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw(bloom, (Projectile.Center - Projectile.velocity) - Main.screenPosition, null, new Color(214, 158, 79, 0) * MathHelper.Lerp(1f, 0f, (1f - Projectile.timeLeft / 30f)), Projectile.rotation, bloom.Size() / 2f, 0.35f, 0f, 0f);
            }
            Main.spriteBatch.Draw(star, (Projectile.Center - Projectile.velocity) - Main.screenPosition, null, new Color(214, 158, 79, 0) * MathHelper.Lerp(1f, 0f, (1f - Projectile.timeLeft / 30f)), Projectile.rotation, star.Size() / 2f, 0.5f, 0f, 0f);
        }

        private void ManageCaches()
        {
            cache = new List<Vector2>();
            for (int i = 0; i < 13; i++)
            {
                cache.Add(Vector2.Lerp(startPos, Projectile.Center, i / 13f));
            }
            cache.Add(Projectile.Center);
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 14, new TriangularTip(1f), factor => 15f * (float)Math.Sin(factor), factor =>
            {
                return new Color(214, 158, 79, 150) * MathHelper.Lerp(1f, 0f, (1f - Projectile.timeLeft / 30f));
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

            effect.Parameters["time"].SetValue(Main.GameUpdateCount * -0.03f);
            effect.Parameters["repeats"].SetValue(1);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);
            trail?.Render(effect);
            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
