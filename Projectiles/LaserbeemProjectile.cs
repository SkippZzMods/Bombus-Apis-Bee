using Terraria;
namespace BombusApisBee.Projectiles
{
    public class LaserbeemProjectile : BeeProjectile, IDrawPrimitive_
    {
        public int bounces;
        private List<NPC> hitNPCs = new();

        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;

        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";

        public override void SafeSetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 480;
            Projectile.extraUpdates = 4;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bouncing Honey Laser");
        }

        public override void AI()
        {
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.UnitY.RotatedBy(Projectile.rotation), 0, new Color(253, 232, 0), 0.2f);

            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), -Vector2.UnitY.RotatedBy(Projectile.rotation), 0, new Color(253, 232, 0), 0.2f);

            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hitNPCs.Add(target);

            DoBounce(true, Vector2.Zero);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            DoBounce(false, oldVelocity);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 25; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), default, 55,
                    Main.rand.NextBool() ? new Color(253, 232, 0) : new Color(255, 255, 204), Main.rand.NextFloat(0.4f, 0.5f));
            }
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Main.spriteBatch.Draw(tex, Projectile.Center - Projectile.velocity - Main.screenPosition, null, new Color(253, 232, 0, 0), Projectile.rotation, tex.Size() / 2f, 0.25f, 0, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Projectile.velocity - Main.screenPosition, null, new Color(255, 255, 204, 0) * 0.5f, Projectile.rotation, tex.Size() / 2f, 0.25f, 0, 0);
        }

        private bool DoBounce(bool onhit, Vector2 oldVelocity) //returns false when no target is found
        {
            NPC target = Main.npc.Where(n => n.active && n.CanBeChasedBy() && !hitNPCs.Contains(n) && n.Distance(Projectile.Center) < 900f && Collision.CanHitLine(Projectile.Center, 1, 1, n.Center, 1, 1)).OrderBy(n => Vector2.Distance(n.Center, Projectile.Center)).FirstOrDefault();
            if (target != default)
            {
                Projectile.velocity = Projectile.DirectionTo(target.Center) * 5f;
                MiscBounceEffects();
                return true;
            }
            else
            {
                if (onhit)
                {
                    Projectile.velocity *= -1f;
                    MiscBounceEffects();
                }
                else
                {
                    if (++bounces > 5)
                        Projectile.Kill();

                    if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                        Projectile.velocity.X = -oldVelocity.X;

                    if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                        Projectile.velocity.Y = -oldVelocity.Y;

                    MiscBounceEffects();
                }
                return false;
            }
        }

        private void MiscBounceEffects()
        {
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Main.rand.NextVector2Circular(6f, 6f), ModContent.ProjectileType<HoneyEnergyBee>(), (int)(Projectile.damage * 0.65f), 0f, Projectile.owner);
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), default, 55,
                    Main.rand.NextBool() ? new Color(253, 232, 0) : new Color(255, 255, 204), Main.rand.NextFloat(0.4f, 0.5f));
            }
        }

        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < 25; i++)
                {
                    cache.Add(Projectile.Center);
                }
            }

            cache.Add(Projectile.Center);

            while (cache.Count > 25)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 25, new TriangularTip(190), factor => factor * 7f, factor =>
            {
                return new Color(253, 232, 0) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center;

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 25, new TriangularTip(190), factor => factor * 6f, factor =>
            {
                return new Color(255, 255, 204) * factor.X;
            });

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = Projectile.Center;
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

            trail2?.Render(effect);

            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/LightningTrail").Value);

            trail?.Render(effect);

            trail2?.Render(effect);
        }
    }
}
