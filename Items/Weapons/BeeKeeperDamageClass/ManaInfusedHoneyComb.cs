using BombusApisBee.BeeDamageClass;
using BombusApisBee.Projectiles;
using Terraria.DataStructures;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class ManaInfusedHoneycomb : BeeDamageItem
    {
        public override bool SafeCanUseItem(Player player) => player.statMana > 5 && player.ownedProjectileCounts<ManaHoneycombHoldout>() <= 0;
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Hold <left> to charge up a burst of mana infused bees\nMana bees spawn homing mana stars on hit");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 24;
            Item.noMelee = true;

            Item.useAnimation = Item.useTime = 60;

            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0f;
            Item.value = Item.sellPrice(0, 3, 0, 0);

            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<ManaHoneycombHoldout>();
            beeResourceCost = 1;
            Item.noUseGraphic = true;
            Item.channel = true;

            Item.mana = 5;
        }
    }
}