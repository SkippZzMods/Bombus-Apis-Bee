using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class MegaHornet : BeeDamageItem
    {
        public override bool SafeCanUseItem(Player player) => player.ownedProjectileCounts<MegaHornetHoldout>() <= 0;
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Megahornet");
            Tooltip.SetDefault("Charges up the longer you fire, dealing increased damage\n'Minihornet's older sister'");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 34;
            Item.noMelee = true;
            Item.width = 50;
            Item.height = 18;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.value = Item.sellPrice(gold: 7, silver: 50);
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MegaHornetHoldout>();
            Item.shootSpeed = 19f;
            beeResourceCost = 1;
            ResourceChance = 0.33f;
            Item.channel = true;
            Item.noUseGraphic = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<Pollen>(), 25);
            recipe.AddIngredient(ModContent.ItemType<MiniHornet>());
            recipe.AddIngredient(ItemID.SoulofMight, 15);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
