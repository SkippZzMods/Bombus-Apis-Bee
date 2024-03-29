﻿namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class ElectricHoneycomb : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Throws an electric-charged honeycomb, electrifying all that come near and exploding into zapbees");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 43;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 90;
            Item.useAnimation = 90;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.value = Item.sellPrice(0, 5, 15, 75);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ElectricHoneycombProj>();
            Item.shootSpeed = 15f;
            Item.UseSound = SoundID.Item1;
            honeyCost = 5;
            Item.noUseGraphic = true;
        }
    }
}