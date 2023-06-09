using BombusApisBee.Items.Other.Crafting;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class Ambrosia : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Ambrosial Throw");
            Tooltip.SetDefault("Throws a yoyo of pure nectar\nCauses nectar explosions on hit, healing the user");

            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
        }

        public override void SafeSetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.width = 24;
            Item.height = 24;
            Item.useAnimation = 35;
            Item.useTime = 35;
            Item.shootSpeed = 16f;
            Item.knockBack = 2.5f;
            Item.damage = 21;
            Item.value = Item.sellPrice(gold: 1, silver: 45);
            Item.rare = ItemRarityID.Orange;

            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<AmbrosiaProjectile>();
            beeResourceCost = 2;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BeeWax, 10).
                AddIngredient(ItemID.JungleYoyo, 1).
                AddIngredient(ModContent.ItemType<Pollen>(), 25).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
