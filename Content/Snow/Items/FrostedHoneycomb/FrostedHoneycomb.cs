using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Core.BeekeeperClass;

namespace BombusApisBee.Content.Snow.Items.FrostedHoneycomb
{
    public class FrostedHoneycomb : BeekeeperWeapon
    {
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Throws a freezing honeycomb which shatters into multiple ice shards\nInflicts Glacialstruck\nCritically striking an enemy which is Glacialstruck causes them to explode into a burst of bees");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 11;
            Item.noMelee = true;
            Item.width = 28;
            Item.height = 28;
            Item.useTime = 65;
            Item.useAnimation = 65;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ProjectileType<FrostedHoneycombProj>();
            Item.shootSpeed = 15f;
            Item.UseSound = SoundID.Item1;
            Item.scale = 1;
            honeyCost = 5;
            critAdd = 8;
            Item.noUseGraphic = true;
        }

        public override void AddRecipes()
        {

            CreateRecipe(1).AddIngredient(ItemID.IceBlock, 35).AddIngredient(ItemType<PollenItem>(), 15).AddIngredient(ItemID.PlatinumBar, 8).AddTile(TileID.WorkBenches).Register();

            CreateRecipe(1).AddIngredient(ItemID.IceBlock, 35).AddIngredient(ItemType<PollenItem>(), 15).AddIngredient(ItemID.GoldBar, 8).AddTile(TileID.WorkBenches).Register();
        }
    }
}