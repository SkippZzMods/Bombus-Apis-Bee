using BombusApisBee.Assets;
using BombusApisBee.Content.Dusts.Pixelized;
using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Content.Underground.Items.EnchantedCharm;
using BombusApisBee.Core.Common.Apiary;
using BombusApisBee.Core.Common.BeeProjectile;
using BombusApisBee.Core.Systems.ParticleSystem;
using BombusApisBee.Core.Systems.PixelationSystem;
using BombusApisBee.Core.Systems.PrimitiveSystem;
using Terraria;

namespace BombusApisBee.Content.Space.Items.StarryApiary
{
    public class StarryApiary : ApiaryItem
    {
        public int cooldown;

        public override int BaseUseTime => 23;
        public override int AltUseTime => 35;
        public override void AddStaticDefaults()
        {
            DisplayName.SetDefault("Starfall");
            Tooltip.SetDefault("" +
                "Hold <left> to rapidly fire bees\n" +
                "Hold <right> to fire bees slower, but take control over the bees causing them to rain stars on nearby enemies\n" +
                "Stars get more powerful during nightfall");
        }

        public override void AddDefaults()
        {
            Item.damage = 21;
            Item.noMelee = true;
            Item.width = 32;
            Item.height = 32;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3f;

            Item.value = Item.sellPrice(gold: 2, silver: 75);

            Item.rare = ItemRarityID.Green;

            Item.autoReuse = true;

            Item.shoot = ProjectileType<StarryApiaryHoldout>();

            Item.shootSpeed = 6f;

            Item.UseSound = SoundID.Item11;

            honeyCost = 1;
            altHoneyCost = 3;
        }

        public override void OnApiaryKill(Projectile projectile, int timeLeft)
        {         
            for (int i = 0; i < 3; i++)
            {
                Color color = Main.rand.Next(StarryApiaryHoldout.starColors) with { A = 0 };

                ParticleHandler.SpawnParticle(new StarParticle(projectile.Center, Main.rand.NextVector2Circular(5f, 5f), color, 0.9f, 45));
            }
        }

        public override void HoldAI(Projectile Projectile)
        {
            base.HoldAI(Projectile);

            if (cooldown <= 0)
            {
                NPC[] closestFive = Main.npc.Where(n => n.CanBeChasedBy(Projectile) && n.DistanceSQ(Projectile.Center) < 250000f).OrderBy(n => n.DistanceSQ(Projectile.Center)).Take(5).ToArray();

                if (closestFive.Length > 0)
                {
                    NPC chosen = closestFive[Main.rand.Next(closestFive.Length)];

                    Vector2 targetPosition = chosen.Center;
                    targetPosition += chosen.velocity * 5f;
                    cooldown = Main.rand.Next(4, 10) * 60;
                    //cooldown = 45;

                    Vector2 pos = targetPosition + new Vector2(Main.rand.Next(-250, 250), -650) + Main.rand.NextVector2Circular(50f, 50f);

                    Player owner = Main.player[Projectile.owner];

                    int damage = Projectile.damage * 3;
                    float knockBack = 4f;
                    float speed = 30f;

                    int[] starTypes = [
                        ProjectileType<StarryApiaryStar>(),
                        ProjectileType<StarryApiaryStarGreen>(),
                        ProjectileType<StarryApiaryStarBlue>(),
                        ProjectileType<StarryApiaryStarPink>(),
                        ProjectileType<StarryApiaryStarPurple>()
                    ];

                    int type = Main.rand.Next(starTypes);

                    if (!Main.dayTime)
                    {
                        damage = (int)(damage * 1.2f);
                        knockBack *= 1.2f;

                        if (Main.rand.NextBool(4))
                        {
                            type = ProjectileType<StarryApiaryStarGolden>();
                            damage *= 2;
                            knockBack *= 2f;
                            speed = 33.5f;
                        }           
                    }   

                    Projectile.NewProjectileDirect(owner.GetSource_Misc("BombusApisBee: Starry Apiary Star Spawn"),
                        pos,
                        pos.DirectionTo(targetPosition) * speed,
                        type,
                        damage,
                        knockBack,
                        owner.whoAmI
                        );
                }
                else
                    cooldown = 20;
            }

            if (Main.rand.NextBool(90))
            {
                Color color = BeeUtils.MulticolorLerp((float)Math.Abs(Math.Sin(Main.GlobalTimeWrappedHourly * 1f)), StarryApiaryHoldout.starColors);

                color.A = 0;

                Vector2 velocity = Projectile.velocity.RotatedByRandom(0.25f) * Main.rand.NextFloat(0.2f, 1f);

                ParticleHandler.SpawnParticle(new FallenStarParticle(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f),
                    velocity, color, Main.rand.NextFloat(0.6f, 1.1f), 40));
            }
        }

        public override void PostDrawApiaryBees(Projectile projectile, Color lightColor, bool active)
        {
            bool giant = (projectile.ModProjectile as CommonBeeProjectile).Giant;

            Texture2D tex = Request<Texture2D>(giant ? AssetDirectory.GiantBeeOutline : AssetDirectory.BeeOutline).Value;
            Texture2D bloom = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Player player = Main.player[projectile.owner];

            int holdTimer = player.GetModPlayer<ApiaryPlayer>().apiaryVisualTimer;

            Color color = BeeUtils.MulticolorLerp((float)Math.Abs(Math.Sin(Main.GlobalTimeWrappedHourly * 1f)), StarryApiaryHoldout.starColors);

            color.A = 0;

            Rectangle frame = tex.Frame(1, 4, frameY: projectile.frame);

            if (holdTimer > 0)
            {
                Main.spriteBatch.Draw(tex, projectile.Center + new Vector2(0, -1).RotatedBy(projectile.rotation) - Main.screenPosition, frame, color * 0.5f * (holdTimer / (float)player.GetModPlayer<ApiaryPlayer>().maxVisualTimer), projectile.rotation, frame.Size() / 2f, projectile.scale, projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(bloom, projectile.Center - Main.screenPosition, null, color * (holdTimer / (float)player.GetModPlayer<ApiaryPlayer>().maxVisualTimer) * 0.2f, 0f, bloom.Size() / 2f, 0.35f, 0, 0f);
            }

        }

        public override void UpdateInventory(Player player)
        {
            if (cooldown > 0)
                cooldown--;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PollenItem>(15).
                AddIngredient(ItemID.MeteoriteBar, 20).
                AddIngredient(ItemID.FallenStar, 5).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }

    public class StarryApiaryHoldout : ApiaryHoldout
    {
        public static Color[] starColors = [new(255, 255, 161, 0), new(150, 240, 131, 0), new(131, 240, 206, 0), new(131, 183, 240, 0), new(200, 131, 240, 0), new(240, 131, 240, 0)];

        public override Color GlowColor => BeeUtils.MulticolorLerp((float)Math.Abs(Math.Sin(Main.GlobalTimeWrappedHourly * 1f)), starColors);//Color.Lerp(new Color(255, 180, 0), new Color(255, 225, 0, 0), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1f));
        //public override string Texture => "BombusApisBee/Content/Forest/Items/WoodenApiary/WoodenApiary";

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void PostAI()
        {
            Lighting.AddLight(Projectile.Center, GlowColor.ToVector3());
        }

        protected override void Shoot()
        {
            flashTimer = 20;
            swingRotation += Main.rand.NextFloat(-0.22f, 0.22f);
            shakeTimer = 15;


            SoundID.Item97.PlayWith(Projectile.Center, 0, 0.1f, 1.25f);
            BombusApisBee.HoneycombWeapon.PlayWith(Projectile.Center, volume: 0.5f);

            for (int j = 0; j < 4; j++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<PixelatedGlow>(), Main.rand.NextVector2Circular(3f, 3f), 0, GlowColor with { A = 0 }, 0.15f);

                ParticleHandler.SpawnParticle(new FallenStarParticle(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.8f, 2.5f) + Main.rand.NextVector2CircularEdge(1f, 1f), Main.rand.Next(starColors) with { A = 0 }, Main.rand.NextFloat(0.8f, 1.3f), 70));
            }

            Vector2 offset = Main.rand.NextVector2Circular(15f, 15f);

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + offset, Projectile.velocity.RotatedByRandom(1f) * 2f + Main.rand.NextVector2CircularEdge(1f, 1f), ProjectileTypeToFire, Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }

    public class StarryApiaryStar : ModProjectile
    {
        public virtual Color StarColor => new Color(241, 238, 92);
        public virtual Color AuraColor_01 => new Color(20, 80, 150);
        public virtual Color AuraColor_02 => new Color(40, 100, 230);
        public virtual Color AuraColor_03 => Color.LightCyan;

        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;
        public override string Texture => BombusApisBee.Invisible;

        public NPC Target = null;
        public int Timer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = BeeUtils.BeeDamageClass();
            Projectile.friendly = true;

            Projectile.Size = new(12);

            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.ArmorPenetration = 5;
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                ManageCache();
                ManageTrail();
            }

            Timer++;

            Projectile.rotation += Projectile.velocity.Length() * 0.02f;

            Lighting.AddLight(Projectile.Center, StarColor.ToVector3() * TrailFade());

            if (Projectile.penetrate < 0)
                Projectile.velocity *= 0.9f;

            if (Projectile.velocity.Length() > 3f)
            {
                if (Projectile.soundDelay == 0)
                {
                    Projectile.soundDelay = 28;
                    SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
                }

                if (Main.rand.NextBool(20))
                    Dust.NewDustPerfect(Projectile.Center, DustID.YellowStarDust, -Projectile.velocity * 0.1f, 50, default, 0.8f);

                if (Main.rand.NextBool(30))
                {
                    ParticleHandler.SpawnParticle(new FallenStarParticle(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), -Projectile.velocity * 0.5f, StarColor with { A = 0 }, Main.rand.NextFloat(0.8f, 1.3f), 70));
                }

                if (Main.rand.NextBool(25))
                {
                    Color[] colors = [StarColor with { A = 0 }, AuraColor_01 with { A = 0 }];

                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(25f, 25f), DustType<PixelatedEmber>(), -Projectile.velocity * 0.1f, 50, Main.rand.Next(colors), 0.2f).customData = Main.rand.NextBool() ? -1 : 1;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].Bombus().AddDirectionalShake(-Projectile.velocity.SafeNormalize(Vector2.One) * 1.2f);

            SoundID.DD2_WitherBeastCrystalImpact.PlayWith(Projectile.Center);
            SoundID.DD2_CrystalCartImpact.PlayWith(Projectile.Center);

            for (int i = 0; i < 7; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f),
                    DustType<StarDust>(), -Projectile.velocity.RotatedByRandom(0.2f) * Main.rand.NextFloat(0.5f), 100, StarColor with { A = 0 }, 0.3f).customData = true;

                Dust.NewDustPerfect(target.Center + Main.rand.NextVector2Circular(target.width, target.height),
                    DustType<StarDustWhite>(), -Vector2.UnitY * Main.rand.NextFloat(2f), 100, StarColor with { A = 0 }, 0.3f);

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f),
                    DustType<StarDustWhite>(), Main.rand.NextVector2Circular(5f, 5f), 20, AuraColor_02 with { A = 0 }, 0.5f).customData = true;

                if (Main.rand.NextBool())
                    ParticleHandler.SpawnParticle(new FallenStarParticle(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f),
                        Main.rand.NextVector2Circular(5f, 5f), StarColor with { A = 0 }, Main.rand.NextFloat(0.8f, 1.5f), 90));
            }

            ParticleHandler.SpawnParticle(new StarImpactParticle(target.Center, [StarColor with { A = 0 }, AuraColor_02 with { A = 0 }], [StarColor with { A = 0 }, AuraColor_02 with { A = 0 }], new(0.5f, 0.4f), new(2.5f, 0.2f), 120));

            Projectile.timeLeft = 40;
        }

        private void ManageCache()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < 16; i++)
                {
                    cache.Add(Projectile.Center);
                }
            }

            cache.Add(Projectile.Center);

            while (cache.Count > 16)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 16, new RoundedTip(), factor => MathHelper.Lerp(5f, 2f, EaseFunction.EaseCircularIn.Ease(1f - factor)), factor =>
            {
                return Color.Lerp(StarColor, AuraColor_02, 1f - factor.X) * TrailFade() * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center;

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 16, new RoundedTip(), factor => MathHelper.Lerp(3f, 1f, EaseFunction.EaseCircularIn.Ease(1f - factor)), factor =>
            {
                return Color.White with { A = 0 } * TrailFade() * factor.X;
            });

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = Projectile.Center;
        }

        public float TrailFade()
        {
            float fadeIn = 1f;

            if (Timer < 30)
                fadeIn = Timer / 30f;

            if (Projectile.timeLeft < 30f)
                fadeIn = Projectile.timeLeft / 30f;

            return fadeIn;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (cache is null)
                return false;

            DrawPrimitives();

            Main.instance.LoadProjectile(9);

            var starTexture = TextureAssets.Projectile[9].Value;
            var trail = TextureAssets.Extra[91].Value;

            var bloom = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            float fadeIn = TrailFade();

            if (Projectile.velocity.Length() > 3f)
            {
                float fade = (Projectile.velocity.Length() - 3f) / 3f;
                fade = Utils.Clamp(fade, 0, 1) * fadeIn;

                Vector2 offset = Main.rand.NextVector2Circular(2f, 2f) * fade;

                Vector2 pos = Projectile.Center + new Vector2(-24f, 0f).RotatedBy(Projectile.velocity.ToRotation()) + offset;

                Main.spriteBatch.Draw(trail, pos - Main.screenPosition, null, AuraColor_01 with { A = 0 } * 0.3f * fade, Projectile.velocity.ToRotation() + MathHelper.PiOver2, trail.Size() / 2f, 1.1f, 0f, 0f);

                Main.spriteBatch.Draw(trail, pos - Main.screenPosition, null, AuraColor_02 with { A = 0 } * 0.3f * fade, Projectile.velocity.ToRotation() + MathHelper.PiOver2, trail.Size() / 2f, 1.1f, 0f, 0f);

                Main.spriteBatch.Draw(trail, pos - Main.screenPosition, null, AuraColor_03 with { A = 0 } * 0.4f * fade, Projectile.velocity.ToRotation() + MathHelper.PiOver2, trail.Size() / 2f, 0.9f, 0f, 0f);

                Main.spriteBatch.Draw(trail, pos - Main.screenPosition, null, Color.White with { A = 0 } * 0.5f * fade, Projectile.velocity.ToRotation() + MathHelper.PiOver2, trail.Size() / 2f, 0.6f, 0f, 0f);

                DrawGlow(trail, pos, AuraColor_02 with { A = 0 }, Projectile.velocity.ToRotation() + MathHelper.PiOver2, 1.25f, 0.2f * fade);
            }

            for (int i = 0; i < cache.Count; i += 2)
            {
                float lerp = i / (float)cache.Count * fadeIn;

                if (Projectile.velocity.Length() > 3f)
                {
                    float fade = (Projectile.velocity.Length() - 3f) / 3f;
                    fade = Utils.Clamp(fade, 0, 1);

                    Vector2 pos = cache[i] + new Vector2(-24f, 0f).RotatedBy(Projectile.velocity.ToRotation());

                    Vector2 trailPos = cache[i] + new Vector2(-4f, 0f).RotatedBy(Projectile.velocity.ToRotation());

                    Main.spriteBatch.Draw(trail, trailPos - Main.screenPosition, null, AuraColor_01 with { A = 0 } * 0.3f * fade * lerp, Projectile.velocity.ToRotation() + MathHelper.PiOver2, trail.Size() / 2f, 1.4f * lerp, 0f, 0f);

                    Main.spriteBatch.Draw(trail, trailPos - Main.screenPosition, null, AuraColor_02 with { A = 0 } * 0.3f * fade * lerp, Projectile.velocity.ToRotation() + MathHelper.PiOver2, trail.Size() / 2f, 1.3f * lerp, 0f, 0f);

                    Main.spriteBatch.Draw(trail, trailPos - Main.screenPosition, null, AuraColor_03 with { A = 0 } * 0.4f * fade * lerp, Projectile.velocity.ToRotation() + MathHelper.PiOver2, trail.Size() / 2f, 1.2f * lerp, 0f, 0f);

                    Main.spriteBatch.Draw(trail, trailPos - Main.screenPosition, null, Color.White with { A = 0 } * 0.5f * fade * lerp, Projectile.velocity.ToRotation() + MathHelper.PiOver2, trail.Size() / 2f, 0.9f * lerp, 0f, 0f);

                    DrawGlow(trail, pos, AuraColor_02 with { A = 0 }, Projectile.velocity.ToRotation() + MathHelper.PiOver2, 1.25f * fade * lerp, 0.3f * fade * lerp);
                }

                Main.spriteBatch.Draw(bloom, cache[i] - Main.screenPosition, null, StarColor with { A = 0 } * lerp, Projectile.rotation, bloom.Size() / 2f, Projectile.scale * 0.4f, 0f, 0f);

                Main.spriteBatch.Draw(starTexture, cache[i] - Main.screenPosition, null, StarColor * 0.5f * lerp, Projectile.rotation, starTexture.Size() / 2f, Projectile.scale, 0f, 0f);

                DrawGlow(starTexture, cache[i], StarColor with { A = 0 }, Projectile.rotation, Projectile.scale, 0.25f * lerp);
            }

            Vector2 shake = Vector2.Zero;
            if (fadeIn < 1f)
                shake = Main.rand.NextVector2CircularEdge(1f, 1f) * fadeIn;

            Main.spriteBatch.Draw(bloom, Projectile.Center + shake - Main.screenPosition, null, StarColor with { A = 0 } * fadeIn, Projectile.rotation, bloom.Size() / 2f, Projectile.scale * 0.5f, 0f, 0f);

            Main.spriteBatch.Draw(starTexture, Projectile.Center + shake - Main.screenPosition, null, StarColor * fadeIn, Projectile.rotation, starTexture.Size() / 2f, Projectile.scale, 0f, 0f);

            DrawGlow(starTexture, Projectile.Center + shake, StarColor with { A = 0 } * fadeIn, Projectile.rotation, Projectile.scale);

            void DrawGlow(Texture2D tex, Vector2 position, Color color, float rotation, float scale, float opacity = 1f)
            {
                for (int i = 0; i < 12; i++)
                {
                    Vector2 offset = new Vector2(2).RotatedBy(6.28f * i / 12f);

                    Main.spriteBatch.Draw(tex, position + offset - Main.screenPosition, null, color * opacity * 0.1f, rotation, tex.Size() / 2f, scale, 0f, 0f);
                }
            }

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

                trail?.Render(effect);
                trail2?.Render(effect);
            });
        }
    }

    public class StarryApiaryStarGreen : StarryApiaryStar
    {
        public override Color StarColor => new Color(114, 226, 93);
        public override Color AuraColor_01 => new Color(99, 31, 175);
        public override Color AuraColor_02 => new Color(110, 48, 195);
        public override Color AuraColor_03 => new Color(232, 192, 255);
    }

    public class StarryApiaryStarBlue : StarryApiaryStar
    {
        public override Color StarColor => new Color(92, 178, 241);
        public override Color AuraColor_01 => new Color(20, 41, 150);
        public override Color AuraColor_02 => new Color(59, 43, 230);
        public override Color AuraColor_03 => new Color(200, 186, 255);
    }

    public class StarryApiaryStarPurple : StarryApiaryStar
    {
        public override Color StarColor => new Color(170, 93, 226);
        public override Color AuraColor_01 => new Color(255, 161, 0);
        public override Color AuraColor_02 => new Color(255, 206, 25);
        public override Color AuraColor_03 => new Color(241, 255, 163);
    }

    public class StarryApiaryStarPink : StarryApiaryStar
    {
        public override Color StarColor => new Color(226, 93, 219);
        public override Color AuraColor_01 => new Color(20, 143, 150);
        public override Color AuraColor_02 => new Color(43, 195, 179);
        public override Color AuraColor_03 => new Color(186, 255, 244);
    }

    public class StarryApiaryStarGolden : StarryApiaryStar
    {
        public override Color StarColor => new Color(255, 148, 0);
        public override Color AuraColor_01 => new Color(255, 94, 0);
        public override Color AuraColor_02 => new Color(255, 215, 0);
        public override Color AuraColor_03 => new Color(246, 255, 168);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.ArmorPenetration += 10;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].Bombus().AddShake(6);

            new SoundStyle("BombusApisBee/Sounds/Item/StarHit").PlayWith(Projectile.Center);

            for (int i = 0; i < 6; i++)
            {
                Vector2 velocity = -Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.5f);

                ParticleHandler.SpawnParticle(new GlowLineParticle(Projectile.Center, velocity, StarColor with { A = 0}, velocity.ToRotation(), new Vector2(0.5f, 2f), 40, extraUpdateAction: ExtraUpdate));

                ParticleHandler.SpawnParticle(new GlowLineParticle(Projectile.Center, velocity, Color.White with { A = 0 }, velocity.ToRotation(), new Vector2(0.5f, 2f) * 0.6f, 40, extraUpdateAction: ExtraUpdate));
            }

            static void ExtraUpdate(Particle p)
            {
                p.Velocity *= 0.92f;
            }

            base.OnHitNPC(target, hit, damageDone);
        }
    }
}
