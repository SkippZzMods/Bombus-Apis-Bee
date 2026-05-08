using BombusApisBee.Core.BeekeeperClass;
using BombusApisBee.Core.Common.Apiary;
using Terraria.DataStructures;

namespace BombusApisBee.Core.Common.BeeProjectile
{
    public abstract class CommonBeeProjectile : ModProjectile
    {
        internal const int DEFAULT_SIZE = 8;
        internal const int DEFAULT_GIANT_SIZE = 16;

        internal const float DEFAULT_SPEED = 7f;
        internal const float DEFAULT_GIANT_SPEED = 8.25f;

        internal const float DEFAULT_RANGE = 1000f;
        internal const float DEFAULT_GIANT_RANGE = 1200f;

        public bool fromApiary;

        internal bool _canBeGiant;
        internal bool _initialized;

        internal int _frameTimer;
        internal int _projFrames;

        // Amount of i-Frames to give to targets on hit, defaults to 10
        internal int _hitCooldown;
        internal int _penetrate;

        internal float _homingSpeed;
        internal float _giantHomingSpeed;

        internal float _homingRange;
        internal float _giantHomingRange;

        internal string _displayName;

        internal Vector2 _giantSize = new(DEFAULT_GIANT_SIZE);
        internal Vector2 _size = new(DEFAULT_SIZE);

        public float speedMultiplier = 1f;

        public bool Giant => _canBeGiant && Projectile.ai[0] == 1f;
        public float Speed => (Giant ? _giantHomingSpeed : _homingSpeed) * speedMultiplier;
        public float Range => Giant ? _giantHomingRange : _homingRange;
        public Vector2 Size => Giant ? _giantSize : _size;
        public int Timer
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }

        public NPC Target = null;

        protected CommonBeeProjectile(
            bool canBeGiant = true,
            int frameTimer = 3,
            int frameCount = 4,
            int hitCooldown = 10,
            int penetrate = 3,
            float speed = DEFAULT_SPEED,
            float giantSpeed = DEFAULT_GIANT_SPEED,
            float range = DEFAULT_RANGE,
            float giantRange = DEFAULT_GIANT_RANGE,
            string name = "Bee",
            Vector2? size = null,
            Vector2? giantSize = null) : base()
        {
            _canBeGiant = canBeGiant;

            _frameTimer = frameTimer;
            _projFrames = frameCount;

            _hitCooldown = hitCooldown;
            _penetrate = penetrate;

            _homingSpeed = speed;
            _giantHomingSpeed = giantSpeed;

            _homingRange = range;
            _giantHomingRange = giantRange;

            _displayName = name;

            if (size.HasValue)
                _size = size.Value;

            if (giantSize.HasValue)
                _giantSize = giantSize.Value;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(_displayName);
            Main.projFrames[Type] = _projFrames;
        }

        public virtual void SafeAI()
        {

        }

        public virtual void SafeSetDefaults()
        {

        }

        public override void OnSpawn(IEntitySource source)
        {
            if (!_canBeGiant)
                return;

            if (source is EntitySource_ItemUse_WithAmmo { Entity: Player p, Item: Item i })
            {
                if (i.ModItem is ApiaryItem)
                    fromApiary = true;
            }

            if (Main.player[Projectile.owner].Beekeeper().BeeStrengthenChance > 0f)
                Projectile.ai[0] = Main.rand.NextFloat() < Main.player[Projectile.owner].Beekeeper().BeeStrengthenChance ? 1f : 0f;

            Projectile.width = (int)Size.X;
            Projectile.height = (int)Size.Y;
        }

        public sealed override bool? CanHitNPC(NPC target)
        {
            if (target.GetGlobalNPC<BombusApisBeeGlobalNPCs>().BeeHitCooldown[Projectile.owner] > 0)
                return false;

            return null;
        }

        public sealed override void SetDefaults()
        {
            Projectile.DamageType = GetInstance<BeekeeperDamage>();

            Projectile.extraUpdates = 1;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 1200;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.penetrate = _penetrate;

            Projectile.usesIDStaticNPCImmunity = true; //uses custom i-Frame logic
            Projectile.idStaticNPCHitCooldown = _hitCooldown;

            SafeSetDefaults();
        }

        public sealed override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.penetrate > 0)
            {
                Projectile.penetrate--;
                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = -oldVelocity.X;

                if (Projectile.velocity.Y != oldVelocity.Y)
                    Projectile.velocity.Y = -oldVelocity.Y;
            }

            return false;
        }

        public virtual void SafePreAI() { }
        public sealed override bool PreAI()
        {
            speedMultiplier = 1f;

            SafePreAI();
            Player player = Main.player[Projectile.owner];

            if (player.GetModPlayer<BombusApisBeePlayer>().IgnoreWater)
                Projectile.ignoreWater = true;
            else if (Projectile.wet && !Projectile.honeyWet)
                Projectile.Kill();
            
            return base.PreAI();
        }
        public sealed override void AI()
        {
            SafeAI();

            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : 3.1415927f);
            Projectile.rotation += Projectile.spriteDirection * MathHelper.ToRadians(45f);

            if (++Projectile.frameCounter >= _frameTimer)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }

            if (++Timer > 30)
            {
                if (Target is null)
                    DefaultBehavior();
                else
                    HomingBehavior(Target);
            }
        }

        internal NPC GetTarget()
        {
            const int TARGET_THRESHOLD = 5;

            int[] beeCounts = new int[Main.maxNPCs];

            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.type == Type && p.owner == Projectile.owner)
                    if (p.ModProjectile is CommonBeeProjectile { Target: not null } bee)
                        beeCounts[bee.Target.whoAmI]++;
            }

            NPC closest = null, closestUnderThreshold = null;
            float closestDist = Range * Range, thresholdClosestdist = Range * Range;

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.CanBeChasedBy())
                {
                    float dist = Projectile.DistanceSQ(npc.Center);

                    if (dist >= Range * Range)
                        continue;

                    bool isCloser = dist < closestDist;
                    bool isCloserAndUnderBeeThreshold = dist < thresholdClosestdist && beeCounts[npc.whoAmI] < TARGET_THRESHOLD;

                    if (isCloser || isCloserAndUnderBeeThreshold)
                    {
                        if (!Projectile.tileCollide || Collision.CanHit(Projectile, npc))
                        {
                            if (isCloser)
                            {
                                closestDist = dist;
                                closest = npc;
                            }

                            if (isCloserAndUnderBeeThreshold)
                            {
                                thresholdClosestdist = dist;
                                closestUnderThreshold = npc;
                            }
                        }
                    }
                }
            }

            return closestUnderThreshold ?? closest;
        }

        internal void DefaultBehavior()
        {
            Target = GetTarget();

            Projectile.timeLeft--;

            if (Projectile.velocity.Length() < Speed * 0.66f)
                Projectile.velocity = Projectile.velocity * 1.05f;
            else
                Projectile.velocity = Utils.SafeNormalize(Projectile.velocity, Vector2.One) * Speed * 0.66f;
        }

        internal void HomingBehavior(NPC target)
        {
            // double safety check
            if (target is null)
                return;

            if (!target.active)
            {
                Target = null;
                return;
            }

            float distance = Projectile.Distance(target.Center);

            float range = 500f;
            float speed = Speed;
            float minSpeed = speed / 2;
            float inertia = 40f;

            if (distance < range)
                speed = MathHelper.Lerp(minSpeed, speed, distance / range);

            Projectile.velocity = (Projectile.velocity * inertia + Projectile.DirectionTo(target.Center) * speed) / (inertia + 1f);
        }

        public virtual bool SafePreDraw(ref Color lightColor) { return true; }
        public sealed override bool PreDraw(ref Color lightColor)
        {
            if (SafePreDraw(ref lightColor))
            {
                Texture2D tex = Request<Texture2D>(Texture).Value;
                Texture2D giantTex = Giant ? Request<Texture2D>(Texture + "_Giant").Value : Request<Texture2D>(Texture).Value;
                Rectangle sourceRectangle = Giant ? giantTex.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame) : tex.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);

                Main.spriteBatch.Draw(Giant ? giantTex : tex, Projectile.Center - Main.screenPosition, sourceRectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, sourceRectangle.Size() / 2f,
                    Projectile.scale, Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            }

            return false;
        }

        public virtual void ModifyGiantHit(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= 1.15f;
            modifiers.Knockback *= 1.25f;
        }

        public virtual void SafeModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) { }
        public sealed override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Giant)
                ModifyGiantHit(target, ref modifiers);

            SafeModifyHitNPC(target, ref modifiers);
        }

        public virtual void SafeOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) { }
        public sealed override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.GetGlobalNPC<BombusApisBeeGlobalNPCs>().BeeHitCooldown[Projectile.owner] = _hitCooldown;

            SafeOnHitNPC(target, hit, damageDone);
        }
    }
}