using BombusApisBee.BeeDamageClass;
using BombusApisBee.Items.Other.Crafting;
using BombusApisBee.Projectiles;
using Terraria.DataStructures;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class TheStarSwarmer : BeeDamageItem
    {
        public int Starshoot = 0;
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Rapidly fires star bees\nEvery seven shots a large star will fire\nThe large star splits into several smaller stars that follow the cursor");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 36;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1f;
            Item.value = Item.sellPrice(0, 7, 50, 25);
            Item.rare = ItemRarityID.LightRed;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AstralBee>();
            Item.shootSpeed = 6;
            Item.UseSound = SoundID.Item11;
            Item.scale = 1;
            Item.crit = 4;
            beeResourceCost = 1;
            ResourceChance = 0.25f;
        }


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Starshoot += 1;
            if (Starshoot >= 7)
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<BigStar>(), damage * 135 / 100, 1f, player.whoAmI);
                Starshoot = 0;
                player.UseBeeResource(4);
            }

            return true;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(15.AsRadians());
        }
        public override void AddRecipes()
        {

            CreateRecipe(1).AddIngredient(ModContent.ItemType<TheStarStrap>()).AddIngredient(ItemID.SoulofLight, 8).AddIngredient(ItemID.TitaniumBar, 12).AddIngredient(ModContent.ItemType<Pollen>(), 15).AddTile(TileID.MythrilAnvil).Register();

            CreateRecipe(1).AddIngredient(ModContent.ItemType<TheStarStrap>()).AddIngredient(ItemID.SoulofLight, 8).AddIngredient(ItemID.AdamantiteBar, 12).AddIngredient(ModContent.ItemType<Pollen>(), 15).AddTile(TileID.MythrilAnvil).Register();
        }


    }
}