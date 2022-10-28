using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BombusApisBee.Core
{
    //wowzer slr is cool
    class PlayerRenderTarget
    {

        private MethodInfo PlayerDrawMethod;

        public static RenderTarget2D Target;

        public static bool canUseTarget = false;

        public static RenderTarget2D ScaledTileTarget { get; set; }

        public static int sheetSquareX;
        public static int sheetSquareY;
        private static Dictionary<int, int> PlayerIndexLookup;
        private static int prevNumPlayers;

        static Vector2 oldPos;
        static Vector2 oldCenter;
        static Vector2 oldMountedCenter;
        static Vector2 oldScreen;
        static Vector2 oldItemLocation;
        static Vector2 positionOffset;

        public static void Load()
        {
            if (Main.dedServ)
                return;

            sheetSquareX = 200;
            sheetSquareY = 300;

            PlayerIndexLookup = new Dictionary<int, int>();
            prevNumPlayers = -1;

            Main.QueueMainThreadAction(() =>
            {
                Target = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                ScaledTileTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            });

            On.Terraria.Main.SetDisplayMode += RefreshTargets;
            On.Terraria.Main.CheckMonoliths += DrawTargets;
            On.Terraria.Lighting.GetColor_int_int += getColorOverride;
            On.Terraria.Lighting.GetColor_Point += getColorOverride;
            On.Terraria.Lighting.GetColor_int_int_Color += getColorOverride;
            On.Terraria.Lighting.GetColor_Point_Color += GetColorOverride;
            On.Terraria.Lighting.GetColorClamped += GetColorOverride;
        }

        private static Color GetColorOverride(On.Terraria.Lighting.orig_GetColorClamped orig, int x, int y, Color oldColor)
        {
            if (canUseTarget)
                return orig.Invoke(x, y, oldColor);

            return orig.Invoke(x + (int)((oldPos.X - positionOffset.X) / 16), y + (int)((oldPos.Y - positionOffset.Y) / 16), oldColor);
        }
        private static Color GetColorOverride(On.Terraria.Lighting.orig_GetColor_Point_Color orig, Point point, Color originalColor)
        {
            if (canUseTarget)
                return orig.Invoke(point, originalColor);

            return orig.Invoke(new Point(point.X + (int)((oldPos.X - positionOffset.X) / 16), point.Y + (int)((oldPos.Y - positionOffset.Y) / 16)), originalColor);
        }

        public static Color getColorOverride(On.Terraria.Lighting.orig_GetColor_Point orig, Point point)
        {
            if (canUseTarget)
                return orig.Invoke(point);

            return orig.Invoke(new Point(point.X + (int)((oldPos.X - positionOffset.X) / 16), point.Y + (int)((oldPos.Y - positionOffset.Y) / 16)));
        }

        public static Color getColorOverride(On.Terraria.Lighting.orig_GetColor_int_int orig, int x, int y)
        {
            if (canUseTarget)
                return orig.Invoke(x, y);

            return orig.Invoke(x + (int)((oldPos.X - positionOffset.X) / 16), y + (int)((oldPos.Y - positionOffset.Y) / 16));
        }

        public static Color getColorOverride(On.Terraria.Lighting.orig_GetColor_int_int_Color orig, int x, int y, Color c)
        {
            if (canUseTarget)
                return orig.Invoke(x, y, c);

            return orig.Invoke(x + (int)((oldPos.X - positionOffset.X) / 16), y + (int)((oldPos.Y - positionOffset.Y) / 16), c);
        }

        public static Rectangle getPlayerTargetSourceRectangle(int whoAmI)
        {
            if (PlayerIndexLookup.ContainsKey(whoAmI))
                return new Rectangle(PlayerIndexLookup[whoAmI] * sheetSquareX, 0, sheetSquareX, sheetSquareY);

            return Rectangle.Empty;
        }

        public static Vector2 getPlayerTargetPosition(int whoAmI)
        {
            return Main.player[whoAmI].position - Main.screenPosition - new Vector2(sheetSquareX / 2, sheetSquareY / 2);
        }

        private static void RefreshTargets(On.Terraria.Main.orig_SetDisplayMode orig, int width, int height, bool fullscreen)
        {
            if (!Main.gameInactive && (width != Main.screenWidth || height != Main.screenHeight))
                ScaledTileTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, width, height);

            orig(width, height, fullscreen);
        }

        private static void DrawTargets(On.Terraria.Main.orig_CheckMonoliths orig)
        {

            orig();

            if (Main.gameMenu)
                return;

            if (Main.player.Any(n => n.active))
                DrawPlayerTarget();

            if (Main.instance.tileTarget.IsDisposed)
                return;

            RenderTargetBinding[] oldtargets1 = Main.graphics.GraphicsDevice.GetRenderTargets();

            Matrix matrix = Main.GameViewMatrix.ZoomMatrix;

            GraphicsDevice GD = Main.graphics.GraphicsDevice;
            SpriteBatch sb = Main.spriteBatch;

            GD.SetRenderTarget(ScaledTileTarget);
            GD.Clear(Color.Transparent);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, matrix);
            Main.spriteBatch.Draw(Main.instance.tileTarget, Main.sceneTilePos - Main.screenPosition, Color.White);
            sb.End();

            Main.graphics.GraphicsDevice.SetRenderTargets(oldtargets1);

        }

        public static Vector2 getPositionOffset(int whoAmI)
        {
            if (PlayerIndexLookup.ContainsKey(whoAmI))
                return new Vector2(PlayerIndexLookup[whoAmI] * sheetSquareX + sheetSquareX / 2, sheetSquareY / 2);

            return Vector2.Zero;
        }

        private static void DrawPlayerTarget()
        {
            var activePlayerCount = Main.player.Count(n => n.active);

            if (activePlayerCount != prevNumPlayers)
            {
                prevNumPlayers = activePlayerCount;
                Target = new RenderTarget2D(Main.graphics.GraphicsDevice, 300 * activePlayerCount, 300);
                int activeCount = 0;
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if (Main.player[i].active)
                    {
                        PlayerIndexLookup[i] = activeCount;
                        activeCount++;
                    }
                }
            }

            RenderTargetBinding[] oldtargets2 = Main.graphics.GraphicsDevice.GetRenderTargets();
            canUseTarget = false;
            Main.graphics.GraphicsDevice.SetRenderTarget(Target);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            Main.spriteBatch.Begin();
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                var player = Main.player[i];

                if (player.active && player.dye.Length > 0)
                {
                    oldPos = player.position;
                    oldCenter = player.Center;
                    oldMountedCenter = player.MountedCenter;
                    oldScreen = Main.screenPosition;
                    oldItemLocation = player.itemLocation;
                    int oldHeldProj = player.heldProj;
                    positionOffset = getPositionOffset(i);
                    player.position = positionOffset;
                    player.Center = oldCenter - oldPos + positionOffset;
                    player.itemLocation = oldItemLocation - oldPos + positionOffset;
                    player.MountedCenter = oldMountedCenter - oldPos + positionOffset;
                    player.heldProj = -1;
                    Main.screenPosition = Vector2.Zero;

                    Main.PlayerRenderer.DrawPlayer(Main.Camera, player, player.position, player.fullRotation, player.fullRotationOrigin, 0f);

                    player.position = oldPos;
                    player.Center = oldCenter;
                    Main.screenPosition = oldScreen;
                    player.itemLocation = oldItemLocation;
                    player.MountedCenter = oldMountedCenter;
                    player.heldProj = oldHeldProj;
                }
            }

            Main.spriteBatch.End();

            Main.graphics.GraphicsDevice.SetRenderTargets(oldtargets2);
            canUseTarget = true;
        }
    }
}