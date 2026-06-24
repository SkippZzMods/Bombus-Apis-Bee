using BombusApisBee.Assets;
using BombusApisBee.Core.Helpers;
using BombusApisBee.Core.Systems.ParticleSystem;
using BombusApisBee.Core.Systems.PixelationSystem;

namespace BombusApisBee.Content.Particles
{
    public class SmokeParticle : Particle
    {
        internal Color SmokeColor;
        internal Color BloomColor;
        internal Color OutlineColor;

        internal int _variant;

        internal bool _addLight;
        internal bool _addBloom;

        private readonly Action<Particle> _action;

        public override ParticleDrawType DrawType => ParticleDrawType.Custom;
        public override RenderLayer PixelLayer => LayerPixel;

        public SmokeParticle(Vector2 position, Vector2 velocity, Color color, Color outlineColor, float scale, int maxTime, bool addLight = true, bool addBloom = true, Action<Particle> extraUpdateAction = null)
        {
            Position = position;
            Velocity = velocity;
            Rotation = Main.rand.NextFloat(6.28f);
            Scale = scale;
            MaxTime = maxTime;

            SmokeColor = color;
            BloomColor = color;

            OutlineColor = outlineColor;

            _addLight = addLight;
            _addBloom = addBloom;
            _action = extraUpdateAction;

            drawPixellated = true;
            _variant = 1 + Main.rand.Next(3);
        }

        public SmokeParticle(Vector2 position, Vector2 velocity, Color color, Color bloomColor, Color outlineColor, float scale, int maxTime, bool addLight = true, bool addBloom = true, Action<Particle> extraUpdateAction = null) : this(position, velocity, Color.White, outlineColor, scale, maxTime, addLight, addBloom, extraUpdateAction)
        {
            SmokeColor = color;
            BloomColor = bloomColor;
        }

        public override void Update()
        {
            Scale *= 1.02f;

            Rotation += Velocity.Length() * 0.01f;
            Velocity *= 0.98f;

            if (_addLight)
                Lighting.AddLight(Position, BloomColor.R / 255f, BloomColor.G / 255f, BloomColor.B / 255f);

            _action?.Invoke(this);
        }

        public override void PixelatedDraw(SpriteBatch spriteBatch)
        {
            var texture = Request<Texture2D>("BombusApisBee/Assets/ExtraTextures/smoke_0" + _variant).Value;

            float progress = Progress;

            float fadeIn;

            if (progress < 0.25f)
                fadeIn = EaseBuilder.EaseCircularOut.Ease(progress / 0.25f);
            else
                fadeIn = EaseBuilder.EaseCircularIn.Ease(1f - (progress - 0.25f) / 0.75f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);

            Effect effect = Filters.Scene["DissipatingSmoke"].GetShader().Shader;

            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
            effect.Parameters["uProgress"].SetValue(progress);
            effect.Parameters["uImage1"].SetValue(Request<Texture2D>(AssetDirectory.Assets + "ShaderTextures/voronoiNoise_01").Value);
            effect.Parameters["uColor"].SetValue(SmokeColor.ToVector4() * fadeIn);
            effect.Parameters["uSecondaryColor"].SetValue(OutlineColor.ToVector4() * fadeIn);
            effect.Parameters["pixelRes"].SetValue(texture.Width / 16);

            //Rectangle frame = texture.Frame(1, 3, 0, _variant);

            effect.CurrentTechnique.Passes[0].Apply();
           
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color.White, Rotation, texture.Size() / 2, Scale, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.BeginDefault();
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            var bloom = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            float progress = Progress;

            float fadeIn;

            if (progress < 0.25f)
                fadeIn = EaseBuilder.EaseCircularOut.Ease(progress / 0.25f);
            else
                fadeIn = EaseBuilder.EaseCircularIn.Ease(1f - (progress - 0.25f) / 0.75f);

            if (_addBloom)
                spriteBatch.Draw(bloom, Position - Main.screenPosition, null, BloomColor with { A = 0 } * 0.15f * fadeIn, Rotation, bloom.Size() / 2, Scale * 0.45f, SpriteEffects.None, 0);
        }
    }
}
