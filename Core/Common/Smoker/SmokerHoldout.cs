using BombusApisBee.Core.BeekeeperClass;
using BombusApisBee.Core.Common.BeeProjectile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombusApisBee.Core.Common.Smoker
{
    public abstract class SmokerHoldout : ModProjectile
    {
        public SmokerHoldout(int smokerItemType, int frameCount, int maxAttackTime)
        {
            _smokerItemType = smokerItemType;
            _frameCount = frameCount;
            _maxAttackTime = maxAttackTime;
        }

        // should be assigned to the SmokerItem this holdout is associated with
        private int _smokerItemType;
        private int _frameCount;

        private int TrueFrameCount => _frameCount - 1;

        private int _maxAttackTime;

        public bool _dying;
        public int _recoilTimer;

        public int UseTime;

        public int Timer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public int AttackCooldown
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public int AttackTime
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }

        // if the player can hold the held projectile
        public bool CanHold => Owner.HeldItem.ModItem.Type == _smokerItemType && Owner.channel && !Owner.CCed && !Owner.noItems;

        public Vector2 ArmPosition => Owner.RotatedRelativePoint(Owner.MountedCenter, true) + new Vector2(20f, 0f).RotatedBy(Projectile.rotation) + ArmOffset;
        public Vector2 ArmOffset;
        public Player Owner => Main.player[Projectile.owner];

        public override bool? CanDamage() => false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = _frameCount;
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.frame = TrueFrameCount;
        }

        public override void AI()
        {
            if (!CanHold && !_dying && Timer > 10f && AttackTime <= 0)
            {
                _dying = true;
                Projectile.timeLeft = 10;
            }

            if (AttackTime > 0)
            {
                AttackTime--;

                Projectile.frame = Utils.Clamp((int)MathHelper.Lerp(TrueFrameCount, 0, 1f - AttackTime / (float)_maxAttackTime), 0, TrueFrameCount);
            }
            else if (AttackCooldown > 0)
            {
                AttackCooldown--;

                float interpolant = 1f - AttackCooldown / (float)(UseTime + _maxAttackTime);

                Projectile.frame = Utils.Clamp((int)MathHelper.Lerp(0, TrueFrameCount, interpolant), 0, TrueFrameCount);
            }

            if (_recoilTimer > 0)
            {
                int offset = (int)MathHelper.Min(60, _recoilTimer);

                ArmOffset = new Vector2(-15f * (offset / 60f), 0f).RotatedBy(Projectile.rotation);

                _recoilTimer--;
            }

            if (Timer == 0f)
            {
                // we would like to run code that deals with the players mouse only on the players client (for multiplayer purposes)
                if (Main.myPlayer == Projectile.owner)
                    Projectile.velocity = Owner.DirectionTo(Main.MouseWorld);

                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.netUpdate = true;
                UseTime = CombinedHooks.TotalUseTime(Owner.itemTime, Owner, Owner.HeldItem);
            }

            if (!_dying)
            {
                if (AttackCooldown <= 0)
                {
                    AttackTime = _maxAttackTime;
                    AttackCooldown = _maxAttackTime + UseTime;
                }

                UpdateHeldProjectile();

                Timer++;

                // this code runs once every five ticks when attack time is greater than zero
                const int ticks = 5;
                if (AttackTime > 0 && AttackTime % ticks == 0)
                {
                    if (!Owner.UseBeeResource((Owner.HeldItem.ModItem as SmokerItem).honeyCost))
                    {
                        _dying = true;
                        Projectile.timeLeft = 10;
                    }
                    else
                        AttackBehavior();           
                }
            }
            else
            {
                UpdateHeldProjectile(false, false);
            }
        }

        /// <summary>
        /// Used for spawning projectiles, recoil, etc
        /// Call base for default recoil and rotational randomness
        /// </summary>
        public virtual void AttackBehavior()
        {
            Projectile.velocity = Projectile.velocity.RotatedByRandom(0.15f);
            _recoilTimer += Main.rand.Next(7, 15);
        }

        /// <summary>
        /// Updates the basic variables needed for a held projectile
        /// </summary>
        protected void UpdateHeldProjectile(bool updateTimeleft = true, bool updateVelocity = true)
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

                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Owner.DirectionTo(Main.MouseWorld), 0.05f);

                if (Projectile.velocity != oldVelocity)
                {
                    Projectile.netSpam = 0;
                    Projectile.netUpdate = true;
                }
            }

            Projectile.spriteDirection = Projectile.direction;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var tex = Request<Texture2D>(Texture).Value;

            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Vector2 position = ArmPosition - Main.screenPosition;

            float rotation = Projectile.rotation + (spriteEffects == SpriteEffects.FlipHorizontally ? MathHelper.Pi : 0f) * Projectile.spriteDirection;

            float fadeIn = 1f;

            var frame = tex.Frame(1, _frameCount, 0, Projectile.frame);

            if (Timer < 10f)
                fadeIn = Timer / 10f;
            else if (_dying)
                fadeIn = Projectile.timeLeft / 10f;

            Main.spriteBatch.Draw(tex, position, frame, lightColor * fadeIn, rotation, frame.Size() / 2f, Projectile.scale, spriteEffects, 0f);

            return false;
        }
    }

    /// <summary>
    /// Handles bee and npc SmokerBuff procs
    /// </summary>
    public abstract class SmokerSmokeProjectile : ModProjectile
    {
        public static int _buffID;
        public static int _buffTime;

        public override void AI()
        {
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.active && p.owner == Projectile.owner && p.TryGetGlobalProjectile<SmokerGlobalProjectile>(out var gp) && p.Hitbox.Intersects(Projectile.Hitbox))
                {
                    SmokerGlobalProjectile.ApplySmokerBuff(p, _buffID, _buffTime);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.TryGetGlobalNPC<SmokerGlobalNPC>(out var gnpc))
            {
                SmokerGlobalNPC.ApplySmokerBuff(target, _buffID, _buffTime);
            }
        }
    }
}
