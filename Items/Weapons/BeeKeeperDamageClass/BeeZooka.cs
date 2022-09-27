using BombusApisBee.BeeDamageClass;
using BombusApisBee.Items.Other.Crafting;
using BombusApisBee.Projectiles;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class BeeZooka : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("'Name dropper.'");
            DisplayName.SetDefault("Beezooka");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 88;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 110;
            Item.useAnimation = 110;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5f;
            Item.value = Item.sellPrice(gold: 5, silver: 75);
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BeezookaRocket>();
            Item.shootSpeed = 14f;
            Item.UseSound = SoundID.Item61;
            Item.scale = 1;
            Item.crit = 4;
            beeResourceCost = 8;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-8, 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<BeenadeLauncher>()).AddIngredient(ItemID.HallowedBar, 10).AddIngredient(ItemID.Hive, 10).AddIngredient(ItemID.BottledHoney, 25).AddIngredient(ModContent.ItemType<Pollen>(), 15).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}