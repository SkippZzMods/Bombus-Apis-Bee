using BombusApisBee.Items.Other.Crafting;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class TrueNectarSlasher : BeeDamageItem
    {

        private int combo = 0;
        public override bool SafeCanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<TrueNectarSlasherHoldout>()] <= 0;
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("True Nectar Slasher");
            Tooltip.SetDefault("Performs a five piece combo of light and heavy swings, and throws\nSlashes through enemies on hit\nReplenishes life on hit");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 75;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3.5f;
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<TrueNectarSlasherHoldout>();
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.value = Item.sellPrice(gold: 15);
            Item.rare = ItemRarityID.Yellow;
            honeyCost = 2;

            Item.Size = new Vector2(60);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, combo);
            combo++;
            if (combo > 4)
                combo = 0;

            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.BrokenHeroSword, 1).AddIngredient(ModContent.ItemType<NectarSlasher>()).AddIngredient(ModContent.ItemType<Pollen>(), 25).AddTile(TileID.MythrilAnvil).Register();
        }

    }
}