namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Head)]
    public class HoneyHoarderMask : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("10% increased hymenoptra damage and critical strike chance\nIncreases maximum honey by 20\nIncreases your amount of Loyal Bees by 1");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 8);
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 15;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return legs.type == ModContent.ItemType<HoneyHoarderPantalones>() && body.type == ModContent.ItemType<HoneyHoarderChestpiece>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Honey materializes on critical hits\nIncreases hymenoptra damage by 15% while your Bees are in Attacking mode\nIncreases damage reduction by 20% while your Bees are in Defense mode";
            var modPlayer = player.Hymenoptra();
            player.Bombus().HoneyHoarderSet = true;
            player.Bombus().HoneyCrit = true;
            player.Hymenoptra().CurrentBees += 1;

            if (modPlayer.HasBees)
            {
                if (modPlayer.CurrentBeeState == (int)BeeDamagePlayer.BeeState.Defense)
                {
                    player.IncreaseBeeDamage(0.15f);
                }
                else if (modPlayer.CurrentBeeState == (int)BeeDamagePlayer.BeeState.Offense)
                {
                    player.endurance += 0.2f;
                }
            }
        }
        public override void UpdateEquip(Player player)
        {
            player.IncreaseBeeDamage(0.10f);
            player.IncreaseBeeCrit(10);
            player.Hymenoptra().BeeResourceMax2 += 20;
        }


        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.ChlorophyteBar, 12).AddIngredient(ItemID.Ectoplasm, 3).AddIngredient(ItemID.HoneyBlock, 10).AddIngredient(ItemID.BottledHoney, 6).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
