using BombusApisBee.Items.Accessories.BeeKeeperDamageClass;

namespace BombusApisBee
{
    public class BombusWorld : ModSystem
    {
        public override void PostWorldGen()
        {
            // gold chest gen
            int itemsToPlaceInGoldChests = ModContent.ItemType<EnchantedApiary>();
            for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest != null && Main.tile[chest.x, chest.y].TileType == TileID.Containers && (Main.tile[chest.x, chest.y].TileFrameX == 1 * 36 || Main.tile[chest.x, chest.y].TileFrameX == 8 * 36 || Main.tile[chest.x, chest.y].TileFrameX == 32 * 36 || Main.tile[chest.x, chest.y].TileFrameX == 51 * 36 || Main.tile[chest.x, chest.y].TileFrameX == 50 * 36))
                {
                    for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                    {
                        if (inventoryIndex == 0)
                        {
                            if (chest.item[inventoryIndex].type == ItemID.FlareGun)
                            {
                                if (WorldGen.genRand.NextFloat() < 0.25f)
                                {
                                    chest.item[0].TurnToAir();
                                    chest.item[1].TurnToAir();
                                    chest.item[0].SetDefaults(itemsToPlaceInGoldChests);
                                    chest.item[0].Prefix(-1);
                                    for (int i = 1; i < 39; i++)
                                    {
                                        chest.item[i] = chest.item[i + 1];
                                    }
                                }
                                break;
                            }
                            else
                            {
                                if (WorldGen.genRand.NextFloat() < 0.25f)
                                {
                                    chest.item[0].SetDefaults(itemsToPlaceInGoldChests);
                                    chest.item[0].Prefix(-1);
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
