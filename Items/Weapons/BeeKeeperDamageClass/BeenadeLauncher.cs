using BombusApisBee.Items.Other.Crafting;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class BeenadeLauncher : BeeDamageItem
    {
        public float shootRotation;
        public int shootDirection;
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Fires bouncing pipebeeoms full of bees\n'Oh they're goin to hav' to glue you back togetha'\n'IN HELL'");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 31;
            Item.noMelee = true;
            Item.width = 50;
            Item.height = 20;
            Item.useTime = 65;
            Item.useAnimation = 65;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(0, 3, 25, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BeenadeLauncherProjectile>();
            Item.shootSpeed = 16f;
            Item.UseSound = SoundID.Item61;
            Item.scale = 1;
            Item.crit = 4;
            honeyCost = 5;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 65f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                position += muzzleOffset;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            float animProgress = 1f - ((float)player.itemTime / (float)player.itemTimeMax);

            if (Main.myPlayer == player.whoAmI)
                player.direction = shootDirection;

            float itemRotation = player.compositeFrontArm.rotation + 1.5707964f * player.gravDir;
            Vector2 itemPosition = player.MountedCenter;

            if (animProgress < 0.3f)
            {
                float lerper = animProgress / 0.3f;
                itemPosition += itemRotation.ToRotationVector2() * MathHelper.Lerp(0f, -10f, EaseBuilder.EaseQuinticOut.Ease(lerper));
            }
            else
            {
                float lerper = (animProgress - 0.3f) / 0.7f;
                itemPosition += itemRotation.ToRotationVector2() * MathHelper.Lerp(-10f, 0f, EaseBuilder.EaseBackInOut.Ease(lerper));
            }

            Vector2 itemSize = new Vector2(82f, 28f);
            Vector2 itemOrigin = new Vector2(-30f, 10f);

            BeeUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, new Vector2?(itemOrigin), false, false, true);
        }

        public override void UseItemFrame(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                player.direction = shootDirection;

            float animProgress = 1f - ((float)player.itemTime / (float)player.itemTimeMax);
            float rotation = shootRotation * player.gravDir + 1.5707964f;

            if (animProgress < 0.15f)
            {
                float lerper = animProgress / 0.15f;
                rotation += MathHelper.Lerp(0f, -0.75f, EaseBuilder.EaseQuinticOut.Ease(lerper)) * player.direction;
            }
            else
            {
                float lerper = (animProgress - 0.15f) / 0.85f;
                rotation += MathHelper.Lerp(-0.75f, 0, EaseBuilder.EaseBackInOut.Ease(lerper)) * player.direction;
            }

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var modPlayer2 = player.GetModPlayer<BombusApisBeePlayer>();
            modPlayer2.AddShake(5);

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

            Vector2 visualPosition = position + new Vector2(0, -15 * player.direction).RotatedBy(velocity.ToRotation());

            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(visualPosition, ModContent.DustType<Dusts.ImpactLineDust>(), velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.2f, 1f), 0, new Color(255, 55, 20, 0), 0.12f);

                Dust.NewDustPerfect(visualPosition, ModContent.DustType<Dusts.GlowFastDecelerate>(), velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.2f, 0.5f), 0, new Color(255, 55, 20), 0.5f);
            }

            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustPerfect(visualPosition, DustID.Honey2, velocity.RotatedByRandom(1f) * Main.rand.NextFloat(0.45f), Main.rand.Next(150), default, 1.2f).noGravity = true;
            }

            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustPerfect(visualPosition, ModContent.DustType<HellfireBeemstickSmokeDust>(), Main.rand.NextVector2Circular(2f, 2f), 50, default, 0.1f);
                dust.rotation = Main.rand.NextFloat(6.28f);

                dust = Dust.NewDustPerfect(visualPosition, ModContent.DustType<HellfireBeemstickSmokeDust>(), Vector2.UnitY * -1.5f, 50, default, 0.1f);
                dust.rotation = Main.rand.NextFloat(6.28f);
            }


            shootRotation = (player.Center - Main.MouseWorld).ToRotation();
            shootDirection = (Main.MouseWorld.X < player.Center.X) ? -1 : 1;
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient(ItemID.HellstoneBar, 5).
                AddIngredient(ItemID.Beenade, 15).
                AddIngredient(ItemID.IronBar, 10).
                AddIngredient(ItemID.IllegalGunParts, 1).
                AddIngredient(ModContent.ItemType<Pollen>(), 10).
                AddTile(TileID.Anvils).
                Register();

            CreateRecipe(1).
                AddIngredient(ItemID.HellstoneBar, 5).
                AddIngredient(ItemID.Beenade, 15).
                AddIngredient(ItemID.LeadBar, 10).
                AddIngredient(ItemID.IllegalGunParts, 1).
                AddIngredient(ModContent.ItemType<Pollen>(), 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}