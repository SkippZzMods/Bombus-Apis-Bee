using BombusApisBee.Content.Forest.Items.Pollen;
using Terraria.DataStructures;

namespace BombusApisBee.Core
{
    public class BombusApisBeeGlobalTile : GlobalTile
    {
        public override void Drop(int i, int j, int type)
        {
            if (type == TileID.Plants || type == TileID.JunglePlants || type == TileID.Plants2 || type == TileID.JunglePlants2)
            {
                Tile tile = Main.tile[i, j];

                int style = tile.TileFrameX / 18;

                if (style >= 6 && Main.rand.NextBool(3))
                {
                    int worldX = i * 16;
                    int worldY = j * 16;

                    var source = new EntitySource_TileBreak(i, j);

                    for (int x = 0; x < Main.rand.Next(1, 4); x++)
                        Item.NewItem(source, worldX, worldY, 16, 16, ModContent.ItemType<PollenItem>());
                }
            }

            if (type == TileID.Sunflower || type == TileID.BloomingHerbs || type == TileID.MatureHerbs)
            {
                if (Main.rand.NextBool())
                {
                    int worldX = i * 16;
                    int worldY = j * 16;

                    var source = new EntitySource_TileBreak(i, j);

                    for (int x = 0; x < Main.rand.Next(1, 4); x++)
                        Item.NewItem(source, worldX, worldY, 16, 16, ModContent.ItemType<PollenItem>());
                }
            }
        }
    }
}
