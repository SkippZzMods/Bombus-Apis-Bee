using BombusApisBee.Items.Other.Crafting;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class StoneHoneycomb : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Petrified Honeycomb");
            Tooltip.SetDefault("Throws a heavy petrified honeycomb\nThe honeycomb always critically strikes when falling with enough velocity\n'Rocks... bees... what could go wrong?'");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 18;
            Item.noMelee = true;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 65;
            Item.useAnimation = 65;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5f;
            Item.value = Item.sellPrice(silver: 15);
            Item.rare = ItemRarityID.White;
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<StoneHoneycombProjectile>();
            Item.shootSpeed = 13;
            beeResourceCost = 5;
            Item.noUseGraphic = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.StoneBlock, 35).AddIngredient(ModContent.ItemType<Pollen>(), 10).AddTile(TileID.WorkBenches).Register();
        }
    }
}