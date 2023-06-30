using Terraria;
namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    public class HoneyedEmblem : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honeyed Emblem");
            Tooltip.SetDefault("12% increased hymenoptra damage\nIncreases maximum honey by 10");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(gold: 6);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = BeeDamagePlayer.ModPlayer(player);
            modPlayer.BeeResourceMax2 += 10;
            player.IncreaseBeeDamage(0.12f);
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.AvengerEmblem).AddCondition(Condition.NearHoney).Register();
        }
    }
}