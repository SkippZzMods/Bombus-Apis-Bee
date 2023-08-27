using Terraria;
namespace BombusApisBee.Projectiles
{
    public class TrueAculeusBladeHoldout : BeeProjectile
    {
        public override string Texture => BombusApisBee.BeeWeapon + "BladeOfAculeus";
        private bool initialized;

        private bool swung;

        private bool flipBlade = true;
        private bool drawAfterImages;
        private bool spawnedProjectile;

        private float maxTimeLeft;
        private float originalDirection;

        private int oldTimeLeft;
        private int pauseTimer;

        private int hits;

        private List<float> oldRotation = new();
        private List<float> oldScale = new();
        private List<Vector2> oldPositions = new();
        private List<bool> oldFlipBlade = new();

        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;
        private Trail trail3;

        private Vector2 tipPosition;

        public float Combo => Projectile.ai[1];
        public Player owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blade of Aculeus");
        }
        public override void SafeSetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.Size = new Vector2(60);
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
            Projectile.Bombus().HeldProj = true;

            Projectile.extraUpdates = 1;
        }

        public override bool PreAI()
        {
            if (pauseTimer > 0)
            {
                Projectile.timeLeft = oldTimeLeft;

                pauseTimer--;
            }

            return true;
        }

        public override void AI()
        {
            UpdateProj();

            switch (Combo)
            {
                case 0:
                    Stab(); break;
                case 1:
                    UpSlash(); break;
                case 2:
                    DownSlash(); break;
                case 3:
                    SpinSlash(); break;
                case 4:
                    SpinToStab(); break;
            }
        }

        private void UpdateProj()
        {
            if (!(owner.HeldItem.ModItem is BladeOfAculeus))
                Projectile.Kill();

            if (Main.rand.NextBool(15))
                Dust.NewDustPerfect(Vector2.Lerp(owner.Center, tipPosition, Main.rand.NextFloat()), DustID.Poisoned, Main.rand.NextVector2Circular(2f, 2f), Main.rand.Next(50, 255), default, 1.5f).noGravity = true;

            if (Main.rand.NextBool(15))
                Dust.NewDustPerfect(Vector2.Lerp(owner.Center, tipPosition, Main.rand.NextFloat()), DustType<StingerDust>(), Main.rand.NextVector2Circular(2f, 2f), Main.rand.Next(50, 255), default, 1.5f).noGravity = true;

            owner.heldProj = Projectile.whoAmI;

            owner.ChangeDir(Projectile.direction);

            if (!drawAfterImages)
                return;

            if (Projectile.timeLeft % 3 == 0)
            {
                oldRotation.Add(Projectile.rotation);

                if (oldRotation.Count > 15)
                    oldRotation.RemoveAt(0);

                oldPositions.Add(Projectile.Center - owner.Center);

                if (oldPositions.Count > 15)
                    oldPositions.RemoveAt(0);

                oldScale.Add(Projectile.scale);

                if (oldScale.Count > 15)
                    oldScale.RemoveAt(0);

                oldFlipBlade.Add(flipBlade);
                if (oldFlipBlade.Count > 15)
                    oldFlipBlade.RemoveAt(0);
            }


            if (!Main.dedServ && Projectile.timeLeft < maxTimeLeft)
            {
                ManageCaches();
                ManageTrail();
            }
        }

        private void Stab()
        {
            if (!initialized)
            {
                initialized = true;
                Projectile.timeLeft = (int)owner.ApplyHymenoptraSpeedTo((int)(owner.HeldItem.useAnimation * 0.75f));
                maxTimeLeft = Projectile.timeLeft;
                Projectile.rotation = owner.DirectionTo(Projectile.Center + Projectile.velocity).ToRotation() + MathHelper.PiOver4;
                Projectile.netUpdate = true;
                Projectile.direction = Main.MouseWorld.X < owner.Center.X ? -1 : 1;
                originalDirection = Projectile.direction;
                owner.itemTime = Projectile.timeLeft / 2;
                owner.itemAnimation = owner.itemTime;
            }

            float progress = 1f - Projectile.timeLeft / maxTimeLeft;

            if (progress < 0.7f)
            {
                float lerper = 1f - (Projectile.timeLeft - maxTimeLeft * 0.3f) / (maxTimeLeft * 0.7f);

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Lerp(0f, 0.35f, EaseBuilder.EaseCubicInOut.Ease(lerper)) * originalDirection;

                Vector2 offset = Vector2.Lerp(new Vector2(0), new Vector2(6, 6 * originalDirection), EaseBuilder.EaseCubicInOut.Ease(lerper)).RotatedBy(Projectile.rotation - MathHelper.PiOver4);

                Projectile.Center = owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * MathHelper.Lerp(0f, -25f, EaseBuilder.EaseCubicOut.Ease(lerper)) + offset;
                
                tipPosition = owner.MountedCenter + offset + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * ((25f + MathHelper.Lerp(0f, -25f, EaseBuilder.EaseCubicOut.Ease(lerper))) * Projectile.scale);

                float armRot = (Projectile.velocity.ToRotation() - MathHelper.PiOver2) + MathHelper.Lerp(0f, MathHelper.ToRadians(200), EaseBuilder.EaseCubicInOut.Ease(lerper)) * originalDirection;

                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);
            }
            else
            {
                if (!drawAfterImages)
                    drawAfterImages = true;

                if (!swung)
                {
                    SoundID.DD2_MonkStaffSwing.PlayWith(Projectile.Center);
                    owner.Bombus().AddShake(7);
                    swung = true;

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 20f, Projectile.velocity * 5f, ModContent.ProjectileType<AculeusBladeStinger>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }

                float lerper = 1f - (Projectile.timeLeft) / (float)Math.Floor(maxTimeLeft - maxTimeLeft * 0.7f);

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Lerp(0.35f, -0.1f, EaseBuilder.EaseQuinticOut.Ease(lerper)) * originalDirection;

                Vector2 offset = Vector2.Lerp(new Vector2(6, 6 * originalDirection), new Vector2(0), EaseBuilder.EaseCubicInOut.Ease(lerper)).RotatedBy(Projectile.rotation - MathHelper.PiOver4);

                Projectile.Center = owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * MathHelper.Lerp(-25f, 35f, EaseBuilder.EaseQuinticOut.Ease(lerper)) + offset;
                
                tipPosition = owner.MountedCenter + offset + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * ((25f + MathHelper.Lerp(-25f, 35f, EaseBuilder.EaseCubicOut.Ease(lerper))) * Projectile.scale);

                float armRot = (Projectile.velocity.ToRotation() - MathHelper.PiOver2) + MathHelper.Lerp(MathHelper.ToRadians(200), 0f, EaseBuilder.EaseQuinticOut.Ease(lerper)) * originalDirection;

                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);
            }
        }

        private void UpSlash()
        {
            if (!initialized)
            {
                initialized = true;
                Projectile.timeLeft = (int)owner.ApplyHymenoptraSpeedTo(owner.HeldItem.useAnimation);
                maxTimeLeft = Projectile.timeLeft;
                Projectile.rotation = owner.DirectionTo(Projectile.Center + Projectile.velocity).ToRotation() + MathHelper.PiOver4;
                Projectile.netUpdate = true;
                Projectile.direction = Main.MouseWorld.X < owner.Center.X ? -1 : 1;
                originalDirection = Projectile.direction;
                owner.itemTime = Projectile.timeLeft / 2;
                owner.itemAnimation = owner.itemTime;
            }

            float progress = 1f - Projectile.timeLeft / maxTimeLeft;

            if (progress < 0.55f)
            {
                float lerper = 1f - (Projectile.timeLeft - maxTimeLeft * 0.45f) / (maxTimeLeft * 0.55f);

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Lerp(-0.1f, -2f, EaseBuilder.EaseCubicInOut.Ease(lerper)) * originalDirection;

                Vector2 offset = Vector2.Lerp(new Vector2(0), new Vector2(20, -10 * originalDirection), EaseBuilder.EaseCubicInOut.Ease(lerper)).RotatedBy(Projectile.rotation - MathHelper.PiOver4);

                Projectile.Center = owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * MathHelper.Lerp(35f, 30f, EaseBuilder.EaseCubicOut.Ease(lerper)) + offset;

                tipPosition = owner.MountedCenter + offset + (Projectile.rotation - MathHelper.PiOver4 - 0.1f * originalDirection).ToRotationVector2() * ((25f + MathHelper.Lerp(35f, 30f, EaseBuilder.EaseCubicOut.Ease(lerper))) * Projectile.scale);

                float armRot = (Projectile.velocity.ToRotation() - MathHelper.PiOver2) + MathHelper.Lerp(0f, -MathHelper.ToRadians(120), EaseBuilder.EaseCubicInOut.Ease(lerper)) * originalDirection;

                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);

                Projectile.scale = MathHelper.Lerp(1f, 0.8f, lerper);

                if (flipBlade && progress >= 0.35f)
                    flipBlade = false;

            }
            else
            {
                if (!drawAfterImages)
                    drawAfterImages = true;

                if (!swung)
                {
                    SoundID.DD2_MonkStaffSwing.PlayWith(Projectile.Center);
                    owner.Bombus().AddShake(7);
                    swung = true;

                    for (int i = -1; i < 2; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), owner.Center, Projectile.velocity.RotatedBy(i * 0.2f) * 5f, ModContent.ProjectileType<AculeusBladeStinger>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }

                float lerper = 1f - (Projectile.timeLeft) / (float)Math.Floor(maxTimeLeft - maxTimeLeft * 0.55f);

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Lerp(-0.65f, 2f, EaseBuilder.EaseQuinticOut.Ease(lerper)) * originalDirection;

                Vector2 offset = Vector2.Lerp(new Vector2(20, -16 * originalDirection), new Vector2(5, 15 * originalDirection), EaseBuilder.EaseQuinticOut.Ease(lerper)).RotatedBy(Projectile.rotation - MathHelper.PiOver4);

                Projectile.Center = owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * MathHelper.Lerp(30f, 35f, EaseBuilder.EaseQuinticOut.Ease(lerper)) + offset;

                tipPosition = owner.MountedCenter + offset + (Projectile.rotation - MathHelper.PiOver4 - 0.1f * originalDirection).ToRotationVector2() * ((25f + MathHelper.Lerp(30f, 35f, EaseBuilder.EaseQuinticOut.Ease(lerper))) * Projectile.scale);

                float armRot = (Projectile.velocity.ToRotation() - MathHelper.PiOver2) + MathHelper.Lerp(-MathHelper.ToRadians(120), MathHelper.ToRadians(150), EaseBuilder.EaseCubicOut.Ease(lerper)) * originalDirection;

                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);

                Projectile.scale = 0.8f + (float)Math.Sin(EaseBuilder.EaseQuinticOut.Ease(lerper) * 3.1415927f) * 0.75f * 0.75f;
            }
        }

        private void DownSlash()
        {
            if (!initialized)
            {
                initialized = true;
                Projectile.timeLeft = (int)owner.ApplyHymenoptraSpeedTo((int)(owner.HeldItem.useAnimation * 1.35f));
                maxTimeLeft = Projectile.timeLeft;
                Projectile.rotation = owner.DirectionTo(Projectile.Center + Projectile.velocity).ToRotation() + MathHelper.PiOver4;
                Projectile.netUpdate = true;
                Projectile.direction = Main.MouseWorld.X < owner.Center.X ? -1 : 1;
                originalDirection = Projectile.direction;
                owner.itemTime = Projectile.timeLeft / 2;
                owner.itemAnimation = owner.itemTime;
            }

            float progress = 1f - Projectile.timeLeft / maxTimeLeft;

            if (progress < 0.35f)
            {
                float lerper = 1f - (Projectile.timeLeft - maxTimeLeft * 0.65f) / (maxTimeLeft * 0.35f);

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Lerp(2, 3.5f, EaseBuilder.EaseCircularInOut.Ease(lerper)) * originalDirection;

                Vector2 offset = Vector2.Lerp(new Vector2(5, 15 * originalDirection), new Vector2(15, -5 * originalDirection), EaseBuilder.Linear.Ease(lerper)).RotatedBy(Projectile.rotation - MathHelper.PiOver4);

                Projectile.Center = owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * MathHelper.Lerp(35f, 20f, EaseBuilder.EaseCircularInOut.Ease(lerper)) + offset;

                tipPosition = owner.MountedCenter + offset + (Projectile.rotation - MathHelper.PiOver4 + 0.15f * originalDirection).ToRotationVector2() * ((25f + MathHelper.Lerp(35f, 20f, EaseBuilder.EaseQuinticOut.Ease(lerper))) * Projectile.scale);

                float armRot = (Projectile.velocity.ToRotation() - MathHelper.PiOver2) + MathHelper.Lerp(MathHelper.ToRadians(150), MathHelper.ToRadians(180), EaseBuilder.EaseCircularInOut.Ease(lerper)) * originalDirection;

                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);

                Projectile.scale = MathHelper.Lerp(0.8f, 0.85f, lerper);
            }
            else
            {
                if (!drawAfterImages && progress >= 0.6f)
                    drawAfterImages = true;

                if (!swung && progress >= 0.7f)
                {
                    SoundID.DD2_MonkStaffSwing.PlayWith(Projectile.Center);
                    owner.Bombus().AddShake(7);
                    swung = true;

                    for (int i = -1; i < 2; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), owner.Center, Projectile.velocity.RotatedBy(i * 0.25f) * 5f, ModContent.ProjectileType<AculeusBladeStinger>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }

                float lerper = 1f - (Projectile.timeLeft) / (float)Math.Floor(maxTimeLeft - maxTimeLeft * 0.35f);

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Lerp(3.5f, -1.35f, EaseBuilder.EaseQuinticInOut.Ease(lerper)) * originalDirection;

                Vector2 offset = Vector2.Lerp(new Vector2(15, -5 * originalDirection), new Vector2(-5, -2 * originalDirection), EaseBuilder.EaseQuinticOut.Ease(lerper)).RotatedBy(Projectile.rotation - MathHelper.PiOver4);

                Projectile.Center = owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * MathHelper.Lerp(20f, 40f, EaseBuilder.EaseQuinticOut.Ease(lerper)) + offset;

                tipPosition = owner.MountedCenter + offset + (Projectile.rotation - MathHelper.PiOver4 + 0.15f * originalDirection).ToRotationVector2() * ((30f + MathHelper.Lerp(20f, 40f, EaseBuilder.EaseQuinticOut.Ease(lerper))) * Projectile.scale);

                float armRot = (Projectile.velocity.ToRotation() - MathHelper.PiOver2) + MathHelper.Lerp(MathHelper.ToRadians(180), -MathHelper.ToRadians(70), EaseBuilder.EaseQuinticInOut.Ease(lerper)) * originalDirection;

                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);


                if (progress > 0.65f)
                {
                    lerper = 1f - (Projectile.timeLeft) / (float)Math.Floor(maxTimeLeft - maxTimeLeft * 0.65f);

                    Projectile.scale = 0.85f + (float)Math.Sin(EaseBuilder.EaseQuinticOut.Ease(lerper) * 3.1415927f) * 0.8f * 0.8f;
                }
                else
                    Projectile.scale = 0.85f;
            }
        }

        private void SpinSlash()
        {
            if (!initialized)
            {
                initialized = true;
                Projectile.timeLeft = (int)owner.ApplyHymenoptraSpeedTo((int)(owner.HeldItem.useAnimation * 1.75f));
                maxTimeLeft = Projectile.timeLeft;
                Projectile.rotation = owner.DirectionTo(Projectile.Center + Projectile.velocity).ToRotation() + MathHelper.PiOver4;
                Projectile.netUpdate = true;
                Projectile.direction = Main.MouseWorld.X < owner.Center.X ? -1 : 1;
                originalDirection = Projectile.direction;
                owner.itemTime = Projectile.timeLeft / 2;
                owner.itemAnimation = owner.itemTime;
            }

            float progress = 1f - Projectile.timeLeft / maxTimeLeft;

            if (progress < 0.3f)
            {
                float lerper = 1f - (Projectile.timeLeft - maxTimeLeft * 0.7f) / (maxTimeLeft * 0.3f);

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Lerp(-1.35f, -2.25f, EaseBuilder.EaseCircularInOut.Ease(lerper)) * originalDirection;

                Vector2 offset = Vector2.Lerp(new Vector2(-5, -2 * originalDirection), new Vector2(10, 5 * originalDirection), EaseBuilder.Linear.Ease(lerper)).RotatedBy(Projectile.rotation - MathHelper.PiOver4);

                Projectile.Center = owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * MathHelper.Lerp(40f, 45f, EaseBuilder.EaseCubicInOut.Ease(lerper)) + offset;

                tipPosition = owner.MountedCenter + offset + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * ((25f + MathHelper.Lerp(40f, 45f, EaseBuilder.EaseQuinticOut.Ease(lerper))) * Projectile.scale);

                float armRot = (Projectile.velocity.ToRotation() - MathHelper.PiOver2) + MathHelper.Lerp(-MathHelper.ToRadians(70), -MathHelper.ToRadians(100), EaseBuilder.EaseCubicInOut.Ease(lerper)) * originalDirection;

                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);

                Projectile.scale = MathHelper.Lerp(0.85f, 1f, lerper);
            }
            else
            {
                if (!drawAfterImages && progress >= 0.55f)
                    drawAfterImages = true;

                if (progress < 0.9f)
                    flipBlade = false;
                else
                    flipBlade = true;

                if (!swung && progress >= 0.7f)
                {
                    SoundID.DD2_MonkStaffSwing.PlayWith(Projectile.Center);
                    owner.Bombus().AddShake(7);
                    swung = true;
                }

                float lerper = 1f - (Projectile.timeLeft) / (float)Math.Floor(maxTimeLeft - maxTimeLeft * 0.3f);

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Lerp(-2.25f, 6f, EaseBuilder.EaseCircularInOut.Ease(lerper)) * originalDirection;

                Vector2 offset = Vector2.Lerp(new Vector2(10, 5 * originalDirection), new Vector2(-5, 5 * originalDirection), lerper).RotatedBy(Projectile.rotation - MathHelper.PiOver4);

                Projectile.Center = owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * MathHelper.Lerp(45f, 30f, EaseBuilder.EaseCircularInOut.Ease(lerper)) + offset;

                tipPosition = owner.MountedCenter + offset + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * ((25f + MathHelper.Lerp(45f, 30f, EaseBuilder.EaseQuinticOut.Ease(lerper))) * Projectile.scale);

                float armRot = (Projectile.velocity.ToRotation() - MathHelper.PiOver2) + MathHelper.Lerp(-MathHelper.ToRadians(100), MathHelper.ToRadians(380), EaseBuilder.EaseCircularInOut.Ease(lerper)) * originalDirection;

                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);

                Projectile.scale = 1f + (float)Math.Sin(EaseBuilder.EaseCubicInOut.Ease(lerper) * 3.1415927f) * 0.6f * 0.6f;

                if (progress >= 0.55f && progress <= 0.8f && Projectile.timeLeft % 5 == 0 && pauseTimer <= 0)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), owner.Center, (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 20f, ModContent.ProjectileType<AculeusBladeStinger>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }

        private void SpinToStab()
        {
            if (!initialized)
            {
                initialized = true;
                Projectile.timeLeft = (int)owner.ApplyHymenoptraSpeedTo((int)(owner.HeldItem.useAnimation * 0.5f));
                maxTimeLeft = Projectile.timeLeft;
                Projectile.rotation = owner.DirectionTo(Projectile.Center + Projectile.velocity).ToRotation() + MathHelper.PiOver4;
                Projectile.netUpdate = true;
                Projectile.direction = Main.MouseWorld.X < owner.Center.X ? -1 : 1;
                originalDirection = Projectile.direction;
                owner.itemTime = Projectile.timeLeft / 2;
                owner.itemAnimation = owner.itemTime;
            }

            float progress = 1f - Projectile.timeLeft / maxTimeLeft;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Lerp(6f - MathHelper.TwoPi, 0f, EaseBuilder.EaseQuinticOut.Ease(progress)) * originalDirection;

            Vector2 offset = Vector2.Lerp(new Vector2(-5, 5 * originalDirection), new Vector2(0), progress).RotatedBy(Projectile.rotation - MathHelper.PiOver4);

            Projectile.Center = owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * MathHelper.Lerp(30f, 0f, EaseBuilder.EaseQuinticOut.Ease(progress)) + offset;
            
            tipPosition = owner.MountedCenter + offset + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * ((25f + MathHelper.Lerp(30f, 0f, EaseBuilder.EaseCubicOut.Ease(progress))) * Projectile.scale);

            float armRot = (Projectile.velocity.ToRotation() - MathHelper.PiOver2) + MathHelper.Lerp(MathHelper.ToRadians(380) - MathHelper.ToRadians(360), 0f, EaseBuilder.EaseQuinticOut.Ease(progress)) * originalDirection;

            owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);
        }

        public override bool? CanDamage()
        {
            return Combo != 4;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = (target.Center.X < owner.Center.X ? -1 : 1);

            switch (Combo)
            {
                case 0: //stab
                    modifiers.SourceDamage *= 1f;
                    break;
                case 1: //up
                    modifiers.SourceDamage *= 1.25f;
                    break;
                case 2: //down
                    modifiers.SourceDamage *= 1.75f;
                    break;
                case 3: //spin
                    modifiers.SourceDamage *= 2f;
                    break;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hits <= 0)
                pauseTimer = 12;

            oldTimeLeft = Projectile.timeLeft;

            hits = 1;

            owner.Bombus().AddShake(3);

            new SoundStyle("BombusApisBee/Sounds/Item/Impale").PlayWith(owner.Center, 0, 0f, 1f);

            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(target.Center + target.DirectionTo(owner.Center) * (target.width / 2), ModContent.DustType<StingerDust>(), target.DirectionTo(owner.Center).RotatedByRandom(0.6f) * Main.rand.NextFloat(0f, 10f), Main.rand.Next(50, 255), default, 1.5f).noGravity = true;

                Dust.NewDustPerfect(target.Center + target.DirectionTo(owner.Center) * (target.width / 2), DustID.Poisoned, target.DirectionTo(owner.Center).RotatedByRandom(0.6f) * Main.rand.NextFloat(0f, 10f), Main.rand.Next(50, 255), default, 1.5f).noGravity = true;

                Dust.NewDustPerfect(target.Center + target.DirectionTo(owner.Center) * (target.width / 2), ModContent.DustType<StingerDust>(), target.DirectionTo(owner.Center).RotatedByRandom(1.5f) * Main.rand.NextFloat(0f, 10f), Main.rand.Next(50, 255), default, 1.5f).noGravity = true;

                Dust.NewDustPerfect(target.Center + target.DirectionTo(owner.Center) * (target.width / 2), DustID.Poisoned, target.DirectionTo(owner.Center).RotatedByRandom(1.5f) * Main.rand.NextFloat(0f, 10f), Main.rand.Next(50, 255), default, 1.5f).noGravity = true;
            }

            target.AddBuff(BuffID.Poisoned, 300);
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;

            if (Combo == 0)
            {
                return null;
            }

            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), owner.Center, owner.Center + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * (70f * Projectile.scale), 10, ref collisionPoint))
                return true;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrimitives();
            Texture2D sword = ModContent.Request<Texture2D>(Texture + (flipBlade ? "_Flipped" : "")).Value;
     
            SpriteEffects flip = owner.direction == -1 ? SpriteEffects.FlipHorizontally : 0;

            if (drawAfterImages)
            {
                for (int i = 15; i > 0; i--)
                {
                    float fade = 1f;

                    if (Projectile.timeLeft < 60f)
                        fade = Projectile.timeLeft / 60f;

                    fade *= 1 - (15f - i) / 15f;

                    if (i > 0 && i < oldRotation.Count)
                    {
                        Texture2D swordBlade = ModContent.Request<Texture2D>(Texture + "_Blade" + (oldFlipBlade[i] ? "_Flipped" : "")).Value;
                        Main.spriteBatch.Draw(swordBlade, owner.Center + oldPositions[i] - Main.screenPosition, null, Color.White * 0.05f * fade, oldRotation[i] + (owner.direction == -1 ? MathHelper.PiOver2 : 0f), sword.Size() / 2f, oldScale[i], flip, 0f);
                    }
                }
            }

            Main.spriteBatch.Draw(sword, Projectile.Center + new Vector2(0f, owner.gfxOffY) - Main.screenPosition, null, lightColor, Projectile.rotation + (owner.direction == -1 ? MathHelper.PiOver2 : 0f), sword.Size() / 2f, Projectile.scale, flip, 0f);

            return false;
        }

        #region Primitive Drawing
        private void ManageCaches()
        {
            var adjustedPos = tipPosition - owner.Center;

            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < 15; i++)
                {
                    cache.Add(owner.Center + adjustedPos);
                }
            }

            cache.Add(owner.Center + adjustedPos);

            while (cache.Count > 15)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 15, new TriangularTip(0), factor => 30f, factor =>
            {
                if (factor.X >= 0.85f)
                    return Color.Transparent;

                return (new Color(40, 150, 20) * 0.3f) * factor.X * (float)Math.Sin(Projectile.timeLeft / maxTimeLeft);
            });

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 15, new TriangularTip(0), factor => 25f, factor =>
            {
                if (factor.X >= 0.85f)
                    return Color.Transparent;

                return (new Color(100, 215, 15) * 0.2f) * factor.X * (float)Math.Sin(Projectile.timeLeft / maxTimeLeft);
            });

            trail3 = trail3 ?? new Trail(Main.instance.GraphicsDevice, 15, new TriangularTip(0), factor => 40f, factor =>
            {
                if (factor.X >= 0.85f)
                    return Color.Transparent;

                return (new Color(200, 255, 40) * 0.1f) * factor.X * (float)Math.Sin(Projectile.timeLeft / maxTimeLeft);
            });

            var realCache = new Vector2[15];

            for (int k = 0; k < 15; k++)
            {
                realCache[k] = cache[k];
            }

            trail.Positions = realCache;
            trail2.Positions = realCache;
            trail3.Positions = realCache;
        }
        public void DrawPrimitives()
        {
            Main.spriteBatch.End();
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["time"].SetValue(Main.GameUpdateCount * -0.025f);
            effect.Parameters["repeats"].SetValue(1f);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/EnergyTrail").Value);

            if (drawAfterImages && Combo > 0 && Combo < 4)
            {
                trail?.Render(effect);

                effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/FireTrail").Value);
                trail2?.Render(effect);

                effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/ShadowTrail").Value);
                trail3?.Render(effect);
            }

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
        #endregion Primitive Drawing
    }

    public class TrueAculeusBladeHoldoutAlt : BeeProjectile
    {
        public override string Texture => BombusApisBee.BeeWeapon + "BladeOfAculeus";

        private float maxTimeLeft;

        private bool initialized;

        private bool pullingBack;

        private bool thrown;

        private int lerpTimer;

        private int originalDirection;

        private bool swinging;

        private bool swung;

        private bool wasStuck;

        int enemyWhoAmI;
        bool stuck = false;
        Vector2 offset = Vector2.Zero;

        private Vector2 oldPos;

        private Vector2 tipPosition;

        private bool drawAfterImages;

        private int oldTimeLeft;
        private int pauseTimer;

        private int hits;

        private List<float> oldRotation = new();
        private List<float> oldScale = new();
        private List<Vector2> oldPositions = new();

        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;
        private Trail trail3;

        public float Combo => Projectile.ai[1];
        public Player owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blade of Aculeus");
        }
        public override void SafeSetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.Size = new Vector2(60);
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
            Projectile.Bombus().HeldProj = true;

            Projectile.extraUpdates = 1;
        }

        public override bool PreAI()
        {
            NPC target = Main.npc[enemyWhoAmI];
            if (stuck)
            {
                Projectile.timeLeft = 2;

                Projectile.position = target.position + offset;

                Projectile.direction = Math.Sign(Projectile.velocity.X);

                if (!target.active || (Main.myPlayer == owner.whoAmI && Main.mouseRight))
                {
                    float mult = target.knockBackResist;

                    if (Main.myPlayer == owner.whoAmI && Main.mouseRight && target.active)
                    {
                        if (mult > 0)
                        {
                            float speed = MathHelper.Lerp(1.5f, 7.5f, Utils.Clamp(Vector2.Distance(target.Center, owner.Center) / 400f, 0, 1f));

                            target.velocity.Y -= 5f * Utils.Clamp(Vector2.Distance(target.Center, owner.Center) / 400f, .2f, 1f);

                            target.velocity += target.DirectionTo(owner.Center + new Vector2(50f * owner.direction, -100f)) * (speed);

                            owner.GiveIFrames(60, true);
                        }
                    }

                    stuck = false;
                    pullingBack = true;
                }

                return false;
            }

            return true;
        }
        public override void AI()
        {
            if (swinging)
                DoSwing();
            else
                ThrownBehavior();

            UpdateProj();
        }

        private void DoSwing()
        {
            if (!initialized)
            {
                initialized = true;
                Projectile.timeLeft = (int)owner.ApplyHymenoptraSpeedTo((int)(owner.HeldItem.useAnimation * 1.25f));
                maxTimeLeft = Projectile.timeLeft;
                Projectile.rotation = owner.DirectionTo(Projectile.Center + Projectile.velocity).ToRotation() + MathHelper.PiOver4;
                Projectile.netUpdate = true;
                Projectile.direction = Main.MouseWorld.X < owner.Center.X ? -1 : 1;
                originalDirection = Projectile.direction;
                owner.itemTime = Projectile.timeLeft / 2;
                owner.itemAnimation = owner.itemTime;
                Projectile.friendly = true;
            }

            float progress = 1f - Projectile.timeLeft / maxTimeLeft;

            if (progress < 0.45f)
            {
                float lerper = 1f - (Projectile.timeLeft - maxTimeLeft * 0.55f) / (maxTimeLeft * 0.45f);

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Lerp(0f, 2f, EaseBuilder.EaseCubicOut.Ease(lerper)) * originalDirection;

                Vector2 offset = Vector2.Lerp(new Vector2(0), new Vector2(30, 5 * originalDirection), EaseBuilder.EaseCubicOut.Ease(lerper)).RotatedBy(Projectile.rotation - MathHelper.PiOver4);

                Projectile.Center = owner.MountedCenter + (Projectile.rotation).ToRotationVector2() * MathHelper.Lerp(0f, 5f, EaseBuilder.EaseCubicOut.Ease(lerper)) + offset;

                tipPosition = owner.MountedCenter + offset + (Projectile.rotation - MathHelper.PiOver4 + 0.02f * originalDirection).ToRotationVector2() * ((25f + MathHelper.Lerp(0f, 5f, EaseBuilder.EaseCubicOut.Ease(lerper))) * Projectile.scale);

                float armRot = (Projectile.velocity.ToRotation() - MathHelper.PiOver2) + MathHelper.Lerp(0f, MathHelper.ToRadians(100), EaseBuilder.EaseCubicOut.Ease(lerper)) * originalDirection;

                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);

                Projectile.scale = MathHelper.Lerp(1f, 0.9f, lerper);
            }
            else
            {
                if (!swung)
                {
                    drawAfterImages = true;
                    SoundID.DD2_MonkStaffSwing.PlayWith(Projectile.Center);
                    owner.Bombus().AddShake(7);
                    swung = true;

                    Vector2 vector = owner.DirectionTo(Main.MouseWorld);

                    if (Math.Sign(vector.X) == Math.Sign(owner.velocity.X))
                        owner.velocity += vector * 7f;

                    owner.GiveIFrames(60, true);
                }

                float lerper = 1f - (Projectile.timeLeft) / (float)Math.Floor(maxTimeLeft - maxTimeLeft * 0.45f);

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Lerp(2f, -2.35f, EaseBuilder.EaseQuinticOut.Ease(lerper)) * originalDirection;

                Vector2 offset = Vector2.Lerp(new Vector2(30, 5 * originalDirection), new Vector2(5, 5 * originalDirection), EaseBuilder.EaseCubicOut.Ease(lerper)).RotatedBy(Projectile.rotation - MathHelper.PiOver4);

                Projectile.Center = owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * MathHelper.Lerp(5f, 50f, EaseBuilder.EaseQuinticOut.Ease(lerper)) + offset;
                
                tipPosition = owner.MountedCenter + offset + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * ((35f + MathHelper.Lerp(5f, 50f, EaseBuilder.EaseCubicOut.Ease(lerper))));

                float armRot = (Projectile.velocity.ToRotation() - MathHelper.PiOver2) + MathHelper.Lerp(MathHelper.ToRadians(100), -MathHelper.ToRadians(130), EaseBuilder.EaseQuinticOut.Ease(lerper)) * originalDirection;

                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);

                Projectile.scale = 0.9f + (float)Math.Sin(EaseBuilder.EaseCircularOut.Ease(lerper) * 3.1415927f) * 0.8f * 0.8f;
            }
        }

        private void ThrownBehavior()
        {
            if (!initialized)
            {
                initialized = true;
                Projectile.timeLeft = (int)owner.ApplyHymenoptraSpeedTo((int)(owner.HeldItem.useAnimation * 0.75f));
                maxTimeLeft = Projectile.timeLeft;
                Projectile.rotation = owner.DirectionTo(Projectile.Center + Projectile.velocity).ToRotation() + MathHelper.PiOver4;
                Projectile.netUpdate = true;
                Projectile.direction = Main.MouseWorld.X < owner.Center.X ? -1 : 1;
                owner.itemTime = Projectile.timeLeft / 2;
                owner.itemAnimation = owner.itemTime;
            }

            if (thrown)
            {
                if (pullingBack && !stuck)
                {
                    Projectile.rotation = Projectile.DirectionTo(owner.Center).ToRotation() + MathHelper.PiOver4 + MathHelper.Pi;

                    owner.ChangeDir(Projectile.Center.X < owner.Center.X ? -1 : 1);

                    owner.heldProj = Projectile.whoAmI;

                    Projectile.timeLeft = 2;

                    if (lerpTimer == 0)
                    {
                        oldPos = Projectile.Center;
                    }

                    lerpTimer++;

                    if (wasStuck)
                    {
                        float lerper = lerpTimer / 30f;

                        Projectile.Center = Vector2.Lerp(oldPos, owner.Center, EaseBuilder.EaseCubicIn.Ease(lerper));

                        if (lerpTimer >= 29)
                        {
                            Projectile.velocity = owner.Center.DirectionTo(Main.MouseWorld) * 5f;
                            swinging = true;
                            initialized = false;
                        }
                    }
                    else
                    {
                        if (lerpTimer < 30)
                        {
                            float lerper = lerpTimer / 30f;

                            Projectile.Center = Vector2.Lerp(oldPos, oldPos + Projectile.velocity * 3f, EaseBuilder.EaseCubicOut.Ease(lerper));
                        }
                        else
                        {
                            float lerper = (lerpTimer - 30) / 60f;

                            Projectile.Center = Vector2.Lerp(oldPos + Projectile.velocity * 3f, owner.Center, EaseBuilder.EaseCubicIn.Ease(lerper));

                            if (lerpTimer >= 89)
                            {
                                Projectile.velocity = owner.Center.DirectionTo(Main.MouseWorld) * 5f;
                                swinging = true;
                                initialized = false;
                            }
                        }
                    }
                }
                else
                {
                    Projectile.timeLeft = 2;

                    owner.heldProj = Projectile.whoAmI;

                    owner.ChangeDir(Projectile.Center.X < owner.Center.X ? -1 : 1);

                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

                    Vector2 tipPoint = Projectile.Center + Vector2.One.RotatedBy(Projectile.rotation - MathHelper.PiOver2) * 50f;

                    Tile tile = Framing.GetTileSafely((int)(tipPoint.X / 16), (int)(tipPoint.Y / 16));

                    if (owner.Distance(Projectile.Center) > 400f || (tile.HasTile && WorldGen.SolidOrSlopedTile(tile)))
                    {
                        if (tile.HasTile && WorldGen.SolidOrSlopedTile(tile))
                            Projectile.velocity *= 0.25f;

                        Projectile.rotation = Projectile.DirectionTo(owner.Center).ToRotation() + MathHelper.PiOver4 + MathHelper.Pi;

                        pullingBack = true;
                        Projectile.friendly = false;
                    }
                }

                return;
            }

            float progress = 1f - Projectile.timeLeft / (float)maxTimeLeft;

            Vector2? mouseWorld = null;
            if (Main.myPlayer == owner.whoAmI)
                mouseWorld = Main.MouseWorld;

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, owner.Center.DirectionTo(mouseWorld.Value), 0.1f);

            if (progress < 0.7f)
            {
                float lerper = 1f - (Projectile.timeLeft - maxTimeLeft * 0.3f) / (maxTimeLeft * 0.7f);

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Lerp(0.2f, -0.2f, EaseBuilder.EaseCubicInOut.Ease(lerper)) * Projectile.direction;

                Vector2 offset = Vector2.Lerp(new Vector2(0), new Vector2(-10, -10 * Projectile.direction), EaseBuilder.EaseCubicInOut.Ease(lerper)).RotatedBy(Projectile.rotation - MathHelper.PiOver4);

                Projectile.Center = owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * MathHelper.Lerp(45f, 20f, EaseBuilder.EaseCubicInOut.Ease(lerper)) + offset;

                float armRot = (Projectile.velocity.ToRotation() - MathHelper.PiOver2) + MathHelper.Lerp(0f, -MathHelper.ToRadians(100), EaseBuilder.EaseCubicInOut.Ease(lerper)) * Projectile.direction;

                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);
            }
            else
            {
                float lerper = 1f - (Projectile.timeLeft) / (maxTimeLeft * 0.3f);

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Lerp(-0.2f, 0f, EaseBuilder.EaseQuinticIn.Ease(lerper)) * Projectile.direction;

                Vector2 offset = Vector2.Lerp(new Vector2(-10, -10 * Projectile.direction), new Vector2(10, -10 * Projectile.direction), EaseBuilder.EaseQuinticIn.Ease(lerper)).RotatedBy(Projectile.rotation - MathHelper.PiOver4);

                Projectile.Center = owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * MathHelper.Lerp(20f, 60f, EaseBuilder.EaseQuinticIn.Ease(lerper)) + offset;

                float armRot = (Projectile.velocity.ToRotation() - MathHelper.PiOver2) + MathHelper.Lerp(-MathHelper.ToRadians(100), 0f, EaseBuilder.EaseQuinticIn.Ease(lerper)) * Projectile.direction;

                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);

                if (Projectile.timeLeft <= 1)
                {
                    Projectile.friendly = true;
                    Projectile.velocity *= 25;
                    Projectile.timeLeft = 10;
                    Projectile.tileCollide = false;
                    thrown = true;
                }
            }

            if (Main.myPlayer == owner.whoAmI)
                Projectile.direction = Main.MouseWorld.X < owner.Center.X ? -1 : 1;
        }

        private void UpdateProj()
        {
            if (!(owner.HeldItem.ModItem is BladeOfAculeus))
                Projectile.Kill();

            owner.heldProj = Projectile.whoAmI;

            owner.ChangeDir(Projectile.direction);

            if (Projectile.timeLeft % 3 == 0)
            {
                oldRotation.Add(Projectile.rotation);

                if (oldRotation.Count > 15)
                    oldRotation.RemoveAt(0);

                oldPositions.Add(Projectile.Center - owner.Center);

                if (oldPositions.Count > 15)
                    oldPositions.RemoveAt(0);

                oldScale.Add(Projectile.scale);

                if (oldScale.Count > 15)
                    oldScale.RemoveAt(0);
            }

            if (!Main.dedServ && Projectile.timeLeft < maxTimeLeft)
            {
                ManageCaches();
                ManageTrail();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!stuck && target.life > 0 && !wasStuck && !swinging)
            {
                stuck = true;
                pullingBack = false;
                Projectile.friendly = false;
                Projectile.tileCollide = false;
                enemyWhoAmI = target.whoAmI;
                offset = Projectile.position - target.position;
                offset += Projectile.velocity * 0.25f;

                Projectile.position = target.position + offset;

                Projectile.netUpdate = true;

                wasStuck = true;
            }

            owner.Bombus().AddShake(3);

            new SoundStyle("BombusApisBee/Sounds/Item/Impale").PlayWith(owner.Center, 0, 0f, 1f);
        }

        public override bool ShouldUpdatePosition()
        {
            return !pullingBack && !stuck;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            pullingBack = true;
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = (target.Center.X < owner.Center.X ? -1 : 1);

            if (swinging)
            {
                modifiers.SourceDamage *= 2f;
                modifiers.Knockback *= 1.5f;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;

            if (swinging)
            {
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), owner.Center, owner.Center + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * (70f * Projectile.scale), 10, ref collisionPoint))
                    return true;

                return false;
            }

            Vector2 startPoint = Projectile.Center + Vector2.One.RotatedBy(Projectile.rotation - MathHelper.PiOver2) * -30f;
            Vector2 tipPoint = Projectile.Center + Vector2.One.RotatedBy(Projectile.rotation - MathHelper.PiOver2) * 30f;

            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), startPoint, tipPoint, 15, ref collisionPoint))
                return true;

            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrimitives();
            Texture2D sword = ModContent.Request<Texture2D>(Texture).Value;
            SpriteEffects flip = owner.direction == -1 ? SpriteEffects.FlipHorizontally : 0;

            if (stuck)
            {
                flip = Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : 0;

                Main.spriteBatch.Draw(sword, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + (Projectile.direction == -1 ? MathHelper.PiOver2 : 0f), sword.Size() / 2f, Projectile.scale, flip, 0f);

                return false;
            }

            Main.spriteBatch.Draw(sword, Projectile.Center + new Vector2(0f, owner.gfxOffY) - Main.screenPosition, null, lightColor, Projectile.rotation + (owner.direction == -1 ? MathHelper.PiOver2 : 0f), sword.Size() / 2f, Projectile.scale, flip, 0f);

            return false;
        }

        #region Primitive Drawing
        private void ManageCaches()
        {
            var adjustedPos = tipPosition - owner.Center;

            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < 15; i++)
                {
                    cache.Add(owner.Center + adjustedPos);
                }
            }

            cache.Add(owner.Center + adjustedPos);

            while (cache.Count > 15)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 15, new TriangularTip(0), factor => 20f, factor =>
            {
                if (factor.X >= 0.85f)
                    return Color.Transparent;

                return (new Color(40, 150, 20) * 0.5f) * factor.X * TrailFade();
            });

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 15, new TriangularTip(0), factor => 20f, factor =>
            {
                if (factor.X >= 0.85f)
                    return Color.Transparent;

                return (new Color(100, 215, 15) * 0.3f) * factor.X * TrailFade();
            });

            trail3 = trail3 ?? new Trail(Main.instance.GraphicsDevice, 15, new TriangularTip(0), factor => 30f, factor =>
            {
                if (factor.X >= 0.85f)
                    return Color.Transparent;

                return (new Color(200, 255, 40) * 0.2f) * factor.X * TrailFade();
            });

            var realCache = new Vector2[15];

            for (int k = 0; k < 15; k++)
            {
                realCache[k] = cache[k];
            }

            trail.Positions = realCache;
            trail2.Positions = realCache;
            trail3.Positions = realCache;
        }

        public float TrailFade()
        {
            float ratio = Projectile.timeLeft / maxTimeLeft;
            if (ratio > 0.25f)
                return (Projectile.timeLeft - maxTimeLeft * 0.25f) / (maxTimeLeft * 0.75f);
            else
                return 0f;
        }

        public void DrawPrimitives()
        {
            Main.spriteBatch.End();
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["time"].SetValue(Main.GameUpdateCount * -0.025f);
            effect.Parameters["repeats"].SetValue(1f);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/EnergyTrail").Value);

            if (drawAfterImages && swung)
            {
                trail?.Render(effect);

                effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/FireTrail").Value);
                trail2?.Render(effect);

                effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/ShadowTrail").Value);
                trail3?.Render(effect);
            }

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
        #endregion Primitive Drawing
    }
}
