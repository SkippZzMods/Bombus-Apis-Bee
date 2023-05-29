using BombusApisBee.BeeDamageClass;
using BombusApisBee.Items.Other.Crafting;
using BombusApisBee.Projectiles;
using System;
using Terraria.DataStructures;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class TheStarSwarmer : BeeDamageItem
    {
        int cooldown;
        public override bool AltFunctionUse(Player player) => cooldown <= 0;
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Rapidly fires star bees\nPress <right> to fire a splitting star\nPicking up the star replenishes honey and grants the user a buff to their damage and attack speed");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 30;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 11;
            Item.useAnimation = 11;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.value = Item.sellPrice(0, 4, 50);
            Item.rare = ItemRarityID.LightRed;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<AstralBee>();
            Item.shootSpeed = 6;

            beeResourceCost = 2;
            ResourceChance = 0.33f;
        }

        public override void HoldItem(Player player)
        {
            if (cooldown > 0)
            {
                if (cooldown == 1)
                {
                    SoundID.MaxMana.PlayWith(player.Center);
                    BeeUtils.DrawStar(player.GetArmPosition(), ModContent.DustType<Dusts.GlowFastDecelerate>(), 0, new Color(181, 127, 207), true, 5, 1.25f, 1f, 0.4f, 0.3f);
                }
                cooldown--;
            }
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-20, 0);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse != 2)
                velocity = velocity.RotatedByRandom(5.AsRadians());

            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 55f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Vector2 pos = position + new Vector2(0, -5 * player.direction).RotatedBy(velocity.ToRotation());

                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.GlowFastDecelerate>(), velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(2f), 0, new Color(181, 127, 207), 0.45f);

                    Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.GlowFastDecelerate>(), velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(1f), 0, new Color(112, 83, 163), 0.4f);
                }
                for (int i = 0; i < 12; i++)
                {
                    Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.Stardust>(), velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(1.25f), 0, default, 1.15f);
                }

                for (float k = 0; k < 6.28f; k += 0.1f)
                {
                    float x = (float)Math.Cos(k) * 50;
                    float y = (float)Math.Sin(k) * 25;

                    Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.GlowFastDecelerate>(), new Vector2(x, y).RotatedBy(velocity.ToRotation() + MathHelper.PiOver2) * 0.05f, 0, new Color(112, 83, 163), 0.45f);
                }

                Projectile.NewProjectile(source, pos, velocity * 2.5f, ModContent.ProjectileType<AstralStarSplitting>(), (int)(damage * 1.5f), 3.5f, player.whoAmI);

                player.UseBeeResource(4);
                player.Bombus().AddShake(5);
                player.velocity += -velocity;
                player.reuseDelay = 30;
                new SoundStyle("BombusApisBee/Sounds/Item/ProjectileLaunch1").PlayWith(position, -0.1f, 0.1f, 1.15f);
                cooldown = 120;
                return false;
            }
            else
            {
                SoundID.Item11.PlayWith(position, -0.15f, 0.1f);
                player.Bombus().AddShake(2);
                Vector2 pos = position + new Vector2(0, -5 * player.direction).RotatedBy(velocity.ToRotation());
                for (int i = 0; i < 15; i++)
                {
                    Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.GlowFastDecelerate>(), velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(2f), 0, new Color(181, 127, 207), 0.45f);

                    Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.GlowFastDecelerate>(), velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(1f), 0, new Color(107, 172, 255), 0.4f);
                }
                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.Stardust>(), velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(2.25f), 0, default, 1.15f);
                }
            }
            return true;
        }
        public override void AddRecipes()
        {

            CreateRecipe(1).AddIngredient(ModContent.ItemType<TheStarStrap>()).
                AddIngredient(ItemID.SoulofLight, 8).
                AddIngredient(ItemID.TitaniumBar, 12).
                AddIngredient(ModContent.ItemType<Pollen>(), 15).
                AddTile(TileID.MythrilAnvil).
                Register();

            CreateRecipe(1).
                AddIngredient(ModContent.ItemType<TheStarStrap>()).
                AddIngredient(ItemID.SoulofLight, 8).
                AddIngredient(ItemID.AdamantiteBar, 12).
                AddIngredient(ModContent.ItemType<Pollen>(), 15).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}