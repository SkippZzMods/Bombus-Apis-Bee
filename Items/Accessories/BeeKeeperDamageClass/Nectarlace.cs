using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Neck)]
    public class Nectarlace : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nectarlace");
            Tooltip.SetDefault("Increases armor penetration by 5\nReleases bees and douses the user in honey when damaged\nCoats hymenoptra attacks in a sweet nectar, granting them lifesteal on critical strikes\nMaximum health increased by 20");
            Item.ResearchUnlockCount = 1;
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
            player.Bombus().NectarVial = true;
            player.statLifeMax2 += 20;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.StingerNecklace).AddIngredient(ModContent.ItemType<NectarVial>()).AddIngredient(ModContent.ItemType<Pollen>(), 15).AddTile(TileID.TinkerersWorkbench).Register();
        }
    }
}