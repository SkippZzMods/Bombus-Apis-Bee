using BombusApisBee.Dusts.Pixelized;

namespace BombusApisBee.BeeDamageClass
{
    public abstract class ApiaryHoldout : ModProjectile
    {
        public bool updateVelocity = true;
        public ref float Timer => ref Projectile.ai[0];
        public ref float UseTime => ref Projectile.ai[1];
        public Vector2 ArmPosition => Owner.RotatedRelativePoint(Owner.MountedCenter, true);
        public Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Apiary");
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (!CanHold())
            {
                Projectile.Kill();
                return;
            }

            if (Timer == 0f)
            {
                Projectile.velocity = Owner.DirectionTo(Main.MouseWorld);
                UseTime = CombinedHooks.TotalUseTime(Owner.itemTime, Owner, Owner.HeldItem);
            }

            if (Timer % UseTime == 0)
                Shoot();

            Timer++;

            UpdateHeldProjectile();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Timer <= 2)
                return false;

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Vector2 position = ArmPosition - Main.screenPosition;

            float rotation = Projectile.rotation + (spriteEffects == SpriteEffects.FlipHorizontally ? MathHelper.Pi : 0f) + MathHelper.PiOver4 * Projectile.spriteDirection;

            Main.spriteBatch.Draw(tex, position, null, lightColor, rotation, tex.Size() / 2f, Projectile.scale, spriteEffects, 0f);

            return false;
        }

        /// <summary>
        /// Called when the held projectile should shoot its projectile
        /// </summary>
        protected virtual void Shoot()
        {
            if (Main.myPlayer == Projectile.owner)
                for (int i = 0; i < 1 + Main.rand.Next(1, 4); i++)
                {
                    Projectile.SpawnBee(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * Main.rand.NextFloat(7f, 8f), Projectile.damage, Projectile.knockBack);
                }            

            updateVelocity = true;
        }

        /// <summary>
        /// Updates the basic variables needed for a held projectile
        /// </summary>
        protected virtual void UpdateHeldProjectile(bool updateTimeleft = true)
        {
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;

            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            if (updateTimeleft)
                Projectile.timeLeft = 2;

            Projectile.rotation = Projectile.velocity.ToRotation();
            Owner.itemRotation = Utils.ToRotation(Projectile.velocity * Projectile.direction);

            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f));

            Projectile.position = ArmPosition - Projectile.Size * 0.5f;

            if (Main.myPlayer == Projectile.owner && updateVelocity)
            {
                Vector2 oldVelocity = Projectile.velocity;

                Projectile.velocity = Owner.DirectionTo(Main.MouseWorld).RotatedByRandom(0.1f);

                if (Projectile.velocity != oldVelocity)
                {
                    Projectile.netSpam = 0;
                    Projectile.netUpdate = true;
                }

                updateVelocity = false;
            }

            Projectile.spriteDirection = Projectile.direction;
        }

        private bool CanHold()
        {
            Owner.TryGetModPlayer(out ControlsPlayer controlsPlayer);
            controlsPlayer.leftClickListener = true;
            controlsPlayer.rightClickListener = true;

            return (controlsPlayer.mouseLeft || controlsPlayer.mouseRight) && !Owner.CCed && Owner.HeldItem.ModItem != null && Owner.HeldItem.ModItem is ApiaryItem;
        }

        public override bool? CanDamage()
        {
            return false;
        }
    }
}
