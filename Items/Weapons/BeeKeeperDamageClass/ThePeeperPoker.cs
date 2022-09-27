using BombusApisBee.BeeDamageClass;
using BombusApisBee.Projectiles;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class ThePeeperPoker : BeeDamageItem
    {
        public override string Texture => "BombusApisBee/Projectiles/PeeperPokerHoldout";
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Rapidly spins an eyeball infested javelin around the player\nRelease the mouse button to throw the javelin\nThe javelin travels faster, does more damage, and pierces more the longer you spin the javelin\nSpawns Cthulubees whilst spinning the javelin, and whilst the javelin is travelling");
        }

        public override void SafeSetDefaults()
        {
            Item.useAnimation = Item.useTime = 30;
            Item.damage = 25;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4f;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<PeeperPokerHoldout>();
            Item.shootSpeed = 1f;
            Item.scale = 1f;
            Item.crit = 4;
            beeResourceCost = 1;
            Item.noUseGraphic = true;
            Item.channel = true;
        }
    }
}
