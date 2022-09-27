using BombusApisBee.BeeDamageClass;
using BombusApisBee.Items.Other.Crafting;
using BombusApisBee.Projectiles;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class Honeycomb : BeeDamageItem
    {
        public int shootCount;
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Fires a burst of three bees before throwing the empty honeycomb\n'Dude this honeycomb only has like three bees in it'");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 8;
            Item.noMelee = true;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = 100;
            Item.rare = ItemRarityID.Gray;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<HoneycombHoldout>();
            Item.shootSpeed = 6f;
            beeResourceCost = 3;
            Item.noUseGraphic = true;
        }

        public override bool SafeCanUseItem(Player player) => player.ownedProjectileCounts<HoneycombHoldout>() <= 0;
        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient(ModContent.ItemType<Pollen>(), 5).
                AddIngredient(ItemID.Wood, 5).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}