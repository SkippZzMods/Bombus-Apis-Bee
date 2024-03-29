﻿namespace BombusApisBee.PrimitiveDrawing
{
    public static class DrawPrimitives
    {
        public static void Load()
        {
            if (Main.dedServ)
                return;

            Terraria.On_Main.DrawDust += DrawPrims;
        }

        private static void DrawPrims(Terraria.On_Main.orig_DrawDust orig, Main self)
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
