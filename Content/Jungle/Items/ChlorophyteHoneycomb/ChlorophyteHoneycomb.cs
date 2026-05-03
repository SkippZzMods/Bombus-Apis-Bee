using BombusApisBee.Content.Forest.Items.Pollen;

namespace BombusApisBee.Content.Jungle.Items.ChlorophyteHoneycomb
{
    public class ChlorophyteHoneycomb : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Chloro-comb");
            Tooltip.SetDefault("Throws a fragile honeycomb which shatters into homing fragments, chloro-spores, and chloro-bees\nChloro-bees materialize into chloro-energy upon death");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 44;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 20;

            Item.useTime = 85;
            Item.useAnimation = 85;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5f;
            Item.value = Item.sellPrice(gold: 10);
            Item.rare = ItemRarityID.Lime;

            Item.autoReuse = true;
            Item.shoot = ProjectileType<ChlorophyteHoneycombProjectile>();
            Item.shootSpeed = 20f;
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.noUseGraphic = true;

            honeyCost = 7;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.ChlorophyteBar, 12).AddIngredient(ItemType<PollenItem>(), 20).AddTile(TileID.MythrilAnvil).Register();

        }
    }
}