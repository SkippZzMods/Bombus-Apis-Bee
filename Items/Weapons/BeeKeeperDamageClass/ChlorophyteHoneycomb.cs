using BombusApisBee.BeeDamageClass;
using BombusApisBee.Items.Other.Crafting;
using BombusApisBee.Projectiles;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class ChlorophyteHoneycomb : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            // DisplayName.SetDefault("Honeycomb"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("Rapidly fires leafy bees\nThe bees cause crystal leafs to materialize upon death");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 47;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 19;
            Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5f;
            Item.value = Item.sellPrice(0, 7, 5, 0);
            Item.rare = ItemRarityID.Lime;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ChlorophyteBee>();
            Item.shootSpeed = 6f;
            Item.UseSound = SoundID.Item11;
            Item.scale = 1.1f;
            Item.crit = 4;
            beeResourceCost = 1;
        }


        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.ChlorophyteBar, 12).AddIngredient(ModContent.ItemType<Pollen>(), 20).AddTile(TileID.MythrilAnvil).Register();

        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-1, 0);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(5));
        }
    }
}