
using BombusApisBee.BeeDamageClass;
using BombusApisBee.Projectiles;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class TheLaserBeem : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Shoots a massive concentration of honey\nSpawns a flurry of bees on hit");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 55;
            Item.noMelee = true;
            Item.channel = true; //Channel so that you can hold the weapon [Important]

            Item.rare = ItemRarityID.Yellow;
            Item.width = 28;
            Item.height = 30;
            Item.useTime = 20;
            Item.UseSound = SoundID.Item13;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shootSpeed = 14f;
            Item.useAnimation = 20;
            Item.shoot = ModContent.ProjectileType<LaserBeem>();
            Item.value = Item.sellPrice(gold: 7);
            beeResourceCost = 5;
            ResourceChance = 0.25f;
        }

        public override bool SafeCanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<LaserBeem>()] < 1;
        }
    }
}