using BombusApisBee.BeeDamageClass;
using BombusApisBee.Projectiles;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class TESTWEAPON : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            // DisplayName.SetDefault("Honeycomb"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("TEST DONT USE");
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
            Item.shoot = ModContent.ProjectileType<AstralStarSplitting>();
            Item.shootSpeed = 10;
            Item.UseSound = SoundID.Item11;
            Item.scale = 1;
            Item.crit = 4;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 300, false);
        }
        public override void HoldItem(Player player)
        {
            player.IncreaseBeeCrit(100);
        }
    }
}