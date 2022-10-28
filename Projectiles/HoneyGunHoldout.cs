using BombusApisBee.Dusts;

namespace BombusApisBee.Projectiles
{
    public class HoneyGunHoldout : BeeProjectile
    {
        public ref float MaxCharge => ref Projectile.ai[1];
        public ref float Charge => ref Projectile.ai[0];
        public Player owner => Main.player[Projectile.owner];
        public override string Texture => "BombusApisBee/Items/Weapons/BeeKeeperDamageClass/HoneyGun";
        public override bool? CanDamage() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Gun");
        }
        public override void SafeSetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.Bombus().HeldProj = true;
        }

        public override void AI()
        {
            Charge++;

            if (Charge % 7 == 0)
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item13, Projectile.position);

            if (MaxCharge == 0f)
                MaxCharge = owner.ApplyHymenoptraSpeedTo(owner.GetActiveItem().useAnimation);

            Vector2 armPos = owner.RotatedRelativePoint(owner.MountedCenter, true);
            armPos += Projectile.velocity.SafeNormalize(owner.direction * Vector2.UnitX) * 10f;
            Vector2 barrelPos = armPos + Projectile.velocity * Projectile.width * 0.5f;
            barrelPos.Y -= 2;

            if (Charge >= MaxCharge)
            {
                ShootThings(barrelPos);
                Projectile.Kill();
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


            if (Main.myPlayer == Projectile.owner)
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

            Projectile.spriteDirection = Projectile.direction;

            if (Charge > 2)
            {
                Dust.NewDustPerfect(barrelPos + Main.rand.NextVector2Circular(1.5f, 1.5f), Main.rand.NextBool() ? DustID.Honey : ModContent.DustType<HoneyDust>(), Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(1f, 2f), Scale: Main.rand.NextFloat(0.7f, 1.2f)).noGravity = true;
            }
        }

        public void ShootThings(Vector2 barrelPos)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(barrelPos, DustID.Honey2, Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(3f, 5f), Scale: Main.rand.NextFloat(0.9f, 1.5f)).noGravity = true;
                Dust.NewDustPerfect(barrelPos, ModContent.DustType<HoneyDust>(), Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(3f, 5f), Scale: Main.rand.NextFloat(0.9f, 1.5f)).noGravity = true;
            }
            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), barrelPos, Projectile.velocity * 10f, ModContent.ProjectileType<HoneyGunShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            new Terraria.Audio.SoundStyle("BombusApisBee/Sounds/Item/HandCannon").PlayWith(Projectile.position, pitchVariance: 0.1f, volume: 0.5f);
            owner.velocity += Projectile.velocity * -5.45f;
        }
    }

    class HoneyGunShot : ModProjectile
    {
        public override string Texture => BombusApisBee.Invisible;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Stream");
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 3;
            Projectile.alpha = 255;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];
            player.AddBuff(BuffID.Honey, 240, true);
        }
        public override void AI()
        {
            if (Projectile.timeLeft >= 599)
                return;

            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(1.5f, 1.5f), DustID.Honey2, Main.rand.NextVector2Circular(0.5f, 0.5f), Main.rand.Next(100), Scale: Main.rand.NextFloat(0.8f, 1.3f)).noGravity = true;
                Dust.NewDustPerfect(Projectile.oldPosition + (new Vector2(Projectile.width, Projectile.height) * 0.5f) + Main.rand.NextVector2CircularEdge(1.5f, 1.5f), DustID.Honey2, Main.rand.NextVector2Circular(0.5f, 0.5f), Main.rand.Next(100), Scale: Main.rand.NextFloat(0.8f, 1.3f)).noGravity = true;
            }

            Projectile.velocity.Y += 0.03f;
            if (Projectile.velocity.Y > 16f)
                Projectile.velocity.Y = 16f;
        }
    }
}
