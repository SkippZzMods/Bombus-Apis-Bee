using BombusApisBee.Items.Other.Crafting;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class HoneyFlareGun : BeeDamageItem
    {
        public float shootRotation;
        public int shootDirection;
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Fires honey flares which stick into enemies and explode into homing honey and honey clouds\nThe honey clouds grant the user the Honey Buff when they are in the cloud");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 11;
            Item.noMelee = true;
            Item.width = 25;
            Item.height = 25;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(0, 1, 25, 0);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<HoneyFlare>();
            Item.shootSpeed = 15;
            Item.UseSound = SoundID.Item11;

            beeResourceCost = 4;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.FlareGun, 1).AddIngredient(ItemID.BottledHoney, 5).AddIngredient(ModContent.ItemType<Pollen>(), 20).AddTile(TileID.Anvils).Register();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 35f;
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

            if (animProgress < 0.2f)
            {
                float lerper = animProgress / 0.2f;
                itemPosition += itemRotation.ToRotationVector2() * MathHelper.Lerp(0f, -5f, EaseBuilder.EaseCircularOut.Ease(lerper));
            }
            else
            {
                float lerper = (animProgress - 0.2f) / 0.8f;
                itemPosition += itemRotation.ToRotationVector2() * MathHelper.Lerp(-5f, 0f, EaseBuilder.EaseBackInOut.Ease(lerper));
            }

            Vector2 itemSize = new Vector2(34f, 24f);
            Vector2 itemOrigin = new Vector2(-35f, 5f);

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
                rotation += MathHelper.Lerp(0f, -.5f, EaseBuilder.EaseCircularOut.Ease(lerper)) * player.direction;
            }
            else
            {
                float lerper = (animProgress - 0.15f) / 0.85f;
                rotation += MathHelper.Lerp(-.5f, 0, EaseBuilder.EaseBackInOut.Ease(lerper)) * player.direction;
            }

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 visualPosition = position + new Vector2(0, -5 * player.direction).RotatedBy(velocity.ToRotation());

            Projectile.NewProjectile(source, visualPosition, velocity, type, damage, knockback, player.whoAmI);

            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(visualPosition + (Vector2.UnitY.RotatedBy(velocity.ToRotation()) * -5f) * player.direction, ModContent.DustType<Dusts.HoneyDust>(), velocity.RotatedByRandom(0.45f) * Main.rand.NextFloat(0.25f), Main.rand.Next(100)).noGravity = true;
                Dust.NewDustPerfect(visualPosition + (Vector2.UnitY.RotatedBy(velocity.ToRotation()) * -5f) * player.direction, DustID.Honey2, velocity.RotatedByRandom(0.45f) * Main.rand.NextFloat(0.25f), Main.rand.Next(100)).noGravity = true;
            }

            shootRotation = (player.Center - Main.MouseWorld).ToRotation();
            shootDirection = (Main.MouseWorld.X < player.Center.X) ? -1 : 1;

            return false;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-11, 0);
        }
    }
}
