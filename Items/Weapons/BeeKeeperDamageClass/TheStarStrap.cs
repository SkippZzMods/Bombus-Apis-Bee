using BombusApisBee.Items.Other.Crafting;
using Terraria.DataStructures;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class TheStarStrap : BeeDamageItem
    {
        public int delay;
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("'Stay strapped.'\nFires a burst of star bees, which spawn stars from the heavens on hit");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 15;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 9;
            Item.useAnimation = 27;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.value = Item.sellPrice(0, 1, 50);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<AstralBee>();
            Item.shootSpeed = 6;

            beeResourceCost = 5;
        }


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(20.AsRadians());
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 35f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item11, position);
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowFastDecelerate>(), velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(2f), 0, new Color(157, 127, 207, 100), 0.3f);

                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowFastDecelerate>(), velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(1f), 0, new Color(107, 172, 255, 100), 0.3f);
            }
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.Stardust>(), velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(2f));
            }
            if (++delay >= 3)
            {
                delay = 0;
                player.reuseDelay = 35;
            }
            player.Hymenoptra().BeeResourceRegenTimer = -120;
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.FallenStar, 15).AddIngredient(ItemID.GoldBar, 8).AddIngredient(ItemID.BeeWax, 10).AddIngredient(ModContent.ItemType<Pollen>(), 15).AddTile(TileID.Anvils).Register();

            CreateRecipe(1).AddIngredient(ItemID.FallenStar, 15).AddIngredient(ItemID.PlatinumBar, 8).AddIngredient(ItemID.BeeWax, 10).AddIngredient(ModContent.ItemType<Pollen>(), 15).AddTile(TileID.Anvils).Register();
        }

    }
}