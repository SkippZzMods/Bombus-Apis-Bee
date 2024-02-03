using BombusApisBee.Core.PixelationSystem;
namespace BombusApisBee.Projectiles
{
    public class HellfireBeeBuckshot : BeeProjectile, IDrawPrimitive_
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
            Projectile.penetrate = 2;
            Projectile.timeLeft = 180;
            Projectile.extraUpdates = 4;
            Projectile.alpha = 255;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 100;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hellfire Bee Buckshot");
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 180)
            {
                Projectile.velocity *= Main.rand.NextFloat(1.75f, 2.25f);
                initVelo = Projectile.velocity;
            }

            Projectile.velocity *= 0.985f;

            if (Main.rand.NextBool(5))
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(0.5f, 0.5f), 0,
                    new Color(255, 150, 20), Main.rand.NextFloat(0.3f, 0.5f));

            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 300);
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            if (Main.myPlayer == Projectile.owner)
            {
                int damage = player.beeDamage((int)(Projectile.damage * 0.65f));
                float knockBack = player.beeKB(Projectile.knockBack);
                Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, initVelo * 0.5f, ModContent.ProjectileType<HellfireBee>(), damage, knockBack, Projectile.owner);
            }
        }

        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < 20; i++)
                {
                    cache.Add(Projectile.Center);
                }
            }

            cache.Add(Projectile.Center);

            while (cache.Count > 20)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 20, new TriangularTip(190), factor => factor * 6f, factor =>
            {
                return new Color(255, 50, 20);
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

    public class HellfireSlugExplosionDust: ModDust
    {
        public override string Texture => BombusApisBee.Invisible;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, 0, 4, 4);
            dust.customData = 1 + Main.rand.Next(3);
        }

        public override bool Update(Dust dust)
        {
            dust.velocity.Y -= 0.015f;
            dust.position += dust.velocity;
            dust.velocity *= 0.95f;
            dust.rotation += dust.velocity.Length() * 0.01f;

            dust.alpha += 5;

            dust.alpha = (int)(dust.alpha * 1.01f);

            dust.scale *= 1.03f;

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            float lerper = 1f - dust.alpha / 255f;

            Color color = Color.Black;
            if (lerper > 0.5f)
                color = Color.Lerp(new Color(250, 150, 30), Color.Black, dust.alpha / 255f);

            Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/SmokeTransparent_" + dust.customData).Value;
            ModContent.GetInstance<PixelationSystem>().QueueRenderAction("OverNPCs", () =>
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, color * lerper, dust.rotation, tex.Size() / 2f, dust.scale, 0f, 0f);

                Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, color * lerper, dust.rotation + MathHelper.PiOver2, tex.Size() / 2f, dust.scale, 0f, 0f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            });

            return false;
        }
    }

    public class HellfireBeemstickSlug : BeeProjectile
    { 
        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;

        public override void SafeSetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hellfire Slug");
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 300);
        }

        public override void Kill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ProjectileType<HellfireSlugExplosion>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, 55);

            new SoundStyle("BombusApisBee/Sounds/Item/FireHit").PlayWith(Projectile.Center, 0, 0, 1.25f);

            Main.player[Projectile.owner].Bombus().AddShake(12);

            for (int i = 0; i < 30; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<HellfireSlugExplosionDust>(), Main.rand.NextVector2Circular(6.5f, 6.5f) + -Vector2.UnitY * 2f, Main.rand.Next(30), default, Main.rand.NextFloat(0.05f, 0.1f)).rotation = Main.rand.NextFloat(6.28f);
            }

            for (int i = 0; i < 25; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(15f, 15f), 0, new Color(255, 200, 0, 0), 0.6f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrimitives();

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(250, 80, 20, 0), Projectile.rotation, bloomTex.Size() / 2f, 0.5f, 0f, 0f);

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(250, 80, 20, 0), Projectile.rotation, bloomTex.Size() / 2f, 0.35f, 0f, 0f);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0f, 0f);
           
            return false;
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
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 15, new TriangularTip(190), factor => factor * 6f, factor =>
            {
                return new Color(255, 50, 20);
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center;

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 15, new TriangularTip(190), factor => factor * 5f, factor =>
            {
                return new Color(255, 150, 20);
            });

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = Projectile.Center;
        }

        public void DrawPrimitives()
        {
            Main.spriteBatch.End();
            Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
            effect.Parameters["repeats"].SetValue(1f);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);
            trail?.Render(effect);

            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/FireTrail").Value);

            trail?.Render(effect);
            trail2?.Render(effect);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }

    class HellfireSlugExplosion : ModProjectile
    {
        private List<Vector2> cache;

        private Trail trail;
        private Trail trail2;
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";
        private float Progress => 1 - (Projectile.timeLeft / 30f);

        private float Radius => Projectile.ai[0] * EaseBuilder.EaseQuinticOut.Ease(Progress);

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
            Projectile.localNPCHitCooldown = 20;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hellfire Explosion");
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

                Dust.NewDustPerfect(Projectile.Center + Vector2.One.RotatedBy(rot) * Radius, ModContent.DustType<Dusts.GlowFastDecelerate>(),
                    Vector2.One.RotatedBy(rot) * 0.5f, 0, Main.rand.NextBool() ? new Color(200, 40, 20) : new Color(255, 90, 20), Main.rand.NextFloat(0.5f, 0.6f));
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 line = targetHitbox.Center.ToVector2() - Projectile.Center;
            line.Normalize();
            line *= Radius;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + line);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 600);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrimitives();
            return false;
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
                cache[k] = (Projectile.Center + Vector2.One.RotatedBy(k / 39f * 6.28f) * (Radius));
            }

            while (cache.Count > 40)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 40, new TriangularTip(1), factor => 40 * (1f - Progress), factor =>
            {
                return new Color(250, 80, 20);
            });

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 40, new TriangularTip(1), factor => 30 * (1f - Progress), factor =>
            {
                return new Color(255, 120, 20);
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = cache[39];

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = cache[39];
        }

        public void DrawPrimitives()
        {
            ModContent.GetInstance<PixelationSystem>().QueueRenderAction("OverNPCs", () =>
            {
                Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.ZoomMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["time"].SetValue(Projectile.timeLeft * -0.01f);
                effect.Parameters["repeats"].SetValue(5f);
                effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/FireTrail").Value);

                trail?.Render(effect);
                trail2?.Render(effect);

                effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/EnergyTrail").Value);

                trail2?.Render(effect);

                //Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth / 2, Main.screenHeight / 2), Color.White);
            });
        }
    }
}
