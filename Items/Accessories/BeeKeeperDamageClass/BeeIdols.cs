namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    public class CopperBeeIdol : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("10% increased chance to not consume honey\n'Clearly ancient civilizations didn't know what bees looked like'");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = 1;
            Item.accessory = true;
            Item.defense = 2;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Hymenoptra().ResourceChanceAdd += 0.1f;
        }
    }
    public class TinBeeIdol : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("4% increased hymenoptra damage and critical strike chance\n'Clearly ancient civilizations didn't know what bees looked like'");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = 1;
            Item.accessory = true;
            Item.defense = 2;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.IncreaseBeeCrit(4);
            player.IncreaseBeeDamage(0.04f);
        }
    }

    class GenerateBeeIdols : ModSystem
    {
        public override void PostWorldGen()
        {
            for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest != null && Main.tile[chest.x, chest.y].TileType == TileID.Containers && Main.tile[chest.x, chest.y].TileFrameX == 0 * 36)
                {
                    for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                    {
                        if (chest.item[inventoryIndex].type == ItemID.None)
                            if (WorldGen.genRand.NextFloat() < 0.2f)
                            {
                                chest.item[inventoryIndex].SetDefaults(WorldGen.copper == TileID.Copper ? ModContent.ItemType<CopperBeeIdol>() : ModContent.ItemType<TinBeeIdol>());
                                chest.item[inventoryIndex].Prefix(-1);
                                break;
                            }
                    }
                }
            }
        }
    }
}
