using BombusApisBee.BeeDamageClass;
using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Head)]
    public class WaspHelmet : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("40% increased wing flight time and increased jump speed\nIncreases maximum honey by 25");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 2);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<WaspChestplate>() && legs.type == ModContent.ItemType<WaspLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "6% increased hymenoptra damage and critical strike chance\nHornets no longer deal contact damage and their stingers deal half damage";
            player.IncreaseBeeDamage(0.06f);
            player.IncreaseBeeCrit(6);
            player.Bombus().WaspArmorSet = true;

            player.npcTypeNoAggro[42] = true;

            player.npcTypeNoAggro[231] = true;

            player.npcTypeNoAggro[232] = true;

            player.npcTypeNoAggro[233] = true;

            player.npcTypeNoAggro[234] = true;

            player.npcTypeNoAggro[235] = true;
        }

        public override void UpdateEquip(Player player)
        {
            var modPlayer = BeeDamagePlayer.ModPlayer(player);
            modPlayer.BeeResourceMax2 += 25;
            player.jumpSpeedBoost += 2.25f;
            player.Bombus().wingFlightTimeBoost += 0.40f;
        }


        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.CrimtaneBar, 8).AddIngredient(ModContent.ItemType<Pollen>(), 9).AddIngredient(ItemID.TissueSample, 8).AddTile(TileID.Anvils).Register();
            CreateRecipe(1).AddIngredient(ItemID.DemoniteBar, 8).AddIngredient(ModContent.ItemType<Pollen>(), 9).AddIngredient(ItemID.ShadowScale, 8).AddTile(TileID.Anvils).Register();
        }
    }
}