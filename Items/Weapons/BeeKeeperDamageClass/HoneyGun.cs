using BombusApisBee.BeeDamageClass;
using BombusApisBee.Items.Other.Crafting;
using BombusApisBee.Projectiles;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class HoneyGun : BeeDamageItem
    {
        public override bool SafeCanUseItem(Player player) => player.ownedProjectileCounts<HoneyGunHoldout>() <= 0;
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Gun");
            Tooltip.SetDefault("Fires a high velocity stream of honey");
        }
        public override void SafeSetDefaults()
        {
            Item.damage = 45;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.value = Item.sellPrice(0, 0, 75, 25);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<HoneyGunHoldout>();
            Item.shootSpeed = 16.5f;
            beeResourceCost = 2;
            Item.noUseGraphic = true;
        }

        public override void AddRecipes()
        {

            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.SlimeGun);
            recipe.AddIngredient(ItemID.BottledHoney, 5);
            recipe.AddIngredient(ModContent.ItemType<Pollen>(), 15);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
