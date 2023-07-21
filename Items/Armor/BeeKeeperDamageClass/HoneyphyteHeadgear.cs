using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Head)]
    public class HoneyphyteHeadgear : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Hoarder Hood");
            Tooltip.SetDefault("Increases maximum honey by 100");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 8);
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 4;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return legs.type == ModContent.ItemType<HoneyphyteGreaves>() && body.type == ModContent.ItemType<HoneyphyteChestpiece>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "20% increased chance to not use honey\nDouble tap " + (Main.ReversedUpDownArmorSetBonuses ? "UP " : "DOWN ") + "to teleport to the cursor, creating a honey explosion, leeching honey from hit enemies";
            player.Hymenoptra().ResourceChanceAdd += 0.2f;
            player.Bombus().HoneyTeleport = true;
        }

        public override void UpdateEquip(Player player)
        {
            var modPlayer = BeeDamagePlayer.ModPlayer(player);
            modPlayer.BeeResourceMax2 += 100;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.ChlorophyteBar, 12).AddIngredient(ItemID.Ectoplasm, 3).AddIngredient(ItemID.HoneyBlock, 10).AddIngredient(ItemID.BottledHoney, 6).AddIngredient(ModContent.ItemType<Pollen>(), 30).AddTile(TileID.MythrilAnvil).Register();
        }
    }

    public class HoneyphyteHeadgearExplosion : ModProjectile
    {
        private List<Vector2> cache;

        private Trail trail;
        private Trail trail2;
        private Trail trail3;
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";
        private float Progress => 1 - (Projectile.timeLeft / 30f);

        private float Radius => Projectile.ai[0] * EaseBuilder.EaseQuinticOut.Ease(Progress);

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.DamageType = BeeUtils.BeeDamageClass();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Explosion");
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
                    Vector2.One.RotatedBy(rot) * 0.5f, 0, new Color(200, 100, 10), Main.rand.NextFloat(0.35f, 0.4f));

                Dust dust = Dust.NewDustPerfect(Projectile.Center + Vector2.One.RotatedBy(rot) * Radius, 
                    ModContent.DustType<HoneyMetaballDust>(), Vector2.One.RotatedBy(rot) * 0.5f + Main.rand.NextVector2Circular(2f, 2f), 0, default, Main.rand.NextFloat(1f, 2f));

                dust.noGravity = true;

                Dust.NewDustPerfect(Projectile.Center + Vector2.One.RotatedBy(rot) * Radius,
                    DustID.Honey2, Vector2.One.RotatedBy(rot) * 0.5f + Main.rand.NextVector2Circular(2f, 2f), Main.rand.Next(200), default, Main.rand.NextFloat(1f, 3f)).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center + Vector2.One.RotatedBy(rot) * Radius,
                   DustID.Honey2, Vector2.One.RotatedBy(rot) * 2f, 200, default, Main.rand.NextFloat(1f, 2f)).noGravity = false;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].IncreaseBeeResource(Main.rand.Next(20, 30));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrimitives();

            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowSoft").Value;

            float fadeOut = 1f;
            if (Projectile.timeLeft < 15f)
                fadeOut = Projectile.timeLeft / 15f;

            float scale = Progress;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 10) * fadeOut, 0f, bloomTex.Size() / 2f, scale * 15f, 0f, 0f);
            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(200, 100, 10) * fadeOut, 0f, bloomTex.Size() / 2f, scale * 14f, 0f, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
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
            Color color = new Color(200, 100, 20, 0) * fadeOut;

            effect.Parameters["uColor"].SetValue(color.ToVector4());
            effect.Parameters["noiseImage1"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);

            effect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, Color.White, 0f, bloomTex.Size() / 2f, scale * 7f, 0f, 0f);

            color = new Color(255, 150, 20, 0) * fadeOut;

            effect.Parameters["uColor"].SetValue(color.ToVector4());
            effect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, Color.White, 0f, bloomTex.Size() / 2f, scale * 6.5f, 0f, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(255, 150, 50, 0) * fadeOut, 0f, bloomTex.Size() / 2f, scale * 5f, 0f, 0f);

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
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 40, new TriangularTip(1), factor => 50 * (1 - Progress), factor =>
            {
                return new Color(255, 125, 20);
            });

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 40, new TriangularTip(1), factor => 40 * (1 - Progress), factor =>
            {
                return new Color(255, 155, 40);
            });

            trail3 = trail3 ?? new Trail(Main.instance.GraphicsDevice, 40, new TriangularTip(1), factor => 50 * (1 - Progress), factor =>
            {
                return new Color(200, 100, 20);
            });

            float nextplace = 33f / 32f;
            var offset = new Vector2((float)Math.Sin(nextplace), (float)Math.Cos(nextplace));
            offset *= Radius;

            trail.Positions = cache.ToArray();
            trail.NextPosition = cache[39];

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = cache[39];

            trail3.Positions = cache.ToArray();
            trail3.NextPosition = cache[39];
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 line = targetHitbox.Center.ToVector2() - Projectile.Center;
            line.Normalize();
            line *= Radius;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + line);
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
