using BombusApisBee.Content.Dusts.Pixelized;
using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Content.Hell.Items.HellfireBeemstick;
using BombusApisBee.Content.Underground.Items.EnchantedCharm;
using BombusApisBee.Core.BeekeeperClass;
using BombusApisBee.Core.Common.Apiary;
using BombusApisBee.Core.Helpers;
using BombusApisBee.Core.Loading;
using BombusApisBee.Core.Systems.ParticleSystem;
using BombusApisBee.Core.Systems.PixelationSystem;
using BombusApisBee.Core.Systems.PrimitiveSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria;


namespace BombusApisBee.Content.Hell.Items.MoltenCharm
{
    [AutoloadEquip(EquipType.Neck)]
    public class MoltenCharm : BeekeeperAccessory
    {
        internal int _cooldown;
        public MoltenCharm() : base("Fireshot Pendant", "Apiaries conjure fireballs") { }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 2);
        }

        public override void ResetEffects(Player player)
        {
            if (IsEquipped(player))
            {
                MoltenCharm acc = GetEquippedInstance(player) as MoltenCharm;
                if (acc._cooldown > 0)
                {
                    acc._cooldown--;

                    if (acc._cooldown == 0)
                        for (int i = 0; i < 5; i++)
                        {
                            Dust.NewDustPerfect(player.Center + Main.rand.NextVector2Circular(7f, 7f),
                                DustType<StarDustWhite>(), Main.rand.NextVector2Circular(1.5f, 1.5f), 20, Color.DarkOrange with { A = 0 }, 0.3f);

                            SoundID.MaxMana.PlayWith(player.Center);
                        }
                }
            }
        }

        public override void OnWeaponUse(Player player, int damage, float knockBack)
        {
            MoltenCharm acc = GetEquippedInstance(player) as MoltenCharm;

            if (player.HeldItem.ModItem is ApiaryItem && acc._cooldown <= 0)
            {
                Vector2 pos = player.Center + new Vector2(-20f * player.direction, -40f) + Main.rand.NextVector2Circular(50f, 50f);

                Projectile.NewProjectile(player.GetSource_Accessory(Item, "BombusApisBee: Molten Charm"),
                    pos,
                    pos.DirectionTo(Main.MouseWorld),
                    ProjectileType<MoltenBolt>(),
                    Main.rand.Next(40, 60),
                    5f,
                    player.whoAmI
                    );

                // 3 - 8 second cooldown
                acc._cooldown = Main.rand.Next(3, 9) * 60;
                //acc._cooldown = 90;

                new SoundStyle("BombusApisBee/Sounds/Item/FireCast").PlayWith(player.Center, -0.2f, 0, 0.35f);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<EnchantedCharm>().
                AddIngredient(ItemID.HellstoneBar, 15).
                AddIngredient(ItemType<PollenItem>(), 15).
                AddTile(TileID.Anvils).
                Register();
        }
    }

    class MoltenBolt : ModProjectile
    {
        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;
        public override string Texture => BombusApisBee.Invisible;
        public int Timer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public float SpawnProgress => Timer < 60f ? Timer / 60f : 1f;

        Vector2 originalCenter;

        public Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = Main.projFrames[34];
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = BeeUtils.BeeDamageClass();
            Projectile.friendly = false;

            Projectile.Size = new(16);

            Projectile.timeLeft = 300;

            Projectile.tileCollide = false;
            Projectile.penetrate = 3;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }

            if (Timer == 0)
                Initialize();

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (++Timer > 60)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    ManageCaches();
                    ManageTrail();
                }

                if (Projectile.wet && Projectile.timeLeft > 40)
                {
                    Projectile.timeLeft = 40;
                    Projectile.friendly = false;
                    Projectile.velocity *= 0.5f;
                }

                Projectile.velocity *= 0.995f;
                if (Projectile.penetrate < 0 || Projectile.timeLeft < 40)
                    Projectile.velocity *= 0.98f;

                if (Main.rand.NextBool(7))
                    Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Main.rand.NextVector2Circular(1f, 1f), 50, default, 3f).noGravity = true;

                if (Main.rand.NextBool(6))
                    Dust.NewDustPerfect(Projectile.Center, DustType<PixelatedEmber>(), -Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.2f), 50, new Color(Main.rand.Next(150, 255), Main.rand.Next(50, 150), 0, 0), 0.15f).customData = Main.rand.NextBool() ? -1 : 1;

                if (Main.rand.NextBool() && Projectile.timeLeft > 40)
                {
                    ParticleHandler.SpawnParticle(new FireParticle(Projectile.Center, Projectile.velocity.RotatedByRandom(0.02f) * Main.rand.NextFloat(0.9f, 1.5f), new Color(0.1f, 0, 0f), Main.rand.NextFloat(0.06f, 0.12f), Main.rand.Next(35, 60), true, Main.rand.NextFloat(0.12f, 0.2f))
                    {
                        LayerPixel = RenderLayer.UnderNPCs
                    });
                }
            }
            else
            {
                if (Timer == 60)
                {
                    Projectile.friendly = true;

                    if (Main.myPlayer == Projectile.owner)
                        Projectile.velocity += Projectile.DirectionTo(Main.MouseWorld) * Main.rand.NextFloat(12f, 15f);

                    Projectile.netUpdate = true;

                    new SoundStyle("BombusApisBee/Sounds/Crossmod/Calamity/HeavyWhooshShort").PlayWith(Projectile.Center, 0.3f, 0.1f, 0.5f);

                    for (int i = 0; i < 3; i++)
                    {
                        ParticleHandler.SpawnParticle(new FireParticle(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), Main.rand.NextVector2Circular(0.2f, 0.2f), new Color(0.1f, 0, 0f), Main.rand.NextFloat(0.07f, 0.1f), 55)
                        {
                            LayerPixel = RenderLayer.UnderNPCs
                        });
                    }
                }

                Vector2 target = Owner.Center + originalCenter;

                Projectile.Center = Vector2.Lerp(Projectile.Center, target + Vector2.Lerp(Vector2.Zero, new Vector2(0f, -20f), EaseBuilder.EaseQuarticIn.Ease(SpawnProgress)), 0.2f);

                if (Main.rand.NextBool((int)MathHelper.Lerp(9, 2, SpawnProgress)))
                {
                    Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Main.rand.NextVector2Circular(1f, 1f) * MathHelper.Lerp(0.5f, 5f, SpawnProgress), 50, default, 3f).noGravity = true;
                    
                    Dust.NewDustPerfect(Projectile.Center, DustType<PixelatedEmber>(), Main.rand.NextVector2Circular(1f, 1f) * MathHelper.Lerp(0.5f, 5f, SpawnProgress), 50, new Color(Main.rand.Next(150, 255), Main.rand.Next(50, 150), 0, 0), 0.2f).customData = Main.rand.NextBool() ? -1 : 1;
                }
            }              
        }

        internal void Initialize()
        {
            originalCenter = Projectile.Center - Owner.Center;
            Projectile.Center = Owner.Center + originalCenter;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            new SoundStyle("BombusApisBee/Sounds/Item/FireHit").PlayWith(Projectile.Center, 0, 0, 0.35f);

            Main.player[Projectile.owner].Bombus().AddShake(5);

            target.AddBuff(BuffID.OnFire3, 300);

            if (Projectile.penetrate == 1)
                Projectile.timeLeft = 40;

            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Main.rand.NextVector2Circular(4f, 4f), 50, default, 3f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, DustType<PixelatedEmber>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 50, new Color(Main.rand.Next(150, 255), Main.rand.Next(50, 150), 0, 0), 0.2f).customData = Main.rand.NextBool() ? -1 : 1;
            }

            for (int i = 0; i < 9; i++)
            {
                ParticleHandler.SpawnParticle(new FireParticle(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), Main.rand.NextVector2Circular(1f, 1f) - Vector2.UnitY * Main.rand.NextFloat(2.5f), new Color(0.1f, 0, 0f), Main.rand.NextFloat(0.07f, 0.1f), 45)
                {
                    LayerPixel = RenderLayer.UnderNPCs
                });
            }
        }

        public override void OnKill(int timeLeft)
        {
            
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

        private float Fade()
        {
            if (Projectile.timeLeft < 30f)
                return Projectile.timeLeft / 30f;

            if (Timer < 60f)
                return 0f;

            if (Timer > 90f)
                return 1f;
            
            return (Timer - 60f) / 30f;
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 15, new RoundedTip(), factor => MathHelper.Lerp(9f, 3f, EaseFunction.EaseCircularIn.Ease(1f - factor)), factor =>
            {
                return new Color(255, 50, 20) * factor.X * Fade();
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 15, new RoundedTip(), factor => MathHelper.Lerp(7f, 2f, EaseFunction.EaseCircularIn.Ease(1f - factor)), factor =>
            {
                return BeeUtils.MulticolorLerp(factor.X, [new Color(200, 110, 20, 0), new Color(200, 50, 0, 0), new Color(255, 150, 20)]) * factor.X * Fade();
            });

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = Projectile.Center + Projectile.velocity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(34);

            var bloom = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            var fireball = TextureAssets.Projectile[34].Value;

            SpriteBatch sb = Main.spriteBatch;

            Rectangle frame = fireball.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);

            if (Timer < 60f)
            {
                Vector2 shake = Main.rand.NextVector2CircularEdge(1f, 1f) * SpawnProgress;

                sb.Draw(bloom, Projectile.Center - Main.screenPosition, null, new Color(255, 150, 0, 0), 0f, bloom.Size() / 2f, 0.45f, 0f, 0f);

                sb.Draw(fireball, Projectile.Center + shake - Main.screenPosition, frame, Color.White, Owner.velocity.X * 0.05f, frame.Size() / 2f, 1f, 0f, 0f);

                return false;
            }

            if (cache is null)
                return false;

            DrawPrimitives();

            float fadeIn = (Timer - 60f) / 30f;
            fadeIn = Utils.Clamp(fadeIn, 0, 1);

            float fadeOut = 1f;

            if (Projectile.timeLeft < 30f)
                fadeOut = Projectile.timeLeft / 30f;

            for (int i = 0; i < cache.Count; i += 2)
            {
                float lerp = i / (float)cache.Count;
                
                sb.Draw(bloom, cache[i] - Main.screenPosition, null, new Color(255, 50, 0, 0) * 0.5f * lerp * fadeIn * fadeOut, 0f, bloom.Size() / 2f, 0.45f, 0f, 0f);

                sb.Draw(bloom, cache[i] - Main.screenPosition, null, new Color(255, 150, 0, 0) * 0.5f * lerp * fadeIn * fadeOut, 0f, bloom.Size() / 2f, 0.45f, 0f, 0f);

                sb.Draw(fireball, cache[i] - Main.screenPosition, frame, Color.White * 0.5f * lerp * fadeOut, Projectile.rotation - MathHelper.PiOver2, frame.Size() / 2f, 1f, 0f, 0f);
            }
            
            sb.Draw(bloom, Projectile.Center - Main.screenPosition, null, new Color(255, 50, 0, 0) * 0.5f * fadeIn * fadeOut, 0f, bloom.Size() / 2f, 0.45f, 0f, 0f);

            sb.Draw(bloom, Projectile.Center - Main.screenPosition, null, new Color(255, 150, 0, 0) * 0.5f * fadeIn * fadeOut, 0f, bloom.Size() / 2f, 0.45f, 0f, 0f);

            sb.Draw(fireball, Projectile.Center - Main.screenPosition, frame, Color.White * fadeOut, Projectile.rotation - MathHelper.PiOver2,  frame.Size() / 2f, 1f, 0f, 0f);

            return false;
        }

        public void DrawPrimitives()
        {
            GetInstance<PixelationSystem>().QueueRenderAction("UnderProjectiles", () =>
            {
                Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.EffectMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["time"].SetValue(Projectile.timeLeft * 0.02f);
                effect.Parameters["repeats"].SetValue(1f);
                effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

                trail2?.Render(effect);

                effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("BombusApisBee/ShaderTextures/FireTrail").Value);

                trail?.Render(effect);
                trail2?.Render(effect);
            });
        }
    }
}
