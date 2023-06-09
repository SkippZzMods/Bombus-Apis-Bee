namespace BombusApisBee.Projectiles
{
    public class HoneycombHoldout : ModProjectile
    {
        public override string Texture => BombusApisBee.BeeWeapon + "Honeycomb";
        public Player owner => Main.player[Projectile.owner];

        public int MaxDelay;
        public ref float shootDelay => ref Projectile.ai[0];
        public int shots;

        public int throwTimer;
        public const int maxThrowTimer = 30;
        public bool thrown;

        public bool adjustVelo = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honeycomb");
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 32;

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
            if (shots < 3)
            {
                if (MaxDelay == 0)
                    MaxDelay = (int)owner.ApplyHymenoptraSpeedTo(owner.HeldItem.useAnimation);

                Vector2 armPos = owner.RotatedRelativePoint(owner.MountedCenter, true);
                armPos += Projectile.velocity.SafeNormalize(owner.direction * Vector2.UnitX) * 10f;
                Vector2 barrelPos = armPos + Projectile.velocity * Projectile.width * 0.5f;
                barrelPos.Y -= 2;

                if (++shootDelay >= MaxDelay)
                {
                    shootDelay = 0;
                    shots++;
                    adjustVelo = true;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), barrelPos, (Projectile.velocity * 6f), owner.beeType(), owner.beeDamage(Projectile.damage), 0f, owner.whoAmI);
                    for (int i = 0; i < 15; i++)
                    {
                        Dust.NewDustPerfect(barrelPos, ModContent.DustType<Dusts.HoneyDust>(), Projectile.velocity.RotatedByRandom(0.75f) * Main.rand.NextFloat(0.5f, 6f), Main.rand.Next(85), Scale: Main.rand.NextFloat(0.8f, 1.2f)).noGravity = true;
                    }
                    SoundID.NPCDeath1.PlayWith(Projectile.Center, pitch: -0.2f, pitchVariance: 0.2f);
                }

                owner.ChangeDir(Projectile.direction);
                owner.heldProj = Projectile.whoAmI;
                owner.itemTime = 2;
                owner.itemAnimation = 2;

                Projectile.timeLeft = 2;
                Projectile.rotation = Utils.ToRotation(Projectile.velocity);
                owner.itemRotation = Utils.ToRotation(Projectile.velocity * Projectile.direction);

                if (Projectile.spriteDirection == -1)
                    Projectile.rotation += 3.1415927f;

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
                    owner.ChangeDir(Main.MouseWorld.X < owner.Center.X ? -1 : 1);

                Projectile.Center = owner.MountedCenter + Vector2.UnitY.RotatedBy((double)(armRotation * owner.gravDir), default) * -20f * owner.gravDir;
                Projectile.rotation = (-1.5707964f + armRotation) * owner.gravDir;
                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, 3.1415927f + armRotation);
            }
            else
            {
                if (!thrown && Main.myPlayer == owner.whoAmI)
                {
                    Projectile.friendly = true;
                    SoundID.DD2_MonkStaffSwing.PlayWith(Projectile.Center);
                    Projectile.velocity = owner.DirectionTo(Main.MouseWorld) * 13f;
                    Projectile.timeLeft = 240;
                    Projectile.penetrate = 1;
                    Projectile.tileCollide = true;
                    Projectile.ignoreWater = false;
                    thrown = true;
                }

                Projectile.rotation += (0.35f * (Projectile.velocity.X * 0.15f)) * Projectile.direction;
                Projectile.velocity.Y += 0.2f;
                if (Projectile.velocity.Y > 0)
                {
                    if (Projectile.velocity.Y < 13f)
                        Projectile.velocity.Y *= 1.085f;
                    else
                        Projectile.velocity.Y *= 1.04f;
                }
                if (Projectile.velocity.Y > 16f)
                    Projectile.velocity.Y = 16f;
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < Main.rand.Next(2, 6); i++)
            {
                Gore.NewGorePerfect(Projectile.GetSource_Death(), Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), Main.rand.NextVector2Circular(5f, 5f), Mod.Find<ModGore>("DriedHoneycombGore_" + Main.rand.Next(1, 4)).Type).timeLeft = 90;
            }

            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.HoneyDust>(), Alpha: Main.rand.Next(100));
            }

            SoundID.NPCDeath1.PlayWith(Projectile.Center, pitchVariance: 0.2f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            if (thrown)
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, null, lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                        Projectile.rotation, tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.75f, (i / (float)Projectile.oldPos.Length)), 0, 0);
                }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public BeeUtils.CurveSegment pullback = new BeeUtils.CurveSegment(BeeUtils.EasingType.PolyOut, 0f, 0f, -1.05f, 2);

        public BeeUtils.CurveSegment throwout = new BeeUtils.CurveSegment(BeeUtils.EasingType.PolyOut, 0.7f, -0.9424779f, 2.2132742f, 3);
    }
}
