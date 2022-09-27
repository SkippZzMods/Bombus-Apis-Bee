using BombusApisBee.BeeDamageClass;
using BombusApisBee.Projectiles;

namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    public class HoneyLocket : BeeKeeperItem
    {
        public int timer;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Locket");
            Tooltip.SetDefault("'It seems to have a deep connection to the Hive'\nSpawns wasps that swarm enemies every 2 seconds");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 3);
            Item.damage = 35;
            Item.DamageType = ModContent.GetInstance<HymenoptraDamageClass>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Bombus().HoneyLocket = true;
            timer++;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<WaspAccessory>()] < 4 && timer >= 120)
            {
                Vector2 vel = new Vector2(Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(-5, 5));
                int damage = (int)player.GetTotalDamage<HymenoptraDamageClass>().ApplyTo(Item.damage);
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, vel, ModContent.ProjectileType<WaspAccessory>(), damage, 1, player.whoAmI);
                timer = 0;
            }
        }
    }
}
