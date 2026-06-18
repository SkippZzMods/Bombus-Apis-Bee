using BombusApisBee.Content.Dusts.Pixelized;
using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Content.Particles;
using BombusApisBee.Core.BeekeeperClass;
using BombusApisBee.Core.Common.Apiary;
using BombusApisBee.Core.Systems.ParticleSystem;
using BombusApisBee.Core.Systems.PixelationSystem;
using BombusApisBee.Core.Systems.PrimitiveSystem;

namespace BombusApisBee.Content.Underground.Items.EnchantedCharm
{
    internal sealed class GenEnchantedCharms : ModSystem
    {
        public override void PostWorldGen()
        {
            // gold chest gen
            int itemsToPlaceInGoldChests = ItemType<EnchantedCharm>();
            for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest != null && Main.tile[chest.x, chest.y].TileType == TileID.Containers && (Main.tile[chest.x, chest.y].TileFrameX == 1 * 36 || Main.tile[chest.x, chest.y].TileFrameX == 8 * 36 || Main.tile[chest.x, chest.y].TileFrameX == 32 * 36 || Main.tile[chest.x, chest.y].TileFrameX == 51 * 36 || Main.tile[chest.x, chest.y].TileFrameX == 50 * 36))
                {
                    for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                    {
                        if (inventoryIndex == 0)
                        {
                            if (chest.item[inventoryIndex].type == ItemID.FlareGun)
                            {
                                if (WorldGen.genRand.NextFloat() < 0.25f)
                                {
                                    chest.item[0].TurnToAir();
                                    chest.item[1].TurnToAir();
                                    chest.item[0].SetDefaults(itemsToPlaceInGoldChests);
                                    chest.item[0].Prefix(-1);

                                    for (int i = 1; i < 39; i++)
                                    {
                                        chest.item[i] = chest.item[i + 1];
                                    }
                                }
                                break;
                            }
                            else
                            {
                                if (WorldGen.genRand.NextFloat() < 0.25f)
                                {
                                    chest.item[0].SetDefaults(itemsToPlaceInGoldChests);
                                    chest.item[0].Prefix(-1);
                                }

                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    [AutoloadEquip(EquipType.Neck)]
    public class EnchantedCharm : BeekeeperAccessory
    {
        internal int _cooldown;
        public EnchantedCharm() : base("Starshot Pendant", "Apiaries conjure damaging stars at night\n'Twinkle twinkle little star'") { }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 1);
        }

        public override void ResetEffects(Player player)
        {
            if (IsEquipped(player))
            {
                EnchantedCharm acc = GetEquippedInstance(player) as EnchantedCharm;
                if (acc._cooldown > 0)
                {
                    acc._cooldown--;

                    if (acc._cooldown == 0)
                        for (int i = 0; i < 5; i++)
                        {
                            Dust.NewDustPerfect(player.Center + Main.rand.NextVector2Circular(7f, 7f),
                                DustType<StarDustWhite>(), Main.rand.NextVector2Circular(0.5f, 0.5f), 20, new Color(241, 238, 92, 0), 0.2f);

                            SoundID.MaxMana.PlayWith(player.Center);
                        }
                }
            }
        }

        public override void OnWeaponUse(Player player, int damage, float knockBack)
        {
            EnchantedCharm acc = GetEquippedInstance(player) as EnchantedCharm;

            if (!Main.dayTime && player.HeldItem.ModItem is ApiaryItem && acc._cooldown <= 0)
            {
                Vector2 pos = player.Center + new Vector2(-50f * player.direction, -80f) + Main.rand.NextVector2Circular(50f, 50f);

                Projectile.NewProjectile(player.GetSource_Accessory(Item, "BombusApisBee: Enchanted Charm"),
                    pos,
                    pos.DirectionTo(Main.MouseWorld),
                    ProjectileType<EnchantedBolt>(),
                    Main.rand.Next(15, 25),
                    5f,
                    player.whoAmI
                    );

                // 3 - 6 second cooldown
                acc._cooldown = Main.rand.Next(3, 7) * 60;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FallenStar, 10).
                AddRecipeGroup(BombusApisBeeModSystem.SilverBarGroupID, 15).
                AddIngredient(ItemType<PollenItem>(), 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }

    class EnchantedBolt : ModProjectile
    {
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
        public float SpawnProgress => Timer < 60f ? Timer / 60f : 1f;
        public override void SetDefaults()
        {
            Projectile.DamageType = BeeUtils.BeeDamageClass();
            Projectile.friendly = false;

            Projectile.Size = new(12);

            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
            }

            Projectile.rotation += Projectile.velocity.Length() * 0.02f;

            if (++Timer > 60)
            {
                if (Projectile.penetrate < 0)
                    Projectile.velocity *= 0.98f;

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
                        ParticleHandler.SpawnParticle(new FallenStarParticle(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), -Projectile.velocity * 0.5f, new Color(241, 238, 92, 0), Main.rand.NextFloat(0.8f, 1.3f), 70));
                    }

                    if (Main.rand.NextBool(25))
                    {
                        Color[] colors = [new Color(241, 238, 92, 0), new Color(20, 80, 150, 0)];

                        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(25f, 25f), DustType<PixelatedEmber>(), -Projectile.velocity * 0.1f, 50, Main.rand.Next(colors), 0.2f).customData = Main.rand.NextBool() ? -1 : 1;
                    }
                }
            }
            else
            {
                if (Timer == 60)
                {
                    Projectile.friendly = true;

                    if (Main.myPlayer == Projectile.owner)
                        Projectile.velocity += Projectile.DirectionTo(Main.MouseWorld) * 10f;

                    Projectile.netUpdate = true;
                }

                //  Star particle here

                if (Main.rand.NextBool(18))
                {
                    Color[] colors = [new Color(241, 238, 92, 0), new Color(20, 80, 150, 0)];

                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(25f, 25f), DustType<PixelatedEmber>(), -Vector2.UnitY, 50, Main.rand.Next(colors), 0.2f).customData = Main.rand.NextBool() ? -1 : 1;
                }

                if (Main.rand.NextBool(20))
                {
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), DustID.YellowStarDust, -Vector2.UnitY, 50, default, 0.8f);
                }
            }
        }


        internal void DefaultBehavior()
        {
            Target = Main.npc.Where(n => n.active && n.CanBeChasedBy() && n.DistanceSQ(Projectile.Center) < 150 * 150).OrderBy(n => n.DistanceSQ(Projectile.Center)).FirstOrDefault();
        }

        internal void HomingBehavior(NPC target)
        {
            // double safety check
            if (target is null)
                return;

            if (!target.active)
            {
                Target = null;
                return;
            }

            float distance = Projectile.Distance(target.Center);

            float range = 200f;
            float speed = 8f;
            float minSpeed = speed / 2;
            float inertia = 30f;

            if (distance < range)
                speed = MathHelper.Lerp(minSpeed, speed, distance / range);

            Projectile.velocity = (Projectile.velocity * inertia + Projectile.DirectionTo(target.Center) * speed) / (inertia + 1f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].Bombus().AddDirectionalShake(-Projectile.velocity.SafeNormalize(Vector2.One) * 3f);

            SoundID.DD2_WitherBeastCrystalImpact.PlayWith(Projectile.Center);
            SoundID.DD2_CrystalCartImpact.PlayWith(Projectile.Center);

            for (int i = 0; i < 7; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f),
                    DustType<StarDust>(), -Projectile.velocity.RotatedByRandom(0.2f) * Main.rand.NextFloat(0.5f), 100, new Color(241, 238, 92, 0), 0.3f).customData = true;

                Dust.NewDustPerfect(target.Center + Main.rand.NextVector2Circular(target.width, target.height),
                    DustType<StarDustWhite>(), -Vector2.UnitY * Main.rand.NextFloat(2f), 100, new Color(241, 238, 92, 0), 0.3f);

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f),
                    DustType<StarDustWhite>(), Main.rand.NextVector2Circular(5f, 5f), 20, new Color(40, 150, 255, 0), 0.5f).customData = true;

                if (Main.rand.NextBool())
                    ParticleHandler.SpawnParticle(new FallenStarParticle(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f),
                        Main.rand.NextVector2Circular(5f, 5f), new Color(241, 238, 92, 0), Main.rand.NextFloat(0.8f, 1.5f), 90));
            }

            Projectile.timeLeft = 40;
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
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 15, new RoundedTip(), factor => MathHelper.Lerp(14f, 4f, EaseFunction.EaseCircularIn.Ease(1f - factor)), factor =>
            {
                return BeeUtils.MulticolorLerp(factor.X, [new(102, 250, 226), new(96, 72, 250), new(223, 69, 249), new(87, 255, 188, 0)]) * factor.X * 0.5f;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center;

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 15, new RoundedTip(), factor => MathHelper.Lerp(12f, 3f, EaseFunction.EaseCircularIn.Ease(1f - factor)), factor =>
            {
                return Color.Lerp(new Color(40, 100, 230), Color.LightCyan, factor.X) * factor.X * 0.2f;
            });

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = Projectile.Center;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (cache is null)
                return false;

            Main.instance.LoadProjectile(9);

            var starTexture = TextureAssets.Projectile[9].Value;
            var trail = TextureAssets.Extra[91].Value;

            var bloom = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            float fadeOut = 1f;

            if (Projectile.timeLeft < 30f)
                fadeOut = Projectile.timeLeft / 30f;

            if (Timer < 60f)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 offset = new Vector2(16f * (1f - SpawnProgress)).RotatedBy(i * 6.28f / 5);
                    offset = offset.RotatedBy((float)Math.Sin(Timer / 20f));
                    Main.spriteBatch.Draw(starTexture, Projectile.Center + offset - Main.screenPosition, null, new Color(241, 238, 92, 0) * 0.2f * SpawnProgress, Projectile.rotation, starTexture.Size() / 2f, Projectile.scale, 0f, 0f);
                }

                Vector2 shake = Main.rand.NextVector2CircularEdge(1f, 1f) * SpawnProgress;

                Main.spriteBatch.Draw(bloom, Projectile.Center + shake - Main.screenPosition, null, new Color(241, 238, 92, 0) * SpawnProgress, Projectile.rotation, bloom.Size() / 2f, Projectile.scale * 0.5f, 0f, 0f);

                Main.spriteBatch.Draw(starTexture, Projectile.Center + shake - Main.screenPosition, null, new Color(241, 238, 92) * SpawnProgress, Projectile.rotation, starTexture.Size() / 2f, Projectile.scale, 0f, 0f);

                DrawGlow(starTexture, Projectile.Center + shake, new Color(241, 238, 92, 0) * SpawnProgress, Projectile.rotation, Projectile.scale);
            }
            else
            {
                if (Timer > 60f && Projectile.velocity.Length() > 3f)
                {
                    float fade = (Projectile.velocity.Length() - 3f) / 3f * Utils.Clamp((Timer - 60f) / 40f, 0f, 1f);
                    fade = Utils.Clamp(fade, 0, 1) * fadeOut;

                    Vector2 offset = Main.rand.NextVector2Circular(2f, 2f) * fade;

                    Vector2 pos = Projectile.Center + new Vector2(-24f, 0f).RotatedBy(Projectile.velocity.ToRotation()) + offset;

                    Main.spriteBatch.Draw(trail, pos - Main.screenPosition, null, new Color(20, 80, 150, 0) * 0.3f * fade, Projectile.velocity.ToRotation() + MathHelper.PiOver2, trail.Size() / 2f, 1.1f, 0f, 0f);

                    Main.spriteBatch.Draw(trail, pos - Main.screenPosition, null, new Color(40, 100, 230, 0) * 0.3f * fade, Projectile.velocity.ToRotation() + MathHelper.PiOver2, trail.Size() / 2f, 1.1f, 0f, 0f);

                    Main.spriteBatch.Draw(trail, pos - Main.screenPosition, null, Color.LightCyan with { A = 0 } * 0.4f * fade, Projectile.velocity.ToRotation() + MathHelper.PiOver2, trail.Size() / 2f, 0.9f, 0f, 0f);

                    Main.spriteBatch.Draw(trail, pos - Main.screenPosition, null, Color.White with { A = 0 } * 0.5f * fade, Projectile.velocity.ToRotation() + MathHelper.PiOver2, trail.Size() / 2f, 0.6f, 0f, 0f);

                    DrawGlow(trail, pos, new Color(40, 100, 230, 0), Projectile.velocity.ToRotation() + MathHelper.PiOver2, 1.25f, 0.2f * fade);
                }

                for (int i = 0; i < cache.Count; i += 3)
                {
                    float lerp = i / (float)cache.Count * fadeOut;

                    if (Timer > 60f && Projectile.velocity.Length() > 3f)
                    {
                        float fade = (Projectile.velocity.Length() - 3f) / 3f * Utils.Clamp((Timer - 60f) / 40f, 0f, 1f);
                        fade = Utils.Clamp(fade, 0, 1) * fadeOut;

                        Vector2 pos = cache[i] + new Vector2(-24f, 0f).RotatedBy(Projectile.velocity.ToRotation());

                        Main.spriteBatch.Draw(trail, pos - Main.screenPosition, null, new Color(40, 100, 230, 0) * 0.4f * fade * lerp, Projectile.velocity.ToRotation() + MathHelper.PiOver2, trail.Size() / 2f, 1.25f * fade * lerp, 0f, 0f);

                        DrawGlow(trail, pos, new Color(40, 100, 230, 0), Projectile.velocity.ToRotation() + MathHelper.PiOver2, 1.25f * fade * lerp, 0.3f * fade * lerp);
                    }

                    Main.spriteBatch.Draw(bloom, cache[i] - Main.screenPosition, null, new Color(241, 238, 92, 0) * lerp, Projectile.rotation, bloom.Size() / 2f, Projectile.scale * 0.4f, 0f, 0f);

                    Main.spriteBatch.Draw(starTexture, cache[i] - Main.screenPosition, null, new Color(241, 238, 92) * 0.5f * lerp, Projectile.rotation, starTexture.Size() / 2f, Projectile.scale, 0f, 0f);

                    DrawGlow(starTexture, cache[i], new Color(241, 238, 92, 0), Projectile.rotation, Projectile.scale, 0.25f * lerp);
                }

                Vector2 shake = Vector2.Zero;
                if (fadeOut < 1f)
                    shake = Main.rand.NextVector2CircularEdge(1f, 1f) * fadeOut;

                Main.spriteBatch.Draw(bloom, Projectile.Center + shake - Main.screenPosition, null, new Color(241, 238, 92, 0) * fadeOut, Projectile.rotation, bloom.Size() / 2f, Projectile.scale * 0.5f, 0f, 0f);

                Main.spriteBatch.Draw(starTexture, Projectile.Center + shake - Main.screenPosition, null, new Color(241, 238, 92) * fadeOut, Projectile.rotation, starTexture.Size() / 2f, Projectile.scale, 0f, 0f);

                DrawGlow(starTexture, Projectile.Center + shake, new Color(241, 238, 92, 0) * fadeOut, Projectile.rotation, Projectile.scale);
            }

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

                trail2?.Render(effect);

                effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("BombusApisBee/ShaderTextures/FireTrail").Value);

                trail?.Render(effect);
                trail2?.Render(effect);
            });
        }
    }
}
