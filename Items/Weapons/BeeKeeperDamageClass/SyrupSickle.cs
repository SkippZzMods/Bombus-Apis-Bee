using BombusApisBee.BeeDamageClass;
using BombusApisBee.Projectiles;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class SyrupSickle : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Nectar Sickle");
            Tooltip.SetDefault("Conjures a sickle of shimmering nectar in front of the user\nCritically striking an enemy causes you to have majorly increased life regeneration for a short time");
        }
        public override void SafeSetDefaults()
        {
            Item.damage = 50;
            Item.useTime = 65;
            Item.useAnimation = 65;
            Item.UseSound = SoundID.Item8;
            Item.shoot = ModContent.ProjectileType<SyrupSickleProjectile>();
            Item.knockBack = 3f;
            Item.rare = ItemRarityID.Orange;
            beeResourceCost = 5;
            Item.width = 32;
            Item.height = 32;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(gold: 2);
            Item.shootSpeed = 5f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += Vector2.Normalize(velocity) * 45f;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            BeeUtils.CircleDust(position, 45, ModContent.DustType<Dusts.GlowFastDecelerate>(), 1.5f, 0, new Color(255, 191, 73), 0.35f);
            return true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-7f, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<NectarBolt>().
                AddIngredient(ItemID.DemonScythe).
                AddIngredient(ItemID.ShadowScale, 10).
                AddTile(TileID.Anvils).
                Register();

            CreateRecipe().
                AddIngredient<NectarBolt>().
                AddIngredient(ItemID.DemonScythe).
                AddIngredient(ItemID.TissueSample, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
