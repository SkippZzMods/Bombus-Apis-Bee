using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Content.Jungle.Items.NectarVial;

namespace BombusApisBee.Content.Forest.Items.HymenoptrianNecklace
{
    [AutoloadEquip(EquipType.Neck)]
    public class HymenoptrianNecklace : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Increases armor penetration by 5\nCoats beekeeper attacks in a sweet nectar, granting them lifesteal on critical strikes\nMaximum health increased by 20\nReleases bees, Cthulhubees, increases beekeeper damage, movement speed, and douses the user in honey when damaged");
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
            player.Bombus().RetinaReleaser = true;
            player.panic = true;
            player.Bombus().NectarVial = true;
            player.statLifeMax2 += 20;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient(ItemID.PanicNecklace).
                AddIngredient(ItemType<Nectarlace.Nectarlace>()).
                AddIngredient(ItemType<RetinaReleaser.RetinaReleaser>()).
                AddIngredient<PollenItem>(20).
                AddTile(TileID.TinkerersWorkbench).
                Register();

            CreateRecipe(1).
                AddIngredient(ItemID.SweetheartNecklace).
                AddIngredient(ItemID.SharkToothNecklace).
                AddIngredient(ItemType<RetinaReleaser.RetinaReleaser>()).
                AddIngredient(ItemType<NectarVial>()).
                AddIngredient<PollenItem>(20).
                AddTile(TileID.TinkerersWorkbench).
                Register();

            CreateRecipe(1).
                AddIngredient(ItemID.PanicNecklace).
                AddIngredient(ItemID.StingerNecklace).
                AddIngredient(ItemType<RetinaReleaser.RetinaReleaser>()).
                AddIngredient(ItemType<NectarVial>()).
                AddIngredient<PollenItem>(20).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
