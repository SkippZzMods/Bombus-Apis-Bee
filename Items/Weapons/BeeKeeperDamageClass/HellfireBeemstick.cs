using BombusApisBee.BeeDamageClass;
using BombusApisBee.Items.Other.Crafting;
using BombusApisBee.Projectiles;
using System;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class HellfireBeemstick : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Blasts a hellish spread of bee buckshot and bees, inflicting Hellfire");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 19;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 80;
            Item.useAnimation = 80;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2.5f;
            Item.value = Item.sellPrice(gold: 3, silver: 50);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<HellfireBee>();
            Item.shootSpeed = 6f;
            Item.UseSound = new Terraria.Audio.SoundStyle("BombusApisBee/Sounds/Item/HeavyShotgun") with { Volume = 0.75f, Pitch = -0.15f };
            Item.scale = 1;
            beeResourceCost = 5;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient(ModContent.ItemType<Beemstick>()).
                AddIngredient(ItemID.HellstoneBar, 15).
                AddIngredient(ModContent.ItemType<Pollen>(), 15).
                AddIngredient(ItemID.ShadowScale, 5).
                AddTile(TileID.Anvils).
                Register();
            CreateRecipe(1).
                AddIngredient(ModContent.ItemType<Beemstick>()).
                AddIngredient(ItemID.HellstoneBar, 15).
                AddIngredient(ModContent.ItemType<Pollen>(), 15).
                AddIngredient(ItemID.TissueSample, 5).
                AddTile(TileID.Anvils).
                Register();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(2, 0);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 40f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (Main.myPlayer == player.whoAmI)
                player.direction = Math.Sign((Main.MouseWorld - player.Center).X);

            float itemRotation = player.compositeFrontArm.rotation + 1.5707964f * player.gravDir;
            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 7f;
            Vector2 itemSize = new Vector2(44f, 18f);

            Vector2 itemOrigin = new Vector2(-18f, 1f);
            BeeUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, new Vector2?(itemOrigin), false, false, true);
        }

        public override void UseItemFrame(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                player.direction = Math.Sign((Main.MouseWorld - player.Center).X);

            float animProgress = 1f - ((float)player.itemTime / (float)player.itemTimeMax);
            float rotation = (player.Center - Main.MouseWorld).ToRotation() * player.gravDir + 1.5707964f;
            if (animProgress < 0.5f)
                rotation += MathHelper.Lerp(-0.65f, 0, animProgress * 2) * player.direction;

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var modPlayer2 = player.GetModPlayer<BombusApisBeePlayer>();
            modPlayer2.shakeTimer = 13;
            for (int i = 0; i < Main.rand.Next(2, 4); i++)
            {
                Projectile.NewProjectileDirect(source, position, velocity.RotatedByRandom(12f.AsRadians()), ModContent.ProjectileType<HellfireBee>(), player.beeDamage(damage),
                    player.beeKB(1f), player.whoAmI);
            }
            for (int i = 0; i < Main.rand.Next(3, 6); i++)
            {
                Projectile.NewProjectileDirect(source, position, velocity.RotatedByRandom(8f.AsRadians()), ModContent.ProjectileType<HellfireBeeBuckshot>(), damage, knockback * 3.5f, player.whoAmI);
            }
            for (int i = 0; i < 4; i++)
            {
                Vector2 velo = velocity.RotatedByRandom(5f.AsRadians()) * Main.rand.NextFloat(0.6f, 0.85f) + (Vector2.UnitY * -2.25f);
                Dust.NewDustDirect(position, 1, 1, ModContent.DustType<Dusts.SmokeDustColor>(), velo.X, velo.Y, 50 + Main.rand.Next(50), Main.rand.NextBool() ? new Color(255, 225, 45) : new Color(255, 150, 20), Main.rand.NextFloat(0.7f, 0.95f));

                Dust.NewDustDirect(position, 1, 1, ModContent.DustType<Dusts.SmokeDustColor>(), velo.X, velo.Y, 50 + Main.rand.Next(50), Main.rand.NextBool() ? new Color(250, 85, 20) : new Color(195, 40, 20), Main.rand.NextFloat(0.7f, 0.95f));
            }
            float rot = velocity.ToRotation();
            float spread = 0.3f;
            Vector2 offset = new Vector2(1, -0.05f * player.direction).RotatedBy(rot);
            for (int i = 0; i < 15; i++)
            {

                var direction = offset.RotatedByRandom(spread);

                Vector2 velo = direction * Main.rand.NextFloat(10);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowFastDecelerate>(), velo, 55, new Color(195, 40, 20), Main.rand.NextFloat(0.3f, 0.5f));

                Vector2 velo2 = direction * Main.rand.NextFloat(11);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowFastDecelerate>(), velo2, 55, new Color(255, 225, 45), Main.rand.NextFloat(0.3f, 0.5f));

                Vector2 velo3 = velocity.RotatedByRandom(7f.AsRadians()) * Main.rand.NextFloat(2f);
                Dust.NewDustDirect(position, 1, 1, DustID.Torch, velo3.X * 2.5f, velo3.Y * 2.5f, 0, default, Main.rand.NextFloat(1.25f, 1.55f));
            }
            player.velocity += velocity * -1.45f;
            return false;
        }
    }
}