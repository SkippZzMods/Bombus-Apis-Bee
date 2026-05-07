using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Core.BeekeeperClass;

namespace BombusApisBee.Content.Forest.Items.Honeycomb
{
    public class Honeycomb : BeekeeperWeapon
    {
        public int shootCount;
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Fires a burst of three bees before throwing the empty honeycomb\n'Dude this honeycomb only has like three bees in it'");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 5;
            Item.noMelee = true;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = 100;
            Item.rare = ItemRarityID.Gray;
            Item.autoReuse = true;
            Item.shoot = ProjectileType<HoneycombHoldout>();
            Item.shootSpeed = 6f;
            honeyCost = 4;
            Item.noUseGraphic = true;
        }

        public override bool SafeCanUseItem(Player player) => player.ownedProjectileCounts<HoneycombHoldout>() <= 0;
        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient(ItemType<PollenItem>(), 5).
                AddIngredient(ItemID.Wood, 5).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}