using BombusApisBee.Content.Hell.Items.HellcombShard;
using Terraria;
using Terraria.DataStructures;

namespace BombusApisBee.Content.Forest.Items.Testing
{
    public class TESTWEAPON : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Test Honeycomb");
            Tooltip.SetDefault("TEST DONT USE");

            SHOWCASEMODE = true;
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 1;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1f;
            Item.value = 10000;
            Item.rare = ItemRarityID.Gray;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ProjectileType<HellcombShardExplosion>();
            Item.shootSpeed = 10;
            Item.UseSound = SoundID.Item11;
            Item.scale = 1;
            Item.crit = 4;
            SHOWCASEMODE = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.Hymenoptra().SHOWCASEMODETIMER = 0;
            Vector2 pos = Main.MouseWorld;
            return false;
        }

        public override void HoldItem(Player player)
        {
            player.Hymenoptra().SHOWCASEMODETIMER = 6000;
            player.Hymenoptra().BeeResourceReserved = 0;
            player.IncreaseBeeCrit(100);
        }
    }
}