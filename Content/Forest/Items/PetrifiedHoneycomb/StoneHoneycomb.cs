using BombusApisBee.Content.Forest.Items.Honeycomb;
using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Core.BeekeeperClass;
using BombusApisBee.Core.Common.Honeycomb;
using BombusApisBee.Core.Systems.ParticleSystem;
using BombusApisBee.Core.Systems.PixelationSystem;
using BombusApisBee.Core.Systems.PrimitiveSystem;

namespace BombusApisBee.Content.Forest.Items.PetrifiedHoneycomb
{
    public class StoneHoneycomb : BaseHoneycombWeapon
    {
        public override int MaxCombo => 3;
        public StoneHoneycomb() : base("Petrified Honeycomb", "Throws a hardened honeycomb\nIncreases combo by 1 on critical hit\nThe honeycomb always crits when falling fast enough\nWhen at max combo, direct strikes hits deal area of effect damage\n'Two bees... one stone..'") { }

        public override void AddDefaults()
        {
            Item.damage = 14;
            critAdd = 8;
            Item.knockBack = 3f;

            Item.noMelee = true;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 55;
            Item.useAnimation = 55;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5f;
            Item.value = Item.sellPrice(silver: 15);

            Item.rare = ItemRarityID.White;
            Item.UseSound = SoundID.DD2_MonkStaffSwing;

            Item.autoReuse = true;
            Item.shoot = ProjectileType<StoneHoneycombProjectile>();
            Item.shootSpeed = 13;
            honeyCost = 2;

            Item.noUseGraphic = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient(ItemID.StoneBlock, 35).
                AddIngredient(ItemType<PollenItem>(), 10).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }

    public class StoneHoneycombProjectile : BaseHoneycombProjectile
    {
        public bool flashed;
        public int flashTimer;

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void AI()
        {
            if (Main.rand.NextBool(6))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Stone);

            if (flashTimer > 0)
                flashTimer--;

            if (!flashed)
            {
                if (ComboProjectile)
                {
                    flashTimer = 60;
                    Projectile.velocity *= 1.2f;

                    new SoundStyle("BombusApisBee/Sounds/Item/ProjectileLaunch1").PlayWith(Projectile.Center, -0.1f, 0, 1.2f);
                    Owner.Bombus().AddShake(3);
                }

                flashed = true;
            }

            Projectile.rotation += Projectile.velocity.Length() * 0.01f;

            if (++Timer > 20)
            {
                Projectile.velocity.Y += 0.3f;

                if (Projectile.velocity.Y > 0)
                {
                    if (Projectile.velocity.Y < 13f)
                        Projectile.velocity.Y *= 1.09f;
                    else
                        Projectile.velocity.Y *= 1.05f;
                }
                if (Projectile.velocity.Y > 16f)
                    Projectile.velocity.Y = 16f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = Request<Texture2D>(Texture).Value;
            var outline = Request<Texture2D>(Texture + "_Outline").Value;
            var bloom = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            var star = Request<Texture2D>("BombusApisBee/Assets/ExtraTextures/StarAlpha").Value;

            SpriteBatch sb = Main.spriteBatch;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float lerp = 1f - i / (float)Projectile.oldPos.Length;

                Vector2 pos = Projectile.oldPos[i] + Projectile.Size / 2f;
                
                if (flashTimer > 0)
                {
                    float interp = EaseBuilder.EaseCircularInOut.Ease(flashTimer / 60f);

                    sb.Draw(outline, pos - Main.screenPosition, null, new Color(172, 105, 6, 0) * lerp * interp, Projectile.rotation, outline.Size() / 2f, Projectile.scale, 0f, 0f);
                }

                sb.Draw(texture, pos - Main.screenPosition, null, lightColor * lerp * 0.5f, Projectile.rotation, texture.Size() / 2f, Projectile.scale, 0f, 0f);
            }

            sb.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, Projectile.scale, 0f, 0f);

            if (flashTimer > 0)
            {
                float lerp = EaseBuilder.EaseCircularInOut.Ease(flashTimer / 60f);

                sb.Draw(bloom, Projectile.Center - Main.screenPosition, null, new Color(172, 105, 6, 0) * lerp * 0.3f, Projectile.rotation * 2f, bloom.Size() / 2f, Projectile.scale, 0f, 0f);

                sb.Draw(outline, Projectile.Center - Main.screenPosition, null, new Color(172, 105, 6, 0) * lerp, Projectile.rotation, outline.Size() / 2f, Projectile.scale, 0f, 0f);

                sb.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(172, 105, 6, 0) * lerp, Projectile.rotation * 2f, star.Size() / 2f, Projectile.scale * 0.6f, 0f, 0f);

                sb.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(235, 144, 10, 0) * lerp, Projectile.rotation * 2f, star.Size() / 2f, Projectile.scale * 0.4f, 0f, 0f);
            }

            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.velocity.Y > 8f)
                modifiers.SetCrit();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (ComboProjectile)
            {
                Owner.Bombus().AddShake(5);

                if (Main.myPlayer == Owner.whoAmI)
                    Projectile.NewProjectile(Projectile.GetSource_OnHit(target), Projectile.Center, Vector2.Zero, ProjectileType<StoneShockwave>(), Projectile.damage * 3, 5f, Owner.whoAmI, 35);

                for (int i = 0; i < 4; i++)
                {
                    ParticleHandler.SpawnParticle(new SmokeParticle(Projectile.Center, Main.rand.NextVector2Circular(4f, 4f),
                        new Color(81, 81, 81), new Color(23, 23, 23), Main.rand.NextFloat(0.04f, 0.08f), 30 + Main.rand.Next(30, 70), false, false, extraUpdateAction: SmokeBehavior));

                    Vector2 velocity = Main.rand.NextVector2CircularEdge(8f, 8f) * Main.rand.NextFloat(0.6f, 1f);

                    ParticleHandler.SpawnParticle(new GlowLineParticle(Projectile.Center, velocity,
                        new Color(150, 150, 150), velocity.ToRotation(), new Vector2(0.5f, 2f) * 0.6f, 30 + Main.rand.Next(10, 20), false, ExtraUpdate));
                    
                    velocity = Main.rand.NextVector2CircularEdge(8f, 8f) * Main.rand.NextFloat(0.6f, 1f);

                    ParticleHandler.SpawnParticle(new GlowLineParticle(Projectile.Center, velocity,
                        new Color(90, 90, 90), velocity.ToRotation(), new Vector2(0.5f, 2f) * 0.6f, 30 + Main.rand.Next(10, 20), false, ExtraUpdate));
                    
                    velocity = Main.rand.NextVector2CircularEdge(8f, 8f) * Main.rand.NextFloat(0.6f, 1f);

                    ParticleHandler.SpawnParticle(new GlowLineParticle(Projectile.Center, velocity,
                        new Color(254, 194, 20), velocity.ToRotation(), new Vector2(0.5f, 2f) * 0.6f, 30 + Main.rand.Next(10, 20), false, ExtraUpdate));
                }

                static void ExtraUpdate(Particle p)
                {
                    p.Velocity *= 0.92f;
                }
            }
            else if (hit.Crit)
            {               
                ParentWeapon?.AddCombo(decayTimer: 600);

                if (ParentWeapon.CurrentCombo == ParentWeapon.MaxCombo)
                {
                    SoundID.MaxMana.PlayWith(Owner.Center);

                    for (int i = 0; i < 5; i++)
                    {
                        ParticleHandler.SpawnParticle(new SmokeParticle(Owner.Center + Main.rand.NextVector2Circular(Owner.width, Owner.height), -Vector2.UnitY,
                            new Color(81, 81, 81), new Color(23, 23, 23), Main.rand.NextFloat(0.02f, 0.03f), 20 + Main.rand.Next(30, 50), false, false, extraUpdateAction: SmokeBehavior));

                        Dust d = Dust.NewDustPerfect(Owner.Center + Main.rand.NextVector2Circular(Owner.width, Owner.height), DustID.Stone, -Vector2.UnitY, 100, default, Main.rand.NextFloat(1.2f, 1.5f));

                        d.noGravity = true;
                        d.fadeIn = 1f;
                    }
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundID.DD2_MonkStaffGroundImpact.PlayWith(Projectile.Center);

            for (int i = 0; i < 7; i++)
            {
                ParticleHandler.SpawnParticle(new SmokeParticle(Projectile.Center, Main.rand.NextVector2Circular(2f, 2f),
                    new Color(128, 128, 128), new Color(23, 23, 23), Main.rand.NextFloat(0.03f, 0.05f), 20 + Main.rand.Next(30, 70), false, false, extraUpdateAction: SmokeBehavior));

                ParticleHandler.SpawnParticle(new SmokeParticle(Projectile.Center, Main.rand.NextVector2Circular(2.5f, 2.5f),
                    new Color(254, 194, 20), new Color(72, 44, 4), Main.rand.NextFloat(0.03f, 0.05f), 20 + Main.rand.Next(30, 70), false, false, extraUpdateAction: SmokeBehavior));
            }

            for (int i = 0; i < 6; i++)
            {
                if (Main.rand.NextBool(4))
                    ParticleHandler.SpawnParticle(new BeeParticle(Projectile.Center, Main.rand.NextVector2Circular(2f, 2f), 0f, 1f, 60));

                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), DustID.Honey2, Main.rand.NextVector2Circular(6f, 6f), 100, default, Main.rand.NextFloat(1.2f, 1.5f));

                d.noGravity = true;
                d.fadeIn = 1f;
                
                d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), DustID.Stone, Main.rand.NextVector2Circular(6f, 6f), 100, default, Main.rand.NextFloat(1.2f, 1.5f));

                d.noGravity = true;
                d.fadeIn = 1f;

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(6.5f, 6.5f), DustID.Honey2, -Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.1f, 0.2f), 50, default, Main.rand.NextFloat(0.4f, 1f));

                ParticleHandler.SpawnParticle(new StoneHoneycombParticle(Projectile.Center, Main.rand.NextVector2Circular(7f, 7f), Main.rand.NextFloat(0.7f, 1.1f), Main.rand.Next(30, 70)));
            }

            for (int i = 0; i < Main.rand.Next(0, 3); i++)
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.One).RotatedByRandom(0.5f) * 3f,
                            ProjectileType<WeakBeeProjectile>(), (int)(Projectile.damage * 0.75f), 2.5f, Owner.whoAmI);
        }

        static void SmokeBehavior(Particle p)
        {
            p.Velocity.Y -= 0.015f;
            p.Velocity.X *= 0.97f;
        }
    }

    class StoneHoneycombParticle : Particle
    {
        internal int _variant;
        public override ParticleDrawType DrawType => ParticleDrawType.Custom;
        public StoneHoneycombParticle(Vector2 position, Vector2 velocity, float scale, int maxTime)
        {
            Position = position;
            Velocity = velocity;
            Rotation = Main.rand.NextFloat(6.28f);
            Scale = scale;
            MaxTime = maxTime;
            Color = Color.White;

            _variant = 1 + Main.rand.Next(4);
        }

        public override void Update()
        {
            Velocity *= 0.9f;
            Rotation += Velocity.Length() * 0.05f;
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            var texture = Request<Texture2D>("BombusApisBee/Content/Forest/Items/PetrifiedHoneycomb/StoneHoneycombProjectileParticle_0" + _variant).Value;

            float progress = Progress;

            float fadeIn;

            if (progress < 0.25f)
                fadeIn = EaseBuilder.EaseCircularOut.Ease(progress / 0.25f);
            else
                fadeIn = EaseBuilder.EaseCircularIn.Ease(1f - (progress - 0.25f) / 0.75f);

            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * fadeIn, Rotation, texture.Size() / 2, Scale, SpriteEffects.None, 0);
        }
    }

    class StoneShockwave : ModProjectile
    {
        private List<Vector2> cache;

        private Trail trail;
        private Trail trail2;
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";
        private float Progress => 1 - Projectile.timeLeft / 30f;

        private float Radius => Projectile.ai[0] * EaseFunction.EaseQuinticOut.Ease(Progress);

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
            DisplayName.SetDefault("Shockwave");
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 line = targetHitbox.Center.ToVector2() - Projectile.Center;
            line.Normalize();
            line *= Radius;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + line);
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
                cache[k] = Projectile.Center + Vector2.One.RotatedBy(k / 39f * 6.28f) * Radius;
            }

            while (cache.Count > 40)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 40, new TriangularTip(1), factor => 15 * (1f - Progress), factor =>
            {
                return new Color(23, 23, 23);
            });

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 40, new TriangularTip(1), factor => 8 * (1f - Progress), factor =>
            {
                return new Color(45, 45, 45);
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = cache[39];

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = cache[39];
        }

        public void DrawPrimitives()
        {
            GetInstance<PixelationSystem>().QueueRenderAction("UnderTiles", () =>
            {
                Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.EffectMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["time"].SetValue(Projectile.timeLeft * -0.01f);
                effect.Parameters["repeats"].SetValue(5f);
                effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

                trail?.Render(effect);
                trail2?.Render(effect);
            });
        }
    }
}