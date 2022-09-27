using BombusApisBee.BeeDamageClass;
using BombusApisBee.Projectiles;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class BeeThrower : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            // DisplayName.SetDefault("Honeycomb"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("'Somehow the bees dont die to the fire'\n'Dont ask me I just work here'");
            DisplayName.SetDefault("Beethrower");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 48;
            Item.noMelee = true;
            Item.width = 70;
            Item.height = 18;
            Item.useTime = 13;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.value = 250000;
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BeeThrowerProj>();
            Item.shootSpeed = 10.5f;
            Item.UseSound = SoundID.Item34;
            Item.scale = 1;
            Item.crit = 4;
            beeResourceCost = 1;
        }


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-7, 0);
        }

    }
}