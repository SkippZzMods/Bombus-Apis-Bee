using BombusApisBee.BeeDamageClass;
using BombusApisBee.Projectiles;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class Skelecomb : BeeDamageItem
    {
        int cooldown;
        public override bool AltFunctionUse(Player player) => cooldown <= 0;
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Conjures skeletal bees which slash enemies\nPress <right> to fire a cursed skull, cursing enemies, causing them to take 15% more damage from all sources");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 27;
            Item.noMelee = true;

            Item.width = 40;
            Item.height = 40;

            Item.useTime = 80;
            Item.useAnimation = 80;

            Item.useStyle = ItemUseStyleID.Shoot;

            Item.knockBack = 0f;

            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Green;

            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SkeletalBee>();

            Item.shootSpeed = 6f;

            Item.UseSound = SoundID.Item43;

            beeResourceCost = 3;
        }

        public override void HoldItem(Player player)
        {
            if (cooldown > 0)
            {
                if (cooldown == 1)
                {
                    SoundID.MaxMana.PlayWith(player.Center, -0.25f);
                    for (int i = 0; i < 15; i++)
                    {
                        Dust.NewDustPerfect(player.GetArmPosition() + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(1f, 1f), 0, new Color(238, 164, 255), 0.45f);
                    }
                }
                cooldown--;
            }
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(8));
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 55f;
            position += muzzleOffset;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(player.Center + new Vector2(28, 1 * player.direction).RotatedBy(velocity.ToRotation()), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(0.5f, 0.5f), 0, new Color(238, 164, 255), 0.45f);
            }
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, velocity * 2f, ModContent.ProjectileType<SkelecombProjectile>(), (int)(damage * 1.5f), 3f, player.whoAmI);

                for (int i = 0; i < 40; i++)
                {
                    Dust.NewDustPerfect(position + Main.rand.NextVector2CircularEdge(25f, 25f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.Zero, 0, new Color(238, 164, 255), 0.55f);
                    Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(3.5f, 3.5f), 0, new Color(100, 70, 107), 0.65f);

                    Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowFastDecelerate>(), velocity.RotatedByRandom(0.25f) * Main.rand.NextFloat(1.5f), 0, new Color(238, 164, 255), 0.75f);
                }

                cooldown = 180;
                player.UseBeeResource(6);
                return false;
            }

            for (int i = 0; i < 35; i++)
            {
                Dust.NewDustPerfect(position + Main.rand.NextVector2CircularEdge(10f, 10f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.Zero, 0, new Color(238, 164, 255), 0.45f);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 0, new Color(100, 70, 107), 0.55f);

                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowFastDecelerate>(), velocity.RotatedByRandom(0.15f) * Main.rand.NextFloat(), 0, new Color(238, 164, 255), 0.65f);
            }

            for (int i = 0; i < Main.rand.Next(1, 4); i++)
            {
                velocity = velocity.RotatedByRandom(MathHelper.ToRadians(8));
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }
    }
}