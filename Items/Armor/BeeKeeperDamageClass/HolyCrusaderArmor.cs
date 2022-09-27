using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Body)]
    public class HolyCrusaderArmor : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("8% increased hymenoptra critical strike chance\nReduces damage taken by 10%");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 4);
            Item.rare = ItemRarityID.Pink;
            Item.defense = 17;
        }

        public override void UpdateEquip(Player player)
        {
            player.IncreaseBeeCrit(8);
            player.endurance += 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HallowedBar, 20).
                AddIngredient(ItemID.SoulofLight, 8).
                AddIngredient(ModContent.ItemType<Pollen>(), 30).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}