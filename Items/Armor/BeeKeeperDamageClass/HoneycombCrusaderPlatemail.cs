using BombusApisBee.Items.Other.Crafting;
using Terraria.Localization;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Body)]
    public class HoneycombCrusaderPlatemail : BeeKeeperItem
    {
        public static int HoneySave = 30;
        public static int LoyalBees = 4;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(HoneySave, LoyalBees);
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Honeycomb Crusader Platemail");
            //Tooltip.SetDefault("30% increased chance to not consume honey\nLoyal Bees increased by 4");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 4);
            Item.rare = ItemRarityID.Pink;
            Item.defense = 10;
        }

        public override void UpdateEquip(Player player)
        {
            player.Hymenoptra().ResourceChanceAdd += 0.3f;
            player.Hymenoptra().CurrentBees += 4;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HallowedBar, 20).
                AddIngredient(ModContent.ItemType<Pollen>(), 30).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}