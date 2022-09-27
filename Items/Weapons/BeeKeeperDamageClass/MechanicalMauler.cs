using BombusApisBee.BeeDamageClass;
using BombusApisBee.Items.Other.Crafting;
using BombusApisBee.Projectiles;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class MechanicalMauler : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            // DisplayName.SetDefault("Honeycomb"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("'The mechatronics power, at the palm of your hand'");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 55;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(0, 12, 50, 0);
            Item.rare = ItemRarityID.LightPurple;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MetalBee>();
            Item.shootSpeed = 6;
            Item.UseSound = SoundID.Item11;
            Item.scale = 1;
            Item.crit = 4;
            beeResourceCost = 2;
        }
        public int shoot;
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(8.AsRadians());
            type = Main.rand.Next(new int[] { type, ModContent.ProjectileType<SpazBee>(), ModContent.ProjectileType<Probee>(), });
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            shoot += 1;
            if (shoot >= 5)
            {
                SoundEngine.PlaySound(SoundID.Item60, player.position);
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<MechaBeam>(), damage, knockback, player.whoAmI);
                shoot = 0;
            }
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<ProbeyComb>(), 1).AddIngredient(ModContent.ItemType<SpazmaCannon>(), 1).AddIngredient(ModContent.ItemType<MetalPlatedHoneycomb>(), 1).AddIngredient(ModContent.ItemType<Pollen>(), 50).AddTile(TileID.MythrilAnvil).Register();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }

    }
}