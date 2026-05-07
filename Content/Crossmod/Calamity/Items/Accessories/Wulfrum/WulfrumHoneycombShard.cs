using BombusApisBee.Content.Crossmod.Calamity.Core;
using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Core.Common.BeeProjectile;
using CalamityMod;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using Terraria.DataStructures;

namespace BombusApisBee.Content.Crossmod.Calamity.Items.Accessories.Wulfrum
{
    public class WulfrumHoneycombShard : CalamityItem
    {
        public override void Load()
        {
            CommonBeeGlobalProjectile.StrongBeePostDrawEvent += PostDrawShield;
            CommonBeeGlobalProjectile.StrongBeeOnHitEvent += CheckShield;
            CommonBeeGlobalProjectile.StrongBeeKillEvent += DestroyShield;
        }

        private void DestroyShield(Projectile proj, int timeLeft)
        {
            if (proj.GetGlobalProjectile<WulfrumHoneycombShardGlobalProjectile>().HasShield && Main.player[proj.owner].GetModPlayer<BombusApisCalamityPlayer>().WulfrumHCShardDraw)
                ShieldBreakEffects(proj);
        }

        private void ShieldBreakEffects(Projectile proj)
        {
            RoverDrive.BreakSound.PlayWith(proj.Center, 0, 0, 0.75f);

            for (int i = 0; i < Main.rand.Next(3, 7); i++)
            {
                GeneralParticleHandler.SpawnParticle(new TechyHoloysquareParticle(proj.Center, Main.rand.NextVector2CircularEdge(4f, 4f), Main.rand.NextFloat(2f, 3f), Main.rand.NextBool() ? new Color(99, 255, 229) : new Color(25, 132, 247), 25, 1f));

                Dust.NewDustPerfect(proj.Center, DustType<ImpactLineDust>(), Main.rand.NextVector2CircularEdge(5f, 5f), 0, new Color(55, 180, 220, 0), 0.085f);
            }
        }

        private void CheckShield(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var gp = proj.GetGlobalProjectile<WulfrumHoneycombShardGlobalProjectile>();
            if (gp.HasShield)
            {
                gp.HasShield = false;
                proj.penetrate += 1;
                if (Main.player[proj.owner].GetModPlayer<BombusApisCalamityPlayer>().WulfrumHCShardDraw)
                    ShieldBreakEffects(proj);
            }
        }

        private void PostDrawShield(Projectile proj, Color lightColor)
        {
            if (proj.GetGlobalProjectile<WulfrumHoneycombShardGlobalProjectile>().HasShield && Main.player[proj.owner].GetModPlayer<BombusApisCalamityPlayer>().WulfrumHCShardDraw)
            {
                float scale = 0.08f + 0.03f * (0.5f + 0.5f * (float)Math.Sin((double)(Main.GlobalTimeWrappedHourly * 0.5f)));
                float noiseScale = MathHelper.Lerp(0.1f, 0.3f, (float)Math.Sin((double)(Main.GlobalTimeWrappedHourly * 0.3f)) * 0.5f + 0.5f);
                Effect effect = Filters.Scene["CalamityMod:RoverDriveShield"].GetShader().Shader;
                effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 0.24f);
                effect.Parameters["blowUpPower"].SetValue(2.5f);
                effect.Parameters["blowUpSize"].SetValue(0.5f);
                effect.Parameters["noiseScale"].SetValue(noiseScale);
                float baseShieldOpacity = 0.9f + 0.1f * (float)Math.Sin((double)(Main.GlobalTimeWrappedHourly * 2f));
                effect.Parameters["shieldOpacity"].SetValue(baseShieldOpacity * (0.5f + 0.5f * 1f));
                effect.Parameters["shieldEdgeBlendStrenght"].SetValue(4f);

                Color shieldColor = new Color(51, 102, 255);
                Color edgeColor = CalamityUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.2f, new Color[]
                {
                shieldColor,
                new Color(71, 202, 255),
                new Color(194, 255, 67) * 0.8f
                });

                effect.Parameters["shieldColor"].SetValue(shieldColor.ToVector3());
                effect.Parameters["shieldEdgeColor"].SetValue(edgeColor.ToVector3());

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.GameViewMatrix.TransformationMatrix);

                Texture2D noiseTex = Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/TechyNoise").Value;

                Main.spriteBatch.Draw(noiseTex, proj.Center - (proj.type == ProjectileID.GiantBee ? new Vector2(-5, proj.direction == -1 ? -6 : 0).RotatedBy(proj.rotation) : Vector2.Zero) - Main.screenPosition, null, Color.White, 0f, noiseTex.Size() / 2f, scale, 0f, 0f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }
        }

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Increases the chance to strengthen friendly bees by 20%\nStrengthened bees spawn with a shield, allowing them to strike an extra time\nDisable visibility on accessory to disable shield drawing and breaking effects on strengthened bees");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var mp = player.GetModPlayer<BombusApisCalamityPlayer>();

            player.Beekeeper().BeeStrengthenChance += 0.20f;
            mp.WulfrumHCShard = true;
            mp.WulfrumHCShardDraw = !hideVisual;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<PollenItem>(25)
            .AddIngredient<WulfrumMetalScrap>(10)
            .AddIngredient<EnergyCore>()
            .AddTile(TileID.Anvils)
            .Register();
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    class WulfrumHoneycombShardGlobalProjectile : GlobalProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        public bool HasShield;
        public override bool InstancePerEntity => true;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            bool giantBee = projectile.type == ProjectileID.GiantBee;
            bool giantModdedBee = projectile.ModProjectile as CommonBeeProjectile != null && (projectile.ModProjectile as CommonBeeProjectile).Giant;
            if ((giantBee || giantModdedBee) && Main.player[projectile.owner].GetModPlayer<BombusApisCalamityPlayer>().WulfrumHCShard)
                HasShield = true;
        }
    }
}
