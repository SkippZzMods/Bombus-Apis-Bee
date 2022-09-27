using BombusApisBee.BeeDamageClass;
using BombusApisBee.Items.Other.Crafting;
using BombusApisBee.Projectiles;
using Terraria.DataStructures;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class SpectralBeeTome : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            // DisplayName.SetDefault("Honeycomb"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("'The souls of forgotten bees seek vengeance'");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(10, 8));

        }

        public override void SafeSetDefaults()
        {
            Item.damage = 58;
            Item.noMelee = true;
            Item.width = 50;
            Item.height = 18;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.value = Item.sellPrice(0, 10, 50, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SpectralBeeTomeHoldout>();
            Item.shootSpeed = 0f;
            Item.UseSound = null;
            Item.scale = 1;
            Item.crit = 4;
            beeResourceCost = 1;
            Item.noUseGraphic = true;
            Item.channel = true;
            ResourceChance = 0.33f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(3, 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.SpectreBar, 12).AddIngredient(ItemID.Book, 1).AddIngredient(ModContent.ItemType<Pollen>(), 35).AddTile(TileID.Bookcases).Register();
        }
        public override bool SafeCanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<SpectralBeeTomeHoldout>()] <= 0;
        }
    }
}