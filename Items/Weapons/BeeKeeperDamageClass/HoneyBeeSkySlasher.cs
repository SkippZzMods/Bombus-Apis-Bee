using BombusApisBee.BeeDamageClass;
using BombusApisBee.Projectiles;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class HoneyBeeSkySlasher : BeeDamageItem
    {
        private int swingDirection = 1;

        private int combo = 0;
        public override bool SafeCanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<HoneybeeSkySlasherHoldout>()] <= 0;
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Nectar Slasher");
            Tooltip.SetDefault("Performs a three piece combo of swift strikes and throws\nSlashes through enemies on hit\nReplenishes life on hit");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 35;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2.5f;
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<HoneybeeSkySlasherHoldout>();
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.value = Item.sellPrice(gold: 10);
            Item.rare = ItemRarityID.Blue;
            beeResourceCost = 2;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            swingDirection *= -1;
            combo++;
            if (combo > 2)
            {
                combo = 0;
                swingDirection = 1;
            }
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, swingDirection, combo);
            return false;
        }
    }
}