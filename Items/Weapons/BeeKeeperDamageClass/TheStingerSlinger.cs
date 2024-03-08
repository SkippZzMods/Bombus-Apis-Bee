using BombusApisBee.Items.Other.Crafting;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class TheStingerSlinger : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Throws a yoyo that shoots stingers at enemies\nPeriodically fires out a burst of homing stingers");

            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
        }

        public override void SafeSetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.width = 32;
            Item.height = 32;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.shootSpeed = 16f;
            Item.knockBack = 3.5f;
            Item.damage = 34;
            Item.value = Item.sellPrice(gold: 6, silver: 75);
            Item.rare = ItemRarityID.LightRed;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<StingerYoyoProj>();
            honeyCost = 2;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.WoodYoyo).AddIngredient(ItemID.Stinger, 12).AddIngredient(ModContent.ItemType<Pollen>(), 30).AddIngredient(ItemID.CobaltBar, 8).AddTile(TileID.Anvils).Register();
            CreateRecipe(1).AddIngredient(ItemID.WoodYoyo).AddIngredient(ItemID.Stinger, 12).AddIngredient(ModContent.ItemType<Pollen>(), 30).AddIngredient(ItemID.PalladiumBar, 8).AddTile(TileID.Anvils).Register();
        }
    }
}