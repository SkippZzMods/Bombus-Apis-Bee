using BombusApisBee.Content.Crossmod.Calamity.Core;
using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Core.Common.BeeProjectile;
using BombusApisBee.Core.Common.HoneycombShard;
using BombusApisBee.Core.Systems.ParticleSystem;
using BombusApisBee.Core.Systems.PixelationSystem;
using CalamityMod.Items.Materials;

namespace BombusApisBee.Content.Crossmod.Calamity.Items.Accessories.Corruption
{
    [JITWhenModsEnabled("CalamityMod")]
    public class ShadesentHoneycombShard : HoneycombShardItem
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public ShadesentHoneycombShard() : base("Shadesent Honeycomb Shard", "Increases chance to strengthen friendly bees by 30%\nStrengthened bees can travel through walls, empowering their critical strike chance", -1) { }
        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Beekeeper().BeeStrengthenChance += 0.30f;
            player.GetModPlayer<BombusApisCalamityPlayer>().ShadesentShard = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<PollenItem>(20)
            .AddIngredient<PearlShard>(2)
            .AddIngredient(ItemID.DemoniteBar, 12)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class ShadesentHoneycombShardGlobalProjectile : GlobalProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            if (!entity.friendly)
                return false;

            if (entity.ModProjectile != null && entity.ModProjectile is CommonBeeProjectile)
                return true;

            return false;
        }

        public override bool InstancePerEntity => true;

        public int _inTileTimer;
        public int _origCrit;

        public override void SetDefaults(Projectile entity)
        {
            if (Enabled(entity))
                _origCrit = entity.CritChance;
        }

        public static Color[] shadowPallete =
                [
                    new Color(5, 5, 5),
                    new Color(53, 42, 81),
                    new Color(87, 62, 132),
                    new Color(152, 137, 255)
                ];

        public override void AI(Projectile projectile)
        {
            if (Enabled(projectile))
            {
                if (projectile.tileCollide)
                    projectile.tileCollide = false;

                if (_inTileTimer > 0)
                {
                    if (Main.rand.NextBool(7))
                    {
                        ParticleHandler.SpawnParticle(new FireParticle(projectile.Center + Main.rand.NextVector2Circular(15f, 15f), -projectile.velocity * 0.2f, new(0.1f, 0.1f, 0.1f), Main.rand.NextFloat(0.03f, 0.06f), 35, palletColors: shadowPallete)
                        {
                            LayerPixel = RenderLayer.UnderNPCs
                        });
                    }

                }
                else
                {
                    if (Collision.SolidTiles(projectile.Center, projectile.width, projectile.height))
                        _inTileTimer = 90;
                }
            }
        }

        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            if (Enabled(projectile))
            {
                _inTileTimer = 90;
            }

            return false;
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Main.player[projectile.owner].Beekeeper().RerollCrit(12))
                modifiers.SetCrit();
        }

        internal static bool Enabled(Projectile p)
        {
            if (p.TryGetOwner(out Player player))
            {
                return player.GetModPlayer<BombusApisCalamityPlayer>().ShadesentShard && BeeUtils.IsStrongBee(p.whoAmI);
            }

            return false;
        }
    }
}
