using BombusApisBee.BeeDamageClass;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace BombusApisBee.UI.Old
{
    internal class HoneyResourceBar : UIState
    {
        private UIText text;
        private UIElement area;
        private UIImage barFrame;
        private Color gradientA;
        private Color gradientB;

        public override void OnInitialize()
        {
            area = new UIElement();
            area.Left.Set(-area.Width.Pixels - 600, 1f);
            area.Top.Set(30, 0f);
            area.Width.Set(182, 0f);
            area.Height.Set(60, 0f);

            barFrame = new UIImage(ModContent.Request<Texture2D>("BombusApisBee/UI/HoneyResourceFrame"));
            barFrame.Left.Set(22, 0f);
            barFrame.Top.Set(0, 0f);
            barFrame.Width.Set(138, 0f);
            barFrame.Height.Set(34, 0f);

            text = new UIText("0/0", 0.8f); // text to show stat
            text.Width.Set(138, 0f);
            text.Height.Set(34, 0f);
            text.Top.Set(40, 0f);
            text.Left.Set(0, 0f);

            gradientA = new Color(255, 128, 0); // A dark purple
            gradientB = new Color(255, 204, 0); // A light purple

            area.Append(text);
            area.Append(barFrame);
            Append(area);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Main.LocalPlayer.HeldItem.CountsAsClass<HymenoptraDamageClass>())
                return;

            base.Draw(spriteBatch);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            var modPlayer = Main.LocalPlayer.GetModPlayer<BeeDamagePlayer>();
            float quotient = (float)modPlayer.BeeResourceCurrent / modPlayer.BeeResourceMax2;
            quotient = Utils.Clamp(quotient, 0f, 1f);

            Rectangle hitbox = barFrame.GetInnerDimensions().ToRectangle();
            hitbox.X += 12;
            hitbox.Width -= 24;
            hitbox.Y += 8;
            hitbox.Height -= 16;

            int left = hitbox.Left;
            int right = hitbox.Right;
            int steps = (int)((right - left) * quotient);
            for (int i = 0; i < steps; i += 1)
            {
                float percent = (float)i / (right - left);
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(left + i, hitbox.Y, 1, hitbox.Height), Color.Lerp(gradientA, gradientB, percent));
            }
        }
        public override void Update(GameTime gameTime)
        {
            if (!Main.LocalPlayer.HeldItem.CountsAsClass<HymenoptraDamageClass>())
                return;

            var modPlayer = Main.LocalPlayer.GetModPlayer<BeeDamagePlayer>();
            text.SetText($"       Honey: {modPlayer.BeeResourceCurrent} / {modPlayer.BeeResourceMax2}");
            base.Update(gameTime);
        }
    }
}
