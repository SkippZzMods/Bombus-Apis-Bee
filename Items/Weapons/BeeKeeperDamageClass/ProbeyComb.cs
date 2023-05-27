using BombusApisBee.BeeDamageClass;
using BombusApisBee.Projectiles;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class ProbeyComb : BeeDamageItem
    {
        public override bool SafeCanUseItem(Player player) => player.ownedProjectileCounts<ProbeycombHoldout>() <= 0;
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Hold <left> to charge up a burst of Probees\nProbees fire deadly lasers at enemies");
            DisplayName.SetDefault("Probeecomb");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 47;
            Item.noMelee = true;
            Item.width = 50;
            Item.height = 32;
            Item.useTime = 120;
            Item.useAnimation = 120;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1f;
            Item.value = Item.sellPrice(0, 4, 50, 0);
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<ProbeycombHoldout>();
            Item.shootSpeed = 1f;
            beeResourceCost = 2;

            Item.channel = true;
            Item.noUseGraphic = true;
        }
    }
}