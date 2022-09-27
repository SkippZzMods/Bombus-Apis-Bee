using BombusApisBee.Items.Accessories.BeeKeeperDamageClass;
using BombusApisBee.UI;
using System.Collections.Generic;
using Terraria.UI;

namespace BombusApisBee
{
    public class BombusApisBeeModSystem : ModSystem
    {
        internal static bool finalizedRegisterCompat = false;
        internal static BombusApisBee mod;
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseIndex = layers.FindIndex((GameInterfaceLayer layer) => layer.Name == "Vanilla: Mouse Text");
            layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Honey UI", delegate ()
            {
                HoneyPlayerUI.Draw();
                return true;
            }, (InterfaceScaleType)2));
            int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (resourceBarIndex != -1)
            {
                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                    "BombusApisBee: Honey Resource Bar",
                    delegate
                    {
                        HoneyResourceUI.Draw();
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public override void AddRecipes()
        {
            Recipe.Create(ItemID.AvengerEmblem).
                AddIngredient(ModContent.ItemType<BeeEmblem>(), 1).
                AddIngredient(ItemID.SoulofMight, 5).
                AddIngredient(ItemID.SoulofSight, 5).
                AddIngredient(ItemID.SoulofFright, 5).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
