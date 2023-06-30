using Terraria;
namespace BombusApisBee.Projectiles
{
    public class ChloroHoney : BeeProjectile, IDrawPrimitive_
    {
        private Vector2 targetCenter;
        private List<Vector2> cache;
        private Trail trail;
        public int hitDelay;
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chloro-Honey");
        }

        public override void SafeSetDefaults()
        {
            Projectile.DamageType = BeeUtils.BeeDamageClass();
            Projectile.friendly = true;
            Projectile.timeLeft = 360;
            Projectile.tileCollide = false;
            Projectile.penetrate = 3;

            Projectile.width = Projectile.height = 12;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (hitDelay > 0)
                hitDelay--;

            bool foundTarget = false;
            float num = 1500f;
            for (int i = 0; i < 200; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(this, false) && Projectile.localNPCImmunity[i] == 0 && hitDelay <= 0)
                {
                    float num2 = Projectile.Distance(npc.Center);
                    if (num > num2)
                    {
                        num = num2;
                        targetCenter = npc.Center;
                        foundTarget = true;
                    }
                }
            }
            if (foundTarget && Projectile.timeLeft < 345)
            {
                Projectile.velocity = (Projectile.velocity * 20f + Utils.SafeNormalize(targetCenter - Projectile.Center, Vector2.UnitX) * 15f) / 21f;
            }

            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), default, 55,
                   Main.rand.NextBool() ? new Color(161, 236, 0) : new Color(105, 212, 0), Main.rand.NextFloat(0.3f, 0.4f));

            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            for (int i = 0; i < 3; i++)
            {
                Color color = new Color(59, 148, 20);
                color.A = 0;
                Main.spriteBatch.Draw(tex, (Projectile.Center - Projectile.velocity) - Main.screenPosition, null, color, Projectile.rotation, tex.Size() / 2f, 0.3f, 0f, 0f);
            }
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.timeLeft <= 345 && hitDelay <= 0;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hitDelay = 30;
        }

        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < 15; i++)
                {
                    cache.Add(Projectile.Center);
                }
            }

            cache.Add(Projectile.Center);

            while (cache.Count > 15)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 15, new TriangularTip(155), factor => 10f * (factor), factor =>
            {
                return new Color(59, 148, 20);
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

            effect.Parameters["time"].SetValue(Projectile.timeLeft * -0.01f);
            effect.Parameters["repeats"].SetValue(3f);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

            trail?.Render(effect);
        }
    }
}
