using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Core.Common.HoneycombShard;

namespace BombusApisBee.Content.Corruption.Items.EboncombShard
{
    public class EboncombShard : HoneycombShardItem
    {
        public EboncombShard() : base("Eboncomb Shard", "Increases the chance to strengthen friendly bees by 30%\nIncreases the damage of strengthened bees by 3", 0) { }

        public override void SafeSetDefaults()
        {
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(silver: 25);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Beekeeper().BeeStrengthenChance += 0.30f;
        }

        public override void ModifyEquippedHit(Player player, Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (BeeUtils.IsStrongBee(proj.whoAmI))
                modifiers.FlatBonusDamage += 3;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PollenItem>(20).
                AddIngredient(ItemID.DemoniteBar, 8).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
