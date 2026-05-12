using BombusApisBee.Core.Systems.ParticleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombusApisBee.Content.Particles
{
    public class GlowLineParticle : Particle
    {
        internal Color[] _colors;
        internal bool _addLight;
        internal Vector2 _scale;
        public override ParticleDrawType DrawType => ParticleDrawType.Custom;

        public GlowLineParticle(Vector2 position, Vector2 velocity, Color color, float rotation, Vector2 scale, int maxTime, bool addLight = true)
        {
            Position = position;
            Velocity = velocity;
            Rotation = rotation;
            _scale = scale;
            MaxTime = maxTime;
            _colors = [color];
            _addLight = addLight;
        }

        public GlowLineParticle(Vector2 position, Vector2 velocity, Color[] colors, float rotation, Vector2 scale, int maxTime, bool addLight = true) : this(position, velocity, Color.White, rotation, scale, maxTime, addLight) 
        {
            _colors = colors;
        }

        public override void Update()
        {
            if (_addLight)
                Lighting.AddLight(Position, Color.R / 255f, Color.G / 255f, Color.B / 255f);
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            var texture = ParticleHandler.GetTexture(Type);

            float progress = EaseBuilder.EaseCircularIn.Ease(1f - Progress);

            Color color = _colors[0];

            if (_colors.Length == 2)
                color = Color.Lerp(_colors[0], _colors[1], progress);
            else if (_colors.Length > 2)
                color = BeeUtils.MulticolorLerp(progress, _colors);

            spriteBatch.Draw(texture, Position - Main.screenPosition, null, color, Rotation, texture.Size() / 2, _scale * progress, SpriteEffects.None, 0);
            
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color.White * 0.5f, Rotation, texture.Size() / 2, _scale * progress * 0.5f, SpriteEffects.None, 0);

            //spriteBatch.Draw(bloomTex, Position - Main.screenPosition, null, color * 0.2f, Rotation, bloomTex.Size() / 2, Scale * progress, SpriteEffects.None, 0);
        }
    }
}
