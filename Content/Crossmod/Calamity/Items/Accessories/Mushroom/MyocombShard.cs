using BombusApisBee.BeeHelperProj;
using BombusApisBee.Content.Crossmod.Calamity.Core;
using BombusApisBee.Content.Dusts.Pixelized;
using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Content.Projectiles;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;

namespace BombusApisBee.Content.Crossmod.Calamity.Items.Accessories.Mushroom
{
    public class MyocombShard : CalamityItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Increases the chance to strengthen friendly bees by 25%\nStrengthened bees leave behind spore clouds");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 20);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Hymenoptra().BeeStrengthenChance += 0.25f;
            player.GetModPlayer<BombusApisCalamityPlayer>().MyocombShard = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<PollenItem>(25)
            .AddIngredient(ItemID.GlowingMushroom, 25)
            .AddIngredient<PearlShard>(5)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class SporeCloud : BeeProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public override string Texture => BombusApisBee.Invisible;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spore Cloud");
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;

            Projectile.scale = Main.rand.NextFloat(0.8f, 1.6f);
        }

        public override bool? CanDamage()
        {
            return Projectile.penetrate > 1 && Projectile.timeLeft > 30;
        }

        public override void AI()
        {
            if (Projectile.timeLeft < 30)
            {
                Projectile.velocity *= 0.9f;
                Projectile.scale *= 0.95f;
            }

            if (Main.rand.NextBool(3))
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(10f, 10f), DustType<SmokeDust2>(),
                   Main.rand.NextVector2Circular(2.5f, 2.5f), Main.rand.Next(120, 200), new Color(70, 90, 166), Main.rand.NextFloat(0.85f, 1f)).noGravity = true;
            }

            if (Main.rand.NextBool(3))
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(10f, 10f), DustType<SmokeDust2>(),
                   Main.rand.NextVector2Circular(2.5f, 2.5f), Main.rand.Next(120, 200), new Color(90, 167, 209), Main.rand.NextFloat(0.85f, 1f)).noGravity = true;
            }

            if (Main.rand.NextBool(10))
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(10f, 10f), DustType<PixelatedGlowAlt>(),
                   Main.rand.NextVector2Circular(2.5f, 2.5f), 0, new Color(90, 167, 209, 0), Main.rand.NextFloat(0.85f, 1f)).noGravity = true;
            }

            if (Main.rand.NextBool(10))
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(10f, 10f), DustType<PixelatedGlowAlt>(),
                   Main.rand.NextVector2Circular(2.5f, 2.5f), 0, new Color(116, 108, 166, 0), Main.rand.NextFloat(0.85f, 1f)).noGravity = true;
            }

            Projectile.velocity *= 0.965f;
            Projectile.rotation += Projectile.velocity.Length() * 0.02f;

            Projectile.alpha = (int)MathHelper.Lerp(160, 255, 1f - Projectile.timeLeft / 180f);

            Vector2 targetCenter = Projectile.Center;
            bool foundTarget = false;
            float num = 1000f;
            for (int i = 0; i < 200; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(this, false))
                {
                    float num2 = Projectile.Distance(npc.Center);
                    if (num > num2)
                    {
                        num = num2;
                        targetCenter = npc.Center;
                        foundTarget = true;
                    }
                }
            }
            if (foundTarget)
                Projectile.velocity = (Projectile.velocity * 20f + (targetCenter - Projectile.Center).SafeNormalize(Vector2.UnitX) * 3.5f) / 21f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float fadeOut = 1f;
            if (Projectile.timeLeft < 30)
                fadeOut = Projectile.timeLeft / 30f;

            Texture2D bloom = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Color color = Projectile.GetAlpha(new Color(70, 90, 166, 0));

            Main.spriteBatch.Draw(bloom, Projectile.Center - Main.screenPosition, null, color * fadeOut * 0.5f, Projectile.rotation, bloom.Size() / 2f, Projectile.scale * 2f, 0f, 0f);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.penetrate == 2)
            {
                Projectile.timeLeft = 30;
            }
        }
    }
}
