using BombusApisBee.Assets;
using BombusApisBee.Content.Dusts.Pixelized;
using BombusApisBee.Core.BeekeeperClass;
using BombusApisBee.Core.Systems.ParticleSystem;
using Terraria;

namespace BombusApisBee.Core.Common.Apiary
{
    public abstract class ApiaryHoldout : ModProjectile
    {
        internal float swingRotation;
        internal float angularVelocity;

        public int shakeTimer;
        public int flashTimer;
        public virtual int ProjectileTypeToFire => ProjectileType<RegularBeeProjectile>();
        public virtual bool UseDefaultTextures => false;
        public virtual Color GlowColor => Color.White;

        public bool updateVelocity = true;
        public ref float Timer => ref Projectile.ai[0];
        public ref float UseTime => ref Projectile.ai[1];
        public Vector2 ArmPosition => Owner.RotatedRelativePoint(Owner.MountedCenter, true) + new Vector2((Owner.altFunctionUse == 2 ? 8f : 10f) * Owner.direction, (Owner.altFunctionUse == 2 ? 2f : 12f) - MathHelper.Clamp((Projectile.Center.Y - Main.MouseWorld.Y) * 0.02f, -6, 6)).RotatedBy(Projectile.rotation) * EaseFunction.EaseBackOut.Ease(Timer < 50f ? Timer / 50f : 1f);
        //public Vector2 ArmPosition => Owner.RotatedRelativePoint(Owner.MountedCenter, true) + new Vector2(18f + MathHelper.Lerp(0f, -6f, EaseFunction.EaseQuarticOut.Ease(Timer < 100f ? Timer / 100f : 1f)), -4f * Owner.direction).RotatedBy(Projectile.velocity.ToRotation());
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

            if (flashTimer > 0)
                flashTimer--;

            if (!CanHold())
            {
                Projectile.Kill();
                Owner.reuseDelay = 30;
                return;
            }

            if (Timer == 0f)
            {
                Projectile.velocity = Owner.DirectionTo(Main.MouseWorld);
                UseTime = Owner.itemTimeMax;
            }

            if (Timer % UseTime == 0)
            {
                if (Main.myPlayer == Projectile.owner)
                    if (Owner.UseBeeResource(Owner.altFunctionUse == 2 ? (Owner.HeldItem.ModItem as ApiaryItem).altHoneyCost : (Owner.HeldItem.ModItem as ApiaryItem).honeyCost))
                    {
                        Shoot();
                        Owner.GetModPlayer<AccessoryPlayer>().OnWeaponUse(Projectile.damage, Projectile.knockBack);
                    }             
                    else
                        Projectile.Kill();
            }


            if (Owner.altFunctionUse == 2 && Main.rand.NextBool(30))
            {
                Color color = GlowColor with { A = 0 };

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), DustType<PixelStar>(), Vector2.Zero, 0, color, 0.25f);
            }

            Timer++;

            (Owner.HeldItem.ModItem as ApiaryItem).UpdateApiaryPlayer(Owner, Owner.altFunctionUse == 2);

            UpdateHeldProjectile();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Timer <= 2)
                return false;

            Texture2D tex = Request<Texture2D>(Texture).Value;
            Texture2D texGlow = Request<Texture2D>(UseDefaultTextures ? AssetDirectory.ApiaryGlow : Texture + "_Glow").Value;
            Texture2D texOutline = Request<Texture2D>(UseDefaultTextures ? AssetDirectory.ApiaryOutline : Texture + "_Outline").Value;

            //var starTexture = Request<Texture2D>(AssetDirectory.ExtraTextures + "StarAlpha").Value;

            SpriteEffects spriteEffects = Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Vector2 position = ArmPosition - Owner.velocity * 0.2f - Main.screenPosition;

            if (shakeTimer > 0)
                position += Main.rand.NextVector2CircularEdge(shakeTimer / 8.5f, shakeTimer / 8.5f);

            float rotation = Projectile.rotation;

            Main.spriteBatch.Draw(texGlow, position, null, GlowColor with { A = 0 } * 0.55f * (Timer < 15 ? Timer / 15f : 1f), rotation, texGlow.Size() / 2f, Projectile.scale, spriteEffects, 0f);

            Main.spriteBatch.Draw(tex, position, null, lightColor * (Timer < 15 ? Timer / 15f : 1f), rotation, tex.Size() / 2f, Projectile.scale, spriteEffects, 0f);

            if (flashTimer > 0)
                Main.spriteBatch.Draw(texGlow, position, null, GlowColor with { A = 0 } * (flashTimer / 20f) * 0.5f, rotation, texGlow.Size() / 2f, Projectile.scale, spriteEffects, 0f);

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
            flashTimer = 20;
            swingRotation += Main.rand.NextFloat(-0.15f, 0.15f);
            shakeTimer = 12;

            
            SoundID.Item97.PlayWith(Projectile.Center, 0, 0.1f, 1.25f);
            BombusApisBee.HoneycombWeapon.PlayWith(Projectile.Center, volume: 0.5f);

            for (int j = 0; j < 4; j++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Honey2, Main.rand.NextVector2Circular(3f, 3f), 50, default, 1.2f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, DustType<PixelatedGlow>(), Main.rand.NextVector2Circular(3f, 3f), 0, GlowColor with { A = 0 }, 0.15f);
            }

            Vector2 offset = Main.rand.NextVector2Circular(15f, 15f);

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + offset, Projectile.velocity.RotatedByRandom(1f) * 2f + Main.rand.NextVector2CircularEdge(1f, 1f), ProjectileTypeToFire, Projectile.damage, Projectile.knockBack, Projectile.owner);
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

            //Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.rotation += Owner.velocity.X * 0.01f;
            Projectile.rotation *= 0.95f;

            float restoringForce = -swingRotation * 0.1f;
            float playerAcceleration = Owner.velocity.X - Owner.oldVelocity.X;
            if (Owner.velocity.Y != 0)
                playerAcceleration *= 0.5f;
           
            angularVelocity += restoringForce + (playerAcceleration * 0.01f * Owner.direction);
            angularVelocity *= 0.9f;
            swingRotation += angularVelocity;
            swingRotation = MathHelper.Clamp(swingRotation, -0.9f, 0.9f);

            Projectile.rotation = swingRotation * Owner.direction;

            Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();

            float adjustRotation = Projectile.Center.Y - Main.MouseWorld.Y;
            adjustRotation *= 0.002f;

            adjustRotation = MathHelper.Clamp(adjustRotation, -0.4f, 0.4f) * -Owner.direction;

            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, adjustRotation + Projectile.rotation + (Owner.direction == -1 ? MathHelper.Pi : 0f) - MathHelper.ToRadians(90f) + (Owner.altFunctionUse == 2 ? -0.9f * Owner.direction : 0f));

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
