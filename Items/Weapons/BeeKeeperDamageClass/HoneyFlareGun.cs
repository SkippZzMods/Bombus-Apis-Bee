using BombusApisBee.BeeDamageClass;
using BombusApisBee.Items.Other.Crafting;
using BombusApisBee.Projectiles;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class HoneyFlareGun : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Fires honey flares which stick into enemies and explode into homing honey and honey clouds\nThe honey clouds grant the user the Honey Buff when they are in the cloud");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 14;
            Item.noMelee = true;
            Item.width = 25;
            Item.height = 25;
            Item.useTime = 35;
            Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1f;
            Item.value = Item.sellPrice(0, 1, 25, 0);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<HoneyFlare>();
            Item.shootSpeed = 15;
            Item.UseSound = SoundID.Item11;
            Item.scale = 1;
            Item.crit = 4;
            beeResourceCost = 2;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.FlareGun, 1).AddIngredient(ItemID.BottledHoney, 5).AddIngredient(ModContent.ItemType<Pollen>(), 20).AddTile(TileID.Anvils).Register();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 25f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                position += muzzleOffset;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(position + (Vector2.UnitY.RotatedBy(velocity.ToRotation()) * -5f) * player.direction, ModContent.DustType<Dusts.HoneyDust>(), velocity.RotatedByRandom(0.45f) * Main.rand.NextFloat(0.25f), Main.rand.Next(100)).noGravity = true;
                Dust.NewDustPerfect(position + (Vector2.UnitY.RotatedBy(velocity.ToRotation()) * -5f) * player.direction, DustID.Honey2, velocity.RotatedByRandom(0.45f) * Main.rand.NextFloat(0.25f), Main.rand.Next(100)).noGravity = true;
            }
            return true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-11, 0);
        }
    }
}
