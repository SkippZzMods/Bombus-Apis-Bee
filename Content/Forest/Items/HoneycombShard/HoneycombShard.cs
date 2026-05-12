using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Core.Common.HoneycombShard;

namespace BombusApisBee.Content.Forest.Items.HoneycombShard
{
    public class HoneycombShard : HoneycombShardItem
    {
        public HoneycombShard() : base("Honeycomb Shard", "Increases the chance to strengthen friendly bees by 15%\n'Crunchy!'", 0) { }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Beekeeper().BeeStrengthenChance += 0.15f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PollenItem>(20).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
