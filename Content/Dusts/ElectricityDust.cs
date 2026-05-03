using BombusApisBee.Core.PixelationSystem;
using BombusApisBee.PrimitiveDrawing;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;

namespace BombusApisBee.Content.Dusts
{
    public class ElectricityDust : ModDust
    {
        public override string Texture => BombusApisBee.Invisible;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, 0, 4, 4);
        }

        public override bool Update(Dust dust)
        {
            if (dust.customData is not object[]) // literal brainrot
            {
                // data is [List<Vector2> cache, Trail trail, and Trail trail_2]

                float rot = dust.customData is null ? MathHelper.TwoPi : (float)dust.customData;

                object[] data = [rot, null, null, null];
                dust.customData = data;
            }
            else if (!Main.dedServ)
            {
                object[] data = dust.customData as object[];

                List<Vector2> cache = (List<Vector2>)data[1];
                Trail trail = (Trail)data[2];
                Trail trail_2 = (Trail)data[3];

                if (cache is null)
                {
                    cache = new List<Vector2>();
                    for (int i = 0; i < 10; i++)
                    {
                        cache.Add(dust.position);
                    }
                }

                cache.Add(dust.position);

                while (cache.Count > 10)
                {
                    cache.RemoveAt(0);
                }

                float lerper = 1f - dust.alpha / 255f;

                // that width function sucks im so sorry

                trail ??= new Trail(Main.instance.GraphicsDevice, 10, new TriangularTip(10), factor => (float)(dust.scale * (factor < 0.25f ? factor / 0.25f : (factor > 0.75f ? (factor - 0.75) / 0.25f : 1f)) * lerper), factor =>
                {
                    return dust.color * lerper;
                });

                trail.Positions = cache.ToArray();
                trail.NextPosition = dust.position;

                trail_2 ??= new Trail(Main.instance.GraphicsDevice, 10, new TriangularTip(10), factor => (float)(dust.scale * 2f * (factor < 0.25f ? factor / 0.25f : (factor > 0.75f ? (factor - 0.75) / 0.25f : 1f)) * lerper), factor =>
                {
                    return dust.color * lerper * 0.65f;
                });

                trail_2.Positions = cache.ToArray();
                trail_2.NextPosition = dust.position;

                data = [data[0], cache, trail, trail_2];
                dust.customData = data;
            }

            dust.position += dust.velocity;

            dust.velocity *= 0.9f;
            if (Main.rand.NextBool(5) && dust.customData as object[] != null)
            {
                object[] data = dust.customData as object[];

                float rot = (float)data[0];

                dust.velocity = dust.velocity.RotatedByRandom(rot);
            }

            Lighting.AddLight(dust.position, dust.color.ToVector3() * (1f - dust.alpha / 255f));

            dust.alpha += 5;

            dust.alpha = (int)(dust.alpha * 1.01f);

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            GetInstance<PixelationSystem>().QueueRenderAction("UnderProjectiles", () =>
            {
                Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.EffectMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);


                effect.Parameters["repeats"].SetValue(1f);
                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

                effect.Parameters["time"].SetValue(dust.alpha * -0.005f);

                object[] data = dust.customData as object[];

                if (data != null)
                {
                    Trail trail = (Trail)data[2];
                    Trail trail_2 = (Trail)data[3];
                    trail?.Render(effect);

                    effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/LightningTrail").Value);

                    trail_2?.Render(effect);
                }
            });       

            return false;
        }
    }
}
