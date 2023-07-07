using BombusApisBee.Items.Other.Crafting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    public class HoneycombShard : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Increases the chance to strengthen friendly bees by 15%\n'Crunchy!'");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Hymenoptra().BeeStrengthenChance += 0.15f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Pollen>(20).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
