using BombusApisBee.Content.Crossmod.Calamity.Core;
using BombusApisBee.Content.Forest.Items.Pollen;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.SunkenSea;
namespace BombusApisBee.Content.Crossmod.Calamity.Items.Accessories.SunkenSea
{
    public class MolluscanHoneycombShard : CalamityItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Increases the chance to strengthen friendly bees by 10%\nStrengthened bees ignore 15 points of defense");
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
            player.Hymenoptra().BeeStrengthenChance += 0.1f;
            player.GetModPlayer<BombusApisCalamityPlayer>().MolluscanShard = true;
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
