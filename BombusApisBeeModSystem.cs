using BombusApisBee.Content.Hell.Items.BeekeeperEmblem;
using BombusApisBee.Core.Systems.ParticleSystem;
using BombusApisBee.UI;
using Terraria.Localization;
using Terraria.UI;

namespace BombusApisBee
{
    public class BombusApisBeeModSystem : ModSystem
    {
        internal static bool finalizedRegisterCompat = false;
        internal static BombusApisBee mod;

        private GameTime _lastUpdateUiGameTime;

        public static int CopperBarGroupID;
        public static int SilverBarGroupID;

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
        public override void PreUpdateItems()
        {
            if (Main.netMode != NetmodeID.Server)
                ParticleHandler.UpdateAllParticles();
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

        public override void AddRecipeGroups()
        {
            RecipeGroup SilverBarRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.SilverBar)}",
            ItemID.SilverBar, ItemID.TungstenBar);

            SilverBarGroupID = RecipeGroup.RegisterGroup(nameof(ItemID.SilverBar), SilverBarRecipeGroup);

            RecipeGroup CopperBarRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.CopperBar)}",
            ItemID.CopperBar, ItemID.TinBar);

            CopperBarGroupID = RecipeGroup.RegisterGroup(nameof(ItemID.CopperBar), CopperBarRecipeGroup);
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
