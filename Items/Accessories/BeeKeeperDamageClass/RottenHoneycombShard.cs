using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    public class RottenHoneycombShard : BeeKeeperItem
    {
        public override void Load()
        {
            BombusApisBeeGlobalProjectile.StrongBeeOnHitEvent += InflictRotten;
        }

        private void InflictRotten(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.player[proj.owner].Bombus().HasRottenHoneycombShard)
                target.AddBuff<RottenDebuff>(600);
        }

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Increases the chance to strengthen friendly bees by 20%\nStrengthened bees apply a weakening debuff\n'Smells horrible'");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Hymenoptra().BeeStrengthenChance += 0.20f;
            player.Bombus().HasRottenHoneycombShard = true;
        }
    }
}
