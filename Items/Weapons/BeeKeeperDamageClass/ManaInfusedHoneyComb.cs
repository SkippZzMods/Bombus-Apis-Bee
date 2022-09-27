using BombusApisBee.BeeDamageClass;
using BombusApisBee.Projectiles;
using Terraria.DataStructures;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class ManaInfusedHoneycomb : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            // DisplayName.SetDefault("Honeycomb"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("'Infused with the powers of the decrepit casters of the dungeon'\nFires giant bees that will spawn mana bolts on impact");
            Item.staff[Item.type] = true;
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 18;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.knockBack = 1f;
            Item.value = Item.sellPrice(0, 4, 25, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item43;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ManaBee>();
            Item.shootSpeed = 6;
            Item.scale = 1;
            Item.crit = 4;
            beeResourceCost = 2;
            Item.mana = 5;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(8));
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 65f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            const int Repeats = 55;
            for (int i = 0; i < Repeats; ++i)
            {
                float angle2 = 6.2831855f * (float)i / (float)Repeats;
                Dust dust3 = Dust.NewDustPerfect(position, DustID.DungeonWater, null, 50, default(Color), 1.1f);
                dust3.velocity = Utils.ToRotationVector2(angle2) * 4f;
                dust3.noGravity = true;
            }
            return true;
        }
        public override bool SafeCanUseItem(Player player)
        {
            return player.statMana > 5;
        }
    }
}