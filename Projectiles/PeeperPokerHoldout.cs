namespace BombusApisBee.Projectiles
{
    public class PeeperPokerHoldout : BeeProjectile
    {
        private Player Owner => Main.player[Projectile.owner];
        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;
        public int DamageTimer;
        public int VelocityTimer;
        public float VelocityFloat = 1f;
        public float DamageFloat = 1f;
        public int PierceTimer;
        public int Pierce = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Peeper Poker");
        }
        public override void SafeSetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
            Projectile.ignoreWater = true;
            Projectile.Bombus().HeldProj = true;
        }
        public override void AI()
        {
            Projectile.timeLeft = 2;
            Player player = Main.player[Projectile.owner];
            var modPlayer = Owner.Hymenoptra();
            modPlayer.BeeResourceRegenTimer = -120;
            Vector2? fallback = null;
            fallback = new Vector2?(Vector2.Zero);
            int expectedDirection = Utils.ToDirectionInt(Utils.SafeNormalize(Main.MouseWorld - player.Center, fallback.Value).X > 0f);
            float num = 50f;
            float num2 = 2f;
            float num3 = -0.7853982f;
            Vector2 value = player.RotatedRelativePoint(player.MountedCenter, true);
            Vector2 value2 = Vector2.Zero;
            if (player.dead)
            {
                Projectile.Kill();
                return;
            }
            Lighting.AddLight(player.Center, 0f, 0.2f, 1.45f);
            int num4 = Math.Sign(Projectile.velocity.X);
            Projectile.velocity = new Vector2((float)num4, 0f);
            if (Projectile.ai[0] == 0f)
            {
                Projectile.rotation = Utils.ToRotation(new Vector2((float)num4, -player.gravDir)) + num3 + 3.1415927f;
                if (Projectile.velocity.X < 0f)
                {
                    Projectile.rotation -= 1.5707964f;
                }
            }
            Projectile.alpha -= 128;
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            float num7 = Projectile.ai[0] / num;
            float num5 = 1f;
            Projectile.ai[0] += num5;
            Projectile.rotation += 6.2831855f * num2 / num * (float)num4;
            bool flag2 = Projectile.ai[0] > (float)((int)(num / 2f));
            if (!OwnerCanShoot || modPlayer.BeeResourceCurrent <= 0 || (flag2 && !player.controlUseItem))
            {
                player.reuseDelay = 60;
                if (Main.myPlayer == Projectile.owner && player.ownedProjectileCounts[ModContent.ProjectileType<PeeperPokerThrow>()] < 2)
                {
                    SoundEngine.PlaySound(SoundID.Item1);
                    int Damage = (int)(Projectile.damage * DamageFloat);
                    int DamageAmt = Utils.Clamp<int>(Damage, 0, 55);
                    Vector2 Value = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
                    Vector2 ShootVelocity = Vector2.Normalize(Value - Projectile.Center);
                    float VelocityFloat2 = Utils.Clamp<float>(VelocityFloat, 1f, 3f);
                    float num10 = 10 * VelocityFloat2;
                    ShootVelocity *= num10;
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, ShootVelocity, ModContent.ProjectileType<PeeperPokerThrow>(), DamageAmt, 1f, Projectile.owner);
                    Main.projectile[proj].penetrate = Pierce;
                    Owner.UseBeeResource(2);
                    Projectile.Kill();
                }
            }
            else if (flag2)
            {
                if ((float)expectedDirection != Projectile.velocity.X)
                {
                    player.ChangeDir(expectedDirection);
                    Projectile.velocity = Vector2.UnitX * (float)expectedDirection;
                    Projectile.rotation -= 3.1415927f;
                    Projectile.netUpdate = true;
                }
            }
            Projectile.position = value - Projectile.Size / 2f;
            Projectile.position += value2;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = MathHelper.WrapAngle(Projectile.rotation);
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 40f)
            {
                Vector2 vel = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                while (vel.X == 0f && vel.Y == 0f)
                {
                    vel = new Vector2(Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                }
                vel.Normalize();
                vel *= (float)Main.rand.Next(70, 101) * 0.1f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, vel, ModContent.ProjectileType<CthulhuBee>(), (int)(Projectile.damage * 0.89f), Projectile.knockBack, Projectile.owner);
                Owner.UseBeeResource(1);
                const int Repeats2 = 175;
                for (int d2 = 0; d2 < Repeats2; ++d2)
                {
                    float angle2 = 6.2831855f * (float)d2 / (float)Repeats2;
                    Dust dust3 = Dust.NewDustPerfect(Projectile.Center, DustID.Blood, null, 0, default(Color), 1.1f);
                    dust3.velocity = Utils.ToRotationVector2(angle2) * 4f;
                    dust3.noGravity = true;
                    dust3.fadeIn = 1f;
                }
                SoundEngine.PlaySound(SoundID.NPCHit13);
                Projectile.localAI[0] = 0;
            }
            VelocityTimer++;
            DamageTimer++;
            PierceTimer++;
            if (VelocityTimer > 10)
            {
                VelocityFloat += 0.5f;
                VelocityTimer = 0;
            }
            if (DamageTimer > 15)
            {
                DamageFloat += 0.15f;
                DamageTimer = 0;
            }
            if (PierceTimer > 35)
            {
                Pierce = 2;
            }
            else if (PierceTimer > 75)
            {
                Pierce = 3;
            }
            else
                Pierce = 1;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            SoundEngine.PlaySound(SoundID.NPCHit18);
        }
    }
}
