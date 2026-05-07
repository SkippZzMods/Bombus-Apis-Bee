using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Core.BeekeeperClass;
using BombusApisBee.Core.Systems.PixelationSystem;
using CalamityMod.Items.Materials;

namespace BombusApisBee.Content.Crossmod.Calamity.Items.Accessories.Victide
{
    public class HydrophobicCoating : CalamityItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Coats your hymenoptra attacks with a special hydrophobic coating, causing them to deal more damage when wet\nCauses your bee projectiles no longer die in water\nDisable visibility on accessory to disable water aura drawing on wet projectiles");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 40);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Bombus().IgnoreWater = true;
            player.GetModPlayer<HydrophobicCoatingPlayer>().equipped = true;
            player.GetModPlayer<HydrophobicCoatingPlayer>().visible = !hideVisual;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ItemType<SeaRemains>(), 4);
            recipe.AddIngredient(ItemType<PollenItem>(), 15);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class HydrophobicCoatingPlayer : ModPlayer
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        public bool equipped;
        public bool visible;

        public override void ResetEffects()
        {
            equipped = false;
            visible = false;
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class HydrophobicCoatingGlobalProjectile : GlobalProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        public int wetTimer;

        public override bool InstancePerEntity => true;

        public override void AI(Projectile projectile)
        {
            if (wetTimer > 0)
                wetTimer--;

            if (Collision.WetCollision(projectile.Center, projectile.width, projectile.height) && Main.player[projectile.owner].GetModPlayer<HydrophobicCoatingPlayer>().equipped && projectile.DamageType == GetInstance<BeekeeperDamage>())
                wetTimer = 600;
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (wetTimer > 0)
                modifiers.FinalDamage *= 1.35f;
        }

        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (wetTimer > 0 && Main.player[projectile.owner].GetModPlayer<HydrophobicCoatingPlayer>().visible)
            {
                GetInstance<PixelationSystem>().QueueRenderAction("OverPlayers", () =>
                {
                    Texture2D bloomTex = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

                    Effect effect = Filters.Scene["HolyShieldShader"].GetShader().Shader;

                    float opacity = wetTimer / 600f;

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);

                    effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.005f);
                    effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.005f);
                    effect.Parameters["screenPos"].SetValue(Main.screenPosition * new Vector2(0.5f, 0.1f) / new Vector2(Main.screenWidth, Main.screenHeight));

                    effect.Parameters["offset"].SetValue(new Vector2(0.001f));
                    effect.Parameters["repeats"].SetValue(2);
                    effect.Parameters["uImage1"].SetValue(Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);
                    effect.Parameters["uImage2"].SetValue(Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);
                    effect.Parameters["noiseImage1"].SetValue(Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);

                    Color color = new Color(0, 125, 255, 0) * opacity * 0.075f;

                    effect.Parameters["uColor"].SetValue(color.ToVector4());

                    effect.CurrentTechnique.Passes[0].Apply();

                    int size = projectile.width;
                    if (projectile.height > size)
                        size = projectile.height;

                    float scale = 0.2f + size / 45f;

                    Main.spriteBatch.Draw(bloomTex, projectile.Center - Main.screenPosition, null, Color.White, 0f, bloomTex.Size() / 2f, scale, 0f, 0f);

                    color = new Color(180, 255, 175, 0) * opacity * 0.1f;

                    effect.Parameters["uColor"].SetValue(color.ToVector4());
                    effect.Parameters["noiseImage1"].SetValue(Request<Texture2D>("BombusApisBee/ExtraTextures/HolyNoise").Value);

                    effect.CurrentTechnique.Passes[0].Apply();

                    Main.spriteBatch.Draw(bloomTex, projectile.Center - Main.screenPosition, null, Color.White, 0f, bloomTex.Size() / 2f, scale, 0f, 0f);

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.EffectMatrix);

                    Main.spriteBatch.Draw(bloomTex, projectile.Center - Main.screenPosition, null, new Color(0, 50, 200, 0) * opacity, 0f, bloomTex.Size() / 2f, scale / 2f, 0f, 0f);
                });
            }

            return true;
        }
    }
}
