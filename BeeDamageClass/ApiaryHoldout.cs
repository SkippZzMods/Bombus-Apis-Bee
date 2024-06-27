using BombusApisBee.Dusts.Pixelized;

namespace BombusApisBee.BeeDamageClass
{
    public abstract class ApiaryHoldout : ModProjectile
    {
        public int shakeTimer;
        public virtual Color GlowColor => Color.White;

        public bool updateVelocity = true;
        public ref float Timer => ref Projectile.ai[0];
        public ref float UseTime => ref Projectile.ai[1];
        public Vector2 ArmPosition => Owner.RotatedRelativePoint(Owner.MountedCenter, true) + new Vector2(18f + MathHelper.Lerp(0f, -6f, EaseBuilder.EaseQuarticOut.Ease(Timer < 100f ? Timer / 100f : 1f)), -4f * Owner.direction).RotatedBy(Projectile.velocity.ToRotation());
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
            if (shakeTimer > 0)
                shakeTimer--;

            if (!CanHold())
            {
                Projectile.Kill();
                Owner.reuseDelay = 30;
                return;
            }

            if (Timer == 0f)
            {
                Projectile.velocity = Owner.DirectionTo(Main.MouseWorld);
                UseTime = CombinedHooks.TotalUseTime(Owner.itemTime, Owner, Owner.HeldItem);
            }

            if (Timer % UseTime == 0)
                Shoot();

            if (Owner.altFunctionUse == 2 && Main.rand.NextBool(30))
            {
                Color color = GlowColor with { A = 0 };

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), DustType<PixelStar>(), Vector2.Zero, 0, color, 0.25f);
            }

            Timer++;

            UpdateHeldProjectile();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Timer <= 2)
                return false;

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texGlow = ModContent.Request<Texture2D>(Texture + "_Glowy").Value;
            Texture2D texOutline = ModContent.Request<Texture2D>(Texture + "_Outline").Value;

            SpriteEffects spriteEffects = Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Vector2 position = ArmPosition - Main.screenPosition;

            if (shakeTimer > 0)
                position += Main.rand.NextVector2CircularEdge(shakeTimer / 8.5f, shakeTimer / 8.5f);

            float rotation = Projectile.rotation + (spriteEffects == SpriteEffects.FlipHorizontally ? MathHelper.Pi : 0f);
            
            Main.spriteBatch.Draw(texGlow, position, null, GlowColor with { A = 0 } * 0.55f * (Timer < 15 ? Timer / 15f : 1f), rotation, texGlow.Size() / 2f, Projectile.scale, spriteEffects, 0f);

            Main.spriteBatch.Draw(tex, position, null, lightColor * (Timer < 15 ? Timer / 15f : 1f), rotation, tex.Size() / 2f, Projectile.scale, spriteEffects, 0f);

            if (Owner.altFunctionUse == 2)
            {
                float opacity = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.5f);
                if (opacity < 0f)
                    opacity *= -1f;

                Main.spriteBatch.Draw(texGlow, position, null, GlowColor with { A = 0 } * 0.15f * opacity * (Timer < 15 ? Timer / 15f : 1f), rotation, texGlow.Size() / 2f, Projectile.scale, spriteEffects, 0f);
                Main.spriteBatch.Draw(texOutline, position, null, GlowColor * opacity * (Timer < 15 ? Timer / 15f : 1f), rotation, texOutline.Size() / 2f, Projectile.scale, spriteEffects, 0f);
            }

            return false;
        }

        /// <summary>
        /// Called when the held projectile should shoot its projectile
        /// </summary>
        protected virtual void Shoot()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                if (Owner.UseBeeResource(Owner.altFunctionUse == 2 ? (Owner.HeldItem.ModItem as ApiaryItem).altHoneyCost : (Owner.HeldItem.ModItem as ApiaryItem).honeyCost))
                {
                    shakeTimer = 12;

                    SoundID.Item97.PlayWith(Projectile.Center, 0, 0.1f, 1.25f);
                    BombusApisBee.HoneycombWeapon.PlayWith(Projectile.Center, volume: 0.5f);
                    
                    for (int j = 0; j < 4; j++)
                    {
                        Dust.NewDustPerfect(Projectile.Center, DustType<PixelatedGlow>(), Projectile.velocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(2f, 4f), 0, GlowColor with { A = 0 }, 0.25f);
                    }

                    for (int i = 0; i < 1 + Main.rand.Next(1, 4); i++)
                    {
                        Vector2 offset = Main.rand.NextVector2Circular(15f, 15f);

                        Projectile.SpawnBee(Projectile.GetSource_FromThis(), Projectile.Center + offset, Projectile.velocity * Main.rand.NextFloat(7f, 8f), Projectile.damage, Projectile.knockBack);
                    }
                }
                else
                    Projectile.Kill();               
            }                         
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

            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 oldVelocity = Projectile.velocity;

                Projectile.velocity = Owner.DirectionTo(Main.MouseWorld);

                if (Projectile.velocity != oldVelocity)
                {
                    Projectile.netSpam = 0;
                    Projectile.netUpdate = true;
                }
            }
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
