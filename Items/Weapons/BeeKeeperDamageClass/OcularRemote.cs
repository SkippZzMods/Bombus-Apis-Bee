using BombusApisBee.BeeDamageClass;
using BombusApisBee.Projectiles;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class OcularRemote : BeeDamageItem
    {
        public override bool SafeCanUseItem(Player player) => player.ownedProjectileCounts<SpazHoneycomb>() <= 0 && player.ownedProjectileCounts<RetinaHoneycomb>() <= 0;
        public override void Load()
        {
            On.Terraria.Player.UpdateEquips += Player_UpdateEquips;
        }

        private void Player_UpdateEquips(On.Terraria.Player.orig_UpdateEquips orig, Player self, int i)
        {
            orig(self, i);

            if (self.ownedProjectileCounts<RetinaHoneycomb>() > 0)
                self.Hymenoptra().BeeResourceReserved += 25;

            if (self.ownedProjectileCounts<SpazHoneycomb>() > 0)
                self.Hymenoptra().BeeResourceReserved += 25;
        }

        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Calls upon the Retinacomb and Spazacomb to temporarily fight for you\nOnly one pair of twins can be alive at once\nEach twin reserves 25 Honey while alive\n<right> while holding the remote to self destruct them, with a cooldown\n'Good thing the Twins don't hold a grudge'");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 65;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 75;
            Item.useAnimation = 75;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.knockBack = 5f;
            Item.value = Item.sellPrice(0, 4, 50, 0);
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SpazHoneycomb>();
            Item.shootSpeed = 13;
            Item.UseSound = SoundID.Item44;
            beeResourceCost = 10;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, Main.MouseWorld, Main.rand.NextVector2Circular(5f, 5f), type, damage, knockback, player.whoAmI);

            Projectile.NewProjectile(source, Main.MouseWorld, Main.rand.NextVector2Circular(5f, 5f), ModContent.ProjectileType<RetinaHoneycomb>(), damage, knockback, player.whoAmI);
            return false;
        }
    }
}