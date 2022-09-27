using BombusApisBee.BeeDamageClass;
using BombusApisBee.Items.Other.Crafting;
using BombusApisBee.Projectiles;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class TheBeeBlade : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("'A near perfect blade, radiating the energy of the Hive'\nCreates a large slash of honey on swing\nStriking an enemie spawns a flurry of stingers behind the player");
        }

        public override void SafeSetDefaults()
        {
            Item.width = (Item.height = 60);
            Item.damage = 142;
            Item.DamageType = DamageClass.Melee;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6.25f;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 15f;
            Item.value = Item.sellPrice(gold: 12);
            beeResourceCost = 4;
            ResourceChance = 0.25f;
        }
        public override bool SafeCanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<BeeBladeSwing>()] <= 0;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Combo != -1f && Combo != 1f)
            {
                Combo = 1f;
            }
            Projectile.NewProjectile(source, player.Center, velocity, ModContent.ProjectileType<BeeBladeSwing>(), damage, knockback, player.whoAmI, Combo);
            Combo *= -1f;
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<TrueHoneyBeeSkySlasher>()).AddIngredient(ModContent.ItemType<TrueStingerScimitar>()).AddIngredient(ItemID.Ectoplasm, 20).AddIngredient(ModContent.ItemType<Pollen>(), 65).AddTile(TileID.MythrilAnvil).Register();
        }
        public float Combo = 1f;
    }
}