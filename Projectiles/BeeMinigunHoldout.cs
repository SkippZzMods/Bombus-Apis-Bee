namespace BombusApisBee.Projectiles
{
    public class BeeMinigunHoldout : BeeProjectile
    {
        private Player Owner
        {
            get
            {
                return Main.player[Projectile.owner];
            }
        }
        private bool OwnerCanShoot
        {
            get
            {
                return Owner.channel && !Owner.noItems && !Owner.CCed;
            }
        }
        private ref float ChargeUp
        {
            get
            {
                return ref Projectile.localAI[0];
            }
        }
        private ref float FramesToNextShot
        {
            get
            {
                return ref Projectile.localAI[1];
            }
        }
        private ref float ShotDelay
        {
            get
            {
                return ref Projectile.ai[0];
            }
        }
        public override string Texture => "BombusApisBee/Items/Weapons/BeeKeeperDamageClass/BeeInyGun";
        public bool SpinningDown;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beenigun");
        }
        public override void SafeSetDefaults()
        {
            Projectile.width = 140;
            Projectile.height = 58;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.Bombus().HeldProj = true;
        }
        public override void AI()
        {
            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            armPosition += Utils.SafeNormalize(Projectile.velocity, Owner.direction * Vector2.UnitX) * 32f;
            armPosition.Y += 22f;
            Vector2 BarrelPosition = armPosition + Projectile.velocity * Projectile.width * 0.5f;
            var modPlayer = Owner.GetModPlayer<BeeDamagePlayer>();
            modPlayer.BeeResourceRegenTimer = -90;
            var modPlayer2 = Owner.GetModPlayer<BombusApisBeePlayer>();
            if (!OwnerCanShoot || modPlayer.BeeResourceCurrent <= 0)
            {
                if (!SpinningDown)
                {
                    SoundStyle winddown = new SoundStyle("BombusApisBee/Sounds/Item/GatlingWindDown", SoundType.Sound);
                    SoundEngine.PlaySound(winddown, Projectile.position);
                    Projectile.timeLeft = 10;
                    SpinningDown = true;
                }
            }
            ChargeUp += 1f;
            if (ChargeUp < 20f)
            {
                //charge up sound
                if (ChargeUp == 2f)
                {
                    SoundStyle windup = new SoundStyle("BombusApisBee/Sounds/Item/GatlingWindUp", SoundType.Sound);
                    SoundEngine.PlaySound(windup, Projectile.position);
                }
            }
            else
            {
                ShotDelay += 1f;
                if (FramesToNextShot == 0f)
                {
                    FramesToNextShot = Owner.GetActiveItem().useAnimation * (1f - (Owner.GetTotalAttackSpeed<HymenoptraDamageClass>() - 1f));
                }
                if (ShotDelay >= FramesToNextShot)
                {
                    if (Owner.UseBeeResource((Owner.HeldItem.ModItem as BeeDamageItem).honeyCost))
                    {
                        Item heldItem = Owner.GetActiveItem();
                        float shootSpeed = heldItem.shootSpeed;
                        Vector2 shootVelocity = Utils.SafeNormalize(Projectile.velocity, Vector2.UnitY) * shootSpeed;
                        Vector2 dustOnlySpread = Utils.NextVector2Circular(Main.rand, shootSpeed, shootSpeed);
                        Vector2 dustVelocity = shootVelocity + 0.055f * dustOnlySpread;
                        ShootDusts(BarrelPosition, dustVelocity);
                        ShootBeeBullets(BarrelPosition);

                        //minigun shoot sound
                        SoundStyle shoot = new SoundStyle("BombusApisBee/Sounds/Item/GatlingShoot", SoundType.Sound);
                        SoundEngine.PlaySound(shoot, Projectile.position);
                        ShootShells(armPosition);
                        ShotDelay = 0;
                    }
                    else
                    {
                        if (!SpinningDown)
                        {
                            SoundStyle winddown = new SoundStyle("BombusApisBee/Sounds/Item/GatlingWindDown", SoundType.Sound);
                            SoundEngine.PlaySound(winddown, Projectile.position);
                            Projectile.timeLeft = 10;
                            SpinningDown = true;
                        }
                    }
                }
            }

            ManipulatePlayerVariables();
            UpdateProjectileHeldVariables(armPosition, !SpinningDown);
        }
        public void ShootShells(Vector2 armPosition)
        {
            int goreType = Mod.Find<ModGore>("ShellEjectGore").Type;
            Vector2 shootDirection = Projectile.velocity;
            Vector2 spawnPosition = armPosition + shootDirection * -1.15f;
            if (!WorldGen.SolidTile((int)spawnPosition.X / 16, (int)spawnPosition.Y / 16))
            {
                int gore = Gore.NewGore(Projectile.GetSource_FromAI(), spawnPosition, Projectile.velocity * 0.5f + shootDirection * -7f, goreType, Projectile.scale);
                Main.gore[gore].timeLeft = 95;
            }
        }
        public void ShootDusts(Vector2 gunBarrel, Vector2 velocity)
        {
            int dustRadius = 9;
            float dustRandomness = 17f;
            int dustDiameter = 2 * dustRadius;
            Vector2 dustCorner = gunBarrel - Vector2.One * (float)dustRadius;
            for (int i = 0; i < 8; i++)
            {
                Vector2 dustVel = velocity + Utils.NextVector2Circular(Main.rand, dustRandomness, dustRandomness);
                Dust dust = Dust.NewDustDirect(dustCorner, dustDiameter, dustDiameter, DustID.Honey, dustVel.X, dustVel.Y, 25, default, 1f);
                dust.velocity *= 0.18f;
                dust.noGravity = true;
                dust.scale = 0.7f;
            }
            for (int i = 0; i < 12; i++)
            {
                Vector2 dustVel = velocity + Utils.NextVector2Circular(Main.rand, dustRandomness, dustRandomness);
                Dust dust = Dust.NewDustDirect(dustCorner, dustDiameter, dustDiameter, ModContent.DustType<HoneyDust>(), dustVel.X, dustVel.Y, 0, default, 1f);
                dust.velocity *= 0.22f;
                dust.noGravity = true;
                dust.scale = 0.9f;
            }
            if (Main.rand.NextBool(2))
                Dust.NewDustPerfect(gunBarrel, ModContent.DustType<Dusts.SmokeDustColor>(), velocity * 0.2f + (Vector2.UnitY * -2.5f), 80 + Main.rand.Next(50), new Color(215, 160, 80), Main.rand.NextFloat(0.5f, 0.7f));
            if (Main.rand.NextBool(2))
                Dust.NewDustPerfect(gunBarrel, ModContent.DustType<Dusts.SmokeDustColor>(), velocity * 0.2f + (Vector2.UnitY * -2.5f), 80 + Main.rand.Next(50), new Color(255, 220, 110), Main.rand.NextFloat(0.5f, 0.7f));
        }
        public void ShootBeeBullets(Vector2 position)
        {
            if (Main.myPlayer != Projectile.owner)
            {
                return;
            }
            Item heldItem = Owner.GetActiveItem();
            var modPlayer = Owner.GetModPlayer<BeeDamagePlayer>();
            int honeyDamage = (int)Owner.GetTotalDamage<HymenoptraDamageClass>().ApplyTo(heldItem.damage);
            float shootSpeed = heldItem.shootSpeed;
            float knockback = heldItem.knockBack;
            knockback = Owner.GetWeaponKnockback(heldItem, knockback);
            Vector2 shootVelocity = Utils.SafeNormalize(Projectile.velocity, Vector2.UnitY) * shootSpeed;
            Vector2 perturbedSpeed = new Vector2(shootVelocity.X, shootVelocity.Y).RotatedByRandom(MathHelper.ToRadians(6));
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), position, perturbedSpeed, ModContent.ProjectileType<BeeBullet>(), honeyDamage, knockback, Projectile.owner, 0f, 0f);
        }
        public override bool? CanDamage()
        {
            return false;
        }
        private void ManipulatePlayerVariables()
        {
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = Utils.ToRotation(Projectile.velocity * Projectile.direction);
        }
        private void UpdateProjectileHeldVariables(Vector2 armPosition, bool UpdateTime)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                float interpolant = Utils.GetLerpValue(5f, 25f, Projectile.Distance(Main.MouseWorld), true);
                Vector2 oldVelocity = Projectile.velocity;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Owner.SafeDirectionTo(Main.MouseWorld, null), interpolant);
                if (Projectile.velocity != oldVelocity)
                {
                    Projectile.netSpam = 0;
                    Projectile.netUpdate = true;
                }
            }

            Projectile.position = armPosition - Projectile.Size * 0.5f;
            Projectile.rotation = Utils.ToRotation(Projectile.velocity);

            if (Owner.direction == -1)
                Projectile.rotation += 3.1415927f;

            Projectile.spriteDirection = Projectile.direction;

            if (UpdateTime)
            {
                Projectile.timeLeft = 2;
            }
        }
        public override void Kill(int timeLeft)
        {
            if (ChargeUp > 50f)
            {
                var modPlayer = Owner.GetModPlayer<BeeDamagePlayer>();
                SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
                int goreType = Mod.Find<ModGore>("BeeInyGunGore1").Type;
                Vector2 shootDirection = Projectile.velocity;
                Vector2 spawnPosition = Projectile.Center + Projectile.Size * 0.5f * Projectile.scale * shootDirection * -0.45f;
                if (!WorldGen.SolidTile((int)spawnPosition.X / 16, (int)spawnPosition.Y / 16))
                {
                    int gore = Gore.NewGore(Projectile.GetSource_Death(), spawnPosition, Projectile.velocity * 0.5f + shootDirection * -5f, goreType, Projectile.scale);
                    Main.gore[gore].timeLeft = 120;
                }
            }
        }
    }
}
