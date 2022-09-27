using Microsoft.Xna.Framework.Graphics;

namespace BombusApisBee.PrimitiveDrawing
{
    public static class DrawPrimitives
    {
        public static void Load()
        {
            if (Main.dedServ)
                return;

            On.Terraria.Main.DrawPlayers_BehindNPCs += DrawPrim;
        }

        private static void DrawPrim(On.Terraria.Main.orig_DrawPlayers_BehindNPCs orig, Main self)
        {
            orig(self);

            if (Main.gameMenu)
                return;

            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            for (int k = 0; k < Main.maxProjectiles; k++) // Projectiles.
                if (Main.projectile[k].active && Main.projectile[k].ModProjectile is IDrawPrimitive_)
                    (Main.projectile[k].ModProjectile as IDrawPrimitive_).DrawPrimitives();

            for (int k = 0; k < Main.maxNPCs; k++) // NPCs.
                if (Main.npc[k].active && Main.npc[k].ModNPC is IDrawPrimitive_)
                    (Main.npc[k].ModNPC as IDrawPrimitive_).DrawPrimitives();
        }
    }
}
