using BombusApisBee.BeeDamageClass;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BombusApisBee.Projectiles
{
    public class BeemerangHoldout : ModProjectile
    {
        public Player owner => Main.player[Projectile.owner];

        public bool charging;

        public int chargeTimer;

        public int maxCharge;

        public int originalDirection;

        public float throwTimer;

        public int flashTimer;

        public bool flashed;

        public ref float boomerangTimer => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beemerang");
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;

            Projectile.friendly = false;
            Projectile.DamageType = BeeUtils.BeeDamageClass();
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.Bombus().HeldProj = true;
        }

        public float GetPullbackRotation()
        {
            return BeeUtils.PiecewiseAnimation((float)chargeTimer / (float)maxCharge, new BeeUtils.CurveSegment[]
            {
                pullback
            });
        }

        public float GetThrowRotation()
        {
            return BeeUtils.PiecewiseAnimation(throwTimer / 6f, new BeeUtils.CurveSegment[]
            {
                throwout
            });
        }

        public override void AI()
        {
            if (flashTimer > 0)
                flashTimer--;

            if (maxCharge == 0)
            {
                maxCharge = (int)(owner.GetActiveItem().useAnimation * (1f - (owner.GetTotalAttackSpeed<HymenoptraDamageClass>() - 1f)));
                originalDirection = owner.direction;
            }
            if (owner.channel || chargeTimer < maxCharge * 0.35f)
            {
                if (chargeTimer < maxCharge)
                    chargeTimer++;
                if (chargeTimer == maxCharge && !flashed)
                {
                    flashed = true;
                    flashTimer = 10;
                    SoundEngine.PlaySound(SoundID.MaxMana, Projectile.position);
                    owner.UseBeeResource(1);
                }

                if (Main.myPlayer == owner.whoAmI)
                    owner.ChangeDir(Main.MouseWorld.X < owner.Center.X ? -1 : 1);
                float armRot = GetPullbackRotation() * owner.direction;
                owner.heldProj = Projectile.whoAmI;
                Projectile.Center = owner.MountedCenter + Vector2.UnitY.RotatedBy((double)(armRot * owner.gravDir), default) * -20f * owner.gravDir;
                Projectile.rotation = (-MathHelper.PiOver2 + armRot) * owner.gravDir;
                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, 3.1415927f + armRot);
                owner.Hymenoptra().BeeResourceRegenTimer = -60;
            }
            else
            {
                throwTimer++;
                if (throwTimer < 6)
                {
                    if (throwTimer == 3)
                        SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing, Projectile.position);

                    if (Main.myPlayer == owner.whoAmI)
                        owner.ChangeDir(Main.MouseWorld.X < owner.Center.X ? -1 : 1);
                    float armRot = GetThrowRotation() * owner.direction;
                    owner.heldProj = Projectile.whoAmI;
                    Projectile.Center = owner.MountedCenter + Vector2.UnitY.RotatedBy((double)(armRot * owner.gravDir), default) * -20f * owner.gravDir;
                    Projectile.rotation = (-MathHelper.PiOver2 + armRot) * owner.gravDir;
                    owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, 3.1415927f + armRot);
                }
                else
                {
                    if (Main.myPlayer == owner.whoAmI && boomerangTimer == 0)
                    {
                        owner.UseBeeResource(3);
                        Projectile.friendly = true;
                        Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * MathHelper.Lerp(15f, 22.5f, (float)chargeTimer / (float)maxCharge);
                        Projectile.tileCollide = true;
                    }
                    boomerangTimer++;
                    if (boomerangTimer > 30f)
                    {
                        Projectile.tileCollide = false;
                        boomerangTimer = 30f;
                        float speed = 15f;
                        float inertia = 1f;
                        Vector2 pos = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                        float num = owner.position.X + (float)(owner.width / 2) - pos.X;
                        float num2 = owner.position.Y + (float)(owner.height / 2) - pos.Y;
                        float num3 = (float)Math.Sqrt(num * num + num2 * num2);
                        if (num3 > 3000f)
                            Projectile.Kill();
                        num3 = speed / num3;
                        num *= num3;
                        num2 *= num3;
                        if (Projectile.velocity.X < num)
                        {
                            Projectile.velocity.X += inertia;
                            if (Projectile.velocity.X < 0f && num > 0f)
                            {
                                Projectile.velocity.X += inertia;
                            }
                        }
                        else if (Projectile.velocity.X > num)
                        {
                            Projectile.velocity.X -= inertia;
                            if (Projectile.velocity.X > 0f && num < 0f)
                            {
                                Projectile.velocity.X -= inertia;
                            }
                        }
                        if (Projectile.velocity.Y < num2)
                        {
                            Projectile.velocity.Y += inertia;
                            if (Projectile.velocity.Y < 0f && num2 > 0f)
                            {
                                Projectile.velocity.Y += inertia;
                            }
                        }
                        else if (Projectile.velocity.Y > num2)
                        {
                            Projectile.velocity.Y -= inertia;
                            if (Projectile.velocity.Y > 0f && num2 < 0f)
                            {
                                Projectile.velocity.Y -= inertia;
                            }
                        }

                        if (Projectile.Distance(owner.Center) < 30f)
                            Projectile.Kill();
                    }
                    else
                        Projectile.velocity *= 0.97f;

                    if (Projectile.soundDelay == 0)
                    {
                        Projectile.soundDelay = 8;
                        SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
                    }

                    Projectile.rotation += 0.15f + Projectile.velocity.Length() * 0.025f;

                    owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, owner.Center.DirectionTo(Projectile.Center).ToRotation() - MathHelper.PiOver2);
                    owner.ChangeDir(Projectile.Center.X < owner.Center.X ? -1 : 1);
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = (int)(damage * MathHelper.Lerp(1f, 3f, (float)chargeTimer / (float)maxCharge));
            knockback = knockback * MathHelper.Lerp(1f, 1.5f, (float)chargeTimer / (float)maxCharge);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (boomerangTimer < 30f)
            {
                boomerangTimer = 31f;
                Projectile.velocity *= -1f;
            }

            for (int i = 0; i < (chargeTimer == maxCharge ? 2 : 1); i++)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_OnHit(target), Projectile.Center, Vector2.One.RotatedByRandom(6.28f) * 6f, owner.beeType(),
                    owner.beeDamage(Projectile.damage), owner.beeKB(0.5f), owner.whoAmI).DamageType = BeeUtils.BeeDamageClass();
            }
            const int Repeats = 45;
            for (int i = 0; i < Repeats; ++i)
            {
                float angle2 = 6.2831855f * (float)i / (float)Repeats;
                Dust dust3 = Dust.NewDustPerfect(target.Center, ModContent.DustType<Dusts.HoneyDust>(), null, 0, default(Color), 1.1f);
                dust3.velocity = Utils.ToRotationVector2(angle2) * 1.75f;
                dust3.noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (boomerangTimer < 30f)
            {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
                boomerangTimer = 31f;
                Projectile.velocity *= -1f;
            }
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D textureWhite = ModContent.Request<Texture2D>(Texture + "_White").Value;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            if (throwTimer > 6)
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale - (k / 10f), SpriteEffects.None, 0);
                }
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            if (flashTimer > 0)
                Main.spriteBatch.Draw(textureWhite, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.White, Color.Transparent, 1f - (flashTimer / 10f)), Projectile.rotation, textureWhite.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public BeeUtils.CurveSegment pullback = new BeeUtils.CurveSegment(BeeUtils.EasingType.PolyOut, 0f, 0f, -1.05f, 2);

        public BeeUtils.CurveSegment throwout = new BeeUtils.CurveSegment(BeeUtils.EasingType.PolyOut, 0.7f, -0.9424779f, 2.2132742f, 3);
    }
}
