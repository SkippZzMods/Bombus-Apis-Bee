namespace BombusApisBee.Projectiles
{
    public class ProbeycombHoldout : BeeProjectile
    {
        public bool updateVelo;
        public bool shooting;

        public int ProbesToShoot;
        public int flashTimer;

        public float MaxCharge;

        public ref float shootDelay => ref Projectile.ai[1];
        public ref float Charge => ref Projectile.ai[0];
        public Player owner => Main.player[Projectile.owner];
        public override string Texture => "BombusApisBee/Items/Weapons/BeeKeeperDamageClass/ProbeyComb";
        public override bool? CanDamage() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Probeycomb");
        }
        public override void SafeSetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.Bombus().HeldProj = true;
        }

        public override void AI()
        {
            if (Charge < MaxCharge)
            {
                Charge++;
                if (Charge % (int)(MaxCharge / 5) == 0)
                {
                    owner.UseBeeResource((Main.player[Projectile.owner].HeldItem.ModItem as BeeDamageItem).beeResourceCost - 1);
                    ProbesToShoot++;
                    SoundID.MaxMana.PlayWith(Projectile.Center, -0.2f);
                    flashTimer = 15;
                }
            }

            if (flashTimer > 0)
                flashTimer--;

            if (MaxCharge == 0f)
                MaxCharge = owner.ApplyHymenoptraSpeedTo(owner.GetActiveItem().useAnimation);

            if (!owner.channel || owner.Hymenoptra().BeeResourceCurrent <= 0)
                shooting = true;


            Vector2 armPos = owner.RotatedRelativePoint(owner.MountedCenter, true);
            armPos += Projectile.velocity.SafeNormalize(owner.direction * Vector2.UnitX) * 25f;

            owner.Hymenoptra().BeeResourceRegenTimer = -120;

            if (shooting)
            {
                if (ProbesToShoot <= 0 || !(owner.Hymenoptra().BeeResourceCurrent > owner.Hymenoptra().BeeResourceReserved))
                {
                    Projectile.Kill();
                    return;
                }

                if (--shootDelay <= 0)
                {
                    ProbesToShoot--;
                    shootDelay = 5;
                    ShootThings();
                }
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

            owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(45f) * owner.direction);

            Projectile.position = armPos - Projectile.Size * 0.5f;


            if (Main.myPlayer == Projectile.owner)
            {
                if (shooting && updateVelo)
                {
                    updateVelo = false;
                    float interpolant = Utils.GetLerpValue(5f, 25f, Projectile.Distance(Main.MouseWorld), true);

                    Vector2 oldVelocity = Projectile.velocity;

                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, owner.DirectionTo(Main.MouseWorld), interpolant).RotatedByRandom(0.75f);
                    if (Projectile.velocity != oldVelocity)
                    {
                        Projectile.netSpam = 0;
                        Projectile.netUpdate = true;
                    }
                }
                else if (!shooting)
                {
                    float interpolant = Utils.GetLerpValue(5f, 25f, Projectile.Distance(Main.MouseWorld), true);

                    Vector2 oldVelocity = Projectile.velocity;

                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, owner.DirectionTo(Main.MouseWorld), interpolant);
                    if (Projectile.velocity != oldVelocity)
                    {
                        Projectile.netSpam = 0;
                        Projectile.netUpdate = true;
                    }
                }
            }

            Projectile.spriteDirection = Projectile.direction;

            Vector2 dustPos = owner.RotatedRelativePoint(owner.MountedCenter, true);
            dustPos += Projectile.velocity.SafeNormalize(owner.direction * Vector2.UnitX) * 10f;
            Dust.NewDustPerfect(dustPos, ModContent.DustType<GlowFastDecelerate>(), Vector2.Zero, newColor: new Color(213, 95, 89, 50)).scale = 0.3f;
        }

        public void ShootThings()
        {
            updateVelo = true;
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowFastDecelerate>(), Projectile.velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(2.5f, 5.5f), 0, new Color(213, 95, 89, 50), Main.rand.NextFloat(0.25f, 0.55f));
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowFastDecelerate>(), Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(2f, 5f), 0, new Color(161, 31, 85, 50), Main.rand.NextFloat(0.3f, 0.6f));
            }

            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 6f, ModContent.ProjectileType<Probee>(), Projectile.damage, 0f, Projectile.owner);

            SoundID.Item11.PlayWith(Projectile.Center);
            owner.velocity += Projectile.velocity * -2.45f;

            owner.UseBeeResource((Main.player[Projectile.owner].HeldItem.ModItem as BeeDamageItem).beeResourceCost);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texGlow = ModContent.Request<Texture2D>("BombusApisBee/Projectiles/ProbeycombHoldout_Glow").Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, owner.direction == -1 ? SpriteEffects.FlipHorizontally : 0, 0f);

            if (flashTimer > 0)
                Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, Color.Lerp(new Color(213, 95, 89, 0), Color.Transparent, 1f - flashTimer / 15f), Projectile.rotation, texGlow.Size() / 2f, Projectile.scale, owner.direction == -1 ? SpriteEffects.FlipHorizontally : 0, 0f);

            Vector2 bloomPos = owner.RotatedRelativePoint(owner.MountedCenter, true);
            bloomPos += Projectile.velocity.SafeNormalize(owner.direction * Vector2.UnitX) * 10f;

            Main.spriteBatch.Draw(bloomTex, bloomPos - Main.screenPosition, null, Color.Lerp(new Color(161, 31, 85, 0), Color.Transparent, 1f - Charge / MaxCharge) * 0.5f, Projectile.rotation, bloomTex.Size() / 2f, 0.55f, 0, 0);
            Main.spriteBatch.Draw(bloomTex, bloomPos - Main.screenPosition, null, Color.Lerp(new Color(213, 95, 89, 0), Color.Transparent, 1f - Charge / MaxCharge) * 0.5f, Projectile.rotation, bloomTex.Size() / 2f, 0.55f, 0, 0);
            return false;
        }
    }
}
