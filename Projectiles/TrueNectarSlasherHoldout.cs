namespace BombusApisBee.Projectiles
{
    public class TrueNectarSlasherHoldout : BeeProjectile
    {
        private List<Vector2> cache;
        private Trail trail;

        private List<Vector2> throwCache;
        private Trail throwTrail;
        public override string Texture => BombusApisBee.BeeWeapon + "TrueNectarSlasher";

        private Vector2 direction;
        private bool initialized;
        private float maxTimeLeft;
        private bool thrown;
        private int boomerangTimer;
        private bool spawnedProj;
        public ref float chargeTimer => ref Projectile.ai[0];
        public float Combo => Projectile.ai[1];
        public Player owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("True Nectar Slasher");
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SafeSetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.Size = new Vector2(80);
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.Bombus().HeldProj = true;
            Projectile.scale = 1.5f;
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
            owner.itemTime = 2;
            owner.Hymenoptra().BeeResourceRegenTimer = -90;
            switch (Combo)
            {
                case 0:
                    DownSwing();
                    break;
                case 1:
                    UpSwing();
                    break;
                case 2:
                    HeavySwing();
                    break;
                case 3:
                    Thrown();
                    break;
                case 4:
                    HeavySwing();
                    break;
            }
        }

        private void UpSwing()
        {
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
                owner.Bombus().AddShake(7);
            }

            Projectile.Center = owner.Center + Projectile.rotation.ToRotationVector2() * 25f;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Lerp(3f * owner.direction, -1.5f * owner.direction, SwingRatio());

            owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(owner.direction == -1 ? 90f : 70f));

            Projectile.scale = 1.5f + (float)Math.Sin(SwingRatio() * 3.1415927f) * 0.6f * 0.6f;

            owner.heldProj = Projectile.whoAmI;

            owner.ChangeDir(Projectile.direction);

            Vector2 swordLength = Vector2.Lerp(owner.Center + Projectile.rotation.ToRotationVector2() * (20f * Projectile.scale), owner.Center + Projectile.rotation.ToRotationVector2() * (70f * Projectile.scale), Main.rand.NextFloat());
            Dust.NewDustPerfect(swordLength, ModContent.DustType<Dusts.GlowFastDecelerate>(), null, 0, new Color(214, 158, 79, 50), 0.35f);

            if (!Main.dedServ && Projectile.timeLeft < maxTimeLeft)
            {
                ManageCaches();
                ManageTrail();
            }
        }

        private void DownSwing()
        {
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
                owner.Bombus().AddShake(7);
            }

            Projectile.Center = owner.Center + Projectile.rotation.ToRotationVector2() * 25f;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Lerp(-1.5f * owner.direction, 3f * owner.direction, SwingRatio());

            owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(owner.direction == -1 ? 90f : 70f));

            Projectile.scale = 1.5f + (float)Math.Sin(SwingRatio() * 3.1415927f) * 0.6f * 0.6f;

            owner.heldProj = Projectile.whoAmI;

            owner.ChangeDir(Projectile.direction);

            Vector2 swordLength = Vector2.Lerp(owner.Center + Projectile.rotation.ToRotationVector2() * (20f * Projectile.scale), owner.Center + Projectile.rotation.ToRotationVector2() * (70f * Projectile.scale), Main.rand.NextFloat());
            Dust.NewDustPerfect(swordLength, ModContent.DustType<Dusts.GlowFastDecelerate>(), null, 0, new Color(214, 158, 79, 50), 0.35f);

            if (!Main.dedServ && Projectile.timeLeft < maxTimeLeft)
            {
                ManageCaches();
                ManageTrail();
            }
        }

        private void HeavySwing()
        {
            if (!initialized)
            {
                initialized = true;
                Projectile.timeLeft = (int)owner.ApplyHymenoptraSpeedTo((int)(owner.HeldItem.useAnimation));
                maxTimeLeft = Projectile.timeLeft;
                direction = Projectile.velocity;
                direction.Normalize();
                Projectile.rotation = Utils.ToRotation(direction);
                Projectile.netUpdate = true;
                Projectile.direction = Main.MouseWorld.X < owner.Center.X ? -1 : 1;
            }

            if (++chargeTimer < maxTimeLeft * 1.5f)
            {
                Projectile.timeLeft = (int)maxTimeLeft;

                Projectile.velocity = owner.DirectionTo(Main.MouseWorld);
                Projectile.rotation = Projectile.velocity.ToRotation() + 3.5f * Projectile.direction;

                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(owner.direction == -1 ? 90f : 70f));

                Projectile.Center = owner.Center + Projectile.rotation.ToRotationVector2() * 25f;

                owner.heldProj = Projectile.whoAmI;

                owner.ChangeDir(Projectile.direction);

                Projectile.friendly = false;

                for (int i = 0; i < 3; i++)
                {
                    float lerper = MathHelper.Lerp(75f, 0f, chargeTimer / (maxTimeLeft * 1.5f));
                    Dust.NewDustPerfect((owner.Center + Projectile.rotation.ToRotationVector2() * 25f) + Main.rand.NextVector2CircularEdge(lerper, lerper), ModContent.DustType<Dusts.Glow>(), Vector2.Zero, 0, new Color(214, 158, 79), 0.55f);
                }

                int shakeLerp = (int)MathHelper.Lerp(1, 6, chargeTimer / (maxTimeLeft * 1.5f));
                owner.Bombus().AddShake(shakeLerp);
            }
            else
            {
                if (!thrown)
                {
                    Projectile.timeLeft = (int)maxTimeLeft;
                    owner.UseBeeResource(3);
                    owner.Bombus().AddShake(15);
                    Projectile.friendly = true;
                    SoundID.DD2_SonicBoomBladeSlash.PlayWith(Projectile.Center, -0.25f, 0.1f, 1.25f);
                    thrown = true;
                }

                Projectile.Center = owner.Center + Projectile.rotation.ToRotationVector2() * 25f;

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Lerp(3.5f * Projectile.direction, -0.5f * Projectile.direction, SwingRatio());

                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(owner.direction == -1 ? 90f : 70f));

                Projectile.scale = 1.75f + (float)Math.Sin(SwingRatio() * 3.1415927f) * 0.6f * 0.6f;

                owner.heldProj = Projectile.whoAmI;

                owner.ChangeDir(Projectile.direction);

                if (Projectile.timeLeft % (Math.Floor(maxTimeLeft / 5)) == 0 && Main.myPlayer == Projectile.owner)
                {
                    owner.UseBeeResource(1);
                    for (int i = 0; i < 15; i++)
                    {
                        Dust.NewDustPerfect(owner.Center + Projectile.rotation.ToRotationVector2() * 40f, ModContent.DustType<Dusts.GlowFastDecelerate>(), Projectile.rotation.ToRotationVector2().RotatedByRandom(0.35f) * Main.rand.NextFloat(3f), 0, new Color(214, 158, 79), 0.75f);
                    }
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), owner.Center + Projectile.rotation.ToRotationVector2() * 40f, Projectile.rotation.ToRotationVector2() * 15f, ModContent.ProjectileType<TrueHomingNectar>(), (int)(Projectile.damage * 0.25f), 3f, Projectile.owner);
                }

                Vector2 swordLength = Vector2.Lerp(owner.Center + Projectile.rotation.ToRotationVector2() * (20f * Projectile.scale), owner.Center + Projectile.rotation.ToRotationVector2() * (70f * Projectile.scale), Main.rand.NextFloat());
                Dust.NewDustPerfect(swordLength, ModContent.DustType<Dusts.GlowFastDecelerate>(), null, 0, new Color(214, 158, 79, 50), 0.45f);

                if (!Main.dedServ && Projectile.timeLeft < maxTimeLeft)
                {
                    ManageCaches();
                    ManageTrail();
                }
            }
        }

        public BeeUtils.CurveSegment pullback = new BeeUtils.CurveSegment(BeeUtils.EasingType.PolyOut, 0f, 1f, -1.05f, 2);

        public BeeUtils.CurveSegment throwout = new BeeUtils.CurveSegment(BeeUtils.EasingType.PolyOut, 1f, 0f, 2.2132742f, 2);
        internal float ThrowRatio()
        {
            return BeeUtils.PiecewiseAnimation(1f - chargeTimer / 30f, new BeeUtils.CurveSegment[]
            {
                pullback,
                throwout
            }); ;
        }

        private void Thrown()
        {
            Projectile.ownerHitCheck = false;
            Projectile.timeLeft = 2;
            if (++chargeTimer < 30f)
            {
                if (Main.myPlayer == owner.whoAmI)
                    owner.ChangeDir(Main.MouseWorld.X < owner.Center.X ? -1 : 1);

                float armRot = (MathHelper.Lerp(0f, 7f, ThrowRatio())) * owner.direction;
                owner.heldProj = Projectile.whoAmI;
                Projectile.Center = owner.MountedCenter + Vector2.UnitY.RotatedBy((double)(armRot * owner.gravDir), default) * -40f * owner.gravDir;
                Projectile.rotation = (-MathHelper.PiOver4 + armRot) * owner.gravDir + (owner.direction == -1 ? MathHelper.PiOver2 : 0);

                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, 3.14f + armRot);

                Vector2 swordLength = Vector2.Lerp(owner.Center + (Projectile.rotation - (Projectile.direction == -1 ? MathHelper.PiOver2 : MathHelper.PiOver4)).ToRotationVector2() * (20f * Projectile.scale), owner.Center + (Projectile.rotation - (Projectile.direction == -1 ? MathHelper.PiOver2 : MathHelper.PiOver4)).ToRotationVector2() * (70f * Projectile.scale), Main.rand.NextFloat());
                Dust.NewDustPerfect(swordLength, ModContent.DustType<Dusts.GlowFastDecelerate>(), null, 0, new Color(214, 158, 79, 50), 0.45f);
            }
            else
            {
                if (Main.myPlayer == owner.whoAmI && !thrown)
                {
                    thrown = true;
                    owner.Bombus().AddShake(5);
                    owner.UseBeeResource(3);
                    Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * 25f;
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
                    {
                        BeeUtils.DrawDustImage(owner.Center + new Vector2(3f, 0f), ModContent.DustType<Dusts.GlowFastDecelerate>(), 0.15f, ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/TrueNectarSlasherDustImage").Value, 1f, 0, new Color(255, 255, 150), rot: 0f);

                        for (int i = 0; i < 60; ++i)
                        {
                            float angle2 = 6.28f * (float)i / (float)60;
                            Dust.NewDustPerfect(owner.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Utils.ToRotationVector2(angle2) * 6.75f, 0, new Color(255, 255, 150), 1.45f);
                        }
                        Projectile.Kill();
                    }

                    Dust.NewDustPerfect(Projectile.Center + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * (20f * Projectile.scale), ModContent.DustType<Dusts.GlowFastDecelerate>(), null, 0, new Color(214, 158, 79, 50), 0.55f);
                }

                Projectile.rotation += 0.05f + Projectile.velocity.Length() * 0.015f;

                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, owner.DirectionTo(Projectile.Center).ToRotation() - 1.57f);

                owner.ChangeDir(Projectile.Center.X < owner.Center.X ? -1 : 1);
            }

            if (!Main.dedServ && chargeTimer > 0)
            {
                ManageCaches();
                ManageTrail();
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            switch (Combo)
            {
                case 0: //down
                    damage = (int)(damage * 1.35f);
                    break;
                case 1: //up
                    damage = (int)(damage * 1.35f);
                    break;
                case 2: //heavy
                    damage = (int)(damage * 2.15f);
                    break;
                case 3: //throw
                    damage = (int)(damage * 0.95f);
                    break;
                case 4: //heavy
                    damage = (int)(damage * 2.15f);
                    break;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (NPCID.Sets.ProjectileNPC[target.type])
                return;

            owner.Bombus().AddShake(4);

            Vector2 pos = target.Center + Main.rand.NextVector2CircularEdge(250f, 250f);

            switch (Combo)
            {
                case 0: //down
                    owner.Heal(Main.rand.Next(1, 3));
                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDustPerfect((owner.Center + (Projectile.rotation).ToRotationVector2() * (35f * Projectile.scale)) + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(3.5f, 3.5f), 0, new Color(255, 191, 73), 0.3f);
                    }
                    if (!spawnedProj)
                    {
                        spawnedProj = true;
                        Projectile.NewProjectile(Projectile.GetSource_OnHit(target), pos, pos.DirectionTo(target.Center) * 25f, ModContent.ProjectileType<TrueNectarslash>(), (int)(Projectile.damage * 1.05f), 2f, Projectile.owner);
                    }
                    break;
                case 1: //up
                    owner.Heal(Main.rand.Next(1, 3));
                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDustPerfect((owner.Center + (Projectile.rotation).ToRotationVector2() * (35f * Projectile.scale)) + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(3.5f, 3.5f), 0, new Color(255, 191, 73), 0.3f);
                    }
                    if (!spawnedProj)
                    {
                        spawnedProj = true;
                        Projectile.NewProjectile(Projectile.GetSource_OnHit(target), pos, pos.DirectionTo(target.Center) * 25f, ModContent.ProjectileType<TrueNectarslash>(), (int)(Projectile.damage * 1.05f), 2f, Projectile.owner);
                    }
                    break;
                case 2: //heavy
                    owner.Heal(4);
                    for (int i = 0; i < 30; i++)
                    {
                        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(4.5f, 4.5f), 0, new Color(255, 191, 73), 0.4f);
                    }
                    if (!spawnedProj)
                    {
                        spawnedProj = true;
                        for (int i = 0; i < 2; i++)
                        {
                            pos = target.Center + Main.rand.NextVector2CircularEdge(250f, 250f);
                            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), pos, pos.DirectionTo(target.Center) * 25f, ModContent.ProjectileType<TrueNectarslash>(), (int)(Projectile.damage * 1.35f), 2f, Projectile.owner);
                        }
                    }
                    break;
                case 3: //throw
                    owner.Heal(Main.rand.Next(1, 3));
                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(3.5f, 3.5f), 0, new Color(255, 191, 73), 0.3f);
                    }
                    Projectile.NewProjectile(Projectile.GetSource_OnHit(target), pos, pos.DirectionTo(target.Center) * 25f, ModContent.ProjectileType<TrueNectarslash>(), Projectile.damage, 2f, Projectile.owner);
                    break;
                case 4: //heavy
                    owner.Heal(4);
                    for (int i = 0; i < 30; i++)
                    {
                        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(4.5f, 4.5f), 0, new Color(255, 191, 73), 0.4f);
                    }
                    if (!spawnedProj)
                    {
                        spawnedProj = true;
                        for (int i = 0; i < 2; i++)
                        {
                            pos = target.Center + Main.rand.NextVector2CircularEdge(250f, 250f);
                            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), pos, pos.DirectionTo(target.Center) * 25f, ModContent.ProjectileType<TrueNectarslash>(), (int)(Projectile.damage * 1.35f), 2f, Projectile.owner);
                        }
                    }
                    break;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            if (Combo == 3)
            {
                if (!thrown)
                    return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), owner.Center, owner.Center + (Projectile.rotation - (Projectile.direction == -1 ? MathHelper.PiOver2 : MathHelper.PiOver4)).ToRotationVector2() * (75f * Projectile.scale), 15, ref collisionPoint);
                else
                    return null;
            }

            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), owner.Center, owner.Center + (Projectile.rotation).ToRotationVector2() * (70f * Projectile.scale), 20, ref collisionPoint))
                return true;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrimitives();
            Texture2D sword = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D bladeGlow = ModContent.Request<Texture2D>(Texture + "Glowy").Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            SpriteEffects flip = owner.direction == -1 ? SpriteEffects.FlipHorizontally : 0;

            if (Combo == 3)
            {
                Main.spriteBatch.Draw(sword, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, sword.Size() / 2f, Projectile.scale, flip, 0f);
                for (int i = 0; i < 2; i++)
                {
                    Main.spriteBatch.Draw(bladeGlow, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.Transparent, new Color(214, 158, 79, 0) * 0.75f, chargeTimer / 30f), Projectile.rotation, bladeGlow.Size() / 2f, Projectile.scale, flip, 0f);
                }

                if (thrown)
                    for (int i = 0; i < 2; i++)
                    {
                        Main.spriteBatch.Draw(bloomTex, Projectile.Center + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * (20f * Projectile.scale) - Main.screenPosition, null, new Color(214, 158, 79, 0) * 0.75f, 0f, bloomTex.Size() / 2f, 1.25f, 0, 0);
                    }
            }
            else
            {
                float rotation = Projectile.rotation + MathHelper.PiOver4 + (owner.direction == -1 ? MathHelper.PiOver2 : 0f);
                Vector2 drawPos = owner.Center + Projectile.rotation.ToRotationVector2() * 35f - Main.screenPosition;
                Main.spriteBatch.Draw(sword, drawPos, null, lightColor, rotation, sword.Size() / 2f, Projectile.scale, flip, 0f);

                if (Combo == 2 || Combo == 4)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Main.spriteBatch.Draw(bladeGlow, drawPos, null, Color.Lerp(Color.Transparent, new Color(214, 158, 79, 0) * 0.75f, chargeTimer / (maxTimeLeft * 1.5f)), rotation, bladeGlow.Size() / 2f, Projectile.scale, flip, 0f);
                    }
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Main.spriteBatch.Draw(bladeGlow, drawPos, null, Color.Lerp(Color.Transparent, new Color(214, 158, 79, 0) * 0.75f, Projectile.timeLeft / maxTimeLeft), rotation, bladeGlow.Size() / 2f, Projectile.scale, flip, 0f);
                    }
                }
            }
            return false;
        }

        public BeeUtils.CurveSegment swing => new BeeUtils.CurveSegment(BeeUtils.EasingType.SineOut, 0f, 0f, 1.25f, 3);


        #region Primitive Drawing
        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < 20; i++)
                {
                    cache.Add(owner.Center + Projectile.rotation.ToRotationVector2() * (40f * Projectile.scale));
                }
            }

            cache.Add(owner.Center + Projectile.rotation.ToRotationVector2() * (40f * Projectile.scale));

            while (cache.Count > 20)
            {
                cache.RemoveAt(0);
            }

            if (throwCache == null)
            {
                throwCache = new List<Vector2>();
                for (int i = 0; i < 20; i++)
                {
                    throwCache.Add(Projectile.Center + Projectile.velocity + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * (20f * Projectile.scale));
                }
            }

            throwCache.Add(Projectile.Center + Projectile.velocity + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * (20f * Projectile.scale));

            while (throwCache.Count > 20)
            {
                throwCache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 20, new RoundedTip(240), factor => (35f * MathHelper.Lerp(0f, 1f, 1f - Projectile.timeLeft / maxTimeLeft)) * factor * Projectile.scale * (Projectile.timeLeft <= 10 ? MathHelper.Lerp(1f, 0.5f, 1f - Projectile.timeLeft / 10f) : 1f), factor =>
            {
                if (Projectile.timeLeft <= 10)
                    return new Color(214, 158, 79) * MathHelper.Lerp(1f, 0f, 1f - Projectile.timeLeft / 10f);

                return new Color(214, 158, 79) * MathHelper.Lerp(0f, 1f, 1f - Projectile.timeLeft / maxTimeLeft) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = owner.Center + Projectile.rotation.ToRotationVector2() * (40f * Projectile.scale);

            throwTrail = throwTrail ?? new Trail(Main.instance.GraphicsDevice, 20, new RoundedTip(240), factor => factor * 25f, factor =>
            {
                return new Color(214, 158, 79) * factor.X;
            });

            throwTrail.Positions = throwCache.ToArray();
            throwTrail.NextPosition = Projectile.Center + Projectile.velocity + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * (20f * Projectile.scale);
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

            if (Combo == 3)
                throwTrail?.Render(effect);
            else
                trail?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
        }
        #endregion Primitive Drawing
    }

    class TrueNectarslash : BeeProjectile
    {
        private List<Vector2> cache;
        private Trail trail;

        public Vector2 startPos;
        public override string Texture => BombusApisBee.Invisible;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("True Nectarslash");
        }

        public override void SafeSetDefaults()
        {
            Projectile.DamageType = BeeUtils.BeeDamageClass();

            Projectile.timeLeft = 35;
            Projectile.extraUpdates = 1;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;

            Projectile.width = Projectile.height = 24;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 35)
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
            Main.player[Projectile.owner].Heal(2);

            for (int i = 0; i < 25; i++)
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
                Main.spriteBatch.Draw(bloom, (Projectile.Center - Projectile.velocity) - Main.screenPosition, null, new Color(214, 158, 79, 0) * MathHelper.Lerp(1f, 0f, (1f - Projectile.timeLeft / 30f)), Projectile.rotation, bloom.Size() / 2f, 0.5f, 0f, 0f);
            }
            Main.spriteBatch.Draw(star, (Projectile.Center - Projectile.velocity) - Main.screenPosition, null, new Color(214, 158, 79, 0) * MathHelper.Lerp(1f, 0f, (1f - Projectile.timeLeft / 30f)), Projectile.rotation, star.Size() / 2f, 0.7f, 0f, 0f);
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
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 14, new TriangularTip(1f), factor => 20f * (float)Math.Sin(factor), factor =>
            {
                return new Color(214, 158, 79, 150) * MathHelper.Lerp(1f, 0f, (1f - Projectile.timeLeft / 35f));
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

    class TrueHomingNectar : BeeProjectile
    {
        private List<Vector2> cache;
        private Trail trail;

        public override bool? CanDamage() => Projectile.timeLeft < 225;
        public override string Texture => BombusApisBee.Invisible;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("True Homing Nectar");
        }

        public override void SafeSetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 16;

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

            Projectile.rotation += Projectile.velocity.Length() * 0.015f;

            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), null, Scale: 0.55f, newColor: new Color(214, 158, 79));

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
                Projectile.velocity = (Projectile.velocity * 20f + Utils.SafeNormalize(targetCenter - Projectile.Center, Vector2.UnitX) * 20f) / 21f;
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
            Main.player[Projectile.owner].Heal(Main.rand.Next(1, 3));

            for (int i = 0; i < 35; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(4.5f, 4.5f), 0, new Color(255, 191, 73), 0.3f);
            }
        }

        public override void PostDraw(Color lightColor)
        {
            DrawPrimitives();
            Texture2D bloom = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Texture2D star = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/StarTexture").Value;
            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw(bloom, (Projectile.Center - Projectile.velocity) - Main.screenPosition, null, new Color(214, 158, 79, 0), Projectile.rotation, bloom.Size() / 2f, 0.35f, 0f, 0f);
            }
            Main.spriteBatch.Draw(star, (Projectile.Center - Projectile.velocity) - Main.screenPosition, null, new Color(214, 158, 79, 0), Projectile.rotation, star.Size() / 2f, 0.4f, 0f, 0f);
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
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 15, new TriangularTip(1f), factor => factor * 15f, factor =>
            {
                return new Color(214, 158, 79, 150);
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
