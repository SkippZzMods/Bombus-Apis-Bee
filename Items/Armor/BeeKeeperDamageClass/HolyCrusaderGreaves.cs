using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Legs)]
    public class HolyCrusaderGreaves : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Crusader Greaves");
            Tooltip.SetDefault("10% increased hymenoptra damage\n10% increase chance to not consume honey\nIncreases your amount of Bees by 1");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 3);
            Item.rare = ItemRarityID.Pink;
            Item.defense = 12;
        }

        public override void UpdateEquip(Player player)
        {
            player.IncreaseBeeDamage(0.1f);
            player.Hymenoptra().ResourceChanceAdd += 0.1f;
            player.Hymenoptra().CurrentBees += 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HallowedBar, 16).
                AddIngredient(ItemID.SoulofLight, 5).
                AddIngredient(ModContent.ItemType<Pollen>(), 20).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}