namespace BombusApisBee.Dusts
{
    public class SmokeDustAlt : ModDust
    {
        public override string Texture => BombusApisBee.Invisible;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, 0, 4, 4);
        }

        public override bool Update(Dust dust)
        {
            dust.position.Y -= 0.025f;
            dust.position += dust.velocity;
            dust.velocity *= 0.99f;
            dust.rotation += dust.velocity.Length() * 0.01f;

            dust.alpha += 10;

            dust.alpha = (int)(dust.alpha * 1.01f);

            dust.scale *= 1.01f;

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            float lerper = 1f - dust.alpha / 255f;

            Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/Dusts/SmokeDustAlt").Value;

            Effect effect = Filters.Scene["HolyShieldShader"].GetShader().Shader;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            effect.Parameters["time"].SetValue(dust.alpha);
            effect.Parameters["uTime"].SetValue(dust.alpha);
            effect.Parameters["screenPos"].SetValue(Main.screenPosition * new Vector2(0.5f, 0.1f) / new Vector2(Main.screenWidth, Main.screenHeight));

            effect.Parameters["offset"].SetValue(Vector2.Lerp(new Vector2(0.005f), Vector2.Zero, lerper));
            effect.Parameters["repeats"].SetValue(2);
            effect.Parameters["uImage1"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);
            effect.Parameters["uImage2"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/MiscNoise1").Value);

            Color color = new Color(100, 100, 100, 0) * 0.35f * lerper;
            effect.Parameters["uColor"].SetValue(color.ToVector4());
            effect.Parameters["noiseImage1"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/PerlinNoise").Value);

            effect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, Color.White, dust.rotation, tex.Size() / 2f, dust.scale, 0f, 0f);
            
            color = new Color(70, 70, 70, 0) * 0.2f * lerper;
            effect.Parameters["uColor"].SetValue(color.ToVector4());

            effect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, null, Color.White, dust.rotation + MathHelper.PiOver2, tex.Size() / 2f, dust.scale, 0f, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }

    public class SmokeDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.scale *= Main.rand.NextFloat(0.8f, 2f);
            dust.frame = new Rectangle(0, Main.rand.Next(2) * 32, 32, 32);
            dust.rotation = Main.rand.NextFloat(6.28f);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }

        public override bool Update(Dust dust)
        {
            dust.velocity *= 0.98f;
            dust.velocity.X *= 0.95f;
            dust.color *= 0.98f;

            if (dust.alpha > 100)
            {
                dust.scale *= 0.975f;
                dust.alpha += 2;
            }
            else
            {
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.1f);
                dust.scale *= 0.985f;
                dust.alpha += 4;
            }

            dust.position += dust.velocity;
            dust.rotation += 0.04f;

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
    }

    public class SmokeDustGravity : SmokeDust
    {
        public override string Texture => "BombusApisBee/Dusts/SmokeDust";
        public override bool Update(Dust dust)
        {
            if (dust.velocity.Y > -2)
                dust.velocity.Y -= 0.1f;
            else
                dust.velocity.Y *= 0.92f;

            if (dust.velocity.Y > 0)
                dust.velocity.Y *= 0.85f;

            base.Update(dust);
            return false;
        }
    }
}
