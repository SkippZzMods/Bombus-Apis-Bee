namespace BombusApisBee.Core
{
    public static class BombusApisBee_DoDetours
    {
        public static void Load()
        {
            On.Terraria.Main.DrawInfernoRings += PlayerDraw;
            //On.Terraria.Main.DoDraw += Main_DoDraw;
        }

        /*private static void Main_DoDraw(On.Terraria.Main.orig_DoDraw orig, Main self, GameTime gameTime)
        {
            orig.Invoke(self, gameTime);

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.outOfRange && !player.dead && player.Hymenoptra().CurrentBeeState == (int)BeeDamagePlayer.BeeState.Defense && player.Hymenoptra().HoldingBeeWeaponTimer > 0 && !Main.gamePaused && !Main.mapFullscreen)
                {
                    Effect effect = Terraria.Graphics.Effects.Filters.Scene["HoneyShieldShader"].GetShader().Shader;
                    effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 0.1f);
                    effect.Parameters["blowUpPower"].SetValue(3f);
                    effect.Parameters["blowUpSize"].SetValue(1f);

                    float mult = (1f - player.Hymenoptra().HoneyShieldCD / (float)player.Hymenoptra().MaxHoneyShieldCD) * player.Hymenoptra().HoldingBeeWeaponTimer / 15f;


                    float noiseScale = MathHelper.Lerp(0.45f, 0.65f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.1f) + 1f);
                    effect.Parameters["noiseScale"].SetValue(noiseScale);
                    float opacity = 0.35f * mult;
                    effect.Parameters["shieldOpacity"].SetValue(opacity);
                    effect.Parameters["shieldEdgeColor"].SetValue((new Color(255, 200, 20) * mult).ToVector3());
                    effect.Parameters["shieldEdgeBlendStrenght"].SetValue(5f);

                    effect.Parameters["shieldColor"].SetValue((new Color(255, 100, 20) * mult).ToVector3());

                    effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
                    effect.Parameters["power"].SetValue(0.15f);
                    effect.Parameters["offset"].SetValue(new Vector2(Main.screenPosition.X / Main.screenWidth * 0.5f, 0));
                    effect.Parameters["speed"].SetValue(15f);

                    Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value;
                    Vector2 pos = new Vector2(Main.player[i].Center.X, Main.player[i].Center.Y + Main.player[i].gfxOffY) - Main.screenPosition;

                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.GameViewMatrix.TransformationMatrix);

                    Main.spriteBatch.Draw(tex, pos, null, Color.White, 0f, tex.Size() / 2f, 0.215f, 0, 0f);

                    Main.spriteBatch.End();
                }
            }
        }*/

        public static void Unload()
        {
            On.Terraria.Main.DrawInfernoRings -= PlayerDraw;
            //On.Terraria.Main.DoDraw -= Main_DoDraw;
        }

        private static void PlayerDraw(On.Terraria.Main.orig_DrawInfernoRings orig, Main self)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.outOfRange && !player.dead)
                {
                    if (player.Bombus().LihzardRelicTimer > 0)
                    {
                        var modPlayer = player.Bombus();
                        Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/BloomFlare").Value;
                        Texture2D texBloom = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
                        Vector2 pos = new Vector2(player.Center.X, player.Center.Y + player.gfxOffY) - Main.screenPosition;

                        Main.spriteBatch.Draw(tex, pos, null, new Color(223, 170, 40, 0) * 0.5f, modPlayer.LihzardRelicTimer * 0.025f, tex.Size() / 2f, MathHelper.Lerp(1.25f, 0.05f, 1f - modPlayer.LihzardRelicTimer / 480f), 0, 0f);

                        Main.spriteBatch.Draw(tex, pos, null, new Color(245, 245, 149, 0) * 0.5f, modPlayer.LihzardRelicTimer * -0.0265f, tex.Size() / 2f, MathHelper.Lerp(1.1f, 0.05f, 1f - modPlayer.LihzardRelicTimer / 480f), 0, 0f);

                        Main.spriteBatch.Draw(tex, pos, null, new Color(245, 245, 149, 0) * 0.55f, modPlayer.LihzardRelicTimer * 0.0165f, tex.Size() / 2f, MathHelper.Lerp(0.9f, 0.05f, 1f - modPlayer.LihzardRelicTimer / 480f), 0, 0f);

                        Main.spriteBatch.Draw(texBloom, pos, null, new Color(223, 170, 40, 0) * 0.5f, modPlayer.LihzardRelicTimer * 0.025f, tex.Size() / 2f, MathHelper.Lerp(1.25f, 0.05f, 1f - modPlayer.LihzardRelicTimer / 480f), 0, 0f);

                        Main.spriteBatch.Draw(texBloom, pos, null, new Color(245, 245, 149, 0) * 0.5f, modPlayer.LihzardRelicTimer * 0.0265f, tex.Size() / 2f, MathHelper.Lerp(1.1f, 0.05f, 1f - modPlayer.LihzardRelicTimer / 480f), 0, 0f);
                    }
                }
            }
            orig.Invoke(self);
        }
    }
}
