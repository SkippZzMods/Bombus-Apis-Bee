using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class Beemerang : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Hold <left> to charge up a returning beemerang\nThe beemerang has increased knockback, velocity, damage, and amount of bees spawned on hit depending on charge");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 13;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 75;
            Item.useTime = 75;
            Item.shootSpeed = 3f;
            Item.knockBack = 3;
            Item.width = 32;
            Item.height = 32;
            Item.scale = 1f;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Blue;

            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;

            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<BeemerangHoldout>();
            honeyCost = 4;
        }

        public override bool SafeCanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<BeemerangHoldout>()] <= 0;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient(ModContent.ItemType<Pollen>(), 15).
                AddIngredient(ItemID.EnchantedBoomerang, 1).
                AddIngredient(ItemID.HoneyBlock, 10).
                AddTile(TileID.Anvils).Register();

        }
    }
}