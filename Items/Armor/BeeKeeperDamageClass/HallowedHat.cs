using BombusApisBee.BeeDamageClass;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Head)]
    public class HallowedHat : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("10% increased hymenoptra damage and critical strike chance\nIncreases maximum honey by 40\nIncreases your amount of Bees by 3");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = 10000;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 2;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return legs.type == ItemID.HallowedGreaves && body.type == ItemID.HallowedPlateMail;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Become immune after striking an enemy";
            player.onHitDodge = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.IncreaseBeeCrit(10);
            player.IncreaseBeeDamage(0.1f);
            var modPlayer = BeeDamagePlayer.ModPlayer(player);
            modPlayer.BeeResourceMax2 += 40;
            player.Hymenoptra().CurrentBees += 3;
        }


        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.HallowedBar, 13).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
