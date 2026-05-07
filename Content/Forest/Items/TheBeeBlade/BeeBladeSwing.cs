using System.IO;

namespace BombusApisBee.Content.Forest.Items.TheBeeBlade
{
    public class BeeBladeSwing : BeeProjectile
    {
        public override string Texture => "BombusApisBee/Content/Forest/Items/TheBeeBlade/TheBeeBlade";

        public float SwingDirection
        {
            get
            {
                return Projectile.ai[0] * Math.Sign(direction.X);
            }
        }
        public ref float HasFired
        {
            get
            {
                return ref Projectile.localAI[0];
            }
        }
        public Vector2 DistanceFromPlayer
        {
            get
            {
                return direction * 30f;
            }
        }
        public float Timer
        {
            get
            {
                return MaxTimeLeft - Projectile.timeLeft;
            }
        }
        public Player Owner
        {
            get
            {
                return Main.player[Projectile.owner];
            }
        }
        public int MaxTimeLeft;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bee Blade");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SafeSetDefaults()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.Bombus().HeldProj = true;
            Projectile.timeLeft = 2;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLength = 95f * Projectile.scale;
            Vector2 holdPoint = DistanceFromPlayer.Length() * Projectile.rotation.ToRotationVector2();
            return new bool?(Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center + holdPoint, Owner.Center + holdPoint + Projectile.rotation.ToRotationVector2() * bladeLength, 24f, ref collisionPoint));
        }

        internal float SwingRatio()
        {
            return BeeUtils.PiecewiseAnimation(Timer / (Owner.GetActiveItem().useAnimation * 2), new BeeUtils.CurveSegment[]
            {
                anticipation,
                thrust,
                hold
            });
        }

        public static readonly SoundStyle SlashSound = SoundID.DD2_SonicBoomBladeSlash with
        {
            Volume = 4f
        };
        public override void AI()
        {
            if (!init)
            {
                Projectile.timeLeft = Owner.itemAnimation * 2;
                MaxTimeLeft = Projectile.timeLeft;
                SoundEngine.PlaySound(SlashSound, Projectile.Center);
                direction = Projectile.velocity;
                direction.Normalize();
                Projectile.rotation = direction.ToRotation();
                init = true;
                Projectile.netUpdate = true;
                Projectile.netSpam = 0;
            }
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, Projectile.rotation - MathHelper.ToRadians(60f));
            Projectile.Center = Owner.Center + DistanceFromPlayer;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Lerp(SwingWidth / 2f * SwingDirection, -SwingWidth / 2f * SwingDirection, SwingRatio());
            Projectile.scale = 1.5f + (float)Math.Sin(SwingRatio() * 3.1415927f) * 0.6f * 0.6f;
            if (Owner.whoAmI == Main.myPlayer && SwingRatio() > 0.5f && HasFired == 0f)
            {
                Vector2 Value = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
                Vector2 ShootVelocity = Vector2.Normalize(Value - Projectile.Center);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center + direction * 30f, ShootVelocity * 20, ProjectileType<BeeSwordSlash>(), (int)(Projectile.damage * 0.65f), 2f, Owner.whoAmI, 0f, 0f);
                Owner.GetModPlayer<BombusApisBeePlayer>().AddShake(10);
                HasFired = 1f;
            }
            Owner.heldProj = Projectile.whoAmI;
            Owner.direction = Math.Sign(Projectile.velocity.X);
            Owner.itemRotation = Projectile.rotation;
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= 3.1415927f;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D sword = (Texture2D)Request<Texture2D>(Texture);
            SpriteEffects flip = (SpriteEffects)(Owner.direction < 0 ? 1 : 0);
            float extraAngle = Owner.direction < 0 ? 1.5707964f : 0f;
            float drawAngle = Projectile.rotation;
            float drawRotation = Projectile.rotation + 0.7853982f + extraAngle;
            Vector2 drawOrigin = new Vector2(Owner.direction < 0 ? sword.Width : 0f, sword.Height);
            Vector2 drawOffset = Owner.Center + drawAngle.ToRotationVector2() * 2.5f - Main.screenPosition;
            if (Timer > ProjectileID.Sets.TrailCacheLength[Projectile.type])
            {
                for (int i = 0; i < Projectile.oldRot.Length; i++)
                {
                    Color color = Main.hslToRgb(i / (float)Projectile.oldRot.Length * 34f, 1f, 55.9f);
                    float afterimageRotation = Projectile.oldRot[i] + 0.7853982f;
                    Main.spriteBatch.Draw(sword, drawOffset, null, color * 0.1f, afterimageRotation + extraAngle, drawOrigin, Projectile.scale - 0.5f * (i / (float)Projectile.oldRot.Length), flip, 0f);
                }
            }
            Main.spriteBatch.Draw(sword, drawOffset, null, lightColor, drawRotation, drawOrigin, Projectile.scale, flip, 0f);
            Main.spriteBatch.Draw(sword, drawOffset, null, Color.Lerp(lightColor, Color.White, 0.75f), drawRotation, drawOrigin, Projectile.scale, flip, 0f);
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(init);
            writer.WriteVector2(direction);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            init = reader.ReadBoolean();
            direction = reader.ReadVector2();
        }

        private bool init;
        private Vector2 direction = Vector2.Zero;
        private float SwingWidth = 2.5f;
        public BeeUtils.CurveSegment anticipation = new BeeUtils.CurveSegment(BeeUtils.EasingType.ExpOut, 0f, 0f, 0.15f, 1);
        public BeeUtils.CurveSegment thrust = new BeeUtils.CurveSegment(BeeUtils.EasingType.PolyInOut, 0.1f, 0.15f, 0.85f, 3);
        public BeeUtils.CurveSegment hold = new BeeUtils.CurveSegment(BeeUtils.EasingType.Linear, 0.5f, 1f, 0.2f, 1);
        public BeeUtils.CurveSegment retract = new BeeUtils.CurveSegment(BeeUtils.EasingType.PolyInOut, 0.7f, 0.9f, -0.9f, 3);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Owner.whoAmI == Main.myPlayer)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 center = new Vector2(Owner.Center.X, Owner.Center.Y + Main.rand.NextFloat(-55f, 55f));
                    Projectile.NewProjectile(Projectile.GetSource_OnHit(target), center - direction * Main.rand.NextFloat(95f, 135f), Projectile.velocity * 1.5f, ProjectileType<BeeBladeStinger>(), (int)(Projectile.damage * 0.25f), 2f, Owner.whoAmI, 0f, 0f);
                }
            }
        }
    }
}