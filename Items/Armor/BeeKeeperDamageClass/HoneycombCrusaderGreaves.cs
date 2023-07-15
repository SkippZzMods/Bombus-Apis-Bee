using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Legs)]
    public class HoneycombCrusaderGreaves : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honeycomb Crusader Greaves");
            Tooltip.SetDefault("5% increased hymenoptra damage");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 3);
            Item.rare = ItemRarityID.Pink;
            Item.defense = 7;
        }

        public override void UpdateEquip(Player player)
        {
            player.IncreaseBeeDamage(0.05f);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HallowedBar, 16).
                AddIngredient(ModContent.ItemType<Pollen>(), 20).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}