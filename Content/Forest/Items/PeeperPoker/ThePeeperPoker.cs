using BombusApisBee.Core.BeekeeperClass;

namespace BombusApisBee.Content.Forest.Items.PeeperPoker
{
    public class ThePeeperPoker : BeekeeperWeapon
    {
        public override string Texture => "BombusApisBee/Content/Forest/Items/PeeperPoker/PeeperPokerHoldout";
        public override void SafeSetStaticDefaults()
        {

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
            Item.shoot = ProjectileType<PeeperPokerHoldout>();
            Item.shootSpeed = 1f;
            Item.scale = 1f;
            Item.crit = 4;
            honeyCost = 1;
            Item.noUseGraphic = true;
            Item.channel = true;
        }
    }
}
