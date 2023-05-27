using BombusApisBee.BeeDamageClass;
using BombusApisBee.Items.Other.Crafting;
using BombusApisBee.Projectiles;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class MiniHornet : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Minihornet");
            Tooltip.SetDefault("'Half hornet, half gun, completely awesome.'");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 12;
            Item.noMelee = true;
            Item.width = 50;
            Item.height = 18;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1f;
            Item.value = Item.sellPrice(gold: 7);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<StingerFriendly>();
            Item.shootSpeed = 22;
            Item.UseSound = SoundID.Item11;
            Item.scale = 1;
            beeResourceCost = 1;
            ResourceChance = 0.25f;
        }


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(10));
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 50f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                position += muzzleOffset;

            position += new Vector2(0, 3 * player.direction).RotatedBy(velocity.ToRotation());
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 7; i++)
            {
                Dust.NewDustPerfect(position, DustID.CorruptGibs, velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(0.25f), Main.rand.Next(75)).noGravity = true;
                Dust.NewDustPerfect(position, DustID.Poisoned, velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(0.25f), Main.rand.Next(75)).noGravity = true;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.Minishark, 1).AddIngredient(ItemID.Stinger, 5).AddIngredient(ItemID.Vine, 5).AddIngredient(ModContent.ItemType<Pollen>(), 15).AddTile(TileID.Anvils).Register();
        }

    }
}