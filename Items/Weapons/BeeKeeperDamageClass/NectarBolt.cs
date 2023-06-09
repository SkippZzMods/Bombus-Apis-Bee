using BombusApisBee.Items.Other.Crafting;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class NectarBolt : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Casts a bolt of glistening nectar, which bounces off of tiles\nSpawns homing nectar on bounce, which have slight lifesteal");
        }
        public override void SafeSetDefaults()
        {
            Item.damage = 17;
            Item.width = 28;
            Item.height = 30;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<NectarBoltProjectile>();
            Item.shootSpeed = 9f;
            beeResourceCost = 4;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5f, 0);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(position + (Vector2.Normalize(velocity) * 20f), ModContent.DustType<Dusts.GlowFastDecelerate>(), velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0f, 0.5f), 0, new Color(255, 255, 150), 0.65f);
            }
            Projectile.NewProjectile(source, position + (Vector2.Normalize(velocity) * 20f), velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
        public override void AddRecipes()
        {
            Recipe modRecipe = CreateRecipe();
            modRecipe.AddIngredient(ModContent.ItemType<Pollen>(), 15);
            modRecipe.AddIngredient(ItemID.Moonglow, 5);
            modRecipe.AddIngredient(ItemID.HoneyBucket, 2);
            modRecipe.AddIngredient(ItemID.Book);
            modRecipe.AddTile(TileID.Bookcases);
            modRecipe.Register();
        }
    }
}
