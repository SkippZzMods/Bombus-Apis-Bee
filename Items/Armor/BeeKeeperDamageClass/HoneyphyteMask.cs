using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Head)]
    public class HoneyphyteMask : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("10% increased hymenoptra damage and critical strike chance\nIncreases maximum honey by 75");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 8);
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 7;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return legs.type == ModContent.ItemType<HoneyphyteGreaves>() && body.type == ModContent.ItemType<HoneyphyteChestpiece>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Strike enemies to build up honey energy\nDouble tap " + (Main.ReversedUpDownArmorSetBonuses ? "Up " : "Down ") + "while at full energy to fire a concentrated honey laser\nIncreases hymenoptra damage by 10% while your Bees are in Attacking mode\nIncreases damage reduction by 10% while your Bees are in Defense mode";
            var modPlayer = player.Hymenoptra();
            player.Bombus().HoneyLaser = true;

            if (modPlayer.HasBees)
            {
                if (modPlayer.CurrentBeeState == (int)BeeDamagePlayer.BeeState.Defense)
                {
                    player.IncreaseBeeDamage(0.1f);
                }
                else if (modPlayer.CurrentBeeState == (int)BeeDamagePlayer.BeeState.Offense)
                {
                    player.endurance += 0.1f;
                }
            }
        }
        
        public override void UpdateEquip(Player player)
        {
            player.IncreaseBeeDamage(0.10f);
            player.IncreaseBeeCrit(10);
            player.Hymenoptra().BeeResourceMax2 += 75;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.ChlorophyteBar, 12).AddIngredient(ItemID.Ectoplasm, 3).AddIngredient(ItemID.HoneyBlock, 10).AddIngredient(ItemID.BottledHoney, 6).AddIngredient(ModContent.ItemType<Pollen>(), 30).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
