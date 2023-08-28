using BombusApisBee.Items.Other.Crafting;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class BladeOfAculeus : BeeDamageItem
    {
        private int combo = 0;
        public override bool SafeCanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<AculeusBladeHoldout>()] <= 0;
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Blade of Aculeus");
            Tooltip.SetDefault("Performs hefty strikes, flinging piercing stingers with deadly force\nMelee strikes cleave the armor of enemies");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 90;
            Item.useAnimation = 90;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 8f;
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<AculeusBladeHoldout>();
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.value = Item.sellPrice(gold: 3, silver: 25);
            Item.rare = ItemRarityID.Green;
            Item.Size = new Vector2(50);
            beeResourceCost = 3;
        }
       
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, combo);
            combo++;
            if (combo > 1)
            {
                combo = 0;
            }

            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.Stinger, 7).AddIngredient(ItemID.EnchantedSword).AddIngredient(ModContent.ItemType<Pollen>(), 15).AddTile(TileID.Anvils).Register();
        }
    }
}