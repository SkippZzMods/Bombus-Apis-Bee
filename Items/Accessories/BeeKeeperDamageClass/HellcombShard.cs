using BombusApisBee.Items.Other.Crafting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    public class HellcombDust : ModDust
    {
        public override string Texture => "BombusApisBee/ExtraTextures/GlowAlpha";

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, 0, 160, 160);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.Lerp(new Color(200, 40, 20, 0), new Color(25, 25, 25, 0), dust.alpha / 255f) * ((255 - dust.alpha) / 255f);
        }

        public override bool Update(Dust dust)
        {
            dust.velocity *= 0.925f;
            dust.color *= 0.98f;

            if (dust.alpha > 100)
            {
                dust.scale *= 0.985f;
                dust.alpha += 4;
            }
            else
            {
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.1f);
                dust.scale *= 0.975f;
                dust.alpha += 4;
            }

            dust.position += dust.velocity;
            dust.rotation += 0.04f;

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            float lerper = (1f - dust.alpha / 255f);

            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, new Color(200, 40, 20, 0) * lerper * 0.5f, 0f, Texture2D.Value.Size() / 2f, dust.scale, 0f, 0f);

            Effect effect = Filters.Scene["HolyShieldShader"].GetShader().Shader;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.0001f * dust.scale);
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.0001f * dust.scale);
            effect.Parameters["screenPos"].SetValue(Main.screenPosition * new Vector2(0.5f, 0.1f) / new Vector2(Main.screenWidth, Main.screenHeight));

            effect.Parameters["offset"].SetValue(Vector2.Lerp(new Vector2(0.015f), Vector2.Zero, lerper));
            effect.Parameters["repeats"].SetValue(2);
            effect.Parameters["uImage1"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);
            effect.Parameters["uImage2"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/MiscNoise1").Value);
            Color color = Color.Lerp(new Color(255, 200, 20, 0), new Color(25, 25, 25, 0), lerper) * 0.5f * lerper;

            effect.Parameters["uColor"].SetValue(color.ToVector4());
            effect.Parameters["noiseImage1"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/PerlinNoise").Value);

            effect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, Color.White, 0f, Texture2D.Value.Size() / 2f, dust.scale, 0f, 0f);
           
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }

    public class HellcombShard : BeeKeeperItem
    {
        public override void Load()
        {
            BombusApisBeeGlobalProjectile.StrongBeeKillEvent += SpawnBloodsplosion;
        }

        private void SpawnBloodsplosion(Projectile proj, int timeLeft)
        {
            if (Main.player[proj.owner].Bombus().HasHellcombShard && Main.rand.NextBool(15))
            {
                Projectile.NewProjectileDirect(proj.GetSource_Death(), proj.Center, Vector2.Zero, ProjectileType<HellcombShardExplosion>(), proj.damage * 3, proj.knockBack, proj.owner, 75);

                new SoundStyle("BombusApisBee/Sounds/Item/FireHit").PlayWith(proj.Center, 0, 0, 1.25f);

                Main.player[proj.owner].Bombus().AddShake(18);

                for (int i = 0; i < 20; i++)
                { 
                    Dust.NewDustPerfect(proj.Center, ModContent.DustType<ExplosionDust>(), Main.rand.NextVector2Circular(20f, 20f), 150 + Main.rand.Next(60), default, Main.rand.NextFloat(0.9f, 1.5f)).rotation = Main.rand.NextFloat(6.28f);

                    Dust.NewDustPerfect(proj.Center, ModContent.DustType<ExplosionDustTwo>(), Main.rand.NextVector2Circular(20f, 20f), 50 + Main.rand.Next(80), default, Main.rand.NextFloat(0.9f, 1.5f)).rotation = Main.rand.NextFloat(6.28f);

                    Dust.NewDustPerfect(proj.Center, ModContent.DustType<ExplosionDust>(), Main.rand.NextVector2Circular(20f, 20f), 150 + Main.rand.Next(50), default, Main.rand.NextFloat(0.9f, 1.5f)).rotation = Main.rand.NextFloat(6.28f);
                }

                for (int i = 0; i < 25; i++)
                {
                    Dust.NewDustPerfect(proj.Center, DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(15f, 15f), 0, new Color(255, 200, 0, 0), 0.6f);
                }
            }
        }

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Increases the chance to strengthen friendly bees by 40%\nStrengthened bees have a chance to cause volatile explosions");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.accessory = true;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 5);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Hymenoptra().BeeStrengthenChance += 0.4f;
            player.Bombus().HasHellcombShard = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Pollen>(30).
                AddIngredient(ItemID.HellstoneBar, 12).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }

    class HellcombShardExplosion : ModProjectile
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
            DisplayName.SetDefault("Volatile Explosion");
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

                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustPerfect(Projectile.Center + Vector2.One.RotatedBy(rot) * Radius, ModContent.DustType<HellcombDust>(),
                        Vector2.One.RotatedBy(rot) * 0.5f, 0, default, Main.rand.NextFloat(0.6f, 0.7f));
                }
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

            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            float fadeOut = Projectile.timeLeft / 30f;

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(255, 40, 20, 0) * fadeOut, 0f, bloomTex.Size() / 2f, MathHelper.Lerp(0.5f, 4.5f, Progress), 0f, 0f);

            Effect effect = Filters.Scene["HolyShieldShader"].GetShader().Shader;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.02f);
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.02f);
            effect.Parameters["screenPos"].SetValue(Main.screenPosition * new Vector2(0.5f, 0.1f) / new Vector2(Main.screenWidth, Main.screenHeight));

            effect.Parameters["offset"].SetValue(new Vector2(0.005f));
            effect.Parameters["repeats"].SetValue(1);
            effect.Parameters["uImage1"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);
            effect.Parameters["uImage2"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/MiscNoise1").Value);
            Color color = new Color(255, 0, 0, 0) * fadeOut;

            effect.Parameters["uColor"].SetValue(color.ToVector4());
            effect.Parameters["noiseImage1"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/PerlinNoise").Value);

            effect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, Color.White, 0f, bloomTex.Size() / 2f, MathHelper.Lerp(0.5f, 2.5f, Progress), 0f, 0f);

            color = new Color(255, 200, 0, 0) * fadeOut;

            effect.Parameters["uColor"].SetValue(color.ToVector4());
            effect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, Color.White, 0f, bloomTex.Size() / 2f, MathHelper.Lerp(0.5f, 2.5f, Progress), 0f, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(255, 0, 0, 0) * fadeOut, 0f, bloomTex.Size() / 2f, MathHelper.Lerp(0.5f, 2.5f, Progress), 0f, 0f);

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
                cache[k] = (Projectile.Center + Vector2.One.RotatedBy(k / 19f * 6.28f) * (Radius));
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
                return new Color(200, 40, 20);
            });

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 40, new TriangularTip(1), factor => 30 * (1f - Progress), factor =>
            {
                return new Color(255, 90, 20) * 0.75f;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = cache[39];

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = cache[39];
        }

        public void DrawPrimitives()
        {
            Main.spriteBatch.End();
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

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
