using Terraria.DataStructures;

namespace BombusApisBee.Items.Other.Crafting
{
    public class PhotonFragment : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Photonic Particle");
            Tooltip.SetDefault("'An incessant buzzing sound can be heard coming from this particle'");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
            ItemID.Sets.ItemIconPulse[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            SacrificeTotal = 999;
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 24;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 25, 0);
            Item.rare = ItemRarityID.Cyan;
        }
        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.essScale * Utils.NextFloat(Main.rand, 0.9f, 1.1f);
            Lighting.AddLight(Item.Center, 0.3f * brightness, 0.3f * brightness, 0.05f * brightness);
        }
        public override void AddRecipes()
        {
            CreateRecipe(2).AddIngredient(ItemID.FragmentNebula, 1).AddIngredient(ItemID.FragmentSolar, 1).AddIngredient(ItemID.FragmentStardust, 1).AddIngredient(ItemID.FragmentVortex, 1).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
