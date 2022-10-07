using BombusApisBee.BeeDamageClass;
using BombusApisBee.Projectiles;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class GelatinousHoneycomb : BeeDamageItem
    {
        int shots;
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Fires royal bees which cause bouncy slimeballs to rain from the sky\nFires a volatile ruby every few shots\n'A honeycomb fit for a king'");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 9;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 14;
            Item.useAnimation = 14;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3f;
            Item.value = Item.sellPrice(gold: 2);

            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<GelBee>();
            Item.shootSpeed = 6f;
            Item.UseSound = BombusApisBee.HoneycombWeapon;

            beeResourceCost = 1;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(5));
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 25f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                position += muzzleOffset;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.SlimeDust>(), velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(), Main.rand.Next(100, 165), default, 1.15f).noGravity = true;
            }
            if (++shots >= 4)
            {
                shots = 0;
                player.UseBeeResource(3);
                Projectile.NewProjectile(source, position, velocity * 2.5f, ModContent.ProjectileType<RubyProjectile>(), damage, 3f, player.whoAmI);
                for (int i = 0; i < 12; i++)
                {
                    Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowFastDecelerate>(), velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(), 0, Main.rand.NextBool() ? new Color(243, 172, 140) : new Color(212, 37, 24), 0.35f);
                }
            }
            return true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-8, 0);
        }
    }
}