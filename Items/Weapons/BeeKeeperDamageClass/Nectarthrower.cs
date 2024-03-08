using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class Nectarthrower : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Blasts out molten nectar, encasing enemies in a glaze of nectar\nEnemies who are glazed send out healing bolts when struck");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 29;
            Item.noMelee = true;
            Item.width = 70;
            Item.height = 18;
            Item.useTime = 5;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.value = 250000;
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<NectarthrowerProj>();
            Item.shootSpeed = 12.5f;
            Item.UseSound = SoundID.Item34;
            honeyCost = 2;

            resourceChance = 0.33f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 45f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                position += muzzleOffset;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.HoneyDust>(), velocity.RotatedByRandom(0.95f) * Main.rand.NextFloat(0.35f), Main.rand.Next(150), default, 1.25f).noGravity = true;
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowFastDecelerate>(), velocity.RotatedByRandom(0.95f) * Main.rand.NextFloat(0.2f), 0, new Color(214, 158, 79), 0.35f).noGravity = true;
            }
            return true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-7, 0);
        }
    }
}