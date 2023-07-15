using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Head)]
    public class HoneyphyteHeadgear : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Hoarder Hood");
            Tooltip.SetDefault("Increases maximum honey by 100");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 8);
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 4;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return legs.type == ModContent.ItemType<HoneyphyteGreaves>() && body.type == ModContent.ItemType<HoneyphyteChestpiece>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "20% increased chance to not use honey\nDouble tap " + (Main.ReversedUpDownArmorSetBonuses ? "Up " : "Down ") + "to teleport to the cursor, creating a honey explosion, leeching honey from hit enemies";
            player.Hymenoptra().ResourceChanceAdd += 0.2f;
            player.Bombus().HoneyTeleport = true;
        }

        public override void UpdateEquip(Player player)
        {
            var modPlayer = BeeDamagePlayer.ModPlayer(player);
            modPlayer.BeeResourceMax2 += 100;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.ChlorophyteBar, 12).AddIngredient(ItemID.Ectoplasm, 3).AddIngredient(ItemID.HoneyBlock, 10).AddIngredient(ItemID.BottledHoney, 6).AddIngredient(ModContent.ItemType<Pollen>(), 30).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
