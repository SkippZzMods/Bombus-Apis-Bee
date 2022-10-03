using BombusApisBee.BeeDamageClass;
using BombusApisBee.Items.Other.Crafting;
using BombusApisBee.Projectiles;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class CystComb : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Cystcomb");
            Tooltip.SetDefault("Throws a returning honeycomb infested with tumors\nHitting enemies causes an explosion of ichor\n'A honeycomb overgrown by cysts.. gross'");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 80;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 35;
            Item.useTime = 35;
            Item.shootSpeed = 3f;
            Item.knockBack = 4f;
            Item.width = 32;
            Item.height = 32;
            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.LightRed;

            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.shoot = ModContent.ProjectileType<IchorHoneycombHoldout>();
            beeResourceCost = 4;
        }

        public override bool SafeCanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<IchorHoneycombHoldout>()] <= 0;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient(ItemID.Ichor, 15).
                AddIngredient(ItemID.Vertebrae, 10).
                AddIngredient(ModContent.ItemType<Pollen>(), 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}