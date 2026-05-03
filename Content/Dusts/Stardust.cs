using BombusApisBee.Core.PixelationSystem;

namespace BombusApisBee.Content.Dusts
{
    // set customData to true for rotation
    public class StarDust : ModDust
    {
        public override string Texture => BombusApisBee.Invisible;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, 0, 4, 4);
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.velocity *= 0.95f;

            if (dust.customData is not null && (bool)dust.customData)
                dust.rotation += dust.velocity.Length() * 0.05f;

            dust.alpha += 5;

            dust.alpha = (int)(dust.alpha * 1.01f);
            dust.scale *= 0.965f;

            if (dust.alpha >= 255)
                dust.active = false;

            Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.15f);

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Color color = dust.color;

            float lerper = 1f - dust.alpha / 255f;

            if (dust.customData is Color fadeColor)
                color = Color.Lerp(dust.color, fadeColor, EaseBuilder.EaseCubicOut.Ease(1f - lerper));

            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Texture2D starTex = ModContent.Request<Texture2D>("BombusApisBee/Assets/ExtraTextures/StarAlpha").Value;

            GetInstance<PixelationSystem>().QueueRenderAction("Dusts", () =>
            {
                Main.spriteBatch.Draw(bloomTex, dust.position - Main.screenPosition, null, color * lerper * 0.25f, dust.rotation, bloomTex.Size() / 2f, dust.scale * 2f * lerper, 0f, 0f);

                Main.spriteBatch.Draw(starTex, dust.position - Main.screenPosition, null, color * lerper, dust.rotation, starTex.Size() / 2f, dust.scale * lerper, 0f, 0f);

            });

            return false;
        }
    }
    public class StarDustWhite : ModDust
    {
        public override string Texture => BombusApisBee.Invisible;
        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, 0, 4, 4);
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.velocity *= 0.95f;

            if (dust.customData is not null && (bool)dust.customData)
                dust.rotation += dust.velocity.Length() * 0.05f;

            dust.alpha += 5;

            dust.alpha = (int)(dust.alpha * 1.01f);
            dust.scale *= 0.965f;

            if (dust.alpha >= 255)
                dust.active = false;

            Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.15f);

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Color color = dust.color;

            float lerper = 1f - dust.alpha / 255f;

            if (dust.customData is Color fadeColor)
                color = Color.Lerp(dust.color, fadeColor, EaseBuilder.EaseCubicOut.Ease(1f - lerper));

            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Texture2D starTex = ModContent.Request<Texture2D>("BombusApisBee/Assets/ExtraTextures/StarAlpha").Value;

            GetInstance<PixelationSystem>().QueueRenderAction("Dusts", () =>
            {
                Main.spriteBatch.Draw(bloomTex, dust.position - Main.screenPosition, null, color * lerper * 0.25f, dust.rotation, bloomTex.Size() / 2f, dust.scale * 2f * lerper, 0f, 0f);

                Main.spriteBatch.Draw(starTex, dust.position - Main.screenPosition, null, color * lerper, dust.rotation, starTex.Size() / 2f, dust.scale * lerper, 0f, 0f);

                Main.spriteBatch.Draw(starTex, dust.position - Main.screenPosition, null, new Color(255, 255, 255, 0) * 0.75f * lerper, dust.rotation, starTex.Size() / 2f, dust.scale * lerper, 0f, 0f);
            });

            return false;
        }
    }

    public class Stardust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = false;
            dust.frame = new Rectangle(0, 0, 14, 14);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return new Color(255, 255, 255, 0);
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.rotation += 0.10f;
            dust.velocity *= 0.93f;
            dust.color *= 0.96f;

            dust.scale *= 0.96f;
            if (dust.scale < 0.1f)
                dust.active = false;
            return false;
        }
    }

    public class ManaStardust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = false;
            dust.frame = new Rectangle(0, 0, 14, 14);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return new Color(255, 255, 255, 0);
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.rotation += 0.10f;
            dust.velocity *= 0.93f;
            dust.color *= 0.96f;

            dust.scale *= 0.96f;
            if (dust.scale < 0.1f)
                dust.active = false;
            return false;
        }
    }
}
