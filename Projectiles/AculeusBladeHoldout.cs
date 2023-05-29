using BombusApisBee.Items.Weapons.BeeKeeperDamageClass;
using BombusApisBee.PrimitiveDrawing;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.Graphics.Effects;

namespace BombusApisBee.Projectiles
{
    public class AculeusBladeHoldout : BeeProjectile
    {
        public override string Texture => BombusApisBee.BeeWeapon + "BladeOfAculeus";
        private bool initialized;

        private bool swung;

        private float maxTimeLeft;
        private float originalDirection;
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
            Projectile.localNPCHitCooldown = 10;
            Projectile.Bombus().HeldProj = true;
        }

        public override void AI()
        {
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

            UpdateProj();
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
                owner.itemTime = Projectile.timeLeft;
                owner.itemAnimation = owner.itemTime;
            }

            float progress = 1f - Projectile.timeLeft / maxTimeLeft;

            if (progress < 0.7f)
            {
                float lerper = 1f - (Projectile.timeLeft - maxTimeLeft * 0.3f) / (maxTimeLeft * 0.7f);

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Lerp(0f, 0.35f, EaseBuilder.EaseCubicInOut.Ease(lerper)) * originalDirection;

                Vector2 offset = Vector2.Lerp(new Vector2(0), new Vector2(6, 6 * originalDirection), EaseBuilder.EaseCubicInOut.Ease(lerper)).RotatedBy(Projectile.rotation - MathHelper.PiOver4);

                Projectile.Center = owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * MathHelper.Lerp(0f, -25f, EaseBuilder.EaseCubicOut.Ease(lerper)) + offset;

                float armRot = (Projectile.velocity.ToRotation() - MathHelper.PiOver2) + MathHelper.Lerp(0f, MathHelper.ToRadians(200), EaseBuilder.EaseCubicInOut.Ease(lerper)) * originalDirection;

                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);
            }
            else
            {
                if (!swung)
                {
                    SoundID.DD2_MonkStaffSwing.PlayWith(Projectile.Center);
                    owner.Bombus().AddShake(7);
                    swung = true;
                }

                float lerper = 1f - (Projectile.timeLeft) / (float)Math.Floor(maxTimeLeft - maxTimeLeft * 0.7f);

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Lerp(0.35f, -0.1f, EaseBuilder.EaseQuinticOut.Ease(lerper)) * originalDirection;

                Vector2 offset = Vector2.Lerp(new Vector2(6, 6 * originalDirection), new Vector2(0), EaseBuilder.EaseCubicInOut.Ease(lerper)).RotatedBy(Projectile.rotation - MathHelper.PiOver4);

                Projectile.Center = owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * MathHelper.Lerp(-25f, 35f, EaseBuilder.EaseQuinticOut.Ease(lerper)) + offset;

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
                owner.itemTime = Projectile.timeLeft;
                owner.itemAnimation = owner.itemTime;
            }

            float progress = 1f - Projectile.timeLeft / maxTimeLeft;

            if (progress < 0.55f)
            {
                float lerper = 1f - (Projectile.timeLeft - maxTimeLeft * 0.45f) / (maxTimeLeft * 0.55f);

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Lerp(-0.1f, -2f, EaseBuilder.EaseCubicInOut.Ease(lerper)) * originalDirection;

                Vector2 offset = Vector2.Lerp(new Vector2(0), new Vector2(20, -10 * originalDirection), EaseBuilder.EaseCubicInOut.Ease(lerper)).RotatedBy(Projectile.rotation - MathHelper.PiOver4);

                Projectile.Center = owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * MathHelper.Lerp(35f, 30f, EaseBuilder.EaseCubicOut.Ease(lerper)) + offset;

                float armRot = (Projectile.velocity.ToRotation() - MathHelper.PiOver2) + MathHelper.Lerp(0f, -MathHelper.ToRadians(120), EaseBuilder.EaseCubicInOut.Ease(lerper)) * originalDirection;

                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);

                Projectile.scale = MathHelper.Lerp(1f, 0.8f, lerper);
            }
            else
            {
                if (!swung)
                {
                    SoundID.DD2_MonkStaffSwing.PlayWith(Projectile.Center);
                    owner.Bombus().AddShake(7);
                    swung = true;
                }

                float lerper = 1f - (Projectile.timeLeft) / (float)Math.Floor(maxTimeLeft - maxTimeLeft * 0.55f);

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Lerp(-0.65f, 2f, EaseBuilder.EaseQuinticOut.Ease(lerper)) * originalDirection;

                Vector2 offset = Vector2.Lerp(new Vector2(20, -16 * originalDirection), new Vector2(5, 15 * originalDirection), EaseBuilder.EaseQuinticOut.Ease(lerper)).RotatedBy(Projectile.rotation - MathHelper.PiOver4);

                Projectile.Center = owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * MathHelper.Lerp(30f, 35f, EaseBuilder.EaseQuinticOut.Ease(lerper)) + offset;

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
                owner.itemTime = Projectile.timeLeft;
                owner.itemAnimation = owner.itemTime;
            }

            float progress = 1f - Projectile.timeLeft / maxTimeLeft;

            if (progress < 0.35f)
            {
                float lerper = 1f - (Projectile.timeLeft - maxTimeLeft * 0.65f) / (maxTimeLeft * 0.35f);

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Lerp(2, 3.5f, EaseBuilder.EaseCircularInOut.Ease(lerper)) * originalDirection;

                Vector2 offset = Vector2.Lerp(new Vector2(5, 15 * originalDirection), new Vector2(15, -5 * originalDirection), EaseBuilder.Linear.Ease(lerper)).RotatedBy(Projectile.rotation - MathHelper.PiOver4);

                Projectile.Center = owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * MathHelper.Lerp(35f, 20f, EaseBuilder.EaseCircularInOut.Ease(lerper)) + offset;

                float armRot = (Projectile.velocity.ToRotation() - MathHelper.PiOver2) + MathHelper.Lerp(MathHelper.ToRadians(150), MathHelper.ToRadians(180), EaseBuilder.EaseCircularInOut.Ease(lerper)) * originalDirection;

                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);

                Projectile.scale = MathHelper.Lerp(0.8f, 0.85f, lerper);
            }
            else
            {
                if (!swung && progress >= 0.7f)
                {
                    SoundID.DD2_MonkStaffSwing.PlayWith(Projectile.Center);
                    owner.Bombus().AddShake(7);
                    swung = true;
                }

                float lerper = 1f - (Projectile.timeLeft) / (float)Math.Floor(maxTimeLeft - maxTimeLeft * 0.35f);

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Lerp(3.5f, -1.35f, EaseBuilder.EaseQuinticInOut.Ease(lerper)) * originalDirection;

                Vector2 offset = Vector2.Lerp(new Vector2(15, -5 * originalDirection), new Vector2(-5, -2 * originalDirection), EaseBuilder.EaseQuinticOut.Ease(lerper)).RotatedBy(Projectile.rotation - MathHelper.PiOver4);

                Projectile.Center = owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * MathHelper.Lerp(20f, 40f, EaseBuilder.EaseQuinticOut.Ease(lerper)) + offset;

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
                owner.itemTime = Projectile.timeLeft;
                owner.itemAnimation = owner.itemTime;
            }

            float progress = 1f - Projectile.timeLeft / maxTimeLeft;

            if (progress < 0.3f)
            {
                float lerper = 1f - (Projectile.timeLeft - maxTimeLeft * 0.7f) / (maxTimeLeft * 0.3f);

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Lerp(-1.35f, -2.25f, EaseBuilder.EaseCircularInOut.Ease(lerper)) * originalDirection;

                Vector2 offset = Vector2.Lerp(new Vector2(-5, -2 * originalDirection), new Vector2(10, 5 * originalDirection), EaseBuilder.Linear.Ease(lerper)).RotatedBy(Projectile.rotation - MathHelper.PiOver4);

                Projectile.Center = owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * MathHelper.Lerp(40f, 45f, EaseBuilder.EaseCubicInOut.Ease(lerper)) + offset;

                float armRot = (Projectile.velocity.ToRotation() - MathHelper.PiOver2) + MathHelper.Lerp(-MathHelper.ToRadians(70), -MathHelper.ToRadians(100), EaseBuilder.EaseCubicInOut.Ease(lerper)) * originalDirection;

                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);

                Projectile.scale = MathHelper.Lerp(0.85f, 1f, lerper);
            }
            else
            {
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

                float armRot = (Projectile.velocity.ToRotation() - MathHelper.PiOver2) + MathHelper.Lerp(-MathHelper.ToRadians(100), MathHelper.ToRadians(380), EaseBuilder.EaseCircularInOut.Ease(lerper)) * originalDirection;

                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);

                Projectile.scale = 1f + (float)Math.Sin(EaseBuilder.EaseCubicInOut.Ease(lerper) * 3.1415927f) * 0.6f * 0.6f;
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
                owner.itemTime = Projectile.timeLeft;
                owner.itemAnimation = owner.itemTime;
            }

            float progress = 1f - Projectile.timeLeft / maxTimeLeft;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Lerp(6f - MathHelper.TwoPi, 0f, EaseBuilder.EaseQuinticOut.Ease(progress)) * originalDirection;

            Vector2 offset = Vector2.Lerp(new Vector2(-5, 5 * originalDirection), new Vector2(0), progress).RotatedBy(Projectile.rotation - MathHelper.PiOver4);

            Projectile.Center = owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * MathHelper.Lerp(30f, 0f, EaseBuilder.EaseQuinticOut.Ease(progress)) + offset;

            float armRot = (Projectile.velocity.ToRotation() - MathHelper.PiOver2) + MathHelper.Lerp(MathHelper.ToRadians(380) - MathHelper.ToRadians(360), 0f, EaseBuilder.EaseQuinticOut.Ease(progress)) * originalDirection;

            owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);
        }

        private void UpdateProj()
        {
            if (!(owner.HeldItem.ModItem is BladeOfAculeus))
                Projectile.Kill();

            owner.heldProj = Projectile.whoAmI;

            owner.ChangeDir(Projectile.direction);
        }
        public override bool? CanDamage()
        {
            return Combo != 4;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            switch (Combo)
            {
                case 0: //stab
                    damage = (int)(damage * 1f);
                    break;
                case 1: //up
                    damage = (int)(damage * 1.25f);
                    break;
                case 2: //down
                    damage = (int)(damage * 1.75f);
                    break;
                case 3: //spin
                    damage = (int)(damage * 2f);
                    break;
            }
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
    }

    public class AculeusBladeHoldoutAlt : BeeProjectile
    {
        public override string Texture => BombusApisBee.BeeWeapon + "BladeOfAculeus";

        private int maxTimeLeft;

        private bool initialized;

        private bool pullingBack;

        private bool thrown;

        private int lerpTimer;

        private int originalDirection;

        private Vector2 oldPos;
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
            Projectile.localNPCHitCooldown = 10;
            Projectile.Bombus().HeldProj = true;
        }

        public override void AI()
        {
            if (!initialized)
            {
                initialized = true;
                Projectile.timeLeft = (int)owner.ApplyHymenoptraSpeedTo((int)(owner.HeldItem.useAnimation * 0.75f));
                maxTimeLeft = Projectile.timeLeft;
                Projectile.rotation = owner.DirectionTo(Projectile.Center + Projectile.velocity).ToRotation() + MathHelper.PiOver4;
                Projectile.netUpdate = true;
                Projectile.direction = Main.MouseWorld.X < owner.Center.X ? -1 : 1;
                owner.itemTime = Projectile.timeLeft;
                owner.itemAnimation = owner.itemTime;
            }   

            if (thrown)
            {
                if (pullingBack)
                {
                    Projectile.rotation = Projectile.DirectionTo(owner.Center).ToRotation() + MathHelper.PiOver4 + MathHelper.Pi;

                    owner.ChangeDir(Projectile.Center.X < owner.Center.X ? -1 : 1);
                    
                    owner.heldProj = Projectile.whoAmI;

                    Projectile.timeLeft = 2;

                    lerpTimer++;

                    if (lerpTimer == 1)
                    {
                        oldPos = Projectile.Center;
                    }

                    if (lerpTimer < 15)
                    {
                        float lerper = lerpTimer / 15f;

                        Projectile.Center = Vector2.Lerp(oldPos, oldPos + Projectile.velocity * 3f, EaseBuilder.EaseCubicOut.Ease(lerper));
                    }
                    else
                    {
                        float lerper = (lerpTimer - 15) / 30f;

                        Projectile.Center = Vector2.Lerp(oldPos + Projectile.velocity * 3f, owner.Center, EaseBuilder.EaseCubicIn.Ease(lerper));

                        if (lerpTimer >= 44)
                            Projectile.Kill();
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
                    Projectile.velocity *= 25;
                    Projectile.timeLeft = 10;
                    Projectile.tileCollide = false;
                    thrown = true;
                }
            }

            owner.heldProj = Projectile.whoAmI;

            if (Main.myPlayer == owner.whoAmI)
                Projectile.direction = Main.MouseWorld.X < owner.Center.X ? -1 : 1;

            owner.ChangeDir(Projectile.direction);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            pullingBack = true;
            return false;
        }
    }
}
