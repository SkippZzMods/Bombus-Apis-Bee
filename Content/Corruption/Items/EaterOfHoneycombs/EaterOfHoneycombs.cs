using BombusApisBee.Core.BeekeeperClass;

namespace BombusApisBee.Content.Corruption.Items.EaterOfHoneycombs
{
    public class EaterOfHoneycombs : BeekeeperWeapon
    {
        public override bool SafeCanUseItem(Player player) => player.ownedProjectileCounts<EaterOfHoneycombsHoldout>() <= 0;
        public override void SafeSetStaticDefaults()
        {

        }

        public override void SafeSetDefaults()
        {
            Item.damage = 13;

            Item.noMelee = true;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.sellPrice(0, 1, 50, 0);

            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = ProjectileType<EaterOfHoneycombsHoldout>();
            Item.shootSpeed = 1f;
            Item.noUseGraphic = true;
            honeyCost = 4;
        }
    }
}