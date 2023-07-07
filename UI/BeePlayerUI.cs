using BombusApisBee.Core.UILoading;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using static BombusApisBee.BeeDamageClass.BeeDamagePlayer;

namespace BombusApisBee.UI
{
    public class BeePlayerUI : UIState
    {
        internal BeeState oldState; 
        internal BeeState state;

        internal int switchTimer;
        internal int[] glowTimer = new int[3];

        internal Vector2 PositionBottom = Vector2.Zero;
        internal Vector2 PositionRight = new Vector2(25f, -25f);
        internal Vector2 PositionLeft = new Vector2(-25f, -25f);

        internal static float YOffset => ModContent.GetInstance<BombusConfig>().yOffset;

        public override void Update(GameTime gameTime)
        {
            if (state == 0)
            {
                state = (BeeState)Main.LocalPlayer.Hymenoptra().CurrentBeeState;
            }

            if ((int)state != Main.LocalPlayer.Hymenoptra().CurrentBeeState)
            {
                oldState = state;
                state = (BeeState)Main.LocalPlayer.Hymenoptra().CurrentBeeState;
                switchTimer = 20;
            }

            if (switchTimer > 0)
                switchTimer--;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texOffense = ModContent.Request<Texture2D>("BombusApisBee/UI/BeePlayerUI_Offense").Value;
            Texture2D texGathering = ModContent.Request<Texture2D>("BombusApisBee/UI/BeePlayerUI_Gathering").Value;
            Texture2D texDefense = ModContent.Request<Texture2D>("BombusApisBee/UI/BeePlayerUI_Defense").Value;

            var mp = Main.LocalPlayer.Hymenoptra();

            if (mp.HeldBeeWeaponTimer > 0 || mp.HoldingBeeWeaponTimer > 0)
            {
                float fade = mp.HoldingBeeWeaponTimer / 15f;

                Color color = Color.White * fade;

                
                
                float lerper = 1f;
                if (switchTimer > 0)
                    lerper = 1f - switchTimer / 20f;

                DrawToken(texOffense, color, (int)BeeState.Offense, lerper);

                DrawToken(texGathering, color, (int)BeeState.Gathering, lerper);

                DrawToken(texDefense, color, (int)BeeState.Defense, lerper);
            }
        }

        internal void DrawToken(Texture2D tex, Color color, int type, float lerper)
        {
            Texture2D texGlow = ModContent.Request<Texture2D>("BombusApisBee/UI/BeePlayerUI_Glow").Value;
            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Vector2 position = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2) + new Vector2(0f, 65f + YOffset);
            float uiScale = 1f + (Main.UIScale * 0.5f / 100);

            Vector2 offset = Vector2.Lerp(GetOffset((int)oldState, type), GetOffset((int)state, type), lerper);

            float alpha = MathHelper.Lerp(GetAlpha((int)oldState, type), GetAlpha((int)state, type), lerper);

            float scale = MathHelper.Lerp(GetScale((int)oldState, type), GetScale((int)state, type), lerper);

            ref int timer = ref glowTimer[type - 1];

            if (alpha < 1f && timer > 0)
            {
                alpha = MathHelper.Lerp(alpha, 1f, timer / 10f);
            }


            if (Vector2.Distance(Main.MouseWorld - Main.screenPosition, position + offset) < 20f * scale)
            {
                Main.spriteBatch.Draw(glowTex, position + offset
                , null, Color.Lerp(Color.Transparent, new Color(220, 155, 20, 0), timer / 10f) * 0.45f, 0, glowTex.Size() / 2f, uiScale * scale, 0f, 0f);

                Main.spriteBatch.Draw(texGlow, position + offset
                , null, Color.Lerp(Color.Transparent, new Color(220, 155, 20, 0), timer / 10f), 0, texGlow.Size() / 2f, uiScale * scale, 0f, 0f);

                Main.instance.MouseText(GetHoverText((int)state, type), ItemRarityID.Yellow);
                Main.LocalPlayer.mouseInterface = true;

                if (Main.mouseLeft && Main.mouseLeftRelease && Main.LocalPlayer.Hymenoptra().StateSwitchCooldown <= 0 && Main.LocalPlayer.Hymenoptra().CurrentBeeState != type)
                {
                    Main.LocalPlayer.Hymenoptra().StateSwitchCooldown = 20;
                    Main.LocalPlayer.Hymenoptra().CurrentBeeState = type;
                }

                if (timer < 10)
                    timer++;
            }
            else
            {
                if (timer > 0)
                    timer--;
            }
                

            Main.spriteBatch.Draw(tex, position + offset
                 , null, color * alpha, 0, tex.Size() / 2f, uiScale * scale, 0f, 0f);
        }

        internal string GetHoverText(int state, int type)
        {
            var mp = Main.LocalPlayer.Hymenoptra();

            switch (state)
            {
                case (int)BeeState.Defense:
                    switch (type)
                    {
                        case (int)BeeState.Defense: return "Your bees are in Defending Mode\nYour defense is increased by "
                        + mp.CurrentBees * 2 + ", and you are granted a Honey shield which blocks one attack while your bees are in Defending Mode";

                        case (int)BeeState.Offense: return "Left Click to switch your bees into Attacking Mode\nYou are granted "
                        + mp.CurrentBees + "% increased hymenoptra crit chance and " + mp.CurrentBees * 2 + "% increased hymenoptra damage while your bees are in Attacking Mode";

                        case (int)BeeState.Gathering: return "Left Click to switch your bees into Gathering Mode\nYou are granted increased Honey regeneration, but deal 15% less damage while your bees are in Gathering Mode";
                    }
                    break;

                case (int)BeeState.Offense:
                    switch (type)
                    {
                        case (int)BeeState.Defense: return "Left Click to switch your bees into Defending Mode\nYour defense is increased by "
                        + mp.CurrentBees * 2 + ", and you are granted a Honey shield which blocks one attack while your bees are in Defending Mode";

                        case (int)BeeState.Offense: return "Your bees are in Attacking Mode\nYou are granted "
                        + mp.CurrentBees + "% increased hymenoptra crit chance and " + mp.CurrentBees * 2 + "% increased hymenoptra damage while your bees are in Attacking Mode";

                        case (int)BeeState.Gathering: return "Left Click to switch your bees into Gathering Mode\nYou are granted increased Honey regeneration, but deal 15% less damage while your bees are in Gathering Mode";
                    }
                    break;

                case 3:
                    switch (type)
                    {
                        case (int)BeeState.Defense: return "Left Click to switch your bees into Defending Mode\nYour defense is increased by "
                        + mp.CurrentBees * 2 + ", and you are granted a Honey shield which blocks one attack while your bees are in Defending Mode";

                        case (int)BeeState.Offense: return "Left Click to switch your bees into Attacking Mode\nYou are granted "
                        + mp.CurrentBees + "% increased hymenoptra crit chance and " + mp.CurrentBees * 2 + "% increased hymenoptra damage while your bees are in Attacking Mode";

                        case (int)BeeState.Gathering: return "Your bees are in Gathering Mode\nYou are granted increased Honey regeneration, but deal 15% less damage while your bees are in Gathering Mode";
                    }
                    break;
            }

            return "";
        }
        internal float GetScale(int state, int type)
        {
            switch (state)
            {
                case (int)BeeState.Defense:
                    switch (type)
                    {
                        case (int)BeeState.Defense: return 1.2f;

                        case (int)BeeState.Offense: return 0.85f;

                        case (int)BeeState.Gathering: return 0.85f;
                    }
                    break;

                case (int)BeeState.Offense:
                    switch (type)
                    {
                        case (int)BeeState.Defense: return 0.85f;

                        case (int)BeeState.Offense: return 1.2f;

                        case (int)BeeState.Gathering: return 0.85f;
                    }
                    break;

                case 3:
                    switch (type)
                    {
                        case (int)BeeState.Defense: return 0.85f;

                        case (int)BeeState.Offense: return 0.85f;

                        case (int)BeeState.Gathering: return 1.2f;
                    }
                    break;
            }

            return 1f;
        }

        internal float GetAlpha(int state, int type)
        {
            switch (state)
            {
                case (int)BeeState.Defense:
                    switch (type)
                    {
                        case (int)BeeState.Defense: return 1f;

                        case (int)BeeState.Offense: return 0.5f;

                        case (int)BeeState.Gathering: return 0.5f;
                    }
                    break;

                case (int)BeeState.Offense:
                    switch (type)
                    {
                        case (int)BeeState.Defense: return 0.5f;

                        case (int)BeeState.Offense: return 1f;

                        case (int)BeeState.Gathering: return 0.5f;
                    }
                    break;

                case 3:
                    switch (type)
                    {
                        case (int)BeeState.Defense: return 0.5f;

                        case (int)BeeState.Offense: return 0.5f;

                        case (int)BeeState.Gathering: return 1f;
                    }
                    break;
            }

            return 1f;
        }

        internal Vector2 GetOffset(int state, int type)
        {
            switch (state)
            {
                case (int)BeeState.Defense:
                    switch (type)
                    {
                        case (int)BeeState.Defense: return PositionBottom;

                        case (int)BeeState.Offense: return PositionLeft;

                        case (int)BeeState.Gathering: return PositionRight;
                    }
                    break;

                case (int)BeeState.Offense:
                    switch (type)
                    {
                        case (int)BeeState.Defense: return PositionRight;

                        case (int)BeeState.Offense: return PositionBottom;

                        case (int)BeeState.Gathering: return PositionLeft;
                    }
                    break;

                case 3:
                    switch (type)
                    {
                        case (int)BeeState.Defense: return PositionLeft;

                        case (int)BeeState.Offense: return PositionRight;

                        case (int)BeeState.Gathering: return PositionBottom;
                    }
                    break;
            }

            return Vector2.Zero;
        }
    }
}
/*{
    public class BeePlayerUI : UIState
    {
        internal UIPanel panel;

        internal UIImageButton Middle;

        internal UIImageButton Left;

        internal UIImageButton Right;

        internal UIImage Glow;

        internal UIImage Bar;

        internal int UITimer;

        internal float yOffset => ModContent.GetInstance<BombusConfig>().yOffset;

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

            Glow = new(ModContent.Request<Texture2D>("BombusApisBee/UI/BeePlayerUI_Shield_Glow"));
            Glow.VAlign = 0.5f;
            Glow.HAlign = 0.5f;
            Glow.Width.Set(22, 0);
            Glow.Height.Set(24, 0);
            Glow.Color = new Color(255, 200, 0, 0);

            panel.Append(Glow);
            panel.Append(Middle);

            Left.OnLeftClick += Left_OnClick;

            Right.OnLeftClick += Right_OnClick;
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

                Vector2 position = new Vector2(Main.screenWidth / 2 + 20 + MathHelper.Lerp(0, 45, UITimer / 5f), Main.screenHeight / 2 + 3 + yOffset);

                Main.spriteBatch.Draw(barInsideTex, position, null, Color.White);

                float completionRatio = mp.BeeResourceCurrent / (float)mp.BeeResourceMax2;
                Rectangle barRectangle = new Rectangle(0, 0, honeyBarTex.Width, (int)(honeyBarTex.Height * completionRatio));

                Main.spriteBatch.Draw(honeyBarTex, position + Vector2.UnitY * MathHelper.Lerp(0f, honeyBarTex.Height, 1f - completionRatio), barRectangle, Color.White);

                Main.spriteBatch.Draw(barTex, position, null, Color.White);

                float uiScale = 1f + (Main.UIScale * 0.5f / 100);

                Rectangle mouse = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);
                Rectangle HoneyBar = Utils.CenteredRectangle(position + Utils.Size(barTex) * uiScale * 0.5f, Utils.Size(barTex) * uiScale);
                if (HoneyBar.Intersects(mouse) && mp.BeeResourceMax2 > 0f)
                {
                    int HoneyInt = (int)Math.Round((double)(mp.BeeResourceCurrent));
                    int maxHoneyInt = (int)Math.Round((double)(mp.BeeResourceMax2));
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

        public override void Update(GameTime gameTime)
        {
            if (!(Main.LocalPlayer.Hymenoptra().HeldBeeWeaponTimer > 0 || Main.LocalPlayer.Hymenoptra().HoldingBeeWeaponTimer > 0))
                return;

            var mp = Main.LocalPlayer.Hymenoptra();

            panel.Top.Set(Main.screenHeight / 2 + yOffset, 0f);


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
                Glow.SetImage(ModContent.Request<Texture2D>("BombusApisBee/UI/BeePlayerUI_Shield_Glow"));
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
                Glow.SetImage(ModContent.Request<Texture2D>("BombusApisBee/UI/BeePlayerUI_Sword_Glow"));
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
                Glow.SetImage(ModContent.Request<Texture2D>("BombusApisBee/UI/BeePlayerUI_Jar_Glow"));
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

            Glow.Color = Color.Lerp(new Color(255, 200, 0, 0) * 0.75f, new Color(255, 200, 0, 0) * 0.35f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1.5f) + 1f);
        }
    }
}*/
