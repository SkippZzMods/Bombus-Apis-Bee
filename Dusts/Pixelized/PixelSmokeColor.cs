﻿using BombusApisBee.Core.PixelationSystem;

namespace BombusApisBee.Dusts.Pixelized
{
    public class PixelSmokeColor : ModDust
    {
        public override string Texture => BombusApisBee.Invisible;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, 0, 4, 4);
        }

        public override bool Update(Dust dust)
        {
            if (dust.customData is not object[]) // i LOVE dusts :)
            {
                object[] pair = [dust.customData, 1 + Main.rand.Next(3)];
                dust.customData = pair;
            }

            dust.velocity.Y -= 0.015f;
            dust.position += dust.velocity;
            dust.velocity *= 0.95f;
            dust.rotation += dust.velocity.Length() * 0.01f;

            dust.alpha += 4;

            dust.alpha = (int)(dust.alpha * 1.0075f);

            dust.scale *= 1.03f;

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            float lerper = 1f - dust.alpha / 255f;

            Color? fadeColor = null;

            object[] pair = (object[])dust.customData;

            int variant = (int)pair[1];

            if (pair[0] is Color color_)
                fadeColor = color_;


            Color color = Color.Lerp(dust.color, fadeColor ?? Color.Black, EaseBuilder.EaseQuinticInOut.Ease(1f - lerper));

            Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/SmokeTransparent_" + variant).Value;
            ModContent.GetInstance<PixelationSystem>().QueueRenderAction("UnderProjectiles", () =>
            {
                Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, color * lerper, dust.rotation, tex.Size() / 2f, dust.scale, 0f, 0f);

                Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, color * lerper, dust.rotation + MathHelper.PiOver2, tex.Size() / 2f, dust.scale, 0f, 0f);
            });

            return false;
        }
    }
}
