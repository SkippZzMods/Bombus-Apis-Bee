using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class BrainyHoneycomb : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Fires genius bees that inflict Confused and stacks of Cerebral Disorientation\nEach stack of Cerebral Disorientation causes the target to deal 5% less damage and take 5 damage over time, up to a maxmium of 5 stacks\n'The IQ of this honeycomb is off the charts'");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 11;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BrainyBee>();
            Item.shootSpeed = 6f;
            Item.UseSound = BombusApisBee.HoneycombWeapon;
            beeResourceCost = 3;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(player.Center + Vector2.Normalize(velocity) * 20f, DustID.Blood, velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(0.5f, 5f), Main.rand.Next(85), Scale: Main.rand.NextFloat(1f, 1.3f)).noGravity = true;
            }

            return true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-6, 0);
        }
    }
}