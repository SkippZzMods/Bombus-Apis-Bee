using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Body)]
    public class HoneyHoarderChestpiece : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("8% increased hymenoptra damage and critical strike chance\nIncreases maximum honey by 25\nIncreases your amount of Loyal Bees by 2");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 6, silver: 50);
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 11;
        }

        public override void UpdateEquip(Player player)
        {
            var modPlayer = BeeDamagePlayer.ModPlayer(player);
            player.IncreaseBeeDamage(0.08f);
            player.IncreaseBeeCrit(8);
            modPlayer.BeeResourceMax2 += 25;
            player.Hymenoptra().CurrentBees += 2;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.ChlorophyteBar, 20).AddIngredient(ItemID.Ectoplasm, 5).AddIngredient(ItemID.HoneyBlock, 18).AddIngredient(ItemID.BottledHoney, 12).AddIngredient(ModContent.ItemType<Pollen>(), 35).AddTile(TileID.MythrilAnvil).Register();
        }

    }
}
