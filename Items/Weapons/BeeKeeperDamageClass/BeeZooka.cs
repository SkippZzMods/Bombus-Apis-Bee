using BombusApisBee.BeeDamageClass;
using BombusApisBee.Items.Other.Crafting;
using BombusApisBee.Projectiles;
using System;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class BeeZooka : BeeDamageItem
    {
        int shots;
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Fires a burst of homing bee-rockets, which explode into mini-missiles\nMini-missiles target the closest enemy to the mouse cursor, and explode into shrapnel\n'If God had wanted you to live, he would not have created me!'");
            DisplayName.SetDefault("Beezooka");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 35;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 40;

            Item.useTime = 12;
            Item.useAnimation = 36;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3.5f;
            Item.value = Item.sellPrice(gold: 4, silver: 25);
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BeezookaRocket>();

            Item.shootSpeed = 18f;

            beeResourceCost = 5;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedBy(MathHelper.ToRadians(-5f * shots * player.direction));

            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 60f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
                position += (Vector2.UnitY * -4f * player.direction).RotatedBy(velocity.ToRotation());
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (++shots >= 3)
            {
                player.reuseDelay = 90;
                shots = 0;
            }

            SoundID.Item61.PlayWith(position, -0.1f, 0.1f);
            player.UseBeeResource(2);

            for (float k = 0; k < 6.28f; k += 0.1f)
            {
                float x = (float)Math.Cos(k) * 60;
                float y = (float)Math.Sin(k) * 20;

                Dust.NewDustPerfect(position, DustID.Torch, new Vector2(x, y).RotatedBy(velocity.ToRotation() + MathHelper.PiOver2) * 0.035f, 0, default, 2.15f).noGravity = true;

                Dust.NewDustPerfect(position + velocity, DustID.Torch, new Vector2(x, y).RotatedBy(velocity.ToRotation() + MathHelper.PiOver2) * 0.05f, 0, default, 2f).noGravity = true;
            }

            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.HoneyDustSolid>(), velocity.RotatedByRandom(0.45f) * Main.rand.NextFloat(0.5f), 0, default, 1.25f).noGravity = true;

                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowFastDecelerate>(), velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(0.45f), 0, new Color(163, 97, 66), 0.55f);

                Dust.NewDustPerfect(position, DustID.Torch, velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(0.45f), 0, default, 2.35f).noGravity = true;
            }

            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.ExplosionDust>(), velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(0.1f, 1f), 50 + Main.rand.Next(100), default, 1f).rotation = Main.rand.NextFloat(6.28f);

                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.ExplosionDustTwo>(), velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(0.1f, 1f), 80 + Main.rand.Next(100), default, 1f).rotation = Main.rand.NextFloat(6.28f);

                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.ExplosionDustThree>(), velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(0.1f, 1f), 80 + Main.rand.Next(100), default, 1f).rotation = Main.rand.NextFloat(6.28f);

                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.ExplosionDustFour>(), velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(0.1f, 1f), 50 + Main.rand.Next(100), default, 1f).rotation = Main.rand.NextFloat(6.28f);
            }

            return true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient(ModContent.ItemType<BeenadeLauncher>()).
                AddIngredient(ItemID.HallowedBar, 10).
                AddIngredient(ItemID.Hive, 10).
                AddIngredient(ItemID.BottledHoney, 25).
                AddIngredient(ModContent.ItemType<Pollen>(), 15).
                AddTile(TileID.MythrilAnvil).Register();
        }
    }
}