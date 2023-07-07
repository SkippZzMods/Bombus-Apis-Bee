using BombusApisBee.Items.Other.Crafting;
using Terraria.DataStructures;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class Beemstick : BeeDamageItem
    {
        public float shootRotation;
        public int shootDirection;
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Blasts a spread of bee buckshot and bees\n'This... is my BEEMSTICK'");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 11;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 90;
            Item.useAnimation = 90;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(gold: 2, silver: 50);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = false;
            Item.shoot = ProjectileID.Bee;
            Item.shootSpeed = 6;
            Item.UseSound = new Terraria.Audio.SoundStyle("BombusApisBee/Sounds/Item/HeavyShotgun") with { Volume = 0.6f, PitchVariance = 0.1f };
            Item.scale = 1;
            beeResourceCost = 5;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient(ItemID.Boomstick, 1).
                AddIngredient(ModContent.ItemType<Pollen>(), 15).
                AddIngredient(ItemID.BottledHoney, 5).
                AddTile(TileID.Anvils).
                Register();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(2, 0);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 30f;
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

            float itemRotation = player.compositeFrontArm.rotation + 1.5707964f * player.gravDir;
            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * MathHelper.Lerp(0f, 7f, EaseBuilder.EaseQuinticInOut.Ease(animProgress));
            Vector2 itemSize = new Vector2(44f, 18f);

            Vector2 itemOrigin = new Vector2(-18f, 1f);
            BeeUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, new Vector2?(itemOrigin), false, false, true);
        }

        public override void UseItemFrame(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                player.direction = shootDirection;

            float animProgress = 1f - ((float)player.itemTime / (float)player.itemTimeMax);
            float rotation = shootRotation * player.gravDir + 1.5707964f;
            if (animProgress < 0.5f)
                rotation += MathHelper.Lerp(-0.5f, 0, EaseBuilder.EaseQuinticInOut.Ease(animProgress) * 2) * player.direction;

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var modPlayer2 = player.GetModPlayer<BombusApisBeePlayer>();
            modPlayer2.AddShake(11);
            for (int i = 0; i < Main.rand.Next(1, 3); i++)
            {
                BeeUtils.SpawnBee(player, source, position, velocity.RotatedByRandom(15f.AsRadians()), damage, knockback);
            }
            for (int i = 0; i < Main.rand.Next(2, 4); i++)
            {
                Projectile.NewProjectileDirect(source, position, velocity.RotatedByRandom(10f.AsRadians()), ModContent.ProjectileType<BeeBuckshot>(), damage / 2, knockback * 3, player.whoAmI);
            }
            for (int i = 0; i < 3; i++)
            {
                Vector2 velo = velocity.RotatedByRandom(5f.AsRadians()) * Main.rand.NextFloat(0.6f, 0.85f) + (Vector2.UnitY * -2.25f);
                Dust.NewDustDirect(position, 1, 1, ModContent.DustType<Dusts.SmokeDustColor>(), velo.X, velo.Y, 50 + Main.rand.Next(50), new Color(215, 160, 80), Main.rand.NextFloat(0.7f, 0.95f));

                Dust.NewDustDirect(position, 1, 1, ModContent.DustType<Dusts.SmokeDustColor>(), velo.X, velo.Y, 50 + Main.rand.Next(50), new Color(255, 220, 110), Main.rand.NextFloat(0.7f, 0.95f));
            }
            float rot = velocity.ToRotation();
            float spread = 0.3f;
            Vector2 offset = new Vector2(1, -0.05f * player.direction).RotatedBy(rot);
            for (int i = 0; i < 15; i++)
            {
                var direction = offset.RotatedByRandom(spread);
                Vector2 velo = direction * Main.rand.NextFloat(10);
                Dust.NewDustDirect(position, 1, 1, ModContent.DustType<Dusts.Glow>(), velo.X, velo.Y, 80, new Color(255, 220, 110), Main.rand.NextFloat(0.2f, 0.5f));
                Vector2 velo2 = velocity.RotatedByRandom(7f.AsRadians()) * Main.rand.NextFloat(0.2f, 0.6f);
                Dust.NewDustDirect(position, 1, 1, DustID.Honey2, velo2.X * 3f, velo2.Y * 3f, 0, default, Main.rand.NextFloat(1.25f, 1.65f)).noGravity = true;
                Vector2 velo3 = velocity.RotatedByRandom(7f.AsRadians()) * Main.rand.NextFloat(0.1f, 0.3f);
                Dust.NewDustDirect(position, 1, 1, ModContent.DustType<Dusts.HoneyDust>(), velo3.X * 3f, velo3.Y * 3f, 0, default, Main.rand.NextFloat(1.25f, 1.65f)).noGravity = true;
            }
            player.velocity += velocity * -1;

            shootRotation = (player.Center - Main.MouseWorld).ToRotation();
            shootDirection = (Main.MouseWorld.X < player.Center.X) ? -1 : 1;
            return false;
        }
    }
}