using BombusApisBee.Content.Dusts.Pixelized;
using BombusApisBee.Core.Systems.PixelationSystem;
using BombusApisBee.Core.Systems.PrimitiveSystem;
using Terraria.DataStructures;

namespace BombusApisBee.Content.Crossmod.Calamity.Items.Weapons.Corruption
{
    public class ShadestingerScepter : CalamityDamageItem
    {
        public float shootRotation;
        public int shootDirection;

        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Shadestinger Scepter");
            Tooltip.SetDefault("Casts a volley of unstable shadestingers, infusing Dark Energy within enemies\n" +
                "Strike enemies infused with sufficient Dark Energy to rip Shadestung bees from them\n");
        }

        public override void SafeSetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.damage = 15;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.knockBack = 0.5f;

            Item.rare = ItemRarityID.Orange;

            Item.shoot = ProjectileType<ShadestingerScepterHoldout>();

            Item.value = Item.sellPrice(gold: 1);

            honeyCost = 2;
        }

        public override bool SafeCanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ProjectileType<ShadestingerScepterHoldout>()] <= 0;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

            SoundID.Item1.PlayWith(position, volume: 0.5f);

            return false;
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class ShadestingerScepterHoldout : ModProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public bool dying;
        public int dyingTimer;

        public Vector2[] projectilePoints = new Vector2[3];
        public float Progress => Timer / MaxTimer;
        public ref float Timer => ref Projectile.ai[0];
        public ref float MaxTimer => ref Projectile.ai[1];
        public Player Owner => Main.player[Projectile.owner];
        public override string Texture => "BombusApisBee/Content/Crossmod/Calamity/Items/Weapons/Corruption/ShadestingerScepter";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadestinger Scepter");
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            MaxTimer = (int)Owner.ApplyHymenoptraSpeedTo(Owner.HeldItem.useAnimation);

            for (int i = 0; i < projectilePoints.Length; i++)
            {
                Vector2 offset = Owner.Center + new Vector2(0f, 50f) + Main.rand.NextVector2CircularEdge(55f, 55f);

                projectilePoints[i] = Owner.Center - offset;
            }
        }

        public override void AI()
        {
            UpdateHeldProjectile();

            if (dying)
            {
                dyingTimer++;
                if (dyingTimer > 15)
                    Projectile.Kill();

                Projectile.position += Vector2.Lerp(new Vector2(0f, 5f), new Vector2(0f, 15f), EaseBuilder.EaseBackOut.Ease(dyingTimer / 15f));
            }

            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, MathHelper.Lerp(-2f, -MathHelper.Pi, EaseBuilder.EaseCircularOut.Ease(Progress)) * Owner.direction);

            Projectile.position += Vector2.Lerp(new Vector2(0f, -20f), new Vector2(0f, -45f), EaseBuilder.EaseBackOut.Ease(Progress));

            for (int i = 0; i < projectilePoints.Length; i++)
            {
                ProjEffects(Owner.Center + projectilePoints[i]);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texGlow = ModContent.Request<Texture2D>(Texture + "_Glowy").Value;

            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Texture2D glowSoftTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowSoft").Value;
            Texture2D glowRingTex = ModContent.Request<Texture2D>("BombusApisBee/Assets/ExtraTextures/GlowWithRing").Value;
            Texture2D starTex = ModContent.Request<Texture2D>("BombusApisBee/Assets/ExtraTextures/StarAlpha").Value;
            Texture2D shineTex = ModContent.Request<Texture2D>("BombusApisBee/Assets/ExtraTextures/Shine").Value;

            SpriteEffects flip = Owner.direction == -1 ? SpriteEffects.FlipHorizontally : 0f;

            float rot = Projectile.rotation - MathHelper.PiOver4 + (flip == SpriteEffects.FlipHorizontally ? MathHelper.PiOver2 : 0f);

            Vector2 offset = Main.rand.NextVector2Circular(2f * Progress, 2f * Progress);

            float fade = 1f;

            if (Timer < 10)
                fade = Timer / 10f;

            if (dyingTimer > 0)
                fade = 1f - dyingTimer / 15f;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(default, BlendState.Additive, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < projectilePoints.Length; i++)
            {
                Vector2 pos = Projectile.Center + new Vector2(0f, -20f);

                Main.spriteBatch.Draw(shineTex, pos - Main.screenPosition, shineTex.Frame(), new Color(53, 42, 81) * fade,
                    pos.DirectionTo(Owner.Center + projectilePoints[i]).ToRotation() + MathHelper.PiOver2, new Vector2(shineTex.Width / 2, shineTex.Height), new Vector2(0.05f * (dying ? fade : Progress), 0.1f), 0f, 0f);

                Main.spriteBatch.Draw(shineTex, pos - Main.screenPosition, shineTex.Frame(), new Color(91, 71, 127) * fade,
                    pos.DirectionTo(Owner.Center + projectilePoints[i]).ToRotation() + MathHelper.PiOver2, new Vector2(shineTex.Width / 2, shineTex.Height), new Vector2(0.05f * (dying ? fade : Progress), 0.1f), 0f, 0f);

                if (dyingTimer > 0)
                {
                    Main.spriteBatch.Draw(starTex, Owner.Center + projectilePoints[i] - Main.screenPosition,
                    null, new Color(91, 81, 127) * fade, 0f, starTex.Size() / 2f, new Vector2(MathHelper.Lerp(0.3f, 0.5f, 1f - fade), 0.6f), 0f, 0f);

                    Main.spriteBatch.Draw(starTex, Owner.Center + projectilePoints[i] - Main.screenPosition,
                       null, new Color(255, 255, 255) * fade, 0f, starTex.Size() / 2f, new Vector2(MathHelper.Lerp(0.2f, 0.3f, 1f - fade), 0.4f), 0f, 0f);
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

            Vector2 tipPos = Projectile.Center + new Vector2(0f, -20f);

            Main.spriteBatch.Draw(texGlow, Projectile.Center + offset - Main.screenPosition, null, new Color(164, 168, 80, 0) * (Progress < 1f ? Progress : fade), rot, texGlow.Size() / 2f, Projectile.scale, flip, 0f);

            Main.spriteBatch.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, lightColor * fade, rot, tex.Size() / 2f, Projectile.scale, flip, 0f);

            Main.spriteBatch.Draw(glowTex, tipPos - Main.screenPosition, null, new Color(164, 168, 80, 0) * (Progress < 1f ? Progress : fade), 0f, glowTex.Size() / 2f, 0.5f, 0f, 0f);

            if (dyingTimer > 0)
            {
                Main.spriteBatch.Draw(starTex, Projectile.Center + new Vector2(0f, -20f) - Main.screenPosition,
                    null, new Color(200, 255, 120, 0) * fade, 0f, starTex.Size() / 2f, new Vector2(MathHelper.Lerp(0.6f, 1f, 1f - fade), 0.6f), 0f, 0f);

                Main.spriteBatch.Draw(starTex, Projectile.Center + new Vector2(0f, -20f) - Main.screenPosition,
                   null, new Color(255, 255, 255, 0) * fade, 0f, starTex.Size() / 2f, new Vector2(MathHelper.Lerp(0.4f, 0.8f, 1f - fade), 0.4f), 0f, 0f);
            }

            return false;
        }

        /// <summary>
        /// Helper function for projectile spawning vfx
        /// </summary>
        protected virtual void ProjEffects(Vector2 pos)
        {

        }

        /// <summary>
        /// Called when the held projectile should shoot its projectile
        /// </summary>

        protected virtual void Shoot(Vector2 pos)
        {
            Vector2 velocity = pos.DirectionTo(Main.MouseWorld) * 20f;

            Projectile.NewProjectile(Projectile.GetSource_Death(), pos, velocity,
                ProjectileType<ShadestingerScepterStinger>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);

            SoundID.Item4.PlayWith(Projectile.Center, volume: 0.5f);

            Owner.Bombus().AddShake(1, false);

            for (int i = 0; i < 2; i++)
            {
                Color color = Main.rand.NextBool() ? new Color(152, 137, 255, 0) : new Color(164, 168, 74, 0);

                Dust.NewDustPerfect(pos, DustType<PixelatedGlow>(),
                        Main.rand.NextVector2Circular(2f, 2f), 0, color, 0.5f * Progress);

                Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(25f, 25f),
                    ModContent.DustType<PixelatedEmber>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 0, color, 0.2f * Progress).customData = Main.rand.NextBool() ? -1 : 1;

                Dust.NewDustPerfect(pos, DustType<StarDust>(),
                        Main.rand.NextVector2Circular(1f, 1f), 0, color, 0.5f * Progress);

                Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(15f, 15f),
                    ModContent.DustType<PixelatedEmber>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 0, color, 0.25f * Progress).customData = Main.rand.NextBool() ? -1 : 1;
            }
        }

        /// <summary>
        /// Updates the basic variables needed for a held projectile
        /// </summary>
        protected void UpdateHeldProjectile()
        {
            if (Timer < MaxTimer)
                Timer++;
            else if (!dying)
            {
                if (Main.myPlayer == Owner.whoAmI)
                    for (int i = 0; i < projectilePoints.Length; i++)
                    {
                        Vector2 spawnPos = Owner.Center + projectilePoints[i];
                        Shoot(spawnPos);
                    }

                dying = true;
            }

            Owner.heldProj = Projectile.whoAmI;

            Projectile.timeLeft = 2;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            Projectile.Center = Owner.Center;
            Projectile.direction = Owner.direction;
        }

        public override bool? CanDamage()
        {
            return false;
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class ShadestingerScepterStinger : BeeProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;
        private Trail trail3;

        public Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadestinger");
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;

            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;

            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            if (Main.rand.NextBool(15))
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<PixelatedGlow>(),
                        Main.rand.NextVector2Circular(1f, 1f), 0, new Color(152, 137, 255, 0), 0.2f);
            }

            if (Main.rand.NextBool(15))
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<StarDust>(),
                        Main.rand.NextVector2Circular(1f, 1f), 0, new Color(113, 132, 77, 0), 0.4f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            DarkEnergyGlobalNPC gnpc = target.GetGlobalNPC<DarkEnergyGlobalNPC>();

            if (gnpc.darkEnergy >= DarkEnergyGlobalNPC.MAXENERGY)
                gnpc.Explode(target, Owner);
            else
                gnpc.AddEnergy(Main.rand.Next(1, 5));

            Owner.Bombus().AddShake(2, false);
        }

        public override void OnKill(int timeLeft)
        {
            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f),
                   ModContent.DustType<PixelatedEmber>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 0, new Color(152, 137, 255, 0), 0.15f).customData = Main.rand.NextBool() ? -1 : 1;

            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f),
                               ModContent.DustType<PixelatedEmber>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 0, new Color(201, 255, 119, 0), 0.15f).customData = Main.rand.NextBool() ? -1 : 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrimitives();

            float fade = 1f;
            if (Projectile.timeLeft > 45)
                fade = (Projectile.timeLeft - 45) / 15f;

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor * fade, Projectile.rotation, tex.Size() / 2f, Projectile.scale, Projectile.direction == -1 ? (SpriteEffects)1 : 0, 0f);

            Effect effect = Filters.Scene["HolyShieldShader"].GetShader().Shader;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.005f);
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.005f);
            effect.Parameters["screenPos"].SetValue(Main.screenPosition * new Vector2(0.5f, 0.1f) / new Vector2(Main.screenWidth, Main.screenHeight));

            effect.Parameters["offset"].SetValue(new Vector2(0.001f));
            effect.Parameters["repeats"].SetValue(2);
            effect.Parameters["uImage1"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);
            effect.Parameters["uImage2"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);

            Color color = new Color(130, 200, 70, 0) * fade;

            effect.Parameters["uColor"].SetValue(color.ToVector4());
            effect.Parameters["noiseImage1"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);

            effect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2f, 1.15f, 0f, 0f);

            color = new Color(55, 180, 220, 0) * fade;

            effect.Parameters["uColor"].SetValue(color.ToVector4());
            effect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2f, 1.15f, 0f, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < 10; i++)
                {
                    cache.Add(Projectile.Center + Projectile.velocity);
                }
            }

            cache.Add(Projectile.Center + Projectile.velocity);

            while (cache.Count > 10)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail ??= new Trail(Main.instance.GraphicsDevice, 10, new TriangularTip(190), factor => factor * 7f, factor =>
            Color.Lerp(new Color(53, 42, 81), new Color(55, 81, 55), EaseBuilder.EaseCircularIn.Ease(1f - factor.X)));

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;

            trail2 ??= new Trail(Main.instance.GraphicsDevice, 10, new TriangularTip(190), factor => factor * 4f, factor =>
            Color.Lerp(new Color(119, 89, 227), new Color(201, 255, 119), EaseBuilder.EaseCircularIn.Ease(1f - factor.X)));

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = Projectile.Center + Projectile.velocity;

            trail3 ??= new Trail(Main.instance.GraphicsDevice, 10, new TriangularTip(190), factor => factor * 6f, factor =>
            Color.Lerp(new Color(64, 191, 0), new Color(25, 255, 25), EaseBuilder.EaseCircularIn.Ease(factor.X)) * 0.5f);

            trail3.Positions = cache.ToArray();
            trail3.NextPosition = Projectile.Center + Projectile.velocity;
        }

        public void DrawPrimitives()
        {
            ModContent.GetInstance<PixelationSystem>().QueueRenderAction("UnderTiles", () =>
            {
                Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());

                // !!! IMPORTANT WHEN PIXELIZING, MAKE SURE TO USE Main.GameViewMatrix.EffectMatrix IMPORTANT !!!

                Matrix view = Main.GameViewMatrix.EffectMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
                effect.Parameters["repeats"].SetValue(1f);
                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

                trail3?.Render(effect);

                trail2?.Render(effect);

                effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.015f);
                effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/FireTrail").Value);

                trail?.Render(effect);
            });
        }
    }
}
