using BombusApisBee.Core.PixelationSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Effects;

namespace BombusApisBee.Content.Dusts
{
    public class ElectricSmoke : ModDust
    {
        public override string Texture => BombusApisBee.Invisible;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, 0, 4, 4);
            dust.customData = 1 + Main.rand.Next(3);
        }

        public override bool Update(Dust dust)
        {
            dust.velocity.Y -= 0.015f;
            dust.position += dust.velocity;
            dust.velocity *= 0.95f;
            dust.rotation += dust.velocity.Length() * 0.015f;

            dust.alpha += 8;

            dust.alpha = (int)(dust.alpha * 1.0075f);

            dust.scale *= 1.025f;

            if (dust.alpha >= 255)
                dust.active = false;

            Lighting.AddLight(dust.position, dust.color.ToVector3() * (1f - dust.alpha / 255f));

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            float lerper = 1f - dust.alpha / 255f;

            Color color = dust.color * lerper;

            Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/Assets/ExtraTextures/SmokeAlpha_" + dust.customData).Value;

            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            ModContent.GetInstance<PixelationSystem>().QueueRenderAction("OverPlayers", () =>
            {
                Effect effect = Filters.Scene["HolyShieldShader"].GetShader().Shader;

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);

                effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.005f);
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.005f);
                effect.Parameters["screenPos"].SetValue(Main.screenPosition * new Vector2(0.5f, 0.1f) / new Vector2(Main.screenWidth, Main.screenHeight));

                effect.Parameters["offset"].SetValue(new Vector2(0.001f));
                effect.Parameters["repeats"].SetValue(2);

                effect.Parameters["uImage1"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/Assets/ShaderTextures/ElectricNoise").Value);
                effect.Parameters["uImage2"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/Assets/ShaderTextures/ElectricNoise").Value);

                effect.Parameters["noiseImage1"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/Assets/ShaderTextures/ElectricNoise").Value);

                effect.Parameters["uColor"].SetValue(color.ToVector4());

                effect.CurrentTechnique.Passes[0].Apply();

                Main.spriteBatch.Draw(bloomTex, dust.position - Main.screenPosition, null, Color.White, dust.rotation, bloomTex.Size() / 2f, dust.scale, 0f, 0f);

                color = Color.White with { A = 0 } * lerper * 0.15f;

                effect.Parameters["uColor"].SetValue(color.ToVector4());

                effect.CurrentTechnique.Passes[0].Apply();

                Main.spriteBatch.Draw(bloomTex, dust.position - Main.screenPosition, null, Color.White, dust.rotation, bloomTex.Size() / 2f, dust.scale, 0f, 0f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.EffectMatrix);

                Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, color * 0.35f, dust.rotation, tex.Size() / 2f, dust.scale * 0.35f, 0f, 0f);
            });

            return false;
        }
    }
}
