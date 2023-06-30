using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Head)]
    public class BeeKeepersHelmet : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bee Keeper's Hat");
            Tooltip.SetDefault("3% increased hymenoptra critical strike chance\nIncreases maximum honey by 15");
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
            return body.type == ModContent.ItemType<BeeKeepersChestplate>() && legs.type == ModContent.ItemType<BeeKeepersLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "4% increased hymenoptra damage\nBees become friendly";
            player.IncreaseBeeDamage(0.06f);
            player.npcTypeNoAggro[NPCID.Bee] = true;
            player.npcTypeNoAggro[NPCID.BeeSmall] = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.IncreaseBeeCrit(3f);
            BeeDamagePlayer.ModPlayer(player).BeeResourceMax2 += 15;
        }


        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.Silk, 7).AddIngredient(ModContent.ItemType<Pollen>(), 10).AddTile(TileID.Loom).Register();
        }
    }
}
