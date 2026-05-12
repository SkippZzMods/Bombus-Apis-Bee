using BombusApisBee.Content.Forest.Items.Pollen;

namespace BombusApisBee.Content.Jungle.Items.HoneyphyteArmor
{
    [AutoloadEquip(EquipType.Legs)]
    public class HoneyphyteGreaves : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Hoarder Leggings");
            Tooltip.SetDefault("5% increased beekeeper damage");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 4, silver: 50);
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 10;
        }

        public override void UpdateEquip(Player player)
        {
            player.IncreaseBeeDamage(0.05f);
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.ChlorophyteBar, 16).AddIngredient(ItemID.Ectoplasm, 4).AddIngredient(ItemID.HoneyBlock, 15).AddIngredient(ItemID.BottledHoney, 8).AddIngredient(ModContent.ItemType<PollenItem>(), 25).AddTile(TileID.MythrilAnvil).Register();
        }

    }
}