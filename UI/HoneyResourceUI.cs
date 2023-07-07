using ReLogic.Graphics;

namespace BombusApisBee.UI
{
    // some of the stuff to make the draw effects is bad i know
    public class HoneyResourceUI
    {
        internal static int ReservedToBe;
        internal static bool Dragging;
        public static void Draw()
        {
            if (ModContent.GetInstance<BombusConfig>().DrawLegacyUI)
            {
                switch (Main.ResourceSetsManager.ActiveSetKeyName)
                {
                    case "HorizontalBars": DrawHoneyBar(); break;
                    case "New": DrawNewHoney(); break;
                    case "Default": DrawLegacyHoney(); break;
                }
            }
            else
            {
                DrawModernUI();
            }
        }

        public static void DrawModernUI()
        {
            Texture2D slotTex = ModContent.Request<Texture2D>("BombusApisBee/UI/HoneyResourceUI_Slot").Value;
            Texture2D fillTex = ModContent.Request<Texture2D>("BombusApisBee/UI/HoneyResourceUI_Fill").Value;
            Texture2D reserveTex = ModContent.Request<Texture2D>("BombusApisBee/UI/HoneyResourceUI_FillReservedl").Value;

            var mp = Main.LocalPlayer.Hymenoptra();

            var cfg = GetInstance<BombusConfig>();

            if (ReservedToBe < mp.BeeResourceReserved)
                ReservedToBe++;
            else if (ReservedToBe > mp.BeeResourceReserved)
                ReservedToBe--;

            for (int i = 0; i < GetHoneyAmount(); i++)
            {
                float uiScale = 1f + (Main.UIScale * 0.5f / 100);
                Vector2 drawPos = new Vector2(Main.screenWidth - (i * 16), (i % 2 == 0 ? -16 : 0f)) + ((cfg.ResourceOffX == 0 && cfg.ResourceOffY == 0) ? new Vector2(-320, 50) : new Vector2(cfg.ResourceOffX, cfg.ResourceOffY));
                Main.spriteBatch.Draw(slotTex, drawPos, null, Color.White, 0f, slotTex.Size() / 2f, uiScale, 0f, 0f);

                float barScale = Utils.GetLerpValue(15 * i, 15 * (float)(i + 1), mp.BeeResourceCurrent, true);

                Main.spriteBatch.Draw(fillTex, drawPos, null, Color.White * barScale, 0f, fillTex.Size() / 2f, barScale * uiScale, 0f, 0f);

                float reserveRatio = Utils.GetLerpValue(15 * i, 15 * (float)(i + 1), ReservedToBe, true);
                Rectangle barRectangle = new Rectangle(0, 0, reserveTex.Width, (int)MathHelper.Lerp(0f, reserveTex.Height, reserveRatio));

                Main.spriteBatch.Draw(reserveTex, drawPos, barRectangle, Color.White, 0f, reserveTex.Size() / 2f, uiScale * uiScale, 0f, 0f);
            }

            Vector2 start = new Vector2(Main.screenWidth, 0f) + ((cfg.ResourceOffX == 0 || cfg.ResourceOffY == 0) ? new Vector2(-320, 30) : new Vector2(cfg.ResourceOffX, cfg.ResourceOffY - 20));

            Rectangle bounds = new Rectangle((int)start.X - 15 * GetHoneyAmount(), (int)start.Y, 25 * GetHoneyAmount(), 35);
            Rectangle mouse = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, Dragging ? 50 : 8, Dragging ? 50 : 8);

            if (Dragging)
                bounds.Inflate(200, 200);

            if (mouse.Intersects(bounds))
            {
                if (mp.BeeResourceMax2 > 0f)
                {
                    string reservedString = "";
                    if (mp.BeeResourceReserved > 0)
                        reservedString = ", Reserved: " + mp.BeeResourceReserved + "/" + mp.BeeResourceMax2;

                    int HoneyInt = (int)Math.Round((double)(mp.BeeResourceCurrent));
                    int maxHoneyInt = (int)Math.Round((double)(mp.BeeResourceMax2));
                    Main.instance.MouseText(string.Concat(new object[]
                    {"Honey: ", HoneyInt, "/", maxHoneyInt + reservedString}) ?? "", 0, 0, -1, -1, -1, -1);
                }

                if (Main.mouseLeft && cfg.DragUI)
                {
                    cfg.ResourceOffX = (int)(Main.MouseScreen.X - Main.screenWidth);
                    cfg.ResourceOffY = (int)(Main.MouseScreen.Y);
                    Dragging = true;
                }
            }

            if (Dragging && !Main.mouseLeft)
                Dragging = false;
        }

        public static int GetHoneyAmount()
        {
            var modPlayer = Main.LocalPlayer.GetModPlayer<BeeDamagePlayer>();
            return Utils.Clamp(modPlayer.BeeResourceMax2 / 15, 0, 20);
        }

        #region Legacy
        public static bool[] doChainEffectBar = new bool[4];

        public static bool[] drawBackGlowBar = new bool[4];

        public static int[] chainEffectTimerBar = new int[4];

        public static int[] chainGlowTimerBar = new int[4];

        public static bool[] doChainEffectNew = new bool[10];

        public static bool[] drawBackGlowNew = new bool[10];

        public static int[] chainEffectTimerNew = new int[10];

        public static int[] chainGlowTimerNew = new int[10];

        public static bool[] doChainEffectClassic = new bool[20];

        public static bool[] drawBackGlowClassic = new bool[20];

        public static int[] chainEffectTimerClassic = new int[20];

        public static int[] chainGlowTimerClassic = new int[20];

        public static void DrawHoneyBar()
        {
            var modPlayer = Main.LocalPlayer.GetModPlayer<BeeDamagePlayer>();
            Texture2D honeyTexture = (Texture2D)ModContent.Request<Texture2D>("BombusApisBee/UI/BarStyle_HoneyBarHoney");
            Texture2D barTexture = (Texture2D)ModContent.Request<Texture2D>("BombusApisBee/UI/BarStyle_HoneyBar");
            Texture2D frameTexture = (Texture2D)ModContent.Request<Texture2D>("BombusApisBee/UI/BarStyle_HoneyBarFrame");

            Texture2D reservedTexture = ModContent.Request<Texture2D>("BombusApisBee/UI/BarStyle_HoneyBarReserved").Value;

            Texture2D chainTex0 = ModContent.Request<Texture2D>("BombusApisBee/UI/BarStyle_HoneyBarChains_Phase0").Value;
            Texture2D chainTex1 = ModContent.Request<Texture2D>("BombusApisBee/UI/BarStyle_HoneyBarChains_Phase1").Value;
            Texture2D chainTex2 = ModContent.Request<Texture2D>("BombusApisBee/UI/BarStyle_HoneyBarChains_Phase2").Value;
            Texture2D chainTex3 = ModContent.Request<Texture2D>("BombusApisBee/UI/BarStyle_HoneyBarChains_Phase3").Value;
            Texture2D chainTex4 = ModContent.Request<Texture2D>("BombusApisBee/UI/BarStyle_HoneyBarChains_Phase4").Value;

            Texture2D chainTex1Glow = ModContent.Request<Texture2D>("BombusApisBee/UI/BarStyle_HoneyBarChains_Phase1_Glow").Value;
            Texture2D chainTex2Glow = ModContent.Request<Texture2D>("BombusApisBee/UI/BarStyle_HoneyBarChains_Phase2_Glow").Value;
            Texture2D chainTex3Glow = ModContent.Request<Texture2D>("BombusApisBee/UI/BarStyle_HoneyBarChains_Phase3_Glow").Value;
            Texture2D chainTex4Glow = ModContent.Request<Texture2D>("BombusApisBee/UI/BarStyle_HoneyBarChains_Phase4_Glow").Value;

            float uiScale = 1f + (Main.UIScale * 0.5f / 100);
            Vector2 drawPos = new Vector2(Main.screenWidth - 34, 140);
            Main.spriteBatch.Draw(frameTexture, drawPos, null, Color.White, 0f, frameTexture.Size() / 2f, uiScale, SpriteEffects.None, 0f);
            float completionRatio = (float)modPlayer.BeeResourceCurrent / (float)modPlayer.BeeResourceMax2;
            Rectangle barRectangle = new Rectangle(0, 0, barTexture.Width, (int)((float)barTexture.Height * completionRatio));
            Vector2 drawPos2 = new Vector2(Main.screenWidth - 32, 162);
            Main.spriteBatch.Draw(barTexture, drawPos2, barRectangle, Color.White, 0f, barTexture.Size() / 2f, uiScale, SpriteEffects.None, 0f);

            if (modPlayer.BeeResourceReserved > 0)
            {
                completionRatio = (float)modPlayer.BeeResourceReserved / (float)modPlayer.BeeResourceMax2;
                barRectangle = new Rectangle(0, 0, reservedTexture.Width, (int)((float)reservedTexture.Height * completionRatio));

                Main.spriteBatch.Draw(reservedTexture, drawPos2, barRectangle, Color.White, 0f, reservedTexture.Size() / 2f, uiScale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(honeyTexture, drawPos, null, Color.White, 0f, honeyTexture.Size() / 2f, uiScale, 0, 0f);

            if (modPlayer.BeeResourceReserved > 0)
            {
                Main.spriteBatch.Draw(chainTex0, drawPos, null, Color.White * completionRatio, 0f, chainTex0.Size() / 2f, uiScale, SpriteEffects.None, 0f);

                if (drawBackGlowBar[0])
                    Main.spriteBatch.Draw(chainTex1Glow, drawPos, null, new Color(255, 175, 0, 0) * MathHelper.Lerp(0, 1, (modPlayer.BeeResourceReserved / (float)modPlayer.BeeResourceMax2) * 4), 0f, chainTex1Glow.Size() / 2f, uiScale, SpriteEffects.None, 0f);

                Main.spriteBatch.Draw(chainTex1, drawPos, null, Color.White * MathHelper.Lerp(0, 1, (modPlayer.BeeResourceReserved / (float)modPlayer.BeeResourceMax2) * 4), 0f, chainTex1.Size() / 2f, uiScale, SpriteEffects.None, 0f);

                if (chainGlowTimerBar[0] > 0)
                {
                    Main.spriteBatch.Draw(chainTex1Glow, drawPos, null, new Color(255, 175, 0, 0) * (chainGlowTimerBar[0] / 30f), 0f, chainTex1Glow.Size() / 2f, uiScale, SpriteEffects.None, 0f);
                }

                if (completionRatio > 0.25f)
                {
                    float ratio = modPlayer.BeeResourceMax2 * 0.25f;

                    if (drawBackGlowBar[1])
                        Main.spriteBatch.Draw(chainTex2Glow, drawPos, null, new Color(255, 175, 0, 0), 0f, chainTex2Glow.Size() / 2f, uiScale, SpriteEffects.None, 0f);

                    Main.spriteBatch.Draw(chainTex2, drawPos, null, Color.White * MathHelper.Lerp(0, 1, (modPlayer.BeeResourceReserved - ratio) / (float)(modPlayer.BeeResourceMax2 / 4f)), 0f, chainTex2.Size() / 2f, uiScale, SpriteEffects.None, 0f);

                    if (chainGlowTimerBar[1] > 0)
                    {
                        Main.spriteBatch.Draw(chainTex2Glow, drawPos, null, new Color(255, 175, 0, 0) * (chainGlowTimerBar[1] / 30f), 0f, chainTex2Glow.Size() / 2f, uiScale, SpriteEffects.None, 0f);
                    }

                    if (doChainEffectBar[0])
                    {
                        chainEffectTimerBar[0] = 10;
                        doChainEffectBar[0] = false;
                    }

                    if (chainEffectTimerBar[0] > 0)
                    {
                        float scale = MathHelper.Lerp(2f, 1f, 1f - (chainEffectTimerBar[0] / 10f));

                        Vector2 offset = Vector2.Lerp(new Vector2(0, 40), Vector2.Zero, 1f - (chainEffectTimerBar[0] / 10f));

                        Main.spriteBatch.Draw(chainTex1, drawPos + offset, null, Color.White * 0.5f, 0f, (chainTex1.Size()) / 2f, uiScale * scale, SpriteEffects.None, 0f);

                        if (chainEffectTimerBar[0] == 1)
                        {
                            drawBackGlowBar[0] = true;
                            chainGlowTimerBar[0] = 30;

                            Main.LocalPlayer.Bombus().AddShake(15);
                        }
                    }
                }
                else
                {
                    doChainEffectBar[0] = true;
                    drawBackGlowBar[0] = false;
                }

                if (completionRatio > 0.5f)
                {
                    float ratio = modPlayer.BeeResourceMax2 * 0.5f;

                    if (drawBackGlowBar[2])
                        Main.spriteBatch.Draw(chainTex3Glow, drawPos, null, new Color(255, 175, 0, 0), 0f, chainTex3Glow.Size() / 2f, uiScale, SpriteEffects.None, 0f);

                    Main.spriteBatch.Draw(chainTex3, drawPos, null, Color.White * MathHelper.Lerp(0, 1, (modPlayer.BeeResourceReserved - ratio) / (float)(modPlayer.BeeResourceMax2 / 4f)), 0f, chainTex3.Size() / 2f, uiScale, SpriteEffects.None, 0f);

                    if (chainGlowTimerBar[2] > 0)
                    {
                        Main.spriteBatch.Draw(chainTex3Glow, drawPos, null, new Color(255, 175, 0, 0) * (chainGlowTimerBar[2] / 30f), 0f, chainTex3Glow.Size() / 2f, uiScale, SpriteEffects.None, 0f);
                    }

                    if (doChainEffectBar[1])
                    {
                        chainEffectTimerBar[1] = 10;
                        doChainEffectBar[1] = false;
                    }

                    if (chainEffectTimerBar[1] > 0)
                    {
                        Vector2 offset = Vector2.Lerp(new Vector2(0, -10), Vector2.Zero, 1f - (chainEffectTimerBar[1] / 10f));

                        Main.spriteBatch.Draw(chainTex2, drawPos + offset, null, Color.White * 0.5f, 0f, chainTex2.Size() / 2f, uiScale * MathHelper.Lerp(2f, 1f, 1f - (chainEffectTimerBar[1] / 10f)), SpriteEffects.None, 0f);

                        if (chainEffectTimerBar[1] == 1)
                        {
                            drawBackGlowBar[1] = true;
                            chainGlowTimerBar[1] = 30;

                            Main.LocalPlayer.Bombus().AddShake(15);
                        }
                    }
                }
                else
                {
                    doChainEffectBar[1] = true;
                    drawBackGlowBar[1] = false;
                }

                if (completionRatio > 0.75f)
                {
                    float ratio = modPlayer.BeeResourceMax2 * 0.75f;

                    if (drawBackGlowBar[3])
                        Main.spriteBatch.Draw(chainTex4Glow, drawPos, null, new Color(255, 175, 0, 0), 0f, chainTex4Glow.Size() / 2f, uiScale, SpriteEffects.None, 0f);

                    Main.spriteBatch.Draw(chainTex4, drawPos, null, Color.White * MathHelper.Lerp(0, 1, (modPlayer.BeeResourceReserved - ratio) / (float)(modPlayer.BeeResourceMax2 / 4f)), 0f, chainTex4.Size() / 2f, uiScale, SpriteEffects.None, 0f);

                    if (chainGlowTimerBar[3] > 0)
                    {
                        Main.spriteBatch.Draw(chainTex4Glow, drawPos, null, new Color(255, 175, 0, 0) * (chainGlowTimerBar[3] / 30f), 0f, chainTex4Glow.Size() / 2f, uiScale, SpriteEffects.None, 0f);
                    }

                    if (doChainEffectBar[2])
                    {
                        chainEffectTimerBar[2] = 10;
                        doChainEffectBar[2] = false;
                    }

                    if (chainEffectTimerBar[2] > 0)
                    {
                        Vector2 offset = Vector2.Lerp(new Vector2(0, -60), Vector2.Zero, 1f - (chainEffectTimerBar[2] / 10f));

                        Main.spriteBatch.Draw(chainTex3, drawPos + offset, null, Color.White * 0.5f, 0f, chainTex3.Size() / 2f, uiScale * MathHelper.Lerp(2f, 1f, 1f - (chainEffectTimerBar[2] / 10f)), SpriteEffects.None, 0f);

                        if (chainEffectTimerBar[2] == 1)
                        {
                            chainGlowTimerBar[2] = 30;
                            drawBackGlowBar[2] = true;

                            Main.LocalPlayer.Bombus().AddShake(15);
                        }
                    }
                }
                else
                {
                    doChainEffectBar[2] = true;
                    drawBackGlowBar[2] = false;
                }

                if (completionRatio >= 1f)
                {
                    if (doChainEffectBar[3])
                    {
                        chainEffectTimerBar[3] = 10;
                        doChainEffectBar[3] = false;
                    }

                    if (chainEffectTimerBar[3] > 0)
                    {
                        Vector2 offset = Vector2.Lerp(new Vector2(0, -80), Vector2.Zero, 1f - (chainEffectTimerBar[3] / 10f));

                        Main.spriteBatch.Draw(chainTex4, drawPos + offset, null, Color.White * 0.5f, 0f, chainTex4.Size() / 2f, uiScale * MathHelper.Lerp(2f, 1f, 1f - (chainEffectTimerBar[3] / 10f)), SpriteEffects.None, 0f);

                        if (chainEffectTimerBar[3] == 1)
                        {
                            chainGlowTimerBar[3] = 30;
                            drawBackGlowBar[3] = true;

                            Main.LocalPlayer.Bombus().AddShake(8);
                        }
                    }
                }
                else
                {
                    doChainEffectBar[3] = true;
                    drawBackGlowBar[3] = false;
                }
            }
            else
            {
                if (!doChainEffectBar[0] || !doChainEffectBar[1] || !doChainEffectBar[2] || !doChainEffectBar[3])
                {
                    for (int i = 0; i < doChainEffectBar.Length; i++)
                    {
                        doChainEffectBar[i] = true;
                    }
                }

                if (!drawBackGlowBar[0] || !drawBackGlowBar[1] || !drawBackGlowBar[2] || !drawBackGlowBar[3])
                {
                    for (int i = 0; i < drawBackGlowBar.Length; i++)
                    {
                        drawBackGlowBar[i] = false;
                    }
                }
            }

            Rectangle mouse = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);
            Rectangle HoneyBar = Utils.CenteredRectangle(drawPos, Utils.Size(frameTexture) * uiScale);
            if (HoneyBar.Intersects(mouse) && modPlayer.BeeResourceMax2 > 0f)
            {
                string reservedString = "";
                if (modPlayer.BeeResourceReserved > 0)
                    reservedString = ", Reserved: " + modPlayer.BeeResourceReserved + "/" + modPlayer.BeeResourceMax2;

                int HoneyInt = (int)Math.Round((double)(modPlayer.BeeResourceCurrent));
                int maxHoneyInt = (int)Math.Round((double)(modPlayer.BeeResourceMax2));
                Main.instance.MouseText(string.Concat(new object[]
                {"Honey: ", HoneyInt, "/", maxHoneyInt + reservedString}) ?? "", 0, 0, -1, -1, -1, -1);
            }

            for (int i = 0; i < 4; i++)
            {
                if (chainEffectTimerBar[i] > 0)
                    chainEffectTimerBar[i]--;

                if (chainGlowTimerBar[i] > 0)
                    chainGlowTimerBar[i]--;
            }
        }

        public static void DrawNewHoney()
        {
            var modPlayer = Main.LocalPlayer.GetModPlayer<BeeDamagePlayer>();
            Texture2D frameTexture = (Texture2D)ModContent.Request<Texture2D>("BombusApisBee/UI/NewStyle_HoneyFrame");
            Texture2D barTexture = (Texture2D)ModContent.Request<Texture2D>("BombusApisBee/UI/NewStyle_HoneyBar");

            Texture2D chainTexture = ModContent.Request<Texture2D>("BombusApisBee/UI/NewStyle_HoneyBarReserved").Value;
            Texture2D chainGlowTex = ModContent.Request<Texture2D>("BombusApisBee/UI/NewStyle_HoneyBarReserved_Glow").Value;

            Texture2D greyTexture = ModContent.Request<Texture2D>("BombusApisBee/UI/NewStyle_HoneyBarGrayed").Value;

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

                rect = chainTexture.Frame();
                origin = rect.Size() * 0.5f;

                if (modPlayer.BeeResourceReserved / 20f > i)
                {
                    if (i != GetNewHoneyAmount() - 1)
                    {
                        float ratio = (modPlayer.BeeResourceReserved - (20 * i)) / 20f;

                        Main.spriteBatch.Draw(greyTexture, drawPos, rect, Color.White * ratio * barScale, 0f, origin, barScale, SpriteEffects.None, 0f);

                        if (drawBackGlowNew[i])
                            Main.spriteBatch.Draw(chainGlowTex, drawPos, null, new Color(255, 175, 0, 0), 0f, chainGlowTex.Size() / 2f, uiScale, SpriteEffects.None, 0f);

                        Main.spriteBatch.Draw(chainTexture, drawPos, rect, Color.White * ratio, 0f, origin, uiScale, SpriteEffects.None, 0f);

                        if (chainGlowTimerNew[i] > 0)
                            Main.spriteBatch.Draw(chainGlowTex, drawPos, null, new Color(255, 175, 0, 0) * (chainGlowTimerNew[i] / 30f), 0f, chainGlowTex.Size() / 2f, uiScale, SpriteEffects.None, 0f);

                        if (ratio >= 1f)
                        {
                            if (doChainEffectNew[i])
                            {
                                chainEffectTimerNew[i] = 10;
                                doChainEffectNew[i] = false;
                            }

                            if (chainEffectTimerNew[i] > 0)
                            {
                                Main.spriteBatch.Draw(chainTexture, drawPos, null, Color.White * 0.5f, 0f, chainTexture.Size() / 2f, uiScale * MathHelper.Lerp(2f, 1f, 1f - (chainEffectTimerNew[i] / 10f)), SpriteEffects.None, 0f);

                                if (chainEffectTimerNew[i] == 1)
                                {
                                    chainGlowTimerNew[i] = 30;
                                    drawBackGlowNew[i] = true;

                                    Main.LocalPlayer.Bombus().AddShake(6);
                                }
                            }
                        }
                        else
                        {
                            doChainEffectNew[i] = true;
                            drawBackGlowNew[i] = false;
                        }
                    }
                    else
                    {
                        float leftover = modPlayer.BeeResourceMax2 - (20 * (modPlayer.BeeResourceMax2 / 20));

                        float ratio = ((modPlayer.BeeResourceReserved - (20 * i))) / (20f + leftover);

                        Main.spriteBatch.Draw(greyTexture, drawPos, rect, Color.White * ratio * barScale, 0f, origin, barScale, SpriteEffects.None, 0f);

                        if (drawBackGlowNew[i])
                            Main.spriteBatch.Draw(chainGlowTex, drawPos, null, new Color(255, 175, 0, 0), 0f, chainGlowTex.Size() / 2f, uiScale, SpriteEffects.None, 0f);

                        Main.spriteBatch.Draw(chainTexture, drawPos, rect, Color.White * ratio, 0f, origin, uiScale, SpriteEffects.None, 0f);

                        if (chainGlowTimerNew[i] > 0)
                            Main.spriteBatch.Draw(chainGlowTex, drawPos, null, new Color(255, 175, 0, 0) * (chainGlowTimerNew[i] / 30f), 0f, chainGlowTex.Size() / 2f, uiScale, SpriteEffects.None, 0f);

                        if (ratio >= 1f)
                        {
                            if (doChainEffectNew[i])
                            {
                                chainEffectTimerNew[i] = 10;
                                doChainEffectNew[i] = false;
                            }

                            if (chainEffectTimerNew[i] > 0)
                            {
                                Main.spriteBatch.Draw(chainTexture, drawPos, null, Color.White * 0.5f, 0f, chainTexture.Size() / 2f, uiScale * MathHelper.Lerp(2f, barScale, 1f - (chainEffectTimerNew[i] / 10f)), SpriteEffects.None, 0f);

                                if (chainEffectTimerNew[i] == 1)
                                {
                                    chainGlowTimerNew[i] = 30;
                                    drawBackGlowNew[i] = true;

                                    Main.LocalPlayer.Bombus().AddShake(6);
                                }
                            }
                        }
                        else
                        {
                            doChainEffectNew[i] = true;
                            drawBackGlowNew[i] = false;
                        }
                    }
                }
                else
                {
                    doChainEffectNew[i] = true;
                    drawBackGlowNew[i] = false;
                }

                Rectangle mouse = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);
                Rectangle HoneyBar = Utils.CenteredRectangle(drawPos, Utils.Size(frameTexture) * uiScale);
                if (HoneyBar.Intersects(mouse) && modPlayer.BeeResourceMax2 > 0f)
                {
                    drawText = true;
                }
            }

            if (drawText)
            {
                string reservedString = "";
                if (modPlayer.BeeResourceReserved > 0)
                    reservedString = ", Reserved: " + modPlayer.BeeResourceReserved + "/" + modPlayer.BeeResourceMax2;

                int HoneyInt = (int)Math.Round((double)(modPlayer.BeeResourceCurrent));
                int maxHoneyInt = (int)Math.Round((double)(modPlayer.BeeResourceMax2));
                Main.instance.MouseText(string.Concat(new object[]
                {"Honey: ", HoneyInt, "/", maxHoneyInt + reservedString}) ?? "", 0, 0, -1, -1, -1, -1);
            }

            for (int i = 0; i < 10; i++)
            {
                if (chainEffectTimerNew[i] > 0)
                    chainEffectTimerNew[i]--;

                if (chainGlowTimerNew[i] > 0)
                    chainGlowTimerNew[i]--;
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
            Texture2D chainTexture = ModContent.Request<Texture2D>("BombusApisBee/UI/NewStyle_HoneyBarReserved").Value;
            Texture2D chainGlowTex = ModContent.Request<Texture2D>("BombusApisBee/UI/NewStyle_HoneyBarReserved_Glow").Value;

            Texture2D greyTexture = ModContent.Request<Texture2D>("BombusApisBee/UI/NewStyle_HoneyBarGrayed").Value;

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

                rect = chainTexture.Frame();
                origin = rect.Size() * 0.5f;

                if (modPlayer.BeeResourceReserved / 10f > i)
                {
                    if (i != GetLegacyHoneyAmount() - 1)
                    {
                        float ratio = (modPlayer.BeeResourceReserved - (10 * i)) / 10f;

                        Main.spriteBatch.Draw(greyTexture, drawPos, rect, Color.White * ratio * barScale, 0f, origin, barScale, SpriteEffects.None, 0f);

                        if (drawBackGlowClassic[i])
                            Main.spriteBatch.Draw(chainGlowTex, drawPos, null, new Color(255, 175, 0, 0), 0f, chainGlowTex.Size() / 2f, uiScale, SpriteEffects.None, 0f);

                        Main.spriteBatch.Draw(chainTexture, drawPos, rect, Color.White * ratio, 0f, origin, uiScale, SpriteEffects.None, 0f);

                        if (chainGlowTimerClassic[i] > 0)
                            Main.spriteBatch.Draw(chainGlowTex, drawPos, null, new Color(255, 175, 0, 0) * (chainGlowTimerClassic[i] / 30f), 0f, chainGlowTex.Size() / 2f, uiScale, SpriteEffects.None, 0f);

                        if (ratio >= 1f)
                        {
                            if (doChainEffectClassic[i])
                            {
                                chainEffectTimerClassic[i] = 10;
                                doChainEffectClassic[i] = false;
                            }

                            if (chainEffectTimerClassic[i] > 0)
                            {
                                Main.spriteBatch.Draw(chainTexture, drawPos, null, Color.White * 0.5f, 0f, chainTexture.Size() / 2f, uiScale * MathHelper.Lerp(2f, 1f, 1f - (chainEffectTimerClassic[i] / 10f)), SpriteEffects.None, 0f);

                                if (chainEffectTimerClassic[i] == 1)
                                {
                                    chainGlowTimerClassic[i] = 30;
                                    drawBackGlowClassic[i] = true;

                                    Main.LocalPlayer.Bombus().AddShake(3);
                                }
                            }
                        }
                        else
                        {
                            doChainEffectClassic[i] = true;
                            drawBackGlowClassic[i] = false;
                        }
                    }
                    else
                    {
                        float leftover = modPlayer.BeeResourceMax2 - (10 * (modPlayer.BeeResourceMax2 / 10));

                        float ratio = ((modPlayer.BeeResourceReserved - (10 * i))) / (10f + leftover);

                        Main.spriteBatch.Draw(greyTexture, drawPos, rect, Color.White * ratio * barScale, 0f, origin, barScale, SpriteEffects.None, 0f);

                        if (drawBackGlowClassic[i])
                            Main.spriteBatch.Draw(chainGlowTex, drawPos, null, new Color(255, 175, 0, 0), 0f, chainGlowTex.Size() / 2f, uiScale, SpriteEffects.None, 0f);

                        Main.spriteBatch.Draw(chainTexture, drawPos, rect, Color.White * ratio, 0f, origin, uiScale, SpriteEffects.None, 0f);

                        if (chainGlowTimerClassic[i] > 0)
                            Main.spriteBatch.Draw(chainGlowTex, drawPos, null, new Color(255, 175, 0, 0) * (chainGlowTimerClassic[i] / 30f), 0f, chainGlowTex.Size() / 2f, uiScale, SpriteEffects.None, 0f);

                        if (ratio >= 1f)
                        {
                            if (doChainEffectClassic[i])
                            {
                                chainEffectTimerClassic[i] = 10;
                                doChainEffectClassic[i] = false;
                            }

                            if (chainEffectTimerClassic[i] > 0)
                            {
                                Main.spriteBatch.Draw(chainTexture, drawPos, null, Color.White * 0.5f, 0f, chainTexture.Size() / 2f, uiScale * MathHelper.Lerp(2f, 1f, 1f - (chainEffectTimerClassic[i] / 10f)), SpriteEffects.None, 0f);

                                if (chainEffectTimerClassic[i] == 1)
                                {
                                    chainGlowTimerClassic[i] = 30;
                                    drawBackGlowClassic[i] = true;

                                    Main.LocalPlayer.Bombus().AddShake(3);
                                }
                            }
                        }
                        else
                        {
                            doChainEffectClassic[i] = true;
                            drawBackGlowClassic[i] = false;
                        }
                    }
                }
                else
                {
                    doChainEffectClassic[i] = true;
                    drawBackGlowClassic[i] = false;
                }

                Rectangle mouse = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);
                Rectangle HoneyBar = Utils.CenteredRectangle(drawPos, Utils.Size(barTexture) * uiScale);
                if (HoneyBar.Intersects(mouse) && modPlayer.BeeResourceMax2 > 0f)
                {
                    drawText = true;
                }
            }
            if (drawText)
            {
                string reservedString = "";
                if (modPlayer.BeeResourceReserved > 0)
                    reservedString = ", Reserved: " + modPlayer.BeeResourceReserved + "/" + modPlayer.BeeResourceMax2;

                int HoneyInt = (int)Math.Round((double)(modPlayer.BeeResourceCurrent));
                int maxHoneyInt = (int)Math.Round((double)(modPlayer.BeeResourceMax2));
                Main.instance.MouseText(string.Concat(new object[]
                {"Honey: ", HoneyInt, "/", maxHoneyInt + reservedString}) ?? "", 0, 0, -1, -1, -1, -1);
            }

            Vector2 drawPosition = new Vector2(Main.screenWidth - (GetLegacyHoneyAmount() > 10 ? 355 : 340), 5);
            DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.MouseText.Value, "Honey", drawPosition, new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor));


            for (int i = 0; i < 20; i++)
            {
                if (chainEffectTimerClassic[i] > 0)
                    chainEffectTimerClassic[i]--;

                if (chainGlowTimerClassic[i] > 0)
                    chainGlowTimerClassic[i]--;
            }
        }
        public static int GetLegacyHoneyAmount()
        {
            var modPlayer = Main.LocalPlayer.GetModPlayer<BeeDamagePlayer>();
            return Utils.Clamp(modPlayer.BeeResourceMax2 / 10, 0, 20);
        }
        #endregion Legacy
    }
}
