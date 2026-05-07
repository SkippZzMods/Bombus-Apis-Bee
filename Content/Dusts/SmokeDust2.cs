using BombusApisBee.Core.Systems.PixelationSystem;

namespace BombusApisBee.Content.Dusts
{
    public class SmokeDust2 : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, Main.rand.Next(2), 32, 32);
            dust.rotation = Main.rand.NextFloat(6.28f);
        }

        public override bool PreDraw(Dust dust)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            GetInstance<PixelationSystem>().QueueRenderAction("Dusts", () =>
            {
                Main.spriteBatch.Draw(bloomTex, dust.position - Main.screenPosition, null, dust.color with { A = 0 } * 0.25f * (1f - dust.alpha / 255f), 0f, bloomTex.Size() / 2f, dust.scale * 1.25f, 0f, 0f);
            });

            Rectangle frame = tex.Frame(verticalFrames: 3, frameY: dust.frame.Y);

            Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, frame, dust.color * (1f - dust.alpha / 255f), dust.rotation, frame.Size() / 2f, dust.scale, 0f, 0f);

            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }

        public override bool Update(Dust dust)
        {
            dust.velocity *= 0.98f;
            dust.velocity.X *= 0.98f;
            dust.velocity.Y -= 0.035f;

            dust.position += dust.velocity;

            dust.alpha += 3;
            dust.scale *= 1.015f;

            if (dust.alpha >= 255)
                dust.active = false;

            Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.1f);

            return false;
        }
    }
}
