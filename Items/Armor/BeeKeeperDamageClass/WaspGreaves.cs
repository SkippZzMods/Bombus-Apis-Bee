using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Legs)]
    public class WaspGreaves : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("15% increased movement speed");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 1, silver: 75);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 4;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.15f;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.CrimtaneBar, 9).AddIngredient(ModContent.ItemType<Pollen>(), 10).AddIngredient(ItemID.TissueSample, 9).AddTile(TileID.Anvils).Register();
            CreateRecipe(1).AddIngredient(ItemID.DemoniteBar, 9).AddIngredient(ModContent.ItemType<Pollen>(), 10).AddIngredient(ItemID.ShadowScale, 9).AddTile(TileID.Anvils).Register();
        }
    }
}
