namespace BombusApisBee.Dusts
{
    public class ImpactLineDust : ModDust
    {
        public override string Texture => BombusApisBee.Invisible;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, 0, 4, 4);
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.velocity *= 0.9f;
            dust.rotation = dust.velocity.ToRotation();

            dust.alpha += 10;

            dust.alpha = (int)(dust.alpha * 1.01f);

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            float lerper = 1f - dust.alpha / 255f;

            Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/Dusts/ImpactLineDust").Value;

            Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, dust.color * lerper, dust.rotation, tex.Size() / 2f, new Vector2(dust.scale * lerper, dust.scale), 0f, 0f);

            Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, Color.White with { A = 0 } * 0.25f * lerper, dust.rotation, tex.Size() / 2f, new Vector2(dust.scale * lerper, dust.scale), 0f, 0f);

            return false;
        }
    }

    public class ImpactLineDustGlow : ModDust
    {
        public override string Texture => BombusApisBee.Invisible;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, 0, 4, 4);
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.velocity *= 0.9f;
            dust.rotation = dust.velocity.ToRotation();

            dust.alpha += 10;

            dust.alpha = (int)(dust.alpha * 1.01f);

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            float lerper = 1f - dust.alpha / 255f;

            Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/Dusts/ImpactLineDust").Value;

            Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, dust.color * lerper, dust.rotation, tex.Size() / 2f, new Vector2(dust.scale * lerper, dust.scale), 0f, 0f);

            float glowScale = dust.scale * 0.75f;

			Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, Color.White with { A = 0 } * lerper, dust.rotation, tex.Size() / 2f, new Vector2(glowScale * lerper, glowScale), 0f, 0f);

            return false;
        }
    }
}
