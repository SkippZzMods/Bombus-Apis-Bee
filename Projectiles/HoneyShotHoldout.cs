namespace BombusApisBee.Projectiles
{
    public class HoneyShotHoldout : BeeProjectile
    {
        public override string Texture => BombusApisBee.BeeWeapon + "HoneyShot";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honeyshot");
        }

        public Player owner => Main.player[Projectile.owner];

        private bool CanShoot => owner.channel && !owner.noItems && !owner.CCed;

        private ref float ChargeTime => ref Projectile.ai[1];

        private ref float CurrentCharge => ref Projectile.ai[0];

        private bool updateTime = true;
        private bool hasFired;
        public override bool? CanDamage() => false;
        public override void SafeSetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 68;

            Projectile.friendly = true;
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Vector2 armPosition = owner.RotatedRelativePoint(owner.MountedCenter, true);
            Vector2 offset = Vector2.One.RotatedBy(Projectile.rotation - MathHelper.PiOver4);

            if (ChargeTime == 0f)
                ChargeTime = owner.ApplyHymenoptraSpeedTo(owner.GetActiveItem().useAnimation);


            if (CurrentCharge < ChargeTime && updateTime)
                CurrentCharge++;

            if (updateTime && CurrentCharge >= ChargeTime)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    for (int i = -1; i < 2; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), owner.Center + offset * 10f * Projectile.direction, owner.DirectionTo(Main.MouseWorld).RotatedBy(0.25f * i) * 20f,
                            ModContent.ProjectileType<HoneyArrow>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }

                for (int i = 0; i < 20; i++)
                {
                    Vector2 pos = owner.Center + offset * 15f * Projectile.direction;
                    Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.HoneyDustSolid>(), owner.DirectionTo(Main.MouseWorld).RotatedBy(0.65f).RotatedByRandom(0.3f) * Main.rand.NextFloat(5f), Main.rand.Next(100), default, 1.45f).noGravity = true;

                    Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.HoneyDustSolid>(), owner.DirectionTo(Main.MouseWorld).RotatedByRandom(0.3f) * Main.rand.NextFloat(5f), Main.rand.Next(100), default, 1.45f).noGravity = true;

                    Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.HoneyDustSolid>(), owner.DirectionTo(Main.MouseWorld).RotatedBy(-0.65f).RotatedByRandom(0.3f) * Main.rand.NextFloat(5f), Main.rand.Next(100), default, 1.45f).noGravity = true;
                }

                for (float k = 0; k < 6.28f; k += 0.1f)
                {
                    float x = (float)Math.Cos(k) * 70;
                    float y = (float)Math.Sin(k) * 25;

                    Dust.NewDustPerfect(owner.Center + offset * 15f * Projectile.direction, ModContent.DustType<Dusts.HoneyDustSolid>(), new Vector2(x, y).RotatedBy(Projectile.rotation + MathHelper.PiOver2) * 0.075f, 0, default, 1.45f).noGravity = true;
                }
                new SoundStyle("BombusApisBee/Sounds/Item/BowFire").PlayWith(owner.Center);
                owner.UseBeeResource((Main.player[Projectile.owner].HeldItem.ModItem as BeeDamageItem).honeyCost + 5);
                owner.Bombus().AddShake(4);
                Projectile.timeLeft = 20;
                updateTime = false;
            }

            ManipulatePlayerVariables();
            UpdateProjectileHeldVariables(armPosition);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texGlow = ModContent.Request<Texture2D>("BombusApisBee/Projectiles/HoneyShot_Glow").Value;
            Texture2D arrowTex = ModContent.Request<Texture2D>("BombusApisBee/Projectiles/HoneyArrow").Value;
            Texture2D arrowTexGlow = ModContent.Request<Texture2D>("BombusApisBee/Projectiles/HoneyArrow_Glow").Value;
            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            float radians = Projectile.rotation - (Projectile.direction == 1 ? MathHelper.PiOver4 : -MathHelper.PiOver2 - MathHelper.PiOver4);
            Vector2 offset = Vector2.One.RotatedBy(radians);
            Vector2 tipPos = owner.RotatedRelativePoint(owner.MountedCenter, true) + Projectile.velocity * Projectile.width * 0.5f;

            Color color = new Color(229, 114, 0, 0) * MathHelper.Lerp(0f, 1f, CurrentCharge / ChargeTime);
            if (!updateTime)
                color = new Color(229, 114, 0, 0) * MathHelper.Lerp(MathHelper.Lerp(0f, 1f, CurrentCharge / ChargeTime), 0f, 1f - Projectile.timeLeft / 20f);

            if (updateTime && CurrentCharge > 2)
            {
                for (int i = -1; i < 2; i++)
                {
                    float mult = 25f;
                    if (i == -1 || i == 1)
                        mult = 15f;

                    float multiplier = 10f;
                    if (i == -1 || i == 1)
                        multiplier = 5f;

                    Vector2 arrowPos = Vector2.Lerp(tipPos + offset * mult, tipPos + offset * multiplier, CurrentCharge / ChargeTime) + new Vector2(0f, (-2 * Projectile.direction) + (12 * i)).RotatedBy(Projectile.velocity.ToRotation());

                    Main.spriteBatch.Draw(arrowTexGlow, arrowPos - Main.screenPosition, null, color, Projectile.rotation + (0.25f * i), arrowTexGlow.Size() / 2f, 1f, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : 0f, 0f);

                    Main.spriteBatch.Draw(arrowTex, arrowPos - Main.screenPosition, null, lightColor, Projectile.rotation + (0.25f * i), arrowTex.Size() / 2f, 1f, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : 0f, 0f);
                }
            }

            Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, texGlow.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : 0f, 0f);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : 0f, 0f);

            Main.spriteBatch.Draw(glowTex, tipPos + offset * 5f - Main.screenPosition, null, color, 0f, glowTex.Size() / 2f, 0.45f, 0f, 0f);

            return false;
        }
        private void ManipulatePlayerVariables()
        {
            owner.ChangeDir(Projectile.direction);
            owner.heldProj = Projectile.whoAmI;
            owner.itemTime = 2;
            owner.itemAnimation = 2;
            owner.itemRotation = Utils.ToRotation(Projectile.velocity * Projectile.direction);
        }
        private void UpdateProjectileHeldVariables(Vector2 armPosition)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                float interpolant = Utils.GetLerpValue(5f, 25f, Projectile.Distance(Main.MouseWorld), true);
                Vector2 oldVelocity = Projectile.velocity;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, owner.SafeDirectionTo(Main.MouseWorld, null), interpolant);
                if (Projectile.velocity != oldVelocity)
                {
                    Projectile.netSpam = 0;
                    Projectile.netUpdate = true;
                }
            }

            Projectile.position = armPosition - Projectile.Size * 0.5f + Utils.SafeNormalize(Projectile.velocity, owner.direction * Vector2.UnitX) * 15f;
            Projectile.rotation = Utils.ToRotation(Projectile.velocity);
            int oldDirection = Projectile.spriteDirection;
            if (oldDirection == -1)
            {
                Projectile.rotation += 3.1415927f;
            }
            Projectile.direction = (Projectile.spriteDirection = Utils.ToDirectionInt(Projectile.velocity.X > 0f));
            if (Projectile.spriteDirection != oldDirection)
            {
                Projectile.rotation -= 3.1415927f;
            }

            if (updateTime)
                Projectile.timeLeft = 2;
        }
    }

    class HoneyArrow : BeeProjectile
    {
        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Arrow");
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;

            Projectile.friendly = true;
            Projectile.penetrate = 1;

            Projectile.timeLeft = 360;
        }

        public override void AI()
        {
            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.timeLeft < 350)
            {
                if (Projectile.velocity.Y > 16f)
                    Projectile.velocity.Y = 16f;
                else
                    Projectile.velocity.Y += 0.15f;
            }

            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(1.5f, 1.5f), 0, Main.rand.NextBool() ? new Color(180, 90, 0) : new Color(251, 172, 17), 0.45f);
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.HoneyDustSolid>(), Main.rand.NextVector2Circular(2.5f, 2.5f), Main.rand.Next(100), default, 1.35f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(3f, 3f), 0, Main.rand.NextBool() ? new Color(180, 90, 0) : new Color(251, 172, 17), 0.6f);
            }

            for (int i = 0; i < Main.rand.Next(1, 3); i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2CircularEdge(6f, 6f), ModContent.ProjectileType<HoneyBee>(), (int)(Projectile.damage * 0.65f), 0f, Projectile.owner);
            }

            BombusApisBee.HoneycombWeapon.PlayWith(Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texGlow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            DrawTrail(Main.spriteBatch);

            Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, new Color(229, 114, 0, 0), Projectile.rotation, texGlow.Size() / 2f, Projectile.scale, 0f, 0f);

            Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, new Color(229, 114, 0, 0), Projectile.rotation, texGlow.Size() / 2f, Projectile.scale, 0f, 0f);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0f, 0f);

            Vector2 offset = Vector2.One.RotatedBy(Projectile.rotation - MathHelper.PiOver4) * -10f;
            Main.spriteBatch.Draw(glowTex, Projectile.Center + offset - Main.screenPosition, null, new Color(229, 114, 0, 0), 0f, glowTex.Size() / 2f, 0.35f, 0f, 0f);
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
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 12, new TriangularTip(12), factor => 6f, factor =>
            {
                return new Color(136, 68, 0) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 12, new TriangularTip(12), factor => 2f, factor =>
            {
                return new Color(251, 172, 17) * factor.X;
            });

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = Projectile.Center + Projectile.velocity;
        }

        private void DrawTrail(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["time"].SetValue(Main.GameUpdateCount * -0.03f);
            effect.Parameters["repeats"].SetValue(1);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);
            trail?.Render(effect);

            trail2?.Render(effect);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/LiquidTrail").Value);
            trail?.Render(effect);

            trail2?.Render(effect);
            spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
