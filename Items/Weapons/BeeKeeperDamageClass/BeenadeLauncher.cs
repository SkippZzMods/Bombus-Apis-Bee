using BombusApisBee.BeeDamageClass;
using BombusApisBee.Items.Other.Crafting;
using BombusApisBee.Projectiles;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class BeenadeLauncher : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Fires bouncing pipebeeoms full of bees\n'Oh they're goin to hav' to glue you back togetha'\n'IN HELL'");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 31;
            Item.noMelee = true;
            Item.width = 50;
            Item.height = 20;
            Item.useTime = 65;
            Item.useAnimation = 65;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(0, 3, 25, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BeenadeLauncherProjectile>();
            Item.shootSpeed = 16f;
            Item.UseSound = SoundID.Item61;
            Item.scale = 1;
            Item.crit = 4;
            beeResourceCost = 5;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 35f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                position += muzzleOffset;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient(ItemID.HellstoneBar, 5).
                AddIngredient(ItemID.Beenade, 15).
                AddIngredient(ItemID.IronBar, 10).
                AddIngredient(ItemID.IllegalGunParts, 1).
                AddIngredient(ModContent.ItemType<Pollen>(), 10).
                AddTile(TileID.Anvils).
                Register();

            CreateRecipe(1).
                AddIngredient(ItemID.HellstoneBar, 5).
                AddIngredient(ItemID.Beenade, 15).
                AddIngredient(ItemID.LeadBar, 10).
                AddIngredient(ItemID.IllegalGunParts, 1).
                AddIngredient(ModContent.ItemType<Pollen>(), 10).
                AddTile(TileID.Anvils).
                Register();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }
    }
}