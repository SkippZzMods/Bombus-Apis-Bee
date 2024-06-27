using Terraria;
namespace BombusApisBee.Projectiles
{
    public class NectarBoltProjectile : BeeProjectile, IDrawPrimitive_
    {
        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;
        public override string Texture => BombusApisBee.Invisible;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nectar Bolt");
        }

        public override void SafeSetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 16;

            Projectile.timeLeft = 480;
            Projectile.penetrate = 3;
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
            }

            Projectile.rotation += 0.25f * Projectile.direction;

            Dust.NewDustPerfect(Projectile.Center + (Projectile.rotation.ToRotationVector2() * 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.Zero, Scale: 0.35f, newColor: new Color(255, 255, 150));

            Dust.NewDustPerfect(Projectile.Center + (Projectile.rotation.ToRotationVector2() * -5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.Zero, Scale: 0.35f, newColor: new Color(255, 191, 73));
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            BounceEffects();
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }

            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
                Projectile.Kill();
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D bloom = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Texture2D star = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/StarTexture").Value;
            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw(bloom, (Projectile.Center - Projectile.velocity) - Main.screenPosition, null, new Color(206, 116, 59, 0), Projectile.rotation, bloom.Size() / 2f, 0.35f, 0f, 0f);
            }
            Main.spriteBatch.Draw(star, (Projectile.Center - Projectile.velocity) - Main.screenPosition, null, new Color(206, 116, 59, 0), 0, star.Size() / 2f, 0.25f, 0f, 0f);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(3f, 3f), 0, new Color(255, 191, 73), 0.3f);
            }
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
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 15, new TriangularTip(190), factor => factor * 11.5f, factor =>
            {
                return new Color(206, 116, 59) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center;

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 15, new TriangularTip(190), factor => factor * 9f, factor =>
            {
                return new Color(255, 191, 73) * factor.X;
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

            effect.Parameters["time"].SetValue(Projectile.timeLeft * -0.025f);
            effect.Parameters["repeats"].SetValue(2f);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

            trail?.Render(effect);
            trail2?.Render(effect);
        }

        public void BounceEffects()
        {
            if (Main.myPlayer == Projectile.owner)
                for (int i = 0; i < Main.rand.Next(1, 3); i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2CircularEdge(9f, 9f),
                        ModContent.ProjectileType<NectarBoltHoming>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack * 0.65f, Projectile.owner);
                }

            BeeUtils.CircleDust(Projectile, 35, ModContent.DustType<Dusts.GlowFastDecelerate>(), 2.5f, 0, new Color(255, 191, 73), 0.35f);
        }
    }

    class NectarBoltHoming : BeeProjectile, IDrawPrimitive_
    {
        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;
        public override string Texture => BombusApisBee.Invisible;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nectar Bolt");
        }

        public override void SafeSetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 8;

            Projectile.timeLeft = 240;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
            }

            Projectile.rotation += Projectile.velocity.Length() * 0.035f;

            Dust.NewDustPerfect(Projectile.Center + (Projectile.rotation.ToRotationVector2() * 3f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.Zero, Scale: 0.35f, newColor: new Color(255, 255, 150));

            bool foundTarget = false;
            Vector2 targetCenter = Vector2.Zero;
            float num = 750f;
            for (int i = 0; i < 200; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(this, false))
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
            if (foundTarget && Projectile.timeLeft < 225)
            {
                Projectile.velocity = (Projectile.velocity * 20f + Utils.SafeNormalize(targetCenter - Projectile.Center, Vector2.UnitX) * 9f) / 21f;
            }
            else if (Projectile.timeLeft < 225)
            {
                Projectile.velocity *= 0.99f;
                if (Projectile.velocity.Length() < 1f)
                    Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            player.Heal(1);
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(3f, 3f), 0, new Color(255, 191, 73), 0.3f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D bloom = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Texture2D star = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/StarTexture").Value;
            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw(bloom, (Projectile.Center - Projectile.velocity) - Main.screenPosition, null, new Color(206, 116, 59, 0), Projectile.rotation, bloom.Size() / 2f, 0.2f, 0f, 0f);
            }
            Main.spriteBatch.Draw(star, (Projectile.Center - Projectile.velocity) - Main.screenPosition, null, new Color(206, 116, 59, 0), Projectile.rotation, star.Size() / 2f, 0.2f, 0f, 0f);
            return false;
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
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 12, new TriangularTip(190), factor => factor * 9.5f, factor =>
            {
                return new Color(206, 116, 59) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center;

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 12, new TriangularTip(190), factor => factor * 7f, factor =>
            {
                return new Color(255, 191, 73) * factor.X;
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

            effect.Parameters["time"].SetValue(Projectile.timeLeft * -0.025f);
            effect.Parameters["repeats"].SetValue(2f);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

            trail?.Render(effect);
            trail2?.Render(effect);
        }
    }
}
