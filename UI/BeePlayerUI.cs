using BombusApisBee.BeeDamageClass;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace BombusApisBee.UI
{
    public class BeePlayerUI : UIState
    {
        internal UIPanel panel;

        internal UIImageButton Middle;

        internal UIImageButton Left;

        internal UIImageButton Right;

        internal UIImage Bar;

        internal int UITimer;

        public override void OnInitialize()
        {
            panel = new();

            panel.HAlign = 0.5f;
            panel.Top.Set(100, 0f);
            panel.Width.Set(45, 0f);
            panel.Height.Set(45, 0f);
            Append(panel);

            Middle = new(ModContent.Request<Texture2D>("BombusApisBee/UI/BeePlayerUI_Shield"));
            Middle.VAlign = 0.5f;
            Middle.HAlign = 0.5f;
            Middle.Width.Set(20, 0);
            Middle.Height.Set(22, 0);

            Left = new(ModContent.Request<Texture2D>("BombusApisBee/UI/BeePlayerUI_Sword"));
            Left.VAlign = 0.5f;
            Left.HAlign = 0f;
            Left.Width.Set(20, 0);
            Left.Height.Set(22, 0);

            Right = new(ModContent.Request<Texture2D>("BombusApisBee/UI/BeePlayerUI_Jar"));
            Right.VAlign = 0.5f;
            Right.HAlign = 1f;
            Right.Width.Set(20, 0);
            Right.Height.Set(22, 0);

            panel.Append(Middle);

            Left.OnClick += Left_OnClick;

            Right.OnClick += Right_OnClick;
        }

        private void Right_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            var mp = Main.LocalPlayer.Hymenoptra();

            if (mp.CurrentBeeState == (int)BeeDamagePlayer.BeeState.Defense)
                mp.CurrentBeeState = (int)BeeDamagePlayer.BeeState.Gathering;

            else if (mp.CurrentBeeState == (int)BeeDamagePlayer.BeeState.Offense)
                mp.CurrentBeeState = (int)BeeDamagePlayer.BeeState.Gathering;

            else if (mp.CurrentBeeState == (int)BeeDamagePlayer.BeeState.Gathering)
                mp.CurrentBeeState = (int)BeeDamagePlayer.BeeState.Offense;
        }

        private void Left_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            var mp = Main.LocalPlayer.Hymenoptra();

            if (mp.CurrentBeeState == (int)BeeDamagePlayer.BeeState.Defense)
                mp.CurrentBeeState = (int)BeeDamagePlayer.BeeState.Offense;

            else if (mp.CurrentBeeState == (int)BeeDamagePlayer.BeeState.Offense)
                mp.CurrentBeeState = (int)BeeDamagePlayer.BeeState.Defense;

            else if (mp.CurrentBeeState == (int)BeeDamagePlayer.BeeState.Gathering)
                mp.CurrentBeeState = (int)BeeDamagePlayer.BeeState.Defense;
        }   

        public override void Draw(SpriteBatch spriteBatch)
        {
            var mp = Main.LocalPlayer.Hymenoptra();

            if (Main.LocalPlayer.Hymenoptra().HeldBeeWeaponTimer > 0 || Main.LocalPlayer.Hymenoptra().HoldingBeeWeaponTimer > 0)
            {
                base.Draw(spriteBatch);

                Texture2D barTex = ModContent.Request<Texture2D>("BombusApisBee/UI/BeePlayerUI_BarOutline").Value;

                Texture2D barInsideTex = ModContent.Request<Texture2D>("BombusApisBee/UI/BeePlayerUI_BarInner").Value;

                Texture2D honeyBarTex = ModContent.Request<Texture2D>("BombusApisBee/UI/BeePlayerUI_BarHoney").Value;

                Vector2 position = new Vector2(Main.LocalPlayer.Center.X - 66 + MathHelper.Lerp(0, 45, UITimer / 5f), Main.LocalPlayer.Center.Y - 16) - Main.screenPosition;

                Main.spriteBatch.Draw(barInsideTex, position, null, Color.White);

                float completionRatio = mp.BeeResourceCurrent / (float)mp.BeeResourceMax2;
                Rectangle barRectangle = new Rectangle(0, 0, honeyBarTex.Width, (int)(honeyBarTex.Height * completionRatio));

                Main.spriteBatch.Draw(honeyBarTex, position + Vector2.UnitY * MathHelper.Lerp(0f, honeyBarTex.Height, 1f - completionRatio), barRectangle, Color.White);

                Main.spriteBatch.Draw(barTex, position, null, Color.White);          
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!(Main.LocalPlayer.Hymenoptra().HeldBeeWeaponTimer > 0 || Main.LocalPlayer.Hymenoptra().HoldingBeeWeaponTimer > 0))
                return;

            var mp = Main.LocalPlayer.Hymenoptra();

            panel.Top.Set(Main.LocalPlayer.Center.Y - Main.screenPosition.Y - 20, 0f);

            if (panel.IsMouseHovering)
            {
                if (UITimer < 5)
                    UITimer++;

                panel.Append(Left);
                panel.Append(Right);
            }
            else
            {
                if (UITimer > 0)
                    UITimer--;

                Left.Remove();

                Right.Remove();
            }


            if (mp.CurrentBeeState == (int)BeeDamagePlayer.BeeState.Defense)
            {
                Middle.SetImage(ModContent.Request<Texture2D>("BombusApisBee/UI/BeePlayerUI_Shield"));
                Left.SetImage(ModContent.Request<Texture2D>("BombusApisBee/UI/BeePlayerUI_Sword"));
                Right.SetImage(ModContent.Request<Texture2D>("BombusApisBee/UI/BeePlayerUI_Jar"));

                if (Middle.IsMouseHovering)
                    Main.instance.MouseText("Your bees are in Defending Mode\nYour defense is increased by "
                        + mp.CurrentBees * 2 + ", and you are granted a Honey shield which blocks one attack while your bees are in Defending Mode");

                if (Left.IsMouseHovering)
                    Main.instance.MouseText("Left Click to switch your bees into Attacking Mode\nYou are granted "
                        + mp.CurrentBees + "% increased hymenoptra crit chance and " + mp.CurrentBees * 2 + "% increased hymenoptra damage while your bees are in Attacking Mode");

                if (Right.IsMouseHovering)
                    Main.instance.MouseText("Left Click to switch your bees into Gathering Mode\nYou are granted increased Honey regeneration, but deal 15% less damage while your bees are in Gathering Mode");
            }

            if (mp.CurrentBeeState == (int)BeeDamagePlayer.BeeState.Offense)
            {
                Middle.SetImage(ModContent.Request<Texture2D>("BombusApisBee/UI/BeePlayerUI_Sword"));
                Left.SetImage(ModContent.Request<Texture2D>("BombusApisBee/UI/BeePlayerUI_Shield"));
                Right.SetImage(ModContent.Request<Texture2D>("BombusApisBee/UI/BeePlayerUI_Jar"));

                if (Middle.IsMouseHovering)
                    Main.instance.MouseText("Your bees are in Attacking Mode\nYou are granted "
                        + mp.CurrentBees + "% increased hymenoptra crit chance and " + mp.CurrentBees * 2 + "% increased hymenoptra damage while your bees are in Attacking Mode");

                if (Left.IsMouseHovering)
                    Main.instance.MouseText("Left Click to switch your bees into Defending Mode\nYour defense is increased by "
                        + mp.CurrentBees * 2 + ", and you are granted a Honey shield which blocks one attack while your bees are in Defending Mode");

                if (Right.IsMouseHovering)
                    Main.instance.MouseText("Left Click to switch your bees into Gathering Mode\nYou are granted increased Honey regeneration, but deal 15% less damage while your bees are in Gathering Mode");
            }

            if (mp.CurrentBeeState == (int)BeeDamagePlayer.BeeState.Gathering)
            {
                Middle.SetImage(ModContent.Request<Texture2D>("BombusApisBee/UI/BeePlayerUI_Jar"));
                Left.SetImage(ModContent.Request<Texture2D>("BombusApisBee/UI/BeePlayerUI_Shield"));
                Right.SetImage(ModContent.Request<Texture2D>("BombusApisBee/UI/BeePlayerUI_Sword"));

                if (Middle.IsMouseHovering)
                    Main.instance.MouseText("Your bees are in Gathering Mode\nYou are granted increased Honey regeneration, but deal 15% less damage while your bees are in Gathering Mode");

                if (Left.IsMouseHovering)
                    Main.instance.MouseText("Left Click to switch your bees into Defending Mode\nYour defense is increased by "
                        + mp.CurrentBees * 2 + ", and you are granted a Honey shield which blocks one attack while your bees are in Defending Mode");

                if (Right.IsMouseHovering)
                    Main.instance.MouseText("Left Click to switch your bees into Attacking Mode\nYou are granted "
                        + mp.CurrentBees + "% increased hymenoptra crit chance and " + mp.CurrentBees * 2 + "% increased hymenoptra damage while your bees are in Attacking Mode");
            }

            if (Middle.IsMouseHovering || Left.IsMouseHovering || Right.IsMouseHovering)
                Main.LocalPlayer.mouseInterface = true;


            panel.Width.Set(MathHelper.Lerp(45, 135, UITimer / 5f), 0f);
        }
    }
}
