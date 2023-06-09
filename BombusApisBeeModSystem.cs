using BombusApisBee.UI;
using Terraria.UI;

namespace BombusApisBee
{
    public class BombusApisBeeModSystem : ModSystem
    {
        internal static bool finalizedRegisterCompat = false;
        internal static BombusApisBee mod;

        private GameTime _lastUpdateUiGameTime;

        public override void Load()
        {
            mod = ModContent.GetInstance<BombusApisBee>();
        }
        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if (mod.BeeDamageInterface?.CurrentState != null)
                mod.BeeDamageInterface.Update(gameTime);
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
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

                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                    "BombusApisBee: Bee Player UI",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && mod.BeeDamageInterface?.CurrentState != null)
                            mod.BeeDamageInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);

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
