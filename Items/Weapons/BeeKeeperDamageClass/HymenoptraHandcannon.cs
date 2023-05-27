using BombusApisBee.BeeDamageClass;
using BombusApisBee.Items.Other.Crafting;
using BombusApisBee.Projectiles;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class HymenoptraHandcannon : BeeDamageItem
    {
        public int delay;
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Fires a burst of 6 bee bullets\n'Fear the swarm!'");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 38;
            Item.width = 50;
            Item.height = 36;
            Item.useTime = 5;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = Item.sellPrice(0, 11, 75, 25);
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<BeeBullet>();
            Item.shootSpeed = 18.5f;
            beeResourceCost = 10;
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
                position += muzzleOffset;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Terraria.Audio.SoundEngine.PlaySound(new Terraria.Audio.SoundStyle("BombusApisBee/Sounds/Item/HandCannon") with { Volume = 0.75f, PitchVariance = 0.15f }, position);
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowFastDecelerate>(), velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(0.5f), 0, new Color(255, 220, 110, 50), 0.3f);

                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowFastDecelerate>(), velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(0.25f), 0, new Color(255, 255, 255, 50), 0.3f);
            }
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.HoneyDust>(), velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.35f)).noGravity = true;
            }
            if (++delay >= 6)
            {
                delay = 0;
                player.reuseDelay = 60;
            }
            player.Hymenoptra().BeeResourceRegenTimer = -120;
            Gore.NewGorePerfect(source, player.Center + Vector2.Normalize(velocity) * 5f, velocity * -0.25f + (Vector2.UnitY * -3f), Mod.Find<ModGore>("ShellEjectGore").Type).timeLeft = 90;
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.HallowedBar, 10).AddIngredient(ItemID.Handgun).AddIngredient(ItemID.SoulofSight).AddIngredient<Pollen>(30).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
