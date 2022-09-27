using BombusApisBee.BeeDamageClass;
using BombusApisBee.Projectiles;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class TheQueensLarvae : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            // DisplayName.SetDefault("Honeycomb"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("'Once a royal queen, now your royal guard'\nDrains your honey bank on use\nPartially Ignores Immunity Frames\nOnly one Queen can be alive at once");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(10, 7));
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 175;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1f;
            Item.value = Item.value = Item.sellPrice(2, 50, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<TheQueen>();
            Item.shootSpeed = 13;
            Item.UseSound = SoundID.Roar;
            Item.scale = 1.25f;
            Item.crit = 4;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            beeResourceCost = 150;
        }


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3, 0);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var modPlayer2 = player.GetModPlayer<BombusApisBeePlayer>();
            modPlayer2.shakeTimer = 50;
            Projectile.NewProjectile(source, player.Center, velocity, ModContent.ProjectileType<TheQueen>(), damage, 1f, player.whoAmI);
            return false;
        }
        public override bool SafeCanUseItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<TheQueen>()] < 1)
            {
                return true;
            }
            return false;
        }
    }
}