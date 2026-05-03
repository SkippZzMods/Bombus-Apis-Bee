using BombusApisBee.Core.PixelationSystem;

namespace BombusApisBee.Content.Dusts.Pixelized
{

    public class PixelatedEmber : ModDust
    {
        public override string Texture => BombusApisBee.Invisible;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, 0, 4, 4);
        }

        public override bool Update(Dust dust)
        {
            if (dust.customData is int)
                dust.customData = new object[] { dust.customData, false };

            object[] data = dust.customData as object[];

            dust.position += dust.velocity;
            dust.velocity *= 0.98f;
            dust.velocity = dust.velocity.RotatedBy(0.04f * (int)data[0]);

            if (dust.alpha > 80 && !(bool)data[1])
            {
                data[0] = (int)data[0] * -1;
                data[1] = true;
            }

            dust.alpha += 2;

            dust.alpha = (int)(dust.alpha * 1.01f);
            dust.scale *= 0.99f;

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            float lerper = 1f - dust.alpha / 255f;

            Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            ModContent.GetInstance<PixelationSystem>().QueueRenderAction("Dusts", () =>
            {
                Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, dust.color * lerper, dust.rotation, tex.Size() / 2f, dust.scale * lerper, 0f, 0f);

                float glowScale = dust.scale * 0.25f;

                Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, Color.White with { A = 0 } * lerper, dust.rotation, tex.Size() / 2f, glowScale * lerper, 0f, 0f);
            });

            return false;
        }
    }
}
