using BombusApisBee.BeeHelperProj;
using Terraria;

namespace BombusApisBee.Projectiles
{
    public class SkeletalBee : BeeHelper
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skeletal Bee");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(1f, 1f), 0, new Color(100, 70, 107), 0.35f);

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(1.5f, 1.5f), 0, new Color(238, 164, 255), 0.3f);
            }

            SoundID.NPCHit2.PlayWith(Projectile.Center);
        }

        public override void SafeOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool())
            {
                Vector2 pos = target.Center + Main.rand.NextVector2CircularEdge(250f, 250f) * Main.rand.NextFloat(0.5f, 1f);
                Projectile.NewProjectile(Projectile.GetSource_OnHit(target), pos, pos.DirectionTo(target.Center) * 25f, ModContent.ProjectileType<SkeletalBeeSlash>(), (int)(Projectile.damage * 0.35f), 2.5f, Projectile.owner);
            }
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/BeeGlowPREHM").Value;
            if (Giant)
                tex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/BeeGlowGiant").Value;

            Rectangle frame = tex.Frame(verticalFrames: 4, frameY: Projectile.frame);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, frame, new Color(238, 164, 255, 0) * MathHelper.Lerp(0.75f, 0.5f, Utils.Clamp((float)Math.Cos(Main.timeForVisualEffects * 0.1f), 0, 1)), Projectile.rotation, frame.Size() / 2f, Projectile.scale, Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        }
    }

    class SkeletalBeeSlash : BeeProjectile, IDrawPrimitive_
    {
        private List<Vector2> cache;
        private Trail trail;

        public Vector2 startPos;
        public override string Texture => BombusApisBee.Invisible;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skeletal Slash");
        }
        public override void SafeSetDefaults()
        {
            Projectile.timeLeft = 30;
            Projectile.extraUpdates = 1;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;

            Projectile.width = Projectile.height = 12;

            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }
        public override void AI()
        {
            if (Projectile.timeLeft == 30)
                startPos = Projectile.Center;

            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
            }

            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), -Projectile.velocity.RotatedByRandom(0.35f) * 0.5f, 0, new Color(238, 164, 255), 0.35f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity *= 0.5f;
        }

        private void ManageCaches()
        {
            cache = new List<Vector2>();
            for (int i = 0; i < 13; i++)
            {
                cache.Add(Vector2.Lerp(startPos, Projectile.Center, i / 13f));
            }
            cache.Add(Projectile.Center);
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 14, new TriangularTip(1f),
                factor => 20 * (factor < 0.5f ? factor : MathHelper.Lerp(0.5f, 0f, (factor - 0.5f) / 0.5f)), factor =>
            {
                return new Color(238, 164, 255) * MathHelper.Lerp(1f, 0f, (1f - Projectile.timeLeft / 35f));
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center;
        }

        public void DrawPrimitives()
        {
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["time"].SetValue(Projectile.timeLeft * -0.03f);
            effect.Parameters["repeats"].SetValue(1);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);
            trail?.Render(effect);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/ShadowTrail").Value);
            effect.Parameters["repeats"].SetValue(5);
            trail?.Render(effect);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/EnergyTrail").Value);
            effect.Parameters["time"].SetValue(Projectile.timeLeft * 0.03f);
            trail?.Render(effect);
        }
    }
}