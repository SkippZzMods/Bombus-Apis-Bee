using BombusApisBee.BeeDamageClass;
using BombusApisBee.Projectiles;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class TheQueensLarvae : BeeDamageItem
    {
        public override void Load()
        {
            On.Terraria.Player.UpdateEquips += Player_UpdateEquips;
        }

        private void Player_UpdateEquips(On.Terraria.Player.orig_UpdateEquips orig, Player self, int i)
        {
            orig(self, i);

            if (self.ownedProjectileCounts<TheQueen>() > 0)
                self.Hymenoptra().BeeResourceReserved += (int)(self.Hymenoptra().BeeResourceMax2 * 0.65f);
        }
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Drains your honey on use\nReserves 65% of your honey while alive\n'Once a royal queen, now your royal guard'");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(10, 7));
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 412;

            Item.width = 40;
            Item.height = 20;

            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.HoldUp;

            Item.knockBack = 1f;
            Item.value = Item.value = Item.sellPrice(0, 25, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<TheQueen>();

            Item.shootSpeed = 13;
            Item.UseSound = SoundID.Roar;

            Item.noMelee = true;
            beeResourceCost = 5;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.Bombus().shakeTimer += 20;
            player.Hymenoptra().BeeResourceCurrent = 0;
            player.Hymenoptra().BeeResourceRegenTimer = -600;
            player.AddBuff<Buffs.TheQueensGuard>(120);
            Projectile.NewProjectileDirect(source, Main.MouseWorld, Vector2.One, ModContent.ProjectileType<TheQueen>(), damage, knockback, player.whoAmI);
            for (int i = 0; i < 30; i++)
            {
                Dust.NewDustPerfect(Main.MouseWorld, ModContent.DustType<Dusts.HoneyDust>(), Main.rand.NextVector2Circular(5f, 5f), Main.rand.Next(100, 200), default, 1.45f).noGravity = true;

                Dust.NewDustPerfect(Main.MouseWorld, DustID.Honey2, Main.rand.NextVector2Circular(6f, 6f), Main.rand.Next(50, 150), default, 1.45f).noGravity = true;
            }
            return false;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts<TheQueen>() <= 0 && player.Hymenoptra().BeeResourceCurrent >= player.Hymenoptra().BeeResourceMax2;
        }
    }
}