namespace BombusApisBee.Content.Snow.Items.FrozenStinger
{
    public class FrozenStinger : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 4);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Bombus().FrozenStinger = true;
            player.IncreaseBeeCrit(8);
        }
    }
}
