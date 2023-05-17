using Microsoft.Xna.Framework.Graphics;

namespace BombusApisBee.Projectiles
{
    public class MegaHornetHoldout : BeeProjectile
    {
        public const int MaxSpinUp = 30;
        public const int MAXHEAT = 360;
        public int frameCounter = 10;
        public int SpinUp;

        public ref float Heat => ref Projectile.ai[1];

        public float MaxCharge;
        public ref float Charge => ref Projectile.ai[0];
        public Player owner => Main.player[Projectile.owner];
        public bool CanHold => owner.channel && !owner.CCed && !owner.noItems;
        public override bool? CanDamage() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Megahornet");
            Main.projFrames[Type] = 2;
        }
        public override void SafeSetDefaults()
        {
            Projectile.width = 78;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.Bombus().HeldProj = true;
        }

        public override void AI()
        {
            if (!CanHold || owner.Hymenoptra().BeeResourceCurrent <= 0)
                Projectile.Kill();

            if (SpinUp < MaxSpinUp)
                SpinUp++;

            if (SpinUp >= 30)
            {
                Charge++;
                if (Heat < MAXHEAT)
                    Heat++;
            }

            if (SpinUp % 5 == 0 && SpinUp < 30)
                frameCounter--;

            if (MaxCharge == 0f)
                MaxCharge = owner.ApplyHymenoptraSpeedTo(owner.GetActiveItem().useAnimation);

            if (++Projectile.frameCounter % frameCounter == 0)
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];

            Vector2 armPos = owner.RotatedRelativePoint(owner.MountedCenter, true);
            //armPos += Projectile.velocity.SafeNormalize(owner.direction * Vector2.UnitX) * 10f;
            Vector2 barrelPos = armPos + Projectile.velocity * Projectile.width * 0.5f;
            barrelPos.Y -= 2;

            if (Charge >= MaxCharge)
            {
                ShootThings(barrelPos);
                Charge = 0;
            }

            owner.ChangeDir(Projectile.direction);
            owner.heldProj = Projectile.whoAmI;
            owner.itemTime = 2;
            owner.itemAnimation = 2;

            Projectile.timeLeft = 2;
            Projectile.rotation = Utils.ToRotation(Projectile.velocity);
            owner.itemRotation = Utils.ToRotation(Projectile.velocity * Projectile.direction);

            owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - (Projectile.direction == 1 ? MathHelper.ToRadians(70f) : MathHelper.ToRadians(110f)));
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

            if (SpinUp > 2 && Main.rand.NextBool())
                Dust.NewDustPerfect(barrelPos + Main.rand.NextVector2Circular(1.5f, 1.5f), Main.rand.NextBool() ? DustID.CorruptGibs : DustID.Poisoned, Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(1f, 2f), Main.rand.Next(100), Scale: Main.rand.NextFloat(0.7f, 1.2f)).noGravity = Main.rand.NextBool();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texGlow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Rectangle frame = tex.Frame(verticalFrames: 2, frameY: Projectile.frame);
            Rectangle glowFrame = texGlow.Frame(verticalFrames: 2, frameY: Projectile.frame);

            SpriteEffects spriteEffects = owner.direction == -1 ? (SpriteEffects)1 : 0;

            Color color = Color.Lerp(Color.Transparent, new Color(57, 84, 55, 0), Heat / MAXHEAT);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() / 2f, Projectile.scale, spriteEffects, 0f);

            Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, glowFrame, color, Projectile.rotation, glowFrame.Size() / 2f, Projectile.scale, spriteEffects, 0f);

            Main.spriteBatch.Draw(bloomTex, ((owner.RotatedRelativePoint(owner.MountedCenter, true)) + Projectile.velocity * Projectile.width * 0.5f) - Main.screenPosition, null, color, 0f, bloomTex.Size() / 2f, 0.35f, 0, 0);
            return false;
        }

        public void ShootThings(Vector2 barrelPos)
        {
            if (owner.UseBeeResource(1))
            {
                for (int i = 0; i < 15; i++)
                {
                    Dust.NewDustPerfect(barrelPos, Main.rand.NextBool() ? DustID.CorruptGibs : DustID.Poisoned, Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(3f, 5f), Main.rand.Next(100), Scale: Main.rand.NextFloat(0.9f, 1.5f)).noGravity = true;
                }
                if (Main.myPlayer == Projectile.owner)
                {
                    int damage = (int)(Projectile.damage * MathHelper.Lerp(1f, 1.85f, Heat / MAXHEAT));
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), barrelPos, (Projectile.velocity.RotatedByRandom(0.15f)) * 17.5f, ModContent.ProjectileType<StingerFriendly>(), damage, Projectile.knockBack, Projectile.owner);
                    if (Main.rand.NextBool())
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), barrelPos, (Projectile.velocity.RotatedByRandom(0.15f)) * 15f, ModContent.ProjectileType<HomingStinger>(), damage, Projectile.knockBack, Projectile.owner);
                }
                SoundID.Item11.PlayWith(Projectile.position, pitchVariance: 0.15f);
            }
            else
                Projectile.Kill();
        }
    }
}
