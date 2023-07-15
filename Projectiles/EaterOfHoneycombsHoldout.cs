namespace BombusApisBee.Projectiles
{
    public class EaterOfHoneycombsHoldout : ModProjectile
    {
        public override string Texture => BombusApisBee.BeeWeapon + "EaterOfHoneycombs";
        public Player owner => Main.player[Projectile.owner];

        public int MaxDelay;
        public ref float shootDelay => ref Projectile.ai[0];
        public int shots;

        public int throwTimer;
        public const int maxThrowTimer = 20;
        public bool thrown;

        public bool adjustVelo = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eater of Honeycombs");
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 30;

            Projectile.friendly = false;
            Projectile.DamageType = BeeUtils.BeeDamageClass();
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.Bombus().HeldProj = true;
        }

        public float GetArmRotation()
        {
            return BeeUtils.PiecewiseAnimation((float)throwTimer / (float)maxThrowTimer, new BeeUtils.CurveSegment[]
            {
                pullback,
                throwout
            });
        }

        public override void AI()
        {
            owner.Hymenoptra().BeeResourceRegenTimer = -120;
            if (shots < 10)
            {
                if (MaxDelay == 0)
                    MaxDelay = (int)owner.ApplyHymenoptraSpeedTo(owner.HeldItem.useAnimation);

                Vector2 armPos = owner.RotatedRelativePoint(owner.MountedCenter, true);
                armPos += Projectile.velocity.SafeNormalize(owner.direction * Vector2.UnitX) * 10f;
                Vector2 barrelPos = armPos + Projectile.velocity * Projectile.width * 0.5f;
                barrelPos += Projectile.velocity.SafeNormalize(owner.direction * Vector2.UnitX) * -15f;

                if (++shootDelay >= MaxDelay)
                {
                    shootDelay = 0;
                    shots++;
                    adjustVelo = true;
                    if (shots % 2 == 0)
                    {
                        if (Main.myPlayer == Projectile.owner)
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), barrelPos, Projectile.velocity.RotatedByRandom(0.25f) * 10f, ModContent.ProjectileType<CorruptMiniEater>(), (int)(Projectile.damage * 0.66f), 2.5f, Projectile.owner);

                        SoundEngine.PlaySound(SoundID.NPCDeath13, Projectile.position);
                        owner.UseBeeResource((Main.player[Projectile.owner].HeldItem.ModItem as BeeDamageItem).beeResourceCost - 2);
                    }

                    BombusApisBee.HoneycombWeapon.PlayWith(Projectile.Center);

                    if (Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), barrelPos, Projectile.velocity * 6f, ModContent.ProjectileType<CorruptedBee>(), (int)(Projectile.damage * 0.66f), 0f, owner.whoAmI);

                    for (int i = 0; i < 15; i++)
                    {
                        Dust.NewDustPerfect(barrelPos, DustID.CorruptGibs, Projectile.velocity.RotatedByRandom(0.55f) * Main.rand.NextFloat(0.5f, 6f), Main.rand.Next(85), Scale: Main.rand.NextFloat(0.8f, 1.2f)).noGravity = true;

                        Dust.NewDustPerfect(barrelPos, ModContent.DustType<Dusts.GlowFastDecelerate>(), Projectile.velocity.RotatedByRandom(0.45f) * Main.rand.NextFloat(0.5f, 3f), 0, new Color(140, 169, 44), 0.45f).noGravity = true;
                    }

                    if (!owner.UseBeeResource((Main.player[Projectile.owner].HeldItem.ModItem as BeeDamageItem).beeResourceCost - 3))
                        shots = 11;
                }

                owner.ChangeDir(Projectile.direction);
                owner.heldProj = Projectile.whoAmI;
                owner.itemTime = 2;
                owner.itemAnimation = 2;

                Projectile.timeLeft = 2;
                Projectile.rotation = Utils.ToRotation(Projectile.velocity);
                owner.itemRotation = Utils.ToRotation(Projectile.velocity * Projectile.direction);

                Projectile.position = armPos - Projectile.Size * 0.5f;


                if (Main.myPlayer == Projectile.owner && adjustVelo)
                {
                    adjustVelo = false;
                    float interpolant = Utils.GetLerpValue(5f, 25f, Projectile.Distance(Main.MouseWorld), true);

                    Vector2 oldVelocity = Projectile.velocity;

                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, owner.DirectionTo(Main.MouseWorld), interpolant).RotatedByRandom(0.3f);
                    if (Projectile.velocity != oldVelocity)
                    {
                        Projectile.netSpam = 0;
                        Projectile.netUpdate = true;
                    }
                }

                Projectile.spriteDirection = Projectile.direction;

                if (Projectile.spriteDirection == -1)
                    Projectile.rotation += 3.1415927f;

            }
            else if (throwTimer < maxThrowTimer)
            {
                throwTimer++;
                Projectile.timeLeft = 2;
                float armRotation = GetArmRotation() * owner.direction;
                owner.heldProj = Projectile.whoAmI;
                owner.itemTime = 2;
                owner.itemAnimation = 2;

                if (Main.myPlayer == owner.whoAmI)
                {
                    owner.ChangeDir(Main.MouseWorld.X < owner.Center.X ? -1 : 1);
                    Projectile.rotation = Projectile.DirectionTo(Main.MouseWorld).ToRotation() + (owner.direction == -1 ? MathHelper.Pi : 0f);
                }

                Projectile.Center = owner.MountedCenter + Vector2.UnitY.RotatedBy((double)(armRotation * owner.gravDir), default) * -20f * owner.gravDir;
                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, 3.1415927f + armRotation);

            }
            else
            {
                if (!thrown && Main.myPlayer == owner.whoAmI)
                {
                    Projectile.friendly = true;
                    SoundID.DD2_MonkStaffSwing.PlayWith(Projectile.Center);
                    Projectile.velocity = owner.DirectionTo(Main.MouseWorld) * 13f;
                    Projectile.timeLeft = 360;
                    Projectile.penetrate = 1;
                    Projectile.tileCollide = true;
                    Projectile.ignoreWater = false;
                    thrown = true;
                }
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
                if (foundTarget && Projectile.timeLeft < 340)
                {
                    Vector2 pos = Projectile.Center;

                    float betweenX = targetCenter.X - pos.X;
                    float betweenY = targetCenter.Y - pos.Y;

                    float distance = (float)Math.Sqrt(betweenX * betweenX + betweenY * betweenY);
                    float speed = 15f;
                    float adjust = 0.5f;

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
                }
                else
                {
                    Projectile.velocity.Y += 0.1f;
                    if (Projectile.velocity.Y > 0)
                    {
                        if (Projectile.velocity.Y < 13f)
                            Projectile.velocity.Y *= 1.025f;
                        else
                            Projectile.velocity.Y *= 1.01f;
                    }
                    if (Projectile.velocity.Y > 16f)
                        Projectile.velocity.Y = 16f;
                }

                Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.direction == -1 ? MathHelper.Pi : 0f);
                Projectile.spriteDirection = Projectile.direction;
            }
        }

        public override void Kill(int timeLeft)
        {
            Gore.NewGorePerfect(Projectile.GetSource_Death(), Projectile.Center + new Vector2(15, -5 * Projectile.direction).RotatedBy(Projectile.velocity.ToRotation()), Projectile.velocity * 0.5f, Mod.Find<ModGore>("EaterOfHoneycombs_Gore1").Type).timeLeft = 90;
            Gore.NewGorePerfect(Projectile.GetSource_Death(), Projectile.Center + new Vector2(15, 5 * Projectile.direction).RotatedBy(Projectile.velocity.ToRotation()), Projectile.velocity * 0.5f, Mod.Find<ModGore>("EaterOfHoneycombs_Gore1").Type).timeLeft = 90;

            Gore.NewGorePerfect(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity * 0.5f, Mod.Find<ModGore>("EaterOfHoneycombs_Gore2").Type).timeLeft = 120;

            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptGibs, Alpha: Main.rand.Next(100));
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 0, new Color(140, 169, 44), 0.45f);
            }

            if (Main.myPlayer == Projectile.owner)
                for (int i = 0; i < 4; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2CircularEdge(10f, 10f), ModContent.ProjectileType<CorruptMiniEater>(), Projectile.damage / 2, 2.5f, Projectile.owner);
                }

            SoundID.NPCDeath1.PlayWith(Projectile.Center, pitchVariance: 0.2f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texGlow = ModContent.Request<Texture2D>(Texture + "_Glowy").Value;

            if (thrown)
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, null, new Color(140, 169, 44, 0) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                        Projectile.rotation, tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.5f, (i / (float)Projectile.oldPos.Length)), 0, 0);
                }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : 0f, 0);

            Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, new Color(140, 169, 44, 0) * (shots / 10f) * 0.5f, Projectile.rotation, texGlow.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : 0f, 0);
            return false;
        }

        public BeeUtils.CurveSegment pullback = new BeeUtils.CurveSegment(BeeUtils.EasingType.PolyOut, 0f, 0f, -1.05f, 2);

        public BeeUtils.CurveSegment throwout = new BeeUtils.CurveSegment(BeeUtils.EasingType.PolyOut, 0.7f, -0.9424779f, 1.2132742f, 3);
    }
}