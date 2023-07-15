using Terraria;
namespace BombusApisBee.Projectiles
{
    public class HoneycombPrime : BeeProjectile
    {
        public int flashTimer;

        public float maxShootDelay => owner.ApplyHymenoptraSpeedTo(owner.HeldItem.useAnimation);
        public ref float shootDelay => ref Projectile.ai[0];
        public Player owner => Main.player[Projectile.owner];
        public override string Texture => "BombusApisBee/Items/Weapons/BeeKeeperDamageClass/MetalPlatedHoneycomb";
        public override bool? CanDamage() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honeycomb Prime");
        }
        public override void SafeSetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.Bombus().HeldProj = true;
        }

        public override void AI()
        {
            if (flashTimer > 0)
                flashTimer--;

            if ((!owner.channel && owner.itemAnimation == 0) || !(owner.Hymenoptra().BeeResourceCurrent > owner.Hymenoptra().BeeResourceReserved))
            {
                Projectile.Kill();
                return;
            }

            if (owner.ownedProjectileCounts<HoneycombPrime_Saw>() <= 0)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitY, ModContent.ProjectileType<HoneycombPrime_Saw>(), Projectile.damage * 2, 5f, Projectile.owner);
                (proj.ModProjectile as HoneycombPrime_Saw).parent = Projectile;

                Projectile proj2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, -Vector2.UnitY, ModContent.ProjectileType<HoneycombPrime_Laser>(), Projectile.damage, 2.5f, Projectile.owner);
                (proj2.ModProjectile as HoneycombPrime_Laser).parent = Projectile;
            }

            owner.Hymenoptra().BeeResourceRegenTimer = -120;

            if (++shootDelay >= maxShootDelay)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 velocity = Projectile.DirectionTo(Main.MouseWorld);
                    for (int i = 0; i < 15; i++)
                    {
                        Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowFastDecelerate>(), velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(2.5f, 4.5f), 0, new Color(213, 95, 89), Main.rand.NextFloat(0.35f, 0.4f));
                        Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowFastDecelerate>(), velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(2f, 4f), 0, new Color(161, 31, 85), Main.rand.NextFloat(0.5f, 0.5f));
                    }

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity * 6f, ModContent.ProjectileType<MetalBee>(), Projectile.damage, 0f, Projectile.owner);

                    SoundID.Item11.PlayWith(Projectile.Center);

                    Projectile.velocity += velocity * -4.45f;

                    owner.UseBeeResource((Main.player[Projectile.owner].HeldItem.ModItem as BeeDamageItem).beeResourceCost - 1);

                    flashTimer = 25;
                }

                shootDelay = 0;
            }

            Vector2 idlePos = new Vector2(owner.Center.X + 50 * owner.direction, owner.Center.Y - 50);

            Vector2 toIdlePos = idlePos - Projectile.Center;
            if (toIdlePos.Length() < 0.0001f)
                toIdlePos = Vector2.Zero;

            else
            {
                float speed = Vector2.Distance(idlePos, Projectile.Center) * 0.25f;
                speed = Utils.Clamp(speed, 1f, 20f);
                toIdlePos.Normalize();
                toIdlePos *= speed;
            }

            Projectile.velocity = (Projectile.velocity * (25f - 1) + toIdlePos) / 25f;

            owner.heldProj = Projectile.whoAmI;
            owner.itemTime = 2;

            Projectile.timeLeft = 2;
            if (Main.myPlayer == Projectile.owner)
            {
                float interpolant = Utils.GetLerpValue(5f, 25f, Projectile.Distance(Main.MouseWorld), true);

                Projectile.rotation = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.MouseWorld), interpolant).ToRotation();
            }

            Dust.NewDustPerfect(Main.GetPlayerArmPosition(Projectile) + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Glow>(), Vector2.Zero, 0, new Color(213, 95, 89, 50), 0.4f);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath14, Projectile.Center);

            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(2f, 2f), 55,
                    Main.rand.NextBool() ? new Color(213, 95, 89) : new Color(161, 31, 85), Main.rand.NextFloat(0.5f, 0.6f));
            }

            for (int i = 0; i < 2; i++)
            {
                Gore.NewGorePerfect(Projectile.GetSource_Death(), i == 0 ? Projectile.Top : Projectile.Bottom + Main.rand.NextVector2Circular(25f, 25f), Projectile.velocity, Mod.Find<ModGore>("MetalPlatedHoneycomb_Gore1").Type).timeLeft = 90;

                Gore.NewGorePerfect(Projectile.GetSource_Death(), i == 0 ? Projectile.Top : Projectile.Bottom + Main.rand.NextVector2Circular(25f, 25f), Projectile.velocity, Mod.Find<ModGore>("MetalPlatedHoneycomb_Gore2").Type).timeLeft = 90;

                Gore.NewGorePerfect(Projectile.GetSource_Death(), i == 0 ? Projectile.Top : Projectile.Bottom + Main.rand.NextVector2Circular(25f, 25f), Projectile.velocity, Mod.Find<ModGore>("MetalPlatedHoneycomb_Gore3").Type).timeLeft = 90;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texGlow = ModContent.Request<Texture2D>(Texture + "_Glowy").Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0, 0f);

            if (flashTimer > 0)
                Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, Color.Lerp(new Color(213, 95, 89, 0), Color.Transparent, 1f - flashTimer / 25f), Projectile.rotation, texGlow.Size() / 2f, Projectile.scale, 0, 0f);

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(161, 31, 85, 0) * 0.5f, Projectile.rotation, bloomTex.Size() / 2f, 0.45f, 0, 0);
            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(213, 95, 89, 0) * 0.5f, Projectile.rotation, bloomTex.Size() / 2f, 0.45f, 0, 0);
            return false;
        }
    }

    class HoneycombPrime_Saw : BeeProjectile
    {
        public Projectile parent;

        public int soundTimer;

        public bool thrown;

        public float fadeTimer;

        public float boomerangTimer;
        public ref float rotationTimer => ref Projectile.ai[1];
        public ref float attackDelay => ref Projectile.ai[0];
        public Player owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honeycomb Prime Saw");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SafeSetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.Bombus().HeldProj = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.penetrate = -1;

            Projectile.ArmorPenetration = 10;
        }

        public override void AI()
        {
            if (!parent.active)
            {
                Projectile.Kill();
                return;
            }

            if (fadeTimer > 0)
                fadeTimer--;

            if (++soundTimer % 35 == 0)
                SoundEngine.PlaySound(SoundID.Item22, Projectile.Center);

            Vector2 idlePos = parent.Center + new Vector2(15 * owner.direction, 45);

            Vector2 toIdlePos = idlePos - Projectile.Center;
            if (toIdlePos.Length() < 0.0001f)
                toIdlePos = Vector2.Zero;

            else
            {
                float speed = Vector2.Distance(idlePos, Projectile.Center) * 0.05f;
                speed = Utils.Clamp(speed, 1f, 20f);
                toIdlePos.Normalize();
                toIdlePos *= speed;
            }

            if (attackDelay < 45f)
                Projectile.velocity = (Projectile.velocity * (25f - 1) + toIdlePos) / 25f;

            Projectile.Center += Main.rand.NextVector2Circular(1.5f, 1.5f);
            Projectile.timeLeft = 2;


            Projectile.rotation += Projectile.velocity.Length() * 0.025f;

            if (Main.myPlayer != Projectile.owner || ++attackDelay < 45f)
                return;

            Projectile.direction = Main.MouseWorld.X < owner.Center.X ? -1 : 1;

            if (++rotationTimer < 30f)
            {
                Projectile.velocity = (Projectile.velocity * (25f - 1) + toIdlePos) / 25f;
                Projectile.rotation += MathHelper.Lerp(0.1f, 0.5f, rotationTimer / 30f) * Projectile.direction;
            }
            else
            {
                if (!thrown)
                {
                    owner.UseBeeResource((Main.player[Projectile.owner].HeldItem.ModItem as BeeDamageItem).beeResourceCost - 1);
                    fadeTimer = 30f;
                    Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * 25f;
                    for (int i = 0; i < 35; i++)
                    {
                        Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowFastDecelerate>(), Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.5f), 0, new Color(213, 95, 89), Main.rand.NextFloat(0.35f, 0.4f));

                        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(5f, 5f), 0, new Color(213, 95, 89), Main.rand.NextFloat(0.45f, 0.5f));
                    }
                    SoundID.DD2_MonkStaffSwing.PlayWith(Projectile.position, 0.15f, 0.1f);
                    thrown = true;
                }
                else
                {
                    if (++boomerangTimer < 15f)
                        return;

                    Vector2 pos = Projectile.Center;

                    float betweenX = idlePos.X - pos.X;
                    float betweenY = idlePos.Y - pos.Y;

                    float distance = (float)Math.Sqrt(betweenX * betweenX + betweenY * betweenY);
                    float speed = distance > 450f ? 17f : 14f;
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

                    if (Projectile.soundDelay == 0)
                    {
                        Projectile.soundDelay = 8;
                        SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
                    }

                    if (Projectile.Distance(idlePos) < 5f)
                    {
                        thrown = false;
                        rotationTimer = 0;
                        attackDelay = 0;
                        boomerangTimer = 0;
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            owner.Bombus().AddShake(4);
            new SoundStyle("BombusApisBee/Sounds/Item/StabTiny").PlayWith(Projectile.Center, pitchVariance: 0.6f);
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowFastDecelerate>(), Projectile.velocity.RotatedByRandom(0.4f) * -Main.rand.NextFloat(0.25f), 0, new Color(213, 95, 89), Main.rand.NextFloat(0.35f, 0.4f));
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath14 with { Volume = 0.5f }, Projectile.Center);

            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(1.25f, 1.25f), 55,
                    Main.rand.NextBool() ? new Color(213, 95, 89) : new Color(161, 31, 85), Main.rand.NextFloat(0.5f, 0.6f));
            }

            Gore.NewGorePerfect(Projectile.GetSource_Death(), Projectile.Center + Main.rand.NextVector2Circular(3.5f, 3.5f), Projectile.velocity, Mod.Find<ModGore>(Name + "_Gore1").Type).timeLeft = 90;

            Gore.NewGorePerfect(Projectile.GetSource_Death(), Projectile.Center + Main.rand.NextVector2Circular(3.5f, 3.5f), Projectile.velocity, Mod.Find<ModGore>(Name + "_Gore2").Type).timeLeft = 90;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texGlow = ModContent.Request<Texture2D>(Texture + "_Glowy").Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, null, lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.oldRot[i], tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.45f, (i / (float)Projectile.oldPos.Length)), 0, 0);
            }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0, 0f);

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(161, 31, 85, 0) * 0.5f, Projectile.rotation, bloomTex.Size() / 2f, 0.45f, 0, 0);
            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(213, 95, 89, 0) * 0.5f, Projectile.rotation, bloomTex.Size() / 2f, 0.45f, 0, 0);

            if (rotationTimer > 0 && !thrown)
            {
                Color color = Color.Lerp(Color.Transparent, new Color(213, 95, 89, 0), rotationTimer / 30f);

                Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, texGlow.Size() / 2f, Projectile.scale, 0, 0f);

                Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, color * 0.75f, Projectile.rotation, bloomTex.Size() / 2f, 0.45f, 0, 0);
                Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, color * 0.75f, Projectile.rotation, bloomTex.Size() / 2f, 0.45f, 0, 0);
            }
            else if (fadeTimer > 0)
            {
                Color color = Color.Lerp(new Color(213, 95, 89, 0), Color.Transparent, 1f - (fadeTimer / 30f));

                Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, texGlow.Size() / 2f, Projectile.scale, 0, 0f);

                Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, color * 0.75f, Projectile.rotation, bloomTex.Size() / 2f, 0.45f, 0, 0);
                Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, color * 0.75f, Projectile.rotation, bloomTex.Size() / 2f, 0.45f, 0, 0);
            }
            return false;
        }
    }

    class HoneycombPrime_Laser : BeeProjectile
    {
        public Projectile parent;

        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;

        public const float MAXSHOOTDELAY = 10;
        public const float MAXCHARGE = 90;
        public ref float shootDelay => ref Projectile.ai[0];
        public ref float charge => ref Projectile.ai[1];
        public int shots;
        public float rotTimer;

        public bool laser;
        public float oldRot;
        public int oldDirection;
        public int laserTimer;
        public const int MAXLASERTIMER = 60;

        public Player owner => Main.player[Projectile.owner];
        public override bool? CanDamage() => laser;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honeycomb Prime Laser");
        }
        public override void SafeSetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.Bombus().HeldProj = true;

            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (!parent.active)
            {
                Projectile.Kill();
                return;
            }

            if (rotTimer > 0)
                rotTimer--;

            Vector2 idlePos = parent.Center + new Vector2(25 * owner.direction, -45);

            Vector2 toIdlePos = idlePos - Projectile.Center;
            if (toIdlePos.Length() < 0.0001f)
                toIdlePos = Vector2.Zero;

            else
            {
                float speed = Vector2.Distance(idlePos, Projectile.Center) * 0.05f;
                speed = Utils.Clamp(speed, 1f, 20f);
                toIdlePos.Normalize();
                toIdlePos *= speed;
            }

            Projectile.velocity = (Projectile.velocity * (25f - 1) + toIdlePos) / 25f;
            Projectile.timeLeft = 2;

            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.direction = Main.MouseWorld.X < owner.Center.X ? -1 : 1;

                float interpolant = Utils.GetLerpValue(5f, 25f, Projectile.Distance(Main.MouseWorld), true);

                if (!laser)
                    Projectile.rotation = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.MouseWorld), interpolant).ToRotation() - (Projectile.direction == -1 ? -MathHelper.ToRadians(rotTimer) : MathHelper.ToRadians(rotTimer));
                else
                    Projectile.rotation = oldRot - (oldDirection == -1 ? -MathHelper.ToRadians(rotTimer) : MathHelper.ToRadians(rotTimer));
            }

            if (++charge >= MAXCHARGE && shots < 3)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    if (++shootDelay > MAXSHOOTDELAY)
                    {
                        Vector2 velocity = Projectile.DirectionTo(Main.MouseWorld).RotatedBy(Projectile.direction == -1 ? MathHelper.ToRadians(rotTimer) : -MathHelper.ToRadians(rotTimer));

                        for (int i = 0; i < 25; i++)
                        {
                            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowFastDecelerate>(), velocity * Main.rand.NextFloat(4.5f), 0, new Color(213, 95, 89), Main.rand.NextFloat(0.35f, 0.4f));
                        }

                        for (int i = 0; i < 25; ++i)
                        {
                            float angle2 = 6.28f * (float)i / (float)25;
                            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Utils.ToRotationVector2(angle2) * 0.75f, 0, new Color(161, 31, 85), 0.35f);
                        }

                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, velocity * 8f,
                            ModContent.ProjectileType<RedLaser>(), Projectile.damage, 2.5f, Projectile.owner);

                        SoundID.Item33.PlayWith(Projectile.position, -0.15f, 0.1f);

                        Projectile.velocity += velocity * -2.45f;

                        owner.UseBeeResource((Main.player[Projectile.owner].HeldItem.ModItem as BeeDamageItem).beeResourceCost - 2);

                        shots++;
                        shootDelay = 0;

                        rotTimer += 18;
                    }
                }
            }
            else if (shots >= 3)
            {
                if (rotTimer <= 0)
                {
                    oldRot = Projectile.rotation;
                    oldDirection = Projectile.direction;
                    laser = true;
                    owner.Bombus().AddShake(6);
                }
                else
                {
                    Dust.NewDustPerfect(Projectile.Center + Projectile.rotation.ToRotationVector2() * 7f, ModContent.DustType<GlowFastDecelerate>(), Projectile.rotation.ToRotationVector2().RotatedByRandom(0.4f) * Main.rand.NextFloat(6f), newColor: new Color(213, 95, 89, 50)).scale = 0.4f;
                    if (!laser)
                    {
                        if (Projectile.soundDelay <= 0)
                        {
                            SoundID.Item13.PlayWith(Projectile.Center);
                            Projectile.soundDelay = 5;
                        }
                    }
                }
            }

            if (laser)
            {
                if (!Main.dedServ)
                {
                    ManageCaches();
                    ManageTrail();
                }

                if (++laserTimer < MAXLASERTIMER)
                {
                    rotTimer = MathHelper.Lerp(0, 25f, (laserTimer / (float)MAXLASERTIMER));
                    Projectile.velocity -= Vector2.One.RotatedBy(Projectile.rotation - MathHelper.PiOver4) * 0.25f;

                    Dust.NewDustPerfect(Vector2.Lerp(Projectile.Center, Projectile.Center + Vector2.One.RotatedBy(Projectile.rotation - MathHelper.PiOver4) * 500f, Main.rand.NextFloat()),
                        ModContent.DustType<GlowFastDecelerate>(), newColor: Main.rand.NextBool() ? new Color(213, 95, 89) : new Color(161, 31, 85), Scale: 0.35f).velocity *= 0.5f;

                    if (laserTimer % 7 == 0)
                        SoundID.Item60.PlayWith(Projectile.Center, -0.5f, 0.1f, 1f);

                    if (laserTimer % 15 == 0)
                        owner.UseBeeResource((Main.player[Projectile.owner].HeldItem.ModItem as BeeDamageItem).beeResourceCost - 1);
                }
                else
                {
                    laser = false;
                    shots = 0;
                    laserTimer = 0;
                    charge = 0;
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= 2.65f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float useless = 0f;
            return laser && Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                Projectile.Center + Vector2.One.RotatedBy(Projectile.rotation - MathHelper.PiOver4) * 500f, 5, ref useless);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath14 with { Volume = 0.5f }, Projectile.Center);

            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(1.25f, 1.25f), 55,
                    Main.rand.NextBool() ? new Color(213, 95, 89) : new Color(161, 31, 85), Main.rand.NextFloat(0.5f, 0.6f));
            }

            Gore.NewGorePerfect(Projectile.GetSource_Death(), Projectile.Center + Main.rand.NextVector2Circular(3.5f, 3.5f), Projectile.velocity, Mod.Find<ModGore>(Name + "_Gore1").Type).timeLeft = 90;

            Gore.NewGorePerfect(Projectile.GetSource_Death(), Projectile.Center + Main.rand.NextVector2Circular(3.5f, 3.5f), Projectile.velocity, Mod.Find<ModGore>(Name + "_Gore2").Type).timeLeft = 90;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrail(Main.spriteBatch);
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            if (laser)
            {
                for (int i = 0; i < 2; i++)
                {
                    Main.spriteBatch.Draw(bloomTex, Projectile.Center - Projectile.velocity + (Vector2.One.RotatedBy(Projectile.rotation - MathHelper.PiOver4) * 500f) - Main.screenPosition, null, new Color(161, 31, 85, 0) * TrailFade(), Projectile.rotation, bloomTex.Size() / 2f, 0.25f, 0, 0);
                    Main.spriteBatch.Draw(bloomTex, Projectile.Center - Projectile.velocity + (Vector2.One.RotatedBy(Projectile.rotation - MathHelper.PiOver4) * 500f) - Main.screenPosition, null, new Color(213, 95, 89, 0) * TrailFade(), Projectile.rotation, bloomTex.Size() / 2f, 0.25f, 0, 0);
                }
            }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0, 0f);

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(161, 31, 85, 0) * 0.5f, Projectile.rotation, bloomTex.Size() / 2f, 0.45f, 0, 0);
            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(213, 95, 89, 0) * 0.5f, Projectile.rotation, bloomTex.Size() / 2f, 0.45f, 0, 0);

            if (laser && laserTimer > 0)
                Main.spriteBatch.Draw(bloomTex, Projectile.Center + Projectile.rotation.ToRotationVector2() * 7f - Main.screenPosition, null, new Color(161, 31, 85, 0) * TrailFade(), Projectile.rotation, bloomTex.Size() / 2f, 0.65f, 0, 0);
            return false;
        }

        private void ManageCaches()
        {
            cache = new List<Vector2>();
            for (int i = 0; i < 13; i++)
            {
                cache.Add(Vector2.Lerp((Projectile.Center + Projectile.velocity), Projectile.Center + Vector2.One.RotatedBy(Projectile.rotation - MathHelper.PiOver4) * 500f, i / 13f));
            }
            cache.Add(Projectile.Center + Vector2.One.RotatedBy(Projectile.rotation - MathHelper.PiOver4) * 500f);
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 14, new TriangularTip(4), factor => 6f, factor =>
            {
                return new Color(161, 31, 85) * TrailFade();
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Vector2.One.RotatedBy(Projectile.rotation - MathHelper.PiOver4) * 500f;

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 14, new TriangularTip(4), factor => 3.5f, factor =>
            {
                return new Color(213, 95, 89) * TrailFade();
            });

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = Projectile.Center + Vector2.One.RotatedBy(Projectile.rotation - MathHelper.PiOver4) * 500f;
        }

        private void DrawTrail(SpriteBatch spriteBatch)
        {
            if (!laser)
                return;

            spriteBatch.End();
            Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["time"].SetValue(Main.GameUpdateCount * -0.03f);
            effect.Parameters["repeats"].SetValue(1);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);
            trail?.Render(effect);

            trail2?.Render(effect);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/FireTrail").Value);
            trail?.Render(effect);

            trail2?.Render(effect);
            effect.Parameters["time"].SetValue(Projectile.timeLeft * -0.02f);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/EnergyTrail").Value);
            trail?.Render(effect);

            trail2?.Render(effect);
            spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
        }

        private float TrailFade()
        {
            if (laserTimer < 45)
                return 1f;

            float fade = MathHelper.Lerp(1f, 0f, (laserTimer - 45f) / 15f);
            return fade;
        }
    }
}

