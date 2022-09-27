using BombusApisBee.BeeDamageClass;
using BombusApisBee.Items.Other.Crafting;
using BombusApisBee.Projectiles;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class FlaskOHoney : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Flask O' Honey"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("'Proceed with caution'\nThrows a flask of honey that spawns honey clouds on death");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 60;
            Item.noMelee = true;
            Item.width = 25;
            Item.height = 25;
            Item.useTime = 39;
            Item.useAnimation = 39;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.value = Item.sellPrice(0, 11, 75, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<HoneyFlaskProj>();
            Item.shootSpeed = 13f;
            Item.UseSound = SoundID.Item106;
            Item.scale = 1;
            Item.crit = 4;
            beeResourceCost = 4;
            Item.noUseGraphic = true;
            Item.value = Item.sellPrice(0, 10);
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.ToxicFlask, 1).AddIngredient(ItemID.BottledHoney, 5).AddIngredient(ModContent.ItemType<Pollen>(), 15).AddTile(TileID.MythrilAnvil).Register();
        }

    }
}