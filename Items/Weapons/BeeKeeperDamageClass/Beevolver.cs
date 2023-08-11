/*using BombusApisBee.Items.Other.Crafting;
using Terraria.DataStructures;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class Beevolver : BeeDamageItem
    {
        public float shootRotation;
        public int shootDirection;
        public bool spawnedGore;
        public int chargedTimer;
        public bool rightClicked;
        public float? oldHandRot;
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Press <right> to drain 50% of your honey bank to empower your shots for a short time, causing them to ricochet");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 20;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4f;
            Item.value = Item.sellPrice(gold: 2, silver: 50);

            Item.rare = ItemRarityID.Green;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<BeeBuckshot>();
            Item.shootSpeed = 20f;

            Item.UseSound = new Terraria.Audio.SoundStyle("BombusApisBee/Sounds/Item/HandCannon") with { Volume = 0.6f, PitchVariance = 0.1f };
            beeResourceCost = 3;
        }

        public override bool SafeCanUseItem(Player player)
        {
            shootRotation = (player.Center - Main.MouseWorld).ToRotation();
            shootDirection = (Main.MouseWorld.X < player.Center.X) ? -1 : 1;
            spawnedGore = false;
            rightClicked = player.altFunctionUse == 2;
            oldHandRot = null;

            if (player.altFunctionUse == 2)
            {
                Item.useTime = 120;
                Item.useAnimation = 120;
            }
            else
            {
                Item.useTime = 36;
                Item.useAnimation = 36;
            }

            return true;
        }

        public override void UpdateInventory(Player player)
        {
            if (chargedTimer > 0)
                chargedTimer--;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = new Vector2(36, -5 * player.direction).RotatedBy(velocity.ToRotation());
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            float animProgress = 1f - ((float)player.itemTime / (float)player.itemTimeMax);

            if (Main.myPlayer == player.whoAmI)
                player.direction = shootDirection;

            float itemRotation = 0f;
            Vector2 itemPosition = player.MountedCenter;

            itemRotation = player.compositeFrontArm.rotation + 1.5707964f * player.gravDir;

            if (animProgress < 0.15f)
            {
                float lerper = animProgress / 0.15f;
                itemPosition += itemRotation.ToRotationVector2() * MathHelper.Lerp(0f, -5f, EaseBuilder.EaseCircularOut.Ease(lerper));
            }
            else
            {
                float lerper = (animProgress - 0.15f) / 0.85f;
                itemPosition += itemRotation.ToRotationVector2() * MathHelper.Lerp(-5f, 3f, EaseBuilder.EaseCircularInOut.Ease(lerper));
            }

            Vector2 itemSize = new Vector2(38f, 24f);
            Vector2 itemOrigin = new Vector2(-18f, 1f);

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
                rotation += MathHelper.Lerp(0f, -0.65f, EaseBuilder.EaseCubicOut.Ease(lerper)) * player.direction;
            }
            else
            {
                float lerper = (animProgress - 0.15f) / 0.85f;
                rotation += MathHelper.Lerp(-0.65f, 0f, EaseBuilder.EaseCubicInOut.Ease(lerper)) * player.direction;
            }

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true; //player.Hymenoptra().BeeResourceCurrent >= player.Hymenoptra().BeeResourceMax2 * 0.5f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                //player.Hymenoptra().BeeResourceCurrent -= (int)(player.Hymenoptra().BeeResourceMax2 * 0.5f);
                chargedTimer = 600;
                return false;
            }

            return true;
        }
    }
}*/