namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class Wasparang : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Press <right> to charge up a returning wasparang\nRelease to throw the wasparang, spawning wasps whilst travelling, and creating homing stingers on hit");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 31;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 100;
            Item.useTime = 100;
            Item.shootSpeed = 3f;
            Item.knockBack = 4f;
            Item.width = 32;
            Item.height = 32;
            Item.value = Item.sellPrice(gold: 6);
            Item.rare = ItemRarityID.LightRed;

            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;

            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<WasparangHoldout>();
            honeyCost = 6;
        }

        public override bool SafeCanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<WasparangHoldout>()] <= 0;
        }
    }
}