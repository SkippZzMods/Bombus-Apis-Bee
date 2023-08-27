using Terraria;
namespace BombusApisBee.Projectiles
{
    public class AculeusBladeHoldout : BeeProjectile
    {
        public override string Texture => BombusApisBee.BeeWeapon + "BladeOfAculeus";
        private bool initialized;

        private bool swung;

        private bool flipBlade = true;
        private bool drawAfterImages;
        private bool spawnedProjectile;

        private float maxTimeLeft;
        private float originalDirection;

        private int oldTimeLeft;
        private int pauseTimer;

        private int hits;

        private List<float> oldRotation = new();
        private List<float> oldScale = new();
        private List<Vector2> oldPositions = new();
        private List<bool> oldFlipBlade = new();

        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;
        private Trail trail3;

        private Vector2 tipPosition;

        public float Combo => Projectile.ai[1];
        public Player owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blade of Aculeus");
        }
        public override void SafeSetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.Size = new Vector2(60);
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
            Projectile.Bombus().HeldProj = true;

            Projectile.extraUpdates = 1;
        }

        public override bool PreAI()
        {
            if (pauseTimer > 0)
            {
                Projectile.timeLeft = oldTimeLeft;

                pauseTimer--;
            }

            return true;
        }

        public override void AI()
        {
            UpdateProj();

            switch (Combo)
            {
                case 0:
                    UpSlash(); break;
                case 1:
                    DownSlash(); break;
            }
        }

        private void UpdateProj()
        {
            if (!(owner.HeldItem.ModItem is BladeOfAculeus))
                Projectile.Kill();

            if (Main.rand.NextBool(15))
                Dust.NewDustPerfect(Vector2.Lerp(owner.Center, tipPosition, Main.rand.NextFloat()), DustID.Poisoned, Main.rand.NextVector2Circular(2f, 2f), Main.rand.Next(50, 255), default, 1.5f).noGravity = true;

            if (Main.rand.NextBool(15))
                Dust.NewDustPerfect(Vector2.Lerp(owner.Center, tipPosition, Main.rand.NextFloat()), DustType<StingerDust>(), Main.rand.NextVector2Circular(2f, 2f), Main.rand.Next(50, 255), default, 1.5f).noGravity = true;

            owner.heldProj = Projectile.whoAmI;

            owner.ChangeDir(Projectile.direction);

            if (!drawAfterImages)
                return;

            if (Projectile.timeLeft % 3 == 0)
            {
                oldRotation.Add(Projectile.rotation);

                if (oldRotation.Count > 15)
                    oldRotation.RemoveAt(0);

                oldPositions.Add(Projectile.Center - owner.Center);

                if (oldPositions.Count > 15)
                    oldPositions.RemoveAt(0);

                oldScale.Add(Projectile.scale);

                if (oldScale.Count > 15)
                    oldScale.RemoveAt(0);

                oldFlipBlade.Add(flipBlade);
                if (oldFlipBlade.Count > 15)
                    oldFlipBlade.RemoveAt(0);
            }


            if (!Main.dedServ && Projectile.timeLeft < maxTimeLeft)
            {
                ManageCaches();
                ManageTrail();
            }
        }

        private void UpSlash()
        {
            if (!initialized)
            {
                initialized = true;
                Projectile.timeLeft = (int)owner.ApplyHymenoptraSpeedTo(owner.HeldItem.useAnimation);
                maxTimeLeft = Projectile.timeLeft;
                Projectile.rotation = owner.DirectionTo(Projectile.Center + Projectile.velocity).ToRotation() + MathHelper.PiOver4;
                Projectile.netUpdate = true;
                Projectile.direction = Main.MouseWorld.X < owner.Center.X ? -1 : 1;
                originalDirection = Projectile.direction;
                owner.itemTime = Projectile.timeLeft / 2;
                owner.itemAnimation = owner.itemTime;
                flipBlade = false;
            }

            float progress = 1f - Projectile.timeLeft / maxTimeLeft;

            if (progress < 0.5f)
            {
                float lerper = 1f - (Projectile.timeLeft - maxTimeLeft * 0.5f) / (maxTimeLeft * 0.5f);

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Lerp(-2f, 0f, EaseBuilder.EaseBackIn.Ease(lerper)) * originalDirection;

                Vector2 offset = Vector2.Lerp(new Vector2(0), new Vector2(20, -10 * originalDirection), EaseBuilder.EaseBackIn.Ease(lerper)).RotatedBy(Projectile.rotation - MathHelper.PiOver4);

                Projectile.Center = owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * MathHelper.Lerp(35f, 30f, EaseBuilder.EaseBackIn.Ease(lerper)) + offset;

                tipPosition = owner.MountedCenter + offset + (Projectile.rotation - MathHelper.PiOver4 - 0.1f * originalDirection).ToRotationVector2() * 60f;

                float armRot = (Projectile.velocity.ToRotation() - MathHelper.PiOver2) + MathHelper.Lerp(-1.8f, 0f, EaseBuilder.EaseBackIn.Ease(lerper)) * originalDirection;

                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);

                Projectile.scale = MathHelper.Lerp(0.8f, 1f, EaseBuilder.EaseBackIn.Ease(lerper));

                if (progress >= 0.3f && !drawAfterImages)
                    drawAfterImages = true;
            }
            else
            {
                if (!swung)
                {
                    SoundID.DD2_MonkStaffSwing.PlayWith(Projectile.Center);
                    owner.Bombus().AddShake(7);
                    swung = true;

                    for (int i = -1; i < 2; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), owner.Center, Projectile.velocity.RotatedBy(i * 0.05f) * 5f, ModContent.ProjectileType<AculeusBladeStinger>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }

                float lerper = 1f - (Projectile.timeLeft) / (float)Math.Floor(maxTimeLeft - maxTimeLeft * 0.5f);

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Lerp(0f, 2f, EaseBuilder.EaseQuinticOut.Ease(lerper)) * originalDirection;

                Vector2 offset = Vector2.Lerp(new Vector2(20, -10 * originalDirection), new Vector2(5, 15 * originalDirection), EaseBuilder.EaseQuinticOut.Ease(lerper)).RotatedBy(Projectile.rotation - MathHelper.PiOver4);

                Projectile.Center = owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * MathHelper.Lerp(30f, 35f, EaseBuilder.EaseQuinticOut.Ease(lerper)) + offset;

                tipPosition = owner.MountedCenter + offset + (Projectile.rotation - MathHelper.PiOver4 - 0.1f * originalDirection).ToRotationVector2() * 60f;

                float armRot = (Projectile.velocity.ToRotation() - MathHelper.PiOver2) + MathHelper.Lerp(0f, 2.2f, EaseBuilder.EaseQuinticOut.Ease(lerper)) * originalDirection;

                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);

                Projectile.scale = 0.8f + (float)Math.Sin(EaseBuilder.EaseQuinticOut.Ease(lerper) * 3.1415927f) * 0.75f * 0.75f;
            }
        }

        private void DownSlash()
        {
            if (!initialized)
            {
                initialized = true;
                Projectile.timeLeft = (int)owner.ApplyHymenoptraSpeedTo(owner.HeldItem.useAnimation);
                maxTimeLeft = Projectile.timeLeft;
                Projectile.rotation = owner.DirectionTo(Projectile.Center + Projectile.velocity).ToRotation() + MathHelper.PiOver4;
                Projectile.netUpdate = true;
                Projectile.direction = Main.MouseWorld.X < owner.Center.X ? -1 : 1;
                originalDirection = Projectile.direction;
                owner.itemTime = Projectile.timeLeft / 2;
                owner.itemAnimation = owner.itemTime;
                flipBlade = true;
            }

            float progress = 1f - Projectile.timeLeft / maxTimeLeft;

            if (progress < 0.5f)
            {
                float lerper = 1f - (Projectile.timeLeft - maxTimeLeft * 0.5f) / (maxTimeLeft * 0.5f);

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Lerp(2f, 0f, EaseBuilder.EaseBackIn.Ease(lerper)) * originalDirection;

                Vector2 offset = Vector2.Lerp(new Vector2(5, 15 * originalDirection), new Vector2(20, -10 * originalDirection), EaseBuilder.EaseBackIn.Ease(lerper)).RotatedBy(Projectile.rotation - MathHelper.PiOver4);

                Projectile.Center = owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * MathHelper.Lerp(35f, 30f, EaseBuilder.EaseBackIn.Ease(lerper)) + offset;

                tipPosition = owner.MountedCenter + offset + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 60f;

                float armRot = (Projectile.velocity.ToRotation() - MathHelper.PiOver2) + MathHelper.Lerp(2.2f, 0f, EaseBuilder.EaseBackIn.Ease(lerper)) * originalDirection;

                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);

                Projectile.scale = MathHelper.Lerp(0.8f, 1f, EaseBuilder.EaseBackIn.Ease(lerper));

                if (progress >= 0.3f && !drawAfterImages)
                    drawAfterImages = true;
            }
            else
            {
                if (!swung)
                {
                    SoundID.DD2_MonkStaffSwing.PlayWith(Projectile.Center);
                    owner.Bombus().AddShake(7);
                    swung = true;

                    for (int i = -1; i < 2; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), owner.Center, Projectile.velocity.RotatedBy(i * 0.05f) * 5f, ModContent.ProjectileType<AculeusBladeStinger>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }

                float lerper = 1f - (Projectile.timeLeft) / (float)Math.Floor(maxTimeLeft - maxTimeLeft * 0.5f);

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Lerp(0f, -2f, EaseBuilder.EaseQuinticOut.Ease(lerper)) * originalDirection;

                Vector2 offset = Vector2.Lerp(new Vector2(20, -10 * originalDirection), new Vector2(0), EaseBuilder.EaseQuinticOut.Ease(lerper)).RotatedBy(Projectile.rotation - MathHelper.PiOver4);

                Projectile.Center = owner.MountedCenter + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * MathHelper.Lerp(30f, 35f, EaseBuilder.EaseQuinticOut.Ease(lerper)) + offset;

                tipPosition = owner.MountedCenter + offset + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 60f;

                float armRot = (Projectile.velocity.ToRotation() - MathHelper.PiOver2) + MathHelper.Lerp(0f, -1.8f, EaseBuilder.EaseQuinticOut.Ease(lerper)) * originalDirection;

                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);

                Projectile.scale = 0.8f + (float)Math.Sin(EaseBuilder.EaseQuinticOut.Ease(lerper) * 3.1415927f) * 0.75f * 0.75f;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = (target.Center.X < owner.Center.X ? -1 : 1);

            modifiers.SourceDamage *= 1.75f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hits <= 0)
                pauseTimer = 9;

            oldTimeLeft = Projectile.timeLeft;

            hits = 1;

            owner.Bombus().AddShake(3);

            new SoundStyle("BombusApisBee/Sounds/Item/Impale").PlayWith(owner.Center, 0, 0f, 1f);

            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(target.Center + target.DirectionTo(owner.Center) * (target.width / 2), ModContent.DustType<StingerDust>(), target.DirectionTo(owner.Center).RotatedByRandom(0.6f) * Main.rand.NextFloat(0f, 10f), Main.rand.Next(50, 255), default, 1.5f).noGravity = true;

                Dust.NewDustPerfect(target.Center + target.DirectionTo(owner.Center) * (target.width / 2), DustID.Poisoned, target.DirectionTo(owner.Center).RotatedByRandom(0.6f) * Main.rand.NextFloat(0f, 10f), Main.rand.Next(50, 255), default, 1.5f).noGravity = true;

                Dust.NewDustPerfect(target.Center + target.DirectionTo(owner.Center) * (target.width / 2), ModContent.DustType<StingerDust>(), target.DirectionTo(owner.Center).RotatedByRandom(1.5f) * Main.rand.NextFloat(0f, 10f), Main.rand.Next(50, 255), default, 1.5f).noGravity = true;

                Dust.NewDustPerfect(target.Center + target.DirectionTo(owner.Center) * (target.width / 2), DustID.Poisoned, target.DirectionTo(owner.Center).RotatedByRandom(1.5f) * Main.rand.NextFloat(0f, 10f), Main.rand.Next(50, 255), default, 1.5f).noGravity = true;
            }

            target.AddBuff(BuffID.Poisoned, 300);
            target.AddBuff<BladeOfAculeusCleave>(600);
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;

            if (Combo == 0)
            {
                return null;
            }

            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), owner.Center, owner.Center + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * (70f * Projectile.scale), 10, ref collisionPoint))
                return true;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrimitives();
            Texture2D sword = ModContent.Request<Texture2D>(Texture + (flipBlade ? "_Flipped" : "")).Value;
     
            SpriteEffects flip = owner.direction == -1 ? SpriteEffects.FlipHorizontally : 0;

            if (drawAfterImages)
            {
                for (int i = 15; i > 0; i--)
                {
                    float fade = 1f;

                    if (Projectile.timeLeft < 60f)
                        fade = Projectile.timeLeft / 60f;

                    fade *= 1 - (15f - i) / 15f;

                    if (i > 0 && i < oldRotation.Count)
                    {
                        Texture2D swordBlade = ModContent.Request<Texture2D>(Texture + "_Blade" + (oldFlipBlade[i] ? "_Flipped" : "")).Value;
                        Main.spriteBatch.Draw(swordBlade, owner.Center + oldPositions[i] - Main.screenPosition, null, lightColor * 0.25f * fade, oldRotation[i] + (owner.direction == -1 ? MathHelper.PiOver2 : 0f), sword.Size() / 2f, oldScale[i], flip, 0f);
                    }
                }
            }

            Main.spriteBatch.Draw(sword, Projectile.Center + new Vector2(0f, owner.gfxOffY) - Main.screenPosition, null, lightColor, Projectile.rotation + (owner.direction == -1 ? MathHelper.PiOver2 : 0f), sword.Size() / 2f, Projectile.scale, flip, 0f);

            return false;
        }

        #region Primitive Drawing
        private void ManageCaches()
        {
            var adjustedPos = tipPosition - owner.Center;

            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < 15; i++)
                {
                    cache.Add(owner.Center + adjustedPos);
                }
            }

            cache.Add(owner.Center + adjustedPos);

            while (cache.Count > 15)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 15, new TriangularTip(0), factor => 30f, factor =>
            {
                if (factor.X >= 0.85f)
                    return Color.Transparent;

                return (new Color(40, 150, 20) * 0.6f) * factor.X * TrailFade();
            });

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 15, new TriangularTip(0), factor => 25f, factor =>
            {
                if (factor.X >= 0.85f)
                    return Color.Transparent;

                return (new Color(100, 215, 15) * 0.4f) * factor.X * TrailFade();
            });

            trail3 = trail3 ?? new Trail(Main.instance.GraphicsDevice, 15, new TriangularTip(0), factor => 40f, factor =>
            {
                if (factor.X >= 0.85f)
                    return Color.Transparent;

                return (new Color(200, 255, 40) * 0.2f) * factor.X * TrailFade();
            });

            var realCache = new Vector2[15];

            for (int k = 0; k < 15; k++)
            {
                realCache[k] = cache[k];
            }

            trail.Positions = realCache;
            trail2.Positions = realCache;
            trail3.Positions = realCache;
        }

        public float TrailFade()
        {
            float ratio = Projectile.timeLeft / maxTimeLeft;
            if (ratio > 0.25f)
                return (Projectile.timeLeft - maxTimeLeft * 0.25f) / (maxTimeLeft * 0.75f);
            else
                return 0f;
        }

        public void DrawPrimitives()
        {
            Main.spriteBatch.End();
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["SLRCeirosRing"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["time"].SetValue(Main.GameUpdateCount * -0.025f);
            effect.Parameters["repeats"].SetValue(1f);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/EnergyTrail").Value);

            if (drawAfterImages)
            {
                trail?.Render(effect);

                effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/FireTrail").Value);
                trail2?.Render(effect);

                effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/ShadowTrail").Value);
                trail3?.Render(effect);
            }

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
        #endregion Primitive Drawing
    }
}
