﻿namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class MetalPlatedHoneycomb : BeeDamageItem
    {
        public override bool SafeCanUseItem(Player player) => player.ownedProjectileCounts<HoneycombPrime>() <= 0;
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Hold <left> to channel the power of Honeycomb Prime\n'Initializing proj.Ubersaw'\n'Initializing proj.MetalBee'\n'Initializing proj.DeathLaser'\n'Compiling Honeycomb Prime'");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 31;
            Item.noMelee = true;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 55;
            Item.useAnimation = 55;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1f;
            Item.value = Item.sellPrice(0, 4, 50, 0);
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<HoneycombPrime>();
            Item.shootSpeed = 7f;
            honeyCost = 3;

            Item.channel = true;
            Item.noUseGraphic = true;
        }
    }
}