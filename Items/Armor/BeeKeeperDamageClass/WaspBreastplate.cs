using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Body)]
    public class WaspBreastplate : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("6% increased hymenoptra damage and critical strike chance\nIncreases your amount of Loyal Bees by 1");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 2, silver: 50);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 4;
        }

        public override void UpdateEquip(Player player)
        {
            player.IncreaseBeeDamage(0.06f);
            player.IncreaseBeeCrit(6);
            player.Hymenoptra().CurrentBees += 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.CrimtaneBar, 10).AddIngredient(ModContent.ItemType<Pollen>(), 8).AddIngredient(ItemID.TissueSample, 10).AddTile(TileID.Anvils).Register();
            CreateRecipe(1).AddIngredient(ItemID.DemoniteBar, 10).AddIngredient(ModContent.ItemType<Pollen>(), 8).AddIngredient(ItemID.ShadowScale, 10).AddTile(TileID.Anvils).Register();
        }

    }
}
