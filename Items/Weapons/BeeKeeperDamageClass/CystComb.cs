using BombusApisBee.BeeDamageClass;
using BombusApisBee.Items.Other.Crafting;
using BombusApisBee.Projectiles;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class CystComb : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            // DisplayName.SetDefault("Honeycomb"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("'A honeycomb overgrown by cysts.. gross'");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 25;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1f;
            Item.value = Item.sellPrice(0, 3, 50, 0);
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<IchorBee>();
            Item.shootSpeed = 6;
            Item.UseSound = SoundID.Item11;
            Item.scale = 1;
            Item.crit = 4;
            Item.rare = ItemRarityID.LightRed;
            beeResourceCost = 2;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(5));
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.Ichor, 15).AddIngredient(ModContent.ItemType<Pollen>(), 10).AddIngredient(ItemID.HoneyBlock, 10).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}