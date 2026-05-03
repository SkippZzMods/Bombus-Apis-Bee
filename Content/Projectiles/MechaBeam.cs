namespace BombusApisBee.Content.Projectiles
{
    public class MechaBeam : BeeProjectile
    {
        public override void SafeSetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 12;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 9;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)Lifetime;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Beam");
        }
        public override string Texture => BombusApisBee.Invisible;

        public static Color LightCastColor
        {
            get
            {
                return Color.White;
            }
        }
        public static Color LaserOverlayColor
        {
            get
            {
                return Color.White * 0.9f;
            }
        }
        public static float ScaleExpandRate
        {
            get
            {
                return 4f;
            }
        }
        public static float MaxScale
        {
            get
            {
                return 1f;
            }
        }
        public static float MaxLaserLength
        {
            get
            {
                return 1300f;
            }
        }
        public static float Lifetime
        {
            get
            {
                return 35f;
            }
        }
        public float RotationalSpeed
        {
            get
            {
                return Projectile.ai[0];
            }
            set
            {
                Projectile.ai[0] = value;
            }
        }
        public float Time
        {
            get
            {
                return Projectile.localAI[0];
            }
            set
            {
                Projectile.localAI[0] = value;
            }
        }
        public float LaserLength
        {
            get
            {
                return Projectile.localAI[1];
            }
            set
            {
                Projectile.localAI[1] = value;
            }
        }
        public static Texture2D LaserBeginTexture => (Texture2D)Request<Texture2D>("BombusApisBee/Projectiles/MechaBeamStart");
        public static Texture2D LaserMiddleTexture => (Texture2D)Request<Texture2D>("BombusApisBee/Projectiles/MechaBeamMiddle");
        public static Texture2D LaserEndTexture => (Texture2D)Request<Texture2D>("BombusApisBee/Projectiles/MechaBeamEnd");

        public void Behavior()
        {
            Projectile.velocity = Projectile.velocity.SafeNormalize(-Vector2.UnitY);
            float time = Time;
            Time = time + 1f;
            if (Time >= Lifetime)
            {
                Projectile.Kill();
                return;
            }
            DetermineScale();
            UpdateLaserMotion();
            float idealLaserLength = MaxLaserLength;
            LaserLength = MathHelper.Lerp(LaserLength, idealLaserLength, 0.9f);
            if (LightCastColor != Color.Transparent)
            {
                DelegateMethods.v3_1 = LightCastColor.ToVector3();
                Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * LaserLength, Projectile.width * Projectile.scale, new Utils.TileActionAttempt(DelegateMethods.CastLight));
            }
        }
        public void UpdateLaserMotion()
        {
            float updatedVelocityDirection = Projectile.velocity.ToRotation() + RotationalSpeed;
            Projectile.rotation = updatedVelocityDirection - 1.5707964f;
            Projectile.velocity = updatedVelocityDirection.ToRotationVector2();
        }
        public void DetermineScale()
        {
            Projectile.scale = Projectile.timeLeft / Lifetime * MaxScale;
        }
        public override void AI()
        {
            Behavior();
            if (Projectile.timeLeft == 35f)
            {
                if (Main.dedServ)
                {
                    return;
                }
                const int Repeats = 65;
                for (int i = 0; i < Repeats; ++i)
                {
                    float angle2 = 6.2831855f * i / Repeats;
                    Dust dust3 = Dust.NewDustPerfect(Projectile.Center, DustID.Clentaminator_Red, null, 0, new Color(118, 240, 236), 2f);
                    dust3.velocity = angle2.ToRotationVector2() * 3f;
                    dust3.noGravity = true;
                }
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return new bool?(true);
            }
            float _ = 0f;
            return new bool?(Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * LaserLength, Projectile.Size.Length() * Projectile.scale, ref _));
        }
        protected internal void DrawBeamWithColor(SpriteBatch spriteBatch, Color beamColor, float scale, int startFrame = 0, int middleFrame = 0, int endFrame = 0)
        {
            Rectangle startFrameArea = LaserBeginTexture.Frame(1, Main.projFrames[Projectile.type], 0, startFrame);
            Rectangle middleFrameArea = LaserMiddleTexture.Frame(1, Main.projFrames[Projectile.type], 0, middleFrame);
            Rectangle endFrameArea = LaserEndTexture.Frame(1, Main.projFrames[Projectile.type], 0, endFrame);
            Main.spriteBatch.Draw(LaserBeginTexture, Projectile.Center - Main.screenPosition, new Rectangle?(startFrameArea), beamColor, Projectile.rotation, LaserBeginTexture.Size() / 2f, scale, 0, 0f);
            float laserBodyLength = LaserLength;
            laserBodyLength -= (startFrameArea.Height / 2 + endFrameArea.Height) * scale;
            Vector2 centerOnLaser = Projectile.Center;
            centerOnLaser += Projectile.velocity * scale * startFrameArea.Height / 2f;
            if (laserBodyLength > 0f)
            {
                float laserOffset = middleFrameArea.Height * scale;
                float incrementalBodyLength = 0f;
                while (incrementalBodyLength + 1f < laserBodyLength)
                {
                    Main.spriteBatch.Draw(LaserMiddleTexture, centerOnLaser - Main.screenPosition, new Rectangle?(middleFrameArea), beamColor, Projectile.rotation, LaserMiddleTexture.Width * 0.5f * Vector2.UnitX, scale, 0, 0f);
                    incrementalBodyLength += laserOffset;
                    centerOnLaser += Projectile.velocity * laserOffset;
                }
            }
            if (Math.Abs(LaserLength - MaxLaserLength) < 30f)
            {
                Vector2 laserEndCenter = centerOnLaser - Main.screenPosition;
                Main.spriteBatch.Draw(LaserEndTexture, laserEndCenter, new Rectangle?(endFrameArea), beamColor, Projectile.rotation, LaserEndTexture.Frame(1, 1, 0, 0).Top(), scale, 0, 0f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero)
            {
                return false;
            }
            DrawBeamWithColor(Main.spriteBatch, LaserOverlayColor, Projectile.scale, 0, 0, 0);
            DrawBeamWithColor(Main.spriteBatch, Color.Red, Projectile.scale, 0, 0, 0);
            DrawBeamWithColor(Main.spriteBatch, Color.White, Projectile.scale * 0.3f, 0, 0, 0);
            return false;
        }
    }
}
