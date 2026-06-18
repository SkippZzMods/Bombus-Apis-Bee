using BombusApisBee.Assets;
using BombusApisBee.Core.Helpers;
using BombusApisBee.Core.Loading;
using BombusApisBee.Core.Systems.ParticleSystem;
using BombusApisBee.Core.Systems.PixelationSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;
using static tModPorter.ProgressUpdate;

namespace BombusApisBee.Content.Particles
{
    public class FireParticle : Particle
    {
        internal Color[] _colors = new Color[4];

        internal bool _addLight;
        internal int _variant;

        internal float timeVariance;
        internal float flameIntensity;

        private readonly Action<Particle> _action;

        public override ParticleDrawType DrawType => ParticleDrawType.Custom;
        public override RenderLayer PixelLayer => LayerPixel;

        public FireParticle(Vector2 position, Vector2 velocity, Color outlineColor, float scale, int maxTime, bool addLight = true, float FlameIntensity = 0.1f, Action<Particle> extraUpdateAction = null, Color[] palletColors = null)
        {
            Position = position;
            Velocity = velocity;
            Rotation = Main.rand.NextFloat(6.28f);
            Scale = scale;
            MaxTime = maxTime;

            if (palletColors is not null)
            {
                _colors = palletColors;
            }
            else
            {
                _colors =
                [
                    new Color(0.1f, 0.0f, 0.0f),
                    new Color(0.8f, 0.2f, 0.0f),
                    new Color(1.0f, 0.8f, 0.0f),
                    new Color(1.0f, 1.0f, 1.0f)
                ];
            }

            timeVariance = Main.rand.NextFloat(0.02f, 0.06f);

            Color = outlineColor;
            
            _addLight = addLight;

            _variant = Main.rand.Next(1, 4);
            drawPixellated = true;
            flameIntensity = FlameIntensity;
        }

        public override void Update()
        {
            Velocity *= 0.97f;
            Velocity.Y -= 0.05f;
            Rotation += Velocity.Length() * 0.003f;

            if (_addLight)
                Lighting.AddLight(Position, _colors[2].R / 255f, _colors[2].G / 255f, _colors[2].B / 255f);

            _action?.Invoke(this);
        }

        public override void PixelatedDraw(SpriteBatch spriteBatch)
        {
            var texture = ModContent.Request<Texture2D>("BombusApisBee/Content/Particles/FireParticle_0" + _variant).Value;
            float progress = EaseBuilder.EaseCircularIn.Ease(1f - Progress);

            //Color color;
            //spriteBatch.Draw(bloomTex, Position - Main.screenPosition, null, color * 0.2f, Rotation + MathHelper.PiOver2, bloomTex.Size() / 2, _scale.Length() * progress, SpriteEffects.None, 0);
           
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);

            Effect effect = Filters.Scene["FireShader"].GetShader().Shader;

            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * timeVariance);
            effect.Parameters["uProgress"].SetValue(EaseBuilder.EaseCircularIn.Ease(1f - progress));
            effect.Parameters["uImage1"].SetValue(Request<Texture2D>(AssetDirectory.Assets + "ShaderTextures/voronoiNoise_01").Value);

            effect.Parameters["uSecondaryColor"].SetValue(Color.ToVector3());
            effect.Parameters["uOpacity"].SetValue(Progress < 0.5f ? (Progress / 0.5f) : 1f - (Progress - 0.5f) / 0.5f);
            effect.Parameters["noiseIntensity"].SetValue(flameIntensity);

            Vector3[] arr = new Vector3[_colors.Length];

            for (int i = 0; i < _colors.Length; i++)
            {
                arr[i] = _colors[i].ToVector3();
            }

            effect.Parameters["uColorArray"].SetValue(arr);

            effect.CurrentTechnique.Passes[0].Apply();

            float scale = MathHelper.Lerp(Scale, Scale * 0.5f, 1f - EaseBuilder.EaseCircularOut.Ease(progress));

            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color.White, Rotation + MathHelper.PiOver2, texture.Size() / 2, scale, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.BeginDefault();
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            var bloom = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            float progress = EaseBuilder.EaseCircularIn.Ease(1f - Progress);
           
            float scale = MathHelper.Lerp(Scale, Scale * 0.5f, 1f - EaseBuilder.EaseCircularOut.Ease(progress));

            spriteBatch.Draw(bloom, Position - Main.screenPosition, null, _colors[1] with { A = 0 } * 0.05f * progress, Rotation + MathHelper.PiOver2, bloom.Size() / 2, scale * 15f, SpriteEffects.None, 0);
            spriteBatch.Draw(bloom, Position - Main.screenPosition, null, _colors[2] with { A = 0 } * 0.05f * progress, Rotation + MathHelper.PiOver2, bloom.Size() / 2, scale * 14f, SpriteEffects.None, 0);
        }
    }
}
