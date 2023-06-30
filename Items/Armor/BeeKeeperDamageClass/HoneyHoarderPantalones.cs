using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Legs)]
    public class HoneyHoarderPantalones : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Hoarder Leggings");
            Tooltip.SetDefault("8% increased hymenoptra damage\n15% increased movement speed\nIncreases maximum honey by 25\nIncreases your amount of Loyal Bees by 2");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 4, silver: 50);
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 9;
        }

        public override void UpdateEquip(Player player)
        {
            var modPlayer = BeeDamagePlayer.ModPlayer(player);
            player.IncreaseBeeDamage(0.08f);
            player.moveSpeed += 0.15f;
            modPlayer.BeeResourceMax2 += 25;
            player.Hymenoptra().CurrentBees += 2;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.ChlorophyteBar, 16).AddIngredient(ItemID.Ectoplasm, 4).AddIngredient(ItemID.HoneyBlock, 15).AddIngredient(ItemID.BottledHoney, 8).AddIngredient(ModContent.ItemType<Pollen>(), 25).AddTile(TileID.MythrilAnvil).Register();
        }

    }
}