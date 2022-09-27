using BombusApisBee.BeeDamageClass;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BombusApisBee.UI
{
    public class HoneyResourceUI
    {
        public static void Draw()
        {
            switch (Main.ResourceSetsManager.ActiveSetKeyName)
            {
                case "HorizontalBars": DrawHoneyBar(); break;
                case "New": DrawNewHoney(); break;
                case "Default": DrawLegacyHoney(); break;
            }
        }

        public static void DrawHoneyBar()
        {
            var modPlayer = Main.LocalPlayer.GetModPlayer<BeeDamagePlayer>();
            Texture2D honeyTexture = (Texture2D)ModContent.Request<Texture2D>("BombusApisBee/UI/BarStyle_HoneyBarHoney");
            Texture2D barTexture = (Texture2D)ModContent.Request<Texture2D>("BombusApisBee/UI/BarStyle_HoneyBar");
            Texture2D frameTexture = (Texture2D)ModContent.Request<Texture2D>("BombusApisBee/UI/BarStyle_HoneyBarFrame");
            float uiScale = 1f + (Main.UIScale * 0.5f / 100);
            Vector2 drawPos = new Vector2(Main.screenWidth - 34, 140);
            Main.spriteBatch.Draw(frameTexture, drawPos, null, Color.White, 0f, frameTexture.Size() / 2f, uiScale, SpriteEffects.None, 0f);
            float completionRatio = (float)modPlayer.BeeResourceCurrent / (float)modPlayer.BeeResourceMax2;
            Rectangle barRectangle = new Rectangle(0, 0, barTexture.Width, (int)((float)barTexture.Height * completionRatio));
            Vector2 drawPos2 = new Vector2(Main.screenWidth - 32, 162);
            Main.spriteBatch.Draw(barTexture, drawPos2, barRectangle, Color.White, 0f, barTexture.Size() / 2f, uiScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(honeyTexture, drawPos, null, Color.White, 0f, honeyTexture.Size() / 2f, uiScale, 0, 0f);

            Rectangle mouse = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);
            Rectangle HoneyBar = Utils.CenteredRectangle(drawPos, Utils.Size(frameTexture) * uiScale);
            if (HoneyBar.Intersects(mouse) && modPlayer.BeeResourceMax2 > 0f)
            {
                int HoneyInt = (int)Math.Round((double)(modPlayer.BeeResourceCurrent));
                int maxHoneyInt = (int)Math.Round((double)(modPlayer.BeeResourceMax2));
                Main.instance.MouseText(string.Concat(new object[]
                {
                    "Honey: ",
                    HoneyInt,
                    "/",
                    maxHoneyInt
                }) ?? "", 0, 0, -1, -1, -1, -1);
            }
        }

        public static void DrawNewHoney()
        {
            var modPlayer = Main.LocalPlayer.GetModPlayer<BeeDamagePlayer>();
            Texture2D frameTexture = (Texture2D)ModContent.Request<Texture2D>("BombusApisBee/UI/NewStyle_HoneyFrame");
            Texture2D barTexture = (Texture2D)ModContent.Request<Texture2D>("BombusApisBee/UI/NewStyle_HoneyBar");
            float uiScale = 1f + (Main.UIScale * 0.5f / 100);
            bool drawText = false;
            for (int i = 0; i < GetNewHoneyAmount(); i++)
            {
                Vector2 drawPos = new Vector2(Main.screenWidth - 320, 52 + (i * 53) + (Main.playerInventory ? 40 : 0));
                if (i >= 5)
                    drawPos += new Vector2(-40, -265);
                Main.spriteBatch.Draw(frameTexture, drawPos, null, Color.White, 0f, frameTexture.Size() / 2f, uiScale, SpriteEffects.None, 0f);
                float barScale = Utils.GetLerpValue(20 * i, 20 * (float)(i + 1), modPlayer.BeeResourceCurrent, true);

                Rectangle rect = barTexture.Frame();
                Vector2 origin = rect.Size() * 0.5f;
                drawPos.Y += 7;
                Main.spriteBatch.Draw(barTexture, drawPos, rect, Color.White * barScale, 0f, origin, barScale, SpriteEffects.None, 0f);

                Rectangle mouse = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);
                Rectangle HoneyBar = Utils.CenteredRectangle(drawPos, Utils.Size(frameTexture) * uiScale);
                if (HoneyBar.Intersects(mouse) && modPlayer.BeeResourceMax2 > 0f)
                {
                    drawText = true;
                }
            }

            if (drawText)
            {
                int HoneyInt = (int)Math.Round((double)(modPlayer.BeeResourceCurrent));
                int maxHoneyInt = (int)Math.Round((double)(modPlayer.BeeResourceMax2));
                Main.instance.MouseText(string.Concat(new object[]
                {
                    "Honey: ",
                    HoneyInt,
                    "/",
                    maxHoneyInt
                }) ?? "", 0, 0, -1, -1, -1, -1);
            }
        }

        public static int GetNewHoneyAmount()
        {
            var modPlayer = Main.LocalPlayer.GetModPlayer<BeeDamagePlayer>();
            return Utils.Clamp(modPlayer.BeeResourceMax2 / 20, 0, 10);
        }

        public static void DrawLegacyHoney()
        {
            var modPlayer = Main.LocalPlayer.GetModPlayer<BeeDamagePlayer>();
            Texture2D barTexture = (Texture2D)ModContent.Request<Texture2D>("BombusApisBee/UI/NewStyle_HoneyBar");
            float uiScale = 1f + (Main.UIScale * 0.5f / 100);
            bool drawText = false;
            for (int i = 0; i < GetLegacyHoneyAmount(); i++)
            {
                Vector2 drawPos = new Vector2(Main.screenWidth - 315, 45 + (i * 35) + (Main.playerInventory ? 40 : 0));
                if (i >= 10)
                    drawPos += new Vector2(-30, -350);
                float barScale = Utils.GetLerpValue(10 * i, 10 * (float)(i + 1), modPlayer.BeeResourceCurrent, true);
                Rectangle rect = barTexture.Frame();
                Vector2 origin = rect.Size() * 0.5f;
                Main.spriteBatch.Draw(barTexture, drawPos, rect, Color.White * barScale, 0f, origin, barScale, SpriteEffects.None, 0f);

                Rectangle mouse = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);
                Rectangle HoneyBar = Utils.CenteredRectangle(drawPos, Utils.Size(barTexture) * uiScale);
                if (HoneyBar.Intersects(mouse) && modPlayer.BeeResourceMax2 > 0f)
                {
                    drawText = true;
                }
            }
            if (drawText)
            {
                int HoneyInt = (int)Math.Round((double)(modPlayer.BeeResourceCurrent));
                int maxHoneyInt = (int)Math.Round((double)(modPlayer.BeeResourceMax2));
                Main.instance.MouseText(string.Concat(new object[]
                {
                    "Honey: ",
                    HoneyInt,
                    "/",
                    maxHoneyInt
                }) ?? "", 0, 0, -1, -1, -1, -1);
            }
        }
        public static int GetLegacyHoneyAmount()
        {
            var modPlayer = Main.LocalPlayer.GetModPlayer<BeeDamagePlayer>();
            return Utils.Clamp(modPlayer.BeeResourceMax2 / 10, 0, 20);
        }
    }
}
