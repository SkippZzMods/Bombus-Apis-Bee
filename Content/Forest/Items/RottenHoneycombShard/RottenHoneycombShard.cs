using BombusApisBee.Core.Common.BeeProjectile;
using BombusApisBee.Core.Common.HoneycombShard;

namespace BombusApisBee.Content.Forest.Items.RottenHoneycombShard
{
    public class RottenHoneycombShard : HoneycombShardItem
    {
        public RottenHoneycombShard() : base("Rotten Honeycomb Shard", "Increases the chance to strengthen friendly bees by 20%\nStrengthened bees apply a weakening debuff\n'Smells horrible'", 300) { }
        public override void Load()
        {
            CommonBeeGlobalProjectile.StrongBeeOnHitEvent += InflictRotten;
        }

        private void InflictRotten(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (CanActivateEffect(Main.player[proj.owner]))
            {
                ActivateEffect(Main.player[proj.owner]);
                target.AddBuff<RottenDebuff>(180);
            }            
        }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Beekeeper().BeeStrengthenChance += 0.20f;
            player.Bombus().HasRottenHoneycombShard = true;
        }
    }
}
