using BombusApisBee.Items.Other.Crafting;
using Terraria.DataStructures;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class SpectralBeeTome : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("'The souls of forgotten bees seek vengeance'");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(10, 7));
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 65;
            Item.noMelee = true;
            Item.width = 50;
            Item.height = 18;
            Item.useTime = 15;
            Item.useAnimation = 15;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(0, 10, 50, 0);

            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<SpectralBeeTomeHoldout>();
            Item.shootSpeed = 1f;

            beeResourceCost = 3;
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