using Terraria;
namespace BombusApisBee.Projectiles
{
    public class BeeBuckshot : BeeProjectile, IDrawPrimitive_
    {
        private Vector2 initVelo;
        private List<Vector2> cache;
        private Trail trail;

        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";

        public override void SafeSetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 100;
            Projectile.extraUpdates = 4;
            Projectile.alpha = 255;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bee Buckshot");
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 100)
            {
                Projectile.velocity *= Main.rand.NextFloat(1.5f, 2);
                initVelo = Projectile.velocity;
            }

            Projectile.velocity *= 0.98f;

            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 300);
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            if (Main.myPlayer == Projectile.owner)
            {
                int type = player.beeType();
                int damage = player.beeDamage((int)(Projectile.damage * 0.75f));
                float knockBack = player.beeKB(Projectile.knockBack);
                Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, initVelo * 0.5f, type, damage, knockBack, Projectile.owner).
                    DamageType = BeeUtils.BeeDamageClass();
            }
        }

        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < 12; i++)
                {
                    cache.Add(Projectile.Center);
                }
            }

            cache.Add(Projectile.Center);

            while (cache.Count > 12)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 12, new TriangularTip(175), factor => factor * 5.5f, factor =>
            {
                return Color.Lerp(new Color(160, 100, 65), Color.Black, 1 - (Projectile.timeLeft / 100f));
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center;
        }

        public void DrawPrimitives()
        {
            Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
            effect.Parameters["repeats"].SetValue(2f);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

            trail?.Render(effect);
        }
    }
}
