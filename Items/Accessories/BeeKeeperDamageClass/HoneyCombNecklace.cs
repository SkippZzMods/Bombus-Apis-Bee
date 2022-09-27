using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Neck)]
    public class HoneyCombNecklace : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nectarlace");
            Tooltip.SetDefault("Increases armor penetration by 5\nReleases bees and gives the honey buff when the user is damaged\nIncreases maximum honey by 10");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetArmorPenetration(DamageClass.Generic) += 5;
            player.honeyCombItem = Item;
            player.Hymenoptra().BeeResourceMax2 += 10;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.StingerNecklace).AddIngredient(ModContent.ItemType<JarOfHoney>()).AddIngredient(ModContent.ItemType<Pollen>(), 15).AddTile(TileID.TinkerersWorkbench).Register();
        }
    }
}