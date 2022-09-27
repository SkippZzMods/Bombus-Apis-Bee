using BombusApisBee.BeeDamageClass;
using BombusApisBee.Items.Other.Crafting;
using BombusApisBee.Projectiles;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class TheHive : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Throws a Beehive that spawns bees on contact\nUses 1 honey when an bee is spawned");

            // These are all related to gamepad controls and don't seem to affect anything else
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
        }

        public override void SafeSetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.width = 24;
            Item.height = 24;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.shootSpeed = 16f;
            Item.knockBack = 2.5f;
            Item.damage = 20;
            Item.value = Item.sellPrice(gold: 3, silver: 45);
            Item.rare = ItemRarityID.Orange;

            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<TheHiveProjectile>();
            beeResourceCost = 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.BeeWax, 10).AddIngredient(ItemID.JungleYoyo, 1).AddIngredient(ModContent.ItemType<Pollen>(), 25).AddTile(TileID.WorkBenches).Register();
        }

    }
}
