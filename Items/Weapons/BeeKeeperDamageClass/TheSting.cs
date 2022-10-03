using BombusApisBee.BeeDamageClass;
using BombusApisBee.Projectiles;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class TheSting : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("'This one is gonna sting'\nConjures a miniature Queen Bee that attacks the mouse cursor\nShe cycles through 3 attacks:\nCircling the cursor and firing stingers\nDashing at the cursor\nStaying above the cursor and spawning bees" +
                "\nUses 2 honey for every seconds that she is alive");
            Item.staff[Item.type] = true;
        }

        public override void SafeSetDefaults()
        {
            Item.expertOnly = true;
            Item.damage = 55;
            Item.width = 38;
            Item.height = 38;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.knockBack = 8f;
            Item.value = Item.sellPrice(gold: 3);
            Item.rare = ItemRarityID.Expert;
            Item.UseSound = SoundID.Item9;
            Item.shoot = ModContent.ProjectileType<TheStingProj>();
            Item.shootSpeed = 10f;
            beeResourceCost = 3;
        }

        public override bool SafeCanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<TheStingProj>()] <= 0;
        }
    }
}
