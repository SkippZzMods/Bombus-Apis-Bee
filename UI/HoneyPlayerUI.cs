/*using BombusApisBee.BeeDamageClass;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BombusApisBee.UI
{
    public class HoneyPlayerUI
    {
        public static float DrawFloatY => ModContent.GetInstance<BombusConfig>().YOffset;

        public static float DrawFloatX => ModContent.GetInstance<BombusConfig>().XOffset;

        public static Vector2 DrawPosition => Vector2.Transform(new Vector2(Main.LocalPlayer.Bottom.X + DrawFloatX, Main.LocalPlayer.Bottom.Y + DrawFloatY) - Main.screenPosition, Main.GameViewMatrix.ZoomMatrix);

        public static void Draw()
        {
            if (!(Main.LocalPlayer.HeldItem.CountsAsClass<HymenoptraDamageClass>()))
                return;

            var modPlayer = Main.LocalPlayer.GetModPlayer<BeeDamagePlayer>();
            Texture2D frameTexture = (Texture2D)ModContent.Request<Texture2D>("BombusApisBee/UI/HoneyPlayerUIFrame");
            Texture2D barTexture = (Texture2D)ModContent.Request<Texture2D>("BombusApisBee/UI/HoneyPlayerUIBar");
            float uiScale = Main.UIScale;
            Main.spriteBatch.Draw(frameTexture, DrawPosition, null, Color.White, 0f, frameTexture.Size() / 2f, uiScale, SpriteEffects.None, 0f);
            float completionRatio = (float)modPlayer.BeeResourceCurrent / (float)modPlayer.BeeResourceMax2;
            Rectangle barRectangle = new Rectangle(0, 0, (int)((float)barTexture.Width * completionRatio), barTexture.Height);
            Main.spriteBatch.Draw(barTexture, DrawPosition + new Vector2(0, 1f), barRectangle, Color.White, 0f, barTexture.Size() / 2f, uiScale, SpriteEffects.None, 0f);
            Rectangle mouse = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);
            Rectangle HoneyBar = Utils.CenteredRectangle(DrawPosition, Utils.Size(barTexture) * uiScale);
            if (HoneyBar.Intersects(mouse) && modPlayer.BeeResourceMax2 > 0f)
            {
                //Main.LocalPlayer.mouseInterface = true;
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
    }
}*/
