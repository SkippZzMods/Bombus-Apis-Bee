namespace BombusApisBee.Projectiles
{
    public class CursedFlameball : BeeProjectile, IDrawPrimitive_
    {
        public int bounces = 3;
        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;
        public override string Texture => BombusApisBee.Invisible;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Flameball");
        }

        public override void SafeSetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 20;

            Projectile.timeLeft = 480;
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

            Vector2 targetCenter = Projectile.Center;
            bool foundTarget = false;
            float num = 1500f;
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
            if (foundTarget)
                Projectile.velocity = (Projectile.velocity * 20f + Utils.SafeNormalize(targetCenter - Projectile.Center, Vector2.UnitX) * 15f) / 21f;

            Projectile.rotation += 0.25f * Projectile.direction;

            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(0.5f, 0.5f), 0, new Color(98, 205, 123), 0.4f);

            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowFastDecelerate>(), null, 0, new Color(231, 215, 157), 0.35f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D bloom = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Main.spriteBatch.Draw(bloom, Projectile.Center - Projectile.velocity - Main.screenPosition, null, new Color(60, 128, 88, 0), Projectile.rotation, bloom.Size() / 2f, 0.55f, 0f, 0f);

            Main.spriteBatch.Draw(bloom, Projectile.Center - Projectile.velocity - Main.screenPosition, null, new Color(98, 205, 123, 0), Projectile.rotation, bloom.Size() / 2f, 0.45f, 0f, 0f);

            Main.spriteBatch.Draw(bloom, Projectile.Center - Projectile.velocity - Main.screenPosition, null, new Color(231, 215, 157, 0), Projectile.rotation, bloom.Size() / 2f, 0.35f, 0f, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundID.Item74.PlayWith(Projectile.Center, 0.25f, 0.15f, 0.85f);
            Main.player[Projectile.owner].Bombus().AddShake(5);

            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CursedExplosion>(), Projectile.damage, 0f, Projectile.owner, 65);

            for (int i = 0; i < 25; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(3f, 3f), 0, new Color(98, 205, 123), 0.4f);

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(4.5f, 4.5f), 0, new Color(231, 215, 157), 0.3f);
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
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 15, new TriangularTip(190), factor => factor * 25f, factor =>
            {
                return new Color(60, 128, 88) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center;

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 15, new TriangularTip(190), factor => factor * 15f, factor =>
            {
                return new Color(98, 205, 123) * factor.X;
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
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/EnergyTrail").Value);

            trail?.Render(effect);
            trail2?.Render(effect);

            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/FireTrail").Value);

            trail2?.Render(effect);
        }
    }
    class CursedExplosion : ModProjectile, IDrawPrimitive_
    {
        private List<Vector2> cache;

        private Trail trail;
        private Trail trail2;
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";
        private float Progress => 1 - (Projectile.timeLeft / 25f);

        private float Radius => Projectile.ai[0] * (float)Math.Sqrt(Math.Sqrt(Progress));

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.DamageType = BeeUtils.BeeDamageClass();
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 25;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Explosion");
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
            }

            for (int k = 0; k < 6; k++)
            {
                float rot = Main.rand.NextFloat(0, 6.28f);

                Dust.NewDustPerfect(Projectile.Center + Vector2.One.RotatedBy(rot) * Radius, ModContent.DustType<GlowFastDecelerate>(),
                    Vector2.One.RotatedBy(rot) * 0.5f, 0, Main.rand.NextBool() ? new Color(60, 128, 88) : new Color(98, 205, 123), Main.rand.NextFloat(0.35f, 0.4f));
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 line = targetHitbox.Center.ToVector2() - Projectile.Center;
            line.Normalize();
            line *= Radius;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + line);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, 360);
        }

        private void ManageCaches()
        {
            if (cache is null)
            {
                cache = new List<Vector2>();

                for (int i = 0; i < 40; i++)
                {
                    cache.Add(Projectile.Center);
                }
            }

            for (int k = 0; k < 40; k++)
            {
                cache[k] = (Projectile.Center + Vector2.One.RotatedBy(k / 19f * 6.28f) * (Radius));
            }

            while (cache.Count > 40)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 40, new TriangularTip(1), factor => 35 * (1 - Progress), factor =>
            {
                return new Color(60, 128, 88);
            });

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 40, new TriangularTip(1), factor => 25 * (1 - Progress), factor =>
            {
                return new Color(231, 215, 157);
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = cache[39];

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = cache[39];
        }

        public void DrawPrimitives()
        {
            Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["time"].SetValue(Projectile.timeLeft * -0.01f);
            effect.Parameters["repeats"].SetValue(5f);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/EnergyTrail").Value);

            trail?.Render(effect);
            trail2?.Render(effect);

            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/FireTrail").Value);

            trail?.Render(effect);
            trail2?.Render(effect);
        }
    }
    public class SpazBee : BeeProjectile
    {
        internal int SpazLaserTimer = 1;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spazbee");
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 0;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 1;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, 360);
        }

        public override bool? CanHitNPC(NPC target)
        {
            return new bool?(Projectile.timeLeft < 270 && target.CanBeChasedBy(Projectile, false));
        }

        public override void AI()
        {
            if (++Projectile.frameCounter % 6 == 0)
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];

            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Glow>(), Vector2.Zero, 0, new Color(98, 205, 123), 0.35f);

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            float num373 = Projectile.position.X;
            float num374 = Projectile.position.Y;
            float num375 = 100000f;
            bool flag10 = false;
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 30f)
            {
                Projectile.ai[0] = 30f;
                for (int num376 = 0; num376 < 200; num376++)
                {
                    if (Main.npc[num376].CanBeChasedBy(Projectile, false))
                    {
                        float num377 = Main.npc[num376].position.X + (float)(Main.npc[num376].width / 2);
                        float num378 = Main.npc[num376].position.Y + (float)(Main.npc[num376].height / 2);
                        float num379 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num377) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num378);
                        if (num379 < 800f && num379 < num375 && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Main.npc[num376].position, Main.npc[num376].width, Main.npc[num376].height))
                        {
                            num375 = num379;
                            num373 = num377;
                            num374 = num378;
                            flag10 = true;
                        }
                    }
                }
            }
            if (!flag10)
            {
                num373 = Projectile.position.X + (float)(Projectile.width / 2) + Projectile.velocity.X * 100f;
                num374 = Projectile.position.Y + (float)(Projectile.height / 2) + Projectile.velocity.Y * 100f;
            }
            float num384 = 10f;
            float num380 = 0.16f;
            Vector2 vector30 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
            float num381 = num373 - vector30.X;
            float num382 = num374 - vector30.Y;
            float num383 = (float)Math.Sqrt((double)(num381 * num381 + num382 * num382));
            num383 = num384 / num383;
            num381 *= num383;
            num382 *= num383;
            if (Projectile.velocity.X < num381)
            {
                Projectile.velocity.X = Projectile.velocity.X + num380;
                if (Projectile.velocity.X < 0f && num381 > 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X + num380 * 2f;
                }
            }
            else if (Projectile.velocity.X > num381)
            {
                Projectile.velocity.X = Projectile.velocity.X - num380;
                if (Projectile.velocity.X > 0f && num381 < 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X - num380 * 2f;
                }
            }
            if (Projectile.velocity.Y < num382)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + num380;
                if (Projectile.velocity.Y < 0f && num382 > 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + num380 * 2f;
                    return;
                }
            }
            else if (Projectile.velocity.Y > num382)
            {
                Projectile.velocity.Y = Projectile.velocity.Y - num380;
                if (Projectile.velocity.Y > 0f && num382 < 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - num380 * 2f;
                }
            }
            Projectile.ai[1] += 1f;

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bounce--;
            if (bounce <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
            }
            return false;
        }
        private int bounce = 2;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, tex.Frame(verticalFrames: 2, frameY: Projectile.frame), lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.oldRot[i], tex.Frame(verticalFrames: 2, frameY: Projectile.frame).Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.45f, (i / (float)Projectile.oldPos.Length)), 0, 0);
            }
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, tex.Frame(verticalFrames: 2, frameY: Projectile.frame), lightColor, Projectile.rotation, tex.Frame(verticalFrames: 2, frameY: Projectile.frame).Size() / 2f, Projectile.scale, 0, 0f);

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(60, 128, 88, 0) * 0.5f, Projectile.rotation, bloomTex.Size() / 2f, 0.25f, 0, 0);
            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(98, 205, 123, 0) * 0.5f, Projectile.rotation, bloomTex.Size() / 2f, 0.25f, 0, 0);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundID.Item74.PlayWith(Projectile.Center, 0.35f, 0.1f, 0.65f);
            Main.player[Projectile.owner].Bombus().AddShake(2);

            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CursedExplosion>(), Projectile.damage, 0f, Projectile.owner, 25);

            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(1f, 1f), 0, new Color(98, 205, 123), 0.3f);

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(1.5f, 1.5f), 0, new Color(231, 215, 157), 0.2f);
            }
        }
    }
}
