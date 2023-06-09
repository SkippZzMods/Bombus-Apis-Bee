using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Reflection;

namespace BombusApisBee.Core
{
    public static class BombusApisBee_DoIL
    {
        public static void Load()
        {
            IL.Terraria.Main.DoDraw += IL_InsertShield;
        }

        public static void Unload()
        {
            IL.Terraria.Main.DoDraw -= IL_InsertShield;
        }

        private static void IL_InsertShield(MonoMod.Cil.ILContext il)
        {
            var c = new ILCursor(il);

            if (!c.TryGotoNext(i => i.MatchLdcI4(36)))
                return;

            c.Index++;

            c.Emit(OpCodes.Call, typeof(BombusApisBee_DoIL).GetMethod(nameof(DrawHoneyShield), BindingFlags.NonPublic | BindingFlags.Static));
        }

        private static void DrawHoneyShield()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.outOfRange && !player.dead && player.Hymenoptra().CurrentBeeState == (int)BeeDamagePlayer.BeeState.Defense && player.Hymenoptra().HoldingBeeWeaponTimer > 0)
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
        }
    }
}
