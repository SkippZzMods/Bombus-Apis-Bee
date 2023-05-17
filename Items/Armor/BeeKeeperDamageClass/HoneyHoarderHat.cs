using BombusApisBee.BeeDamageClass;
using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Head)]
    public class HoneyHoarderHat : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Hoarder Hood");
            Tooltip.SetDefault("20% decreased hymenoptra damage\nIncreases maximum honey by 60\nIncreases your amount of Loyal Bees by 2");
            SacrificeTotal = 1;
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
            return legs.type == ModContent.ItemType<HoneyHoarderPantalones>() && body.type == ModContent.ItemType<HoneyHoarderChestpiece>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Doubles passive Honey gain\nIncreases Honey leech from Bees in Gathering Mode by 2\n33% increased chance to not use honey";
            //player.Hymenoptra().RegenRateStart -= 0.15f;
            player.Hymenoptra().ResourceChanceAdd += 0.33f;
            player.Bombus().HoneyHoarderSet = true;
            player.Hymenoptra().CurrentBees += 2;
            player.Hymenoptra().BeeResourceIncrease *= 2;
            player.Hymenoptra().GatheringIncrease += 2;
        }

        public override void UpdateEquip(Player player)
        {
            player.IncreaseBeeDamage(-0.20f);
            var modPlayer = BeeDamagePlayer.ModPlayer(player);
            modPlayer.BeeResourceMax2 += 60;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.ChlorophyteBar, 12).AddIngredient(ItemID.Ectoplasm, 3).AddIngredient(ItemID.HoneyBlock, 10).AddIngredient(ItemID.BottledHoney, 6).AddIngredient(ModContent.ItemType<Pollen>(), 30).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
