using BombusApisBee.BeeHelperProj;

namespace BombusApisBee.Projectiles
{
    public class GelBee : BaseBeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gel Bee");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner && Main.rand.NextBool(3))
            {
                Vector2 pos = Projectile.Center + Vector2.UnitY.RotatedByRandom(0.15f) * -Main.rand.NextFloat(750f, 1000f);
                Projectile.NewProjectile(Projectile.GetSource_Death(), pos, pos.DirectionTo(Projectile.Center) * 7f, ModContent.ProjectileType<VolatileGel>(), Projectile.damage, 0f, Projectile.owner);
            }
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(52, 129, 215, 0) * 0.75f, 0f, bloomTex.Size() / 2f, Giant ? 0.45f : 0.35f, 0, 0);
        }
    }

    class VolatileGel : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bouncy Slimeball");
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = Projectile.height = 20;

            Projectile.friendly = true;

            Projectile.timeLeft = 360;

            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.5f;

            if (Projectile.velocity.Y > 16f)
                Projectile.velocity.Y = 16f;

            Projectile.rotation += Projectile.velocity.Length() * 0.01f;

            if (Main.rand.NextBool(3))
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10f, 10f), ModContent.DustType<Dusts.SlimeDust>(), Main.rand.NextVector2Circular(1f, 1f), Main.rand.Next(50, 100), default, 0.9f).noGravity = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);

            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                Projectile.velocity.X = -oldVelocity.X;

            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                Projectile.velocity.Y = -oldVelocity.Y;

            for (float k = 0; k < 6.28f; k += 0.1f)
            {
                float x = (float)Math.Cos(k) * 30;
                float y = (float)Math.Sin(k) * 10;

                Dust.NewDustPerfect(Projectile.Bottom, ModContent.DustType<Dusts.SlimeDust>(), new Vector2(x, y).RotatedBy(oldVelocity.ToRotation() + MathHelper.PiOver2) * 0.1f, Main.rand.Next(50, 100), default, 1.25f).noGravity = true;
            }

            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.SlimeDust>(), Main.rand.NextVector2Circular(3f, 3f), Main.rand.Next(50, 100), default, 1.35f).noGravity = true;
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1 with { Pitch = -0.1f }, Projectile.Center);

            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.SlimeDust>(), Main.rand.NextVector2Circular(2f, 2f), Main.rand.Next(50, 100), default, 1.35f).noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor * 0.65f, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0, 0f);

            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(52, 129, 125, 0) * 0.65f, Projectile.rotation, glowTex.Size() / 2f, 0.45f, 0, 0f);
            return false;
        }
    }

    class RubyProjectile : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Volatile Ruby");
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            Main.projFrames[Type] = 4;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = Projectile.height = 14;

            Projectile.friendly = true;

            Projectile.timeLeft = 600;

            Projectile.alpha = 255;
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
                Projectile.alpha -= 25;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (++Projectile.frameCounter % 5 == 0)
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
        }

        public override void OnKill(int timeLeft)
        {
            Main.player[Projectile.owner].Bombus().AddShake(5);
            new SoundStyle("BombusApisBee/Sounds/Item/FireHit").PlayWith(Projectile.Center, 0.15f, 0.1f, 1.15f);

            for (int i = 0; i < 25; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5.5f, 5.5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(3f, 3f), 0,
                    Main.rand.NextBool() ? new Color(243, 172, 140) : new Color(212, 37, 24), Main.rand.NextFloat(0.6f, 0.75f));
            }

            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RubyExplosion>(), Projectile.damage, 0f, Projectile.owner, 35);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, tex.Frame(verticalFrames: 4, frameY: Projectile.frame), lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, tex.Frame(verticalFrames: 4, frameY: Projectile.frame).Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.45f, (i / (float)Projectile.oldPos.Length)), 0, 0);

                Main.spriteBatch.Draw(glowTex, (Projectile.oldPos[i] + (Projectile.Size * 0.5f)) - Main.screenPosition, null, new Color(212, 37, 24, 0) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, glowTex.Size() / 2f, Projectile.scale * MathHelper.Lerp(0.5f, 0.2f, (i / (float)Projectile.oldPos.Length)), 0, 0);
            }

            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(212, 37, 24, 0), Projectile.rotation, glowTex.Size() / 2f, 0.45f, 0, 0f);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, tex.Frame(verticalFrames: 4, frameY: Projectile.frame), lightColor, Projectile.rotation, tex.Frame(verticalFrames: 4, frameY: Projectile.frame).Size() / 2f, Projectile.scale, 0, 0f);

            for (int i = 0; i < 3; i++)
            {
                Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(212, 37, 24, 0), Projectile.rotation, glowTex.Size() / 2f, 0.25f, 0, 0f);
            }
            return false;
        }
    }

    class RubyExplosion : ModProjectile, IDrawPrimitive_
    {
        private List<Vector2> cache;

        private Trail trail;
        private Trail trail2;
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";
        private float Progress => 1 - (Projectile.timeLeft / 30f);

        private float Radius => Projectile.ai[0] * (float)Math.Sqrt(Math.Sqrt(Progress));

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.DamageType = BeeUtils.BeeDamageClass();
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ruby Explosion");
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
            }

            for (int k = 0; k < 4; k++)
            {
                float rot = Main.rand.NextFloat(0, 6.28f);

                Dust.NewDustPerfect(Projectile.Center + Vector2.One.RotatedBy(rot) * Radius, ModContent.DustType<Dusts.GlowFastDecelerate>(),
                    Vector2.One.RotatedBy(rot) * 0.5f, 0, Main.rand.NextBool() ? new Color(243, 172, 140) : new Color(212, 37, 24), 0.25f);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 line = targetHitbox.Center.ToVector2() - Projectile.Center;
            line.Normalize();
            line *= Radius;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + line);
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
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 40, new TriangularTip(1), factor => 20 * (1 - Progress), factor =>
            {
                return new Color(212, 37, 24);
            });

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 40, new TriangularTip(1), factor => 10 * (1 - Progress), factor =>
            {
                return new Color(243, 172, 140);
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = cache[39];

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = cache[39];
        }

        public void DrawPrimitives()
        {
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["time"].SetValue(Projectile.timeLeft * -0.01f);
            effect.Parameters["repeats"].SetValue(15f);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

            trail?.Render(effect);

            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/EnergyTrail").Value);

            trail?.Render(effect);
            trail2?.Render(effect);
        }
    }
}