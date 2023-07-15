namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Body)]
    public class SkeletalBeeChestplate : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Reduces damage taken by 10%\nLoyal Bees increased by 2");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 2);
            Item.rare = ItemRarityID.Orange;
            Item.defense = 6;
        }

        public override void UpdateEquip(Player player)
        {
            player.endurance += 0.1f;   
            player.Hymenoptra().CurrentBees += 2;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.Bone, 15).AddIngredient(ItemID.BeeWax, 7).AddIngredient(ItemID.HellstoneBar, 10).AddTile(TileID.Anvils).Register();
        }
    }
}
