using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Head)]
    public class BeekeepersVeil : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beekeeper's Veil");
            Tooltip.SetDefault("5% increased hymenoptra critical strike chance\nIncreases maximum honey by 25");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 2;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<BeekeepersRobe>() && legs.type == ModContent.ItemType<BeekeepersPants>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Bees become friendly\n15% increased chance to not consume honey";
            player.Hymenoptra().ResourceChanceAdd += 0.15f;
            player.npcTypeNoAggro[NPCID.Bee] = true;
            player.npcTypeNoAggro[NPCID.BeeSmall] = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.IncreaseBeeCrit(5);
            player.Hymenoptra().BeeResourceMax2 += 25;
        }


        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient(ItemID.PlatinumBar, 12).
                AddIngredient(ItemID.Silk, 7).
                AddIngredient(ModContent.ItemType<Pollen>(), 10).
                AddTile(TileID.Loom).Register();

            CreateRecipe(1).
                AddIngredient(ItemID.GoldBar, 12).
                AddIngredient(ItemID.Silk, 7).
                AddIngredient(ModContent.ItemType<Pollen>(), 10).
                AddTile(TileID.Loom).Register();
        }
    }
}
