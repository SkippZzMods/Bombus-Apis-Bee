﻿using BombusApisBee.Items.Other.Crafting;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class HellfireBeemstick : BeeDamageItem
    {
        public float shootRotation;
        public int shootDirection;
        public bool spawnedGore;
        public int cooldown;
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Blasts a hellish spread of bee buckshot and bees, inflicting Hellfire\nPress <right> to fire a hellfire slug, exploding on contact");
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 16;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 90;
            Item.useAnimation = 90;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2.5f;
            Item.value = Item.sellPrice(gold: 3, silver: 50);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<HellfireBee>();
            Item.shootSpeed = 6f;
            Item.UseSound = new Terraria.Audio.SoundStyle("BombusApisBee/Sounds/Item/HeavyShotgun") with { Volume = 0.75f, Pitch = -0.15f };
            Item.scale = 1;
            beeResourceCost = 7;
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

        public override void UpdateInventory(Player player)
        {
            if (cooldown > 0)
                cooldown--;
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

        public override bool AltFunctionUse(Player player)
        {
            return cooldown <= 0;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            float animProgress = 1f - ((float)player.itemTime / (float)player.itemTimeMax);

            if (Main.myPlayer == player.whoAmI)
                player.direction = shootDirection;

            float itemRotation = player.compositeFrontArm.rotation + 1.5707964f * player.gravDir;
            Vector2 itemPosition = player.MountedCenter;

            if (animProgress < 0.55f)
            {
                float lerper = animProgress / 0.55f;
                itemPosition += itemRotation.ToRotationVector2() * MathHelper.Lerp(-3f, 7f, EaseBuilder.EaseCircularInOut.Ease(lerper));
            }
            else
            {
                if (animProgress < 0.75f)
                {
                    float lerper = (animProgress - 0.55f) / 0.2f;
                    itemPosition += itemRotation.ToRotationVector2() * MathHelper.Lerp(7f, -4f, EaseBuilder.EaseQuinticInOut.Ease(lerper));
                }
                else
                {
                    float lerper = (animProgress - 0.75f) / 0.25f;
                    itemPosition += itemRotation.ToRotationVector2() * MathHelper.Lerp(-4f, 6f, EaseBuilder.EaseQuinticInOut.Ease(lerper));
                }
            }

            Vector2 itemSize = new Vector2(44f, 18f);
            Vector2 itemOrigin = new Vector2(-18f, 1f);

            Vector2 dustPos = itemPosition + new Vector2(32.5f, -5f * player.direction).RotatedBy(player.compositeFrontArm.rotation + 1.5707964f * player.gravDir);

            if (Main.rand.NextBool(2))
                Dust.NewDustPerfect(dustPos, 54, Vector2.UnitY * -2f, (int)MathHelper.Lerp(175f, 250f, animProgress), default, 1.25f).noGravity = true;

            if (Main.rand.NextBool(20))
                Dust.NewDustPerfect(dustPos, ModContent.DustType<HellfireBeemstickSmokeDust>(), Vector2.UnitY * -2f, 100, default, 0.03f).noGravity = true;

            if (Main.rand.NextBool(10))
                Dust.NewDustPerfect(dustPos, ModContent.DustType<GlowFastDecelerate>(), Vector2.UnitY * -2f, (int)MathHelper.Lerp(100f, 200f, animProgress), new Color(200, 75, 20), 0.5f).noGravity = true;


            BeeUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, new Vector2?(itemOrigin), false, false, true);
        }

        public override void UseItemFrame(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                player.direction = shootDirection;

            float animProgress = 1f - ((float)player.itemTime / (float)player.itemTimeMax);
            float rotation = shootRotation * player.gravDir + 1.5707964f;
            if (animProgress < 0.55f)
            {
                if (animProgress < 0.1f)
                {
                    float lerper = animProgress / 0.1f;
                    rotation += MathHelper.Lerp(0f, -0.65f, EaseBuilder.EaseCircularOut.Ease(lerper)) * player.direction;
                }
                else
                {
                    float lerper = (animProgress - 0.1f) / 0.45f;
                    rotation += MathHelper.Lerp(-0.65f, 0, EaseBuilder.EaseCircularInOut.Ease(lerper)) * player.direction;
                }             
            }
            else
            {
                if (animProgress < 0.75f)
                {
                    float lerper = (animProgress - 0.55f) / 0.2f;
                    rotation += MathHelper.Lerp(0f, 0.25f, EaseBuilder.EaseCircularInOut.Ease(lerper)) * player.direction;
                    if (animProgress >= 0.6f && !spawnedGore)
                    {
                        spawnedGore = true;

                        Vector2 pos = player.MountedCenter + (rotation + 1.5707964f * player.gravDir).ToRotationVector2() * 5f;

                        Gore.NewGorePerfect(null, pos, -(rotation + 1.5707964f * player.gravDir).ToRotationVector2() * 3f + Vector2.UnitY * -2f, Mod.Find<ModGore>("HoneyShotgunShell").Type);
                        new SoundStyle("BombusApisBee/Sounds/Item/PlinkLever").PlayWith(player.Center, 0f, 0f, 1f);

                        for (int i = 0; i < 10; i++)
                        {
                            Dust.NewDustPerfect(pos, ModContent.DustType<GlowFastDecelerate>(), (-(rotation + 1.5707964f * player.gravDir).ToRotationVector2() * 3f + Vector2.UnitY * -2f).RotatedByRandom(0.5f) * Main.rand.NextFloat(0.35f, 0.65f), 0, new Color(200, 75, 20), 0.45f);

                            Dust.NewDustPerfect(pos, DustID.Honey2, (-(rotation + 1.5707964f * player.gravDir).ToRotationVector2() * 3f + Vector2.UnitY * -2f).RotatedByRandom(0.1f) * Main.rand.NextFloat(0.5f, 1f), 150, default, 1f).noGravity = false;

                            Dust.NewDustPerfect(pos, DustID.Torch, (-(rotation + 1.5707964f * player.gravDir).ToRotationVector2() * 3f + Vector2.UnitY * -2f).RotatedByRandom(0.1f) * Main.rand.NextFloat(0.5f, 1f), 150, default, 1f).noGravity = false;

                            Dust.NewDustPerfect(pos, 54, (Vector2.UnitY * -2f).RotatedByRandom(1f) * Main.rand.NextFloat(1.5f), 200, default, 1.55f).noGravity = true;
                        }
                    }
                }
                else
                {
                    float lerper = (animProgress - 0.75f) / 0.25f;
                    rotation += MathHelper.Lerp(0.25f, 0.05f, EaseBuilder.EaseCircularInOut.Ease(lerper)) * player.direction;
                }
            }

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            spawnedGore = false;

            if (player.altFunctionUse == 2 && cooldown <= 0)
            {
                Projectile.NewProjectileDirect(source, position, velocity * 2.5f, ModContent.ProjectileType<HellfireBeemstickSlug>(), damage * 2, knockback * 3.5f, player.whoAmI);

                cooldown = 5;
            }
            else
            {
                Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<HellfireBeeBuckshot>(), damage, knockback * 3.5f, player.whoAmI);

                for (int i = 0; i < Main.rand.Next(2, 4); i++)
                {
                    Projectile.NewProjectileDirect(source, position, velocity.RotatedByRandom(12f.AsRadians()), ModContent.ProjectileType<HellfireBee>(), player.beeDamage((int)(damage * 0.65f)),
                        player.beeKB(1f), player.whoAmI).DamageType = BeeUtils.BeeDamageClass();
                }

                for (int i = 0; i < Main.rand.Next(3, 6); i++)
                {
                    Projectile.NewProjectileDirect(source, position, velocity.RotatedByRandom(8f.AsRadians()), ModContent.ProjectileType<HellfireBeeBuckshot>(), damage, knockback * 3.5f, player.whoAmI);
                }
            }

            player.Bombus().AddShake(13);

            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(position, ModContent.DustType<GlowFastDecelerate>(), velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(2f), 0, new Color(200, 75, 20), 0.65f);

                Dust.NewDustPerfect(position, DustID.Torch, velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(2f), 0, default, 2.5f).noGravity = true;

                Dust.NewDustPerfect(position, DustID.Honey2, velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(2f), 150, default, 1.5f).noGravity = true;
            }

            Dust dust = Dust.NewDustPerfect(position, ModContent.DustType<HellfireBeemstickSmokeDust>(), Main.rand.NextVector2Circular(1f, 1f) + -velocity * 0.35f + Vector2.UnitY * -1.5f, 50, default, 0.1f);
            dust.rotation = Main.rand.NextFloat(6.28f);

            dust = Dust.NewDustPerfect(position, ModContent.DustType<HellfireBeemstickSmokeDust>(), Vector2.UnitY * -1.5f, 50, default, 0.1f);
            dust.rotation = Main.rand.NextFloat(6.28f);

            dust = Dust.NewDustPerfect(position, ModContent.DustType<HellfireBeemstickMuzzleFlashDust>(), Vector2.Zero, 0, default, 1f);

            dust.rotation = velocity.ToRotation();
            dust.customData = player;

            player.velocity += velocity * -1.45f;
            shootRotation = (player.Center - Main.MouseWorld).ToRotation();
            shootDirection = (Main.MouseWorld.X < player.Center.X) ? -1 : 1;

            return false;
        }
    }

    public class HellfireBeemstickSmokeDust : ModDust
    {
        public override string Texture => BombusApisBee.Invisible;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, 0, 4, 4);
            dust.customData = 1 + Main.rand.Next(3);
        }

        public override bool Update(Dust dust)
        {
            dust.position.Y -= 0.025f;
            dust.position += dust.velocity;
            dust.velocity *= 0.99f;
            dust.rotation += dust.velocity.Length() * 0.01f;

            dust.alpha += 10;

            dust.alpha = (int)(dust.alpha * 1.01f);

            dust.scale *= 1.01f;

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            float lerper = 1f - dust.alpha / 255f;

            Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/SmokeTransparent_" + dust.customData).Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, Color.Black * lerper, dust.rotation, tex.Size() / 2f, dust.scale, 0f, 0f);

            Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, Color.Black * lerper, dust.rotation + MathHelper.PiOver2, tex.Size() / 2f, dust.scale, 0f, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }

    class HellfireBeemstickMuzzleFlashDust : ModDust
    {
        public override string Texture => BombusApisBee.Invisible;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, 0, 4, 4);
        }

        public override bool Update(Dust dust)
        {
            var player = dust.customData as Player;

            float itemRotation = player.compositeFrontArm.rotation + 1.5707964f * player.gravDir;
            Vector2 itemPosition = player.MountedCenter;

            dust.position = (itemPosition + itemRotation.ToRotationVector2() * 10f) + new Vector2(32.5f, -5f * player.direction).RotatedBy(player.compositeFrontArm.rotation + 1.5707964f * player.gravDir);
            dust.rotation = itemRotation;

            dust.alpha += 15;
            dust.alpha = (int)(dust.alpha * 1.01f);

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            float lerper = 1f - dust.alpha / 255f;

            Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/HellShotgunMuzzleFlash").Value;
            Texture2D texBlur = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/HellShotgunMuzzleFlash_Blur").Value;
            Texture2D texGlow = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/HellShotgunMuzzleFlash_Glow").Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Main.spriteBatch.Draw(bloomTex, dust.position - Main.screenPosition, null, new Color(255, 100, 50, 0) * 0.5f * lerper, dust.rotation, bloomTex.Size() / 2f, 1.25f, 0f, 0f);

            Main.spriteBatch.Draw(texGlow, dust.position - Main.screenPosition, null, new Color(255, 50, 50, 0) * lerper, dust.rotation, texGlow.Size() / 2f, 1f, 0f, 0f);

            Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, Color.White * lerper, dust.rotation, tex.Size() / 2f, 1f, 0f, 0f);

            Main.spriteBatch.Draw(texBlur, dust.position - Main.screenPosition, null, Color.White with { A = 0 } * 0.5f * lerper, dust.rotation, texBlur.Size() / 2f, 1f, 0f, 0f);

            return false;
        }
    }
}