using BombusApisBee.Assets;
using BombusApisBee.Content.Forest.Items.Honeycomb;
using BombusApisBee.Content.Forest.Items.PetrifiedHoneycomb;
using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Core.BeekeeperClass;
using BombusApisBee.Core.Common.Honeycomb;
using BombusApisBee.Core.Helpers;
using BombusApisBee.Core.Systems.ParticleSystem;
using BombusApisBee.Core.Systems.PixelationSystem;
using BombusApisBee.Core.Systems.PrimitiveSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;

namespace BombusApisBee.Content.Forest.Items.GelatinousHoneycomb
{
    public class GelatinousHoneycomb : BaseHoneycombWeapon
    {
        public override int ThrowDustType => DustID.t_Slime;
        public override int MaxCombo => 7;
        public override void AddDefaults()
        {
            Item.damage = 14;
            Item.knockBack = 4f;

            Item.noMelee = true;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 27;
            Item.useAnimation = 27;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2.5f;
            Item.value = Item.sellPrice(gold: 2);

            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.DD2_MonkStaffSwing;

            Item.autoReuse = true;
            Item.shoot = ProjectileType<GelatinousHoneycombProjectile>();
            Item.shootSpeed = 13;
            honeyCost = 1;

            Item.noUseGraphic = true;
        }

        public override void ItemEffects(Player player, float rotation, float lerper)
        {
            Dust.NewDustPerfect(player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, rotation), ThrowDustType, rotation.ToRotationVector2() * 0.5f, 150, new Color(44, 113, 255, 0), 1.2f * (1f - lerper)).noGravity = true;
        }
    }

    public class GelatinousHoneycombProjectile : BaseHoneycombProjectile
    {
        private static int TRAIL_LEN = 12;
        private static int MAX_FLASH_TIMER = 25;

        public bool flashed;
        public int flashTimer;

        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.Server && ComboProjectile)
            {
                ManageCaches();
                ManageTrail();
            }

            if (ComboProjectile)
            {
                if (Projectile.penetrate == -1)
                    Projectile.velocity *= 0.95f;

                if (Main.rand.NextBool())
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(25, 25), DustID.Torch, Main.rand.NextVector2Circular(1.5f, 1.5f), 0, default, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }
            else
            {
                if (Main.rand.NextBool(10))
                {
                    Dust.NewDustPerfect(Projectile.Center, DustID.t_Slime, Main.rand.NextVector2Circular(2.5f, 2.5f), 150, new Color(69, 148, 255, 0), Main.rand.NextFloat(1.15f, 1.9f)).noGravity = true;

                    Dust.NewDustPerfect(Projectile.Center, DustID.t_Slime, Main.rand.NextVector2Circular(3.5f, 3.5f), 150, new Color(44, 113, 255, 0), Main.rand.NextFloat(1.15f, 1.9f)).noGravity = true;
                }
            }

            if (flashTimer > 0)
                flashTimer--;

            if (!flashed)
            {
                if (ComboProjectile)
                {
                    Projectile.penetrate = 2;
                    Projectile.usesLocalNPCImmunity = true;
                    Projectile.localNPCHitCooldown = 15;

                    flashTimer = MAX_FLASH_TIMER;
                    Projectile.velocity *= 1.2f;

                    new SoundStyle("BombusApisBee/Sounds/Item/ProjectileLaunch1").PlayWith(Projectile.Center, -0.1f, 0, 1.2f);
                    Owner.Bombus().AddShake(3);

                    for (int i = 0; i < 9; i++)
                    {
                        ParticleHandler.SpawnParticle(new FireParticle(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), Projectile.velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(0.3f), Color.OrangeRed, Main.rand.NextFloat(0.05f, 0.1f), Main.rand.Next(40, 80), FlameIntensity: Main.rand.NextFloat(0.3f), extraUpdateAction: p => p.Velocity *= 0.95f)
                        {
                            LayerPixel = RenderLayer.UnderNPCs
                        });
                    }

                    for (int i = 0; i < 35; i++)
                    {
                        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(25, 25), DustID.Torch,
                            Projectile.velocity.RotatedByRandom(0.55f) * Main.rand.NextFloat(0.6f), 0, default, Main.rand.NextFloat(4f)).noGravity = true;
                    }
                }

                ProjectileID.Sets.TrailCacheLength[Type] = ComboProjectile ? 9 : 5;
                Projectile.stopsDealingDamageAfterPenetrateHits = ComboProjectile;

                flashed = true;
            }

            Projectile.rotation += Projectile.velocity.Length() * 0.01f;

            if (++Timer > 20)
            {
                Projectile.velocity.Y += 0.15f;

                if (Projectile.velocity.Y > 0)
                {
                    if (Projectile.velocity.Y < 13f)
                        Projectile.velocity.Y *= 1.06f;
                    else
                        Projectile.velocity.Y *= 1.03f;
                }
                if (Projectile.velocity.Y > 16f)
                    Projectile.velocity.Y = 16f;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (ComboProjectile)
                modifiers.FinalDamage *= 2f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (ComboProjectile)
                DrawPrimitives();

            var texture = Request<Texture2D>(Texture).Value;
            var outline = Request<Texture2D>(Texture + "_Outline").Value;
            var white = Request<Texture2D>(Texture + "_White").Value;
            var glow = Request<Texture2D>(Texture + "_Glow").Value;
            var bloom = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            
            Main.instance.LoadProjectile(9);

            var trail = TextureAssets.Extra[91].Value;

            var ribbon = Request<Texture2D>(Texture + "_Ribbon").Value;

            SpriteBatch sb = Main.spriteBatch;

            float fadeOut = 1f;
            if (Projectile.timeLeft < 15f)
                fadeOut = Projectile.timeLeft / 15f;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float lerp = 1f - i / (float)Projectile.oldPos.Length;

                Vector2 pos = Projectile.oldPos[i] + Projectile.Size / 2f;

                if (ComboProjectile)
                {
                    sb.Draw(trail, pos - Projectile.velocity - Main.screenPosition, null, Color.Lerp(Color.OrangeRed, Color.Orange, lerp) with { A = 0 } * lerp * 0.5f * fadeOut,
                        Projectile.velocity.ToRotation() + MathHelper.PiOver2, trail.Size() / 2f, Projectile.scale * 0.65f, 0f, 0f);
                }

                if (flashTimer > 0)
                {
                    float interp = EaseBuilder.EaseCircularInOut.Ease(flashTimer / (float)MAX_FLASH_TIMER);

                    sb.Draw(outline, pos - Main.screenPosition, null, Color.Lerp(Color.OrangeRed, Color.Orange, lerp) with { A = 0 } * lerp * interp * fadeOut, Projectile.rotation, outline.Size() / 2f, Projectile.scale, 0f, 0f);
                }

                sb.Draw(texture, pos - Main.screenPosition, null, lightColor * lerp * 0.5f * fadeOut, Projectile.rotation, texture.Size() / 2f, Projectile.scale, 0f, 0f);

                if (ComboProjectile)
                    sb.Draw(white, pos - Main.screenPosition, null, Color.DarkOrange with { A = 0 } * lerp * 0.5f * fadeOut, Projectile.rotation, white.Size() / 2f, Projectile.scale, 0f, 0f);
            }

            sb.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor * fadeOut, Projectile.rotation, texture.Size() / 2f, Projectile.scale, 0f, 0f);

            if (ComboProjectile)
            {
                sb.Draw(white, Projectile.Center - Main.screenPosition, null, Color.DarkOrange with { A = 0 } * fadeOut, Projectile.rotation, white.Size() / 2f, Projectile.scale, 0f, 0f);
                
                sb.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.Orange with { A = 0 } * 0.66f * fadeOut, Projectile.rotation, glow.Size() / 2f, Projectile.scale * 1.1f, 0f, 0f);
            }
            else
            {
                var curve = GetCurve();

                float fadeIn = Timer / 30f;
                if (fadeIn > 1)
                    fadeIn = 1;

                int points = 6;

                Vector2[] positions = [.. curve.GetPoints(points)];

                for (int i = 0; i < points; i++)
                {
                    float lerp = i / (float)points;

                    Vector2 pos = positions[i];

                    int variant = (int)MathHelper.Lerp(1, 5, lerp);

                    Rectangle frame = ribbon.Frame(1, 5, 0, variant);

                    float rotation;

                    if (i < points - 1)
                        rotation = positions[i + 1].DirectionTo(pos).ToRotation();
                    else
                        rotation = pos.DirectionTo(positions[i - 1]).ToRotation();

                    sb.Draw(ribbon, pos - Main.screenPosition, frame, lightColor * fadeIn, rotation + MathHelper.PiOver2, frame.Size() / 2f, 1f, 0f, 0f);
                }

                Rectangle rect = ribbon.Frame(1, 5, 0, 0);

                float stampRotation = positions[1].DirectionTo(positions[0]).ToRotation();

                sb.Draw(ribbon, positions[0] + new Vector2(12f, -4f).RotatedBy(stampRotation) - Main.screenPosition, rect, lightColor, stampRotation, rect.Size() / 2f, Projectile.scale, 0f, 0f);
            }

            return false;
        }

        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < TRAIL_LEN; i++)
                {
                    cache.Add(Projectile.Center);
                }
            }

            cache.Add(Projectile.Center + Projectile.velocity);

            while (cache.Count > TRAIL_LEN)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, TRAIL_LEN, new TriangularTip(190), factor => EaseBuilder.EaseCircularInOut.Ease(factor) * 10f, factor =>
            {
                return Color.Lerp(Color.OrangeRed, Color.Orange with { A = 200 }, 1f - factor.X) * factor.X * Fade();
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, TRAIL_LEN, new TriangularTip(190), factor => EaseBuilder.EaseQuinticInOut.Ease(factor) * 12f, factor =>
            {
                return Color.Lerp(Color.Red, Color.DarkOrange with { A = 0 }, factor.X) * factor.X * Fade();
            });

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = Projectile.Center + Projectile.velocity;
        }

        public float Fade() => flashTimer > 0 ? flashTimer / (float)MAX_FLASH_TIMER : 0f;

        public void DrawPrimitives()
        {
            GetInstance<PixelationSystem>().QueueRenderAction("UnderProjectiles", () =>
            {
                Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.EffectMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 0.75f);
                effect.Parameters["repeats"].SetValue(2f);
                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

                trail?.Render(effect);

                effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("BombusApisBee/ShaderTextures/FireTrail").Value);

                trail2?.Render(effect);

                effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 1.5f);
                effect.Parameters["repeats"].SetValue(1f);
                trail?.Render(effect);

            });
        }

        private BezierCurve GetCurve()
        {
            float fadeIn = Timer / 30f;
            if (fadeIn > 1)
                fadeIn = 1;

            Vector2[] points = [
                Projectile.Center - Projectile.velocity.RotatedBy(Projectile.velocity.X * 0.02f) * 1.25f * fadeIn,
                Projectile.Center - Projectile.velocity.RotatedBy(Projectile.velocity.X * 0.01f) * 4 * fadeIn,
                Projectile.Center - Projectile.velocity.RotatedBy(Projectile.velocity.X * 0.005f) * 6 * fadeIn,
            ];

            var curve = new BezierCurve(points);

            return curve;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (ComboProjectile)
            {
                new SoundStyle("BombusApisBee/Sounds/Item/FireHit").PlayWith(Projectile.Center, 0, 0, 0.35f);

                HitEffects();
            }

            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (ComboProjectile)
            {
                new SoundStyle("BombusApisBee/Sounds/Item/FireHit").PlayWith(Projectile.Center, 0, 0, 0.35f);

                Owner.Bombus().AddShake(5);
                target.AddBuff(BuffID.OnFire3, 240);

                Projectile.timeLeft = 20;

                HitEffects();
            }     
            else
            {
                ParentWeapon?.AddCombo();

                if (ParentWeapon.CurrentCombo == ParentWeapon.MaxCombo)
                {
                    SoundID.MaxMana.PlayWith(Owner.Center);

                    for (int i = 0; i < 5; i++)
                        Dust.NewDustPerfect(Owner.Center + Main.rand.NextVector2Circular(25, 25), DustID.Torch, -Vector2.UnitY, 0, default, Main.rand.NextFloat(1f, 2.5f)).noGravity = true;
                }          
            }
        }

        void HitEffects()
        {
            for (int i = 0; i < 25; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(25, 25), DustID.Torch,
                    Main.rand.NextVector2Circular(6f, 6f), 0, default, Main.rand.NextFloat(4f)).noGravity = true;
            }

            ParticleHandler.SpawnParticle(new FireParticle(Projectile.Center, Vector2.Zero, Color.DarkOrange, Main.rand.NextFloat(0.15f, 0.22f), Main.rand.Next(40, 70), FlameIntensity: Main.rand.NextFloat(0.6f))
            {
                LayerPixel = RenderLayer.UnderNPCs
            });

            for (int i = 0; i < 5; i++)
            {
                ParticleHandler.SpawnParticle(new FireParticle(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), Main.rand.NextVector2Circular(3.5f, 3.5f), Color.OrangeRed, Main.rand.NextFloat(0.05f, 0.1f), Main.rand.Next(40, 80), FlameIntensity: Main.rand.NextFloat(0.3f), extraUpdateAction: p => p.Velocity *= 0.9f)
                {
                    LayerPixel = RenderLayer.UnderNPCs
                });
            }
        }

        public override void OnKill(int timeLeft)
        {        
            if (!ComboProjectile)
            {
                for (int i = 0; i < 4; i++)
                {
                    ParticleHandler.SpawnParticle(new RibbonParticle(
                        Projectile.Center - Projectile.velocity * i, 
                        -Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.8f) - Vector2.UnitY * Main.rand.NextFloat(4f),
                        Main.rand.NextFloat(0.8f, 1.2f), Main.rand.Next(40, 80)));
                }

                ParticleHandler.SpawnParticle(new RibbonClipParticle(Projectile.Center, Main.rand.NextVector2Circular(5f, 5f) - Vector2.UnitY * 3f - Projectile.velocity * 0.25f, 1f, 60));

                BombusApisBee.HoneycombWeapon.PlayWith(Projectile.Center);

                for (int i = 0; i < 2; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, DustType<SmokeDust2>(), Main.rand.NextVector2Circular(1.5f, 1.5f), 165, new Color(13, 61, 158), Main.rand.NextFloat(0.3f, 0.6f));

                    Dust.NewDustPerfect(Projectile.Center, DustType<SmokeDust2>(), Main.rand.NextVector2Circular(2f, 2f), 155, new Color(44, 113, 255), Main.rand.NextFloat(0.3f, 0.5f));

                    Dust.NewDustPerfect(Projectile.Center, DustType<SmokeDust2>(), Main.rand.NextVector2Circular(1.25f, 1.25f), 155, new Color(118, 185, 255), Main.rand.NextFloat(0.3f, 0.5f));
                }

                for (int i = 0; i < 5; i++)
                {
                    ParticleHandler.SpawnParticle(new GelatinousHoneycombParticle(Projectile.Center, Main.rand.NextVector2Circular(6f, 6f), Main.rand.NextFloat(0.9f, 1.1f), Main.rand.Next(30, 70)));

                    Dust.NewDustPerfect(Projectile.Center, DustID.t_Slime, Main.rand.NextVector2Circular(5f, 5f), 200, new Color(69, 148, 255), Main.rand.NextFloat(1.15f, 1.9f)).noGravity = true;

                    Dust.NewDustPerfect(Projectile.Center, DustID.t_Slime, Main.rand.NextVector2Circular(3.5f, 3.5f), 200, new Color(44, 113, 255), Main.rand.NextFloat(1.15f, 1.9f)).noGravity = true;
                }
            }         
        }

        static void SmokeBehavior(Particle p)
        {
            p.Velocity.Y -= 0.015f;
            p.Velocity.X *= 0.97f;
        }
    }

    class GelatinousHoneycombParticle : Particle
    {
        internal int _variant;
        internal bool _flaming;
        public override ParticleDrawType DrawType => ParticleDrawType.Custom;
        public GelatinousHoneycombParticle(Vector2 position, Vector2 velocity, float scale, int maxTime, bool flaming = false)
        {
            Position = position;
            Velocity = velocity;
            Rotation = Main.rand.NextFloat(6.28f);
            Scale = scale;
            MaxTime = maxTime;
            Color = Color.White;

            _variant = 1 + Main.rand.Next(3);
            _flaming = flaming;
        }

        public override void Update()
        {
            Velocity *= 0.9f;
            Rotation += Velocity.Length() * 0.05f;
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            var texture = Request<Texture2D>("BombusApisBee/Content/Forest/Items/GelatinousHoneycomb/GelatinousHoneycombParticle_0" + _variant).Value;
            var white = Request<Texture2D>("BombusApisBee/Content/Forest/Items/GelatinousHoneycomb/GelatinousHoneycombParticle_0" + _variant + "_White").Value;
            var bloom = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            float progress = Progress;

            float fadeIn;

            if (progress < 0.25f)
                fadeIn = EaseBuilder.EaseCircularOut.Ease(progress / 0.25f);
            else
                fadeIn = EaseBuilder.EaseCircularIn.Ease(1f - (progress - 0.25f) / 0.75f);

            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * fadeIn, Rotation, texture.Size() / 2, Scale, SpriteEffects.None, 0);
        }
    }

    class RibbonParticle : Particle
    {
        internal int _variant;
        internal bool _flaming;
        public override ParticleDrawType DrawType => ParticleDrawType.Custom;
        public RibbonParticle(Vector2 position, Vector2 velocity, float scale, int maxTime, bool flaming = false)
        {
            Position = position;
            Velocity = velocity;
            Rotation = Main.rand.NextFloat(6.28f);
            Scale = scale;
            MaxTime = maxTime;
            Color = Color.White;

            _variant = Main.rand.Next(3);
        }

        public override void Update()
        {
            Velocity *= 0.95f;
            Velocity.Y += 0.05f;
            Rotation += Velocity.Length() * 0.05f;
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            var texture = ParticleHandler.GetTexture(Type);

            float progress = Progress;

            float fadeIn;

            if (progress < 0.25f)
                fadeIn = EaseBuilder.EaseCircularOut.Ease(progress / 0.25f);
            else
                fadeIn = EaseBuilder.EaseCircularIn.Ease(1f - (progress - 0.25f) / 0.75f);

            var frame = texture.Frame(1, 3, 0, _variant);

            spriteBatch.Draw(texture, Position - Main.screenPosition, frame, Color * fadeIn, Rotation, frame.Size() / 2, Scale, SpriteEffects.None, 0);
        }
    }

    class RibbonClipParticle : Particle
    {
        public override ParticleDrawType DrawType => ParticleDrawType.Custom;
        public RibbonClipParticle(Vector2 position, Vector2 velocity, float scale, int maxTime, bool flaming = false)
        {
            Position = position;
            Velocity = velocity;
            Rotation = Main.rand.NextFloat(6.28f);
            Scale = scale;
            MaxTime = maxTime;
            Color = Color.White;
        }

        public override void Update()
        {
            Velocity *= 0.97f;
            Velocity.Y += 0.075f;
            Rotation += Velocity.Length() * 0.05f;
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            var texture = ParticleHandler.GetTexture(Type);
            float progress = Progress;

            float fadeIn;

            if (progress < 0.25f)
                fadeIn = EaseBuilder.EaseCircularOut.Ease(progress / 0.25f);
            else
                fadeIn = EaseBuilder.EaseCircularIn.Ease(1f - (progress - 0.25f) / 0.75f);

            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * fadeIn, Rotation, texture.Size() / 2, Scale, SpriteEffects.None, 0);
        }
    }
}