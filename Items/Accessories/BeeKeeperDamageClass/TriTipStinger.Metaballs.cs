using BombusApisBee.Core.Metaballs;

namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    public class TriTipStinger_Metaballs : MetaballActor
    {
        public override bool Active => Main.dust.Any(x => x.active && x.type == ModContent.DustType<TriTipStingerDust>());

        public override Color OutlineColor => new Color(180, 255, 0);

        public override bool OverEnemies => false;

        public override void DrawShapes(SpriteBatch spriteBatch)
        {
            Effect borderNoise = Filters.Scene["BorderNoise"].GetShader().Shader;

            if (borderNoise is null)
                return;

            borderNoise.Parameters["offset"].SetValue((float)Main.time / 100f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            borderNoise.CurrentTechnique.Passes[0].Apply();

            var poisonDust = Main.dust.Where(n => n.active && n.type == ModContent.DustType<TriTipStingerDust>());
            foreach (Dust dust in poisonDust)
            {
                borderNoise.Parameters["offset"].SetValue((float)Main.time / 1000f + dust.rotation);
                spriteBatch.Draw(ModContent.Request<Texture2D>("BombusApisBee/Items/Accessories/BeeKeeperDamageClass/BandHoneyDust").Value, (dust.position - Main.screenPosition) / 2, null, Color.White, 0f, Vector2.One * 256f, dust.scale / 64f, SpriteEffects.None, 0);
            }

            spriteBatch.End();
            spriteBatch.Begin();
        }

        public override bool PostDraw(SpriteBatch spriteBatch, Texture2D target)
        {
            Effect effect = Filters.Scene["HoneyNoise"].GetShader().Shader;
            effect.Parameters["noiseScale"].SetValue((new Vector2(Main.screenWidth, Main.screenHeight) / 200) + new Vector2((float)(Main.timeForVisualEffects * 0.001f), 0f));
            effect.Parameters["offset"].SetValue(2 * Main.screenPosition / new Vector2(Main.screenWidth, Main.screenHeight));
            effect.Parameters["codedColor"].SetValue(Color.White.ToVector4());
            effect.Parameters["uColorOne"].SetValue(new Color(50, 100, 0, 200).ToVector4());
            effect.Parameters["uColorTwo"].SetValue(new Color(150, 255, 0).ToVector4());
            effect.Parameters["distort"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/ShaderNoiseLooping").Value);

            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.0001f);

            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.003f);
            effect.Parameters["power"].SetValue(0.05f);
            effect.Parameters["uOffset"].SetValue(new Vector2(Main.screenPosition.X / Main.screenWidth * 0.5f, 0));
            effect.Parameters["speed"].SetValue(50f);

            effect.CurrentTechnique.Passes[0].Apply();

            spriteBatch.Draw(target, Vector2.Zero, null, Color.White, 0, new Vector2(0, 0), 2f, SpriteEffects.None, 0);

            return false;
        }
    }

    class TriTipStingerDust : ModDust
    {
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";
        public override void OnSpawn(Dust dust)
        {
            dust.noLight = true;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;

            if (dust.noGravity)
            {
                dust.velocity *= 0.985f;

                dust.scale *= 0.95f;
            }
            else
            {
                dust.scale *= 0.975f;

                if (dust.scale < 0.5f)
                    dust.velocity *= 0.95f;
                else
                    dust.velocity.Y -= 0.05f;

            }

            dust.rotation = dust.velocity.ToRotation();

            if (dust.scale < 0.2f)
                dust.active = false;

            return false;
        }
    }
}
