using BombusApisBee.BeeDamageClass;

namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    public class HoneyedSkull : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honeyed Skull");
            Tooltip.SetDefault("'Might be cursed'\nStriking enemies gives you an improved Honey buff\nIncreases maximum honey by 15\nGrants the effects of the Obsidian Skull");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 15);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = BeeDamagePlayer.ModPlayer(player);
            modPlayer.BeeResourceMax2 += 15;
            player.buffImmune[BuffID.OnFire] = true;
            var modPlayer2 = player.GetModPlayer<BombusApisBeePlayer>();
            modPlayer2.improvedhoneyskull = true;

        }
        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient(ItemID.ObsidianSkull, 1).
                AddIngredient(ItemID.BottledHoney, 10).
                AddIngredient(ItemID.ChlorophyteBar, 13).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}