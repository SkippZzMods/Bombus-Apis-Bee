using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class WoodenApiary : ApiaryItem
    {
        public override void AddStaticDefaults()
        {
            DisplayName.SetDefault("Wooden Apiary");
            Tooltip.SetDefault("Hold <left> to rapidly fire bees\nHold <right> to fire bees slower, but take control over the bees causing them to deal 55% more damage");
        }

        public override void AddDefaults()
        {
            Item.damage = 5;
            Item.noMelee = true;
            Item.width = 32;
            Item.height = 32;

            Item.useTime = 14;
            Item.useAnimation = 14;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0.15f;

            Item.value = Item.sellPrice(silver: 1);

            Item.rare = ItemRarityID.White;

            Item.autoReuse = true;

            Item.shoot = ProjectileType<WoodenApiaryHoldout>();

            Item.shootSpeed = 6f;

            Item.UseSound = SoundID.Item11;

            honeyCost = 1;
        }

        public override bool SafeCanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useTime = 28;
                Item.useAnimation = 28;

            }
            else
            {
                Item.useTime = 14;
                Item.useAnimation = 14;

            }

            return true;
        }
    }

    public class WoodenApiaryHoldout : ApiaryHoldout
    {
        public override string Texture => "BombusApisBee/Items/Weapons/BeeKeeperDamageClass/WoodenApiary";
    }
}
