using BombusApisBee.BeeDamageClass;
using BombusApisBee.Items.Other.Crafting;
using BombusApisBee.Projectiles;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class CursedHoneycomb : BeeDamageItem
    {
        public override bool SafeCanUseItem(Player player) => player.ownedProjectileCounts<CursedHoneycombThrowout>() <= 0;
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Hold <left> to control a cursed honeycomb\nThe cursed honeycomb spits out cursed teeth and eyes\nTeeth stick to enemies, dealing stacking damage-over-time, and eyes home in on enemies\nSpawns cursed bees on hit");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 55;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.noMelee = true;
            Item.width = 28;
            Item.height = 28;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CursedHoneycombThrowout>();
            Item.shootSpeed = 13f;
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.rare = ItemRarityID.LightRed;
            beeResourceCost = 5;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient(ItemID.CursedFlame, 10).
                AddIngredient(ItemID.WormTooth, 5).
                AddIngredient(ItemID.RottenChunk, 20).
                AddIngredient<Pollen>(20).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}