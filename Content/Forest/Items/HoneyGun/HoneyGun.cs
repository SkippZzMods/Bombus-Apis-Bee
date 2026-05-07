using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Core.BeekeeperClass;

namespace BombusApisBee.Content.Forest.Items.HoneyGun
{
    public class HoneyGun : BeekeeperWeapon
    {
        public override bool SafeCanUseItem(Player player) => player.ownedProjectileCounts<HoneyGunHoldout>() <= 0;
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Gun");
            Tooltip.SetDefault("Fires a high velocity stream of honey");
        }
        public override void SafeSetDefaults()
        {
            Item.damage = 30;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 35;
            Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.value = Item.sellPrice(0, 0, 75, 25);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ProjectileType<HoneyGunHoldout>();
            Item.shootSpeed = 16.5f;
            honeyCost = 4;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {

            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.SlimeGun);
            recipe.AddIngredient(ItemID.BottledHoney, 5);
            recipe.AddIngredient(ItemType<PollenItem>(), 15);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
