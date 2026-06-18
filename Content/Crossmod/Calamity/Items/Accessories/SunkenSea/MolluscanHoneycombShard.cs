using BombusApisBee.Content.Crossmod.Calamity.Core;
using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Core.Common.HoneycombShard;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.SunkenSea;
namespace BombusApisBee.Content.Crossmod.Calamity.Items.Accessories.SunkenSea
{
    [JITWhenModsEnabled("CalamityMod")]
    public class MolluscanHoneycombShard : HoneycombShardItem
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public MolluscanHoneycombShard() : base("Molluscan Honeycomb Shard", "Increases the chance to strengthen friendly bees by 30%\nStrengthened bees ignore 15 points of defense", -1) { }

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Increases the chance to strengthen friendly bees by 30%\nStrengthened bees ignore 25 points of defense");
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 20);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Beekeeper().BeeStrengthenChance += 0.3f;
        }

        public override void ModifyEquippedHit(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ArmorPenetration += 25;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<PollenItem>(15)
            .AddIngredient<SeaPrism>(4)
            .AddIngredient<PearlShard>(2)
            .AddIngredient<Navystone>(15)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}
