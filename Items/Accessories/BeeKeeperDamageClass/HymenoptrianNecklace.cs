using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Neck)]
    public class HymenoptrianNecklace : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Increases armor penetration by 5\nCoats hymenoptra attacks in a sweet nectar, granting them lifesteal on critical strikes\nMaximum health increased by 20\nReleases bees, Cthulhubees, increases hymenoptra damage, movement speed, and douses the user in honey when damaged");
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
            player.Bombus().RetinaReleaser = true;
            player.panic = true;
            player.Bombus().NectarVial = true;
            player.statLifeMax2 += 20;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient(ItemID.PanicNecklace).
                AddIngredient(ModContent.ItemType<Nectarlace>()).
                AddIngredient(ModContent.ItemType<RetinaReleaser>()).
                AddIngredient<Pollen>(20).
                AddTile(TileID.TinkerersWorkbench).
                Register();

            CreateRecipe(1).
                AddIngredient(ItemID.SweetheartNecklace).
                AddIngredient(ItemID.SharkToothNecklace).
                AddIngredient(ModContent.ItemType<RetinaReleaser>()).
                AddIngredient(ModContent.ItemType<NectarVial>()).
                AddIngredient<Pollen>(20).
                AddTile(TileID.TinkerersWorkbench).
                Register();

            CreateRecipe(1).
                AddIngredient(ItemID.PanicNecklace).
                AddIngredient(ItemID.StingerNecklace).
                AddIngredient(ModContent.ItemType<RetinaReleaser>()).
                AddIngredient(ModContent.ItemType<NectarVial>()).
                AddIngredient<Pollen>(20).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
