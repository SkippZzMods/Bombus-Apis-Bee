using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Body)]
    public class HoneyphyteChestpiece : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("30% increased chance to not consume honey\nLoyal Bees increased by 5");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 6, silver: 50);
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 12;
        }

        public override void UpdateEquip(Player player)
        {
            player.Hymenoptra().CurrentBees += 5;
            player.Hymenoptra().ResourceChanceAdd += 0.3f;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient(ItemID.ChlorophyteBar, 20).
                AddIngredient(ItemID.Ectoplasm, 5).
                AddIngredient(ItemID.HoneyBlock, 18).
                AddIngredient(ItemID.BottledHoney, 12).
                AddIngredient(ModContent.ItemType<Pollen>(), 35).
                AddTile(TileID.MythrilAnvil).Register();
        }

    }
}
