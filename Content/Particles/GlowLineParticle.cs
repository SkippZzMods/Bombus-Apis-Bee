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

        private readonly Action<Particle> _action;

        public override ParticleDrawType DrawType => ParticleDrawType.Custom;

        public GlowLineParticle(Vector2 position, Vector2 velocity, Color color, float rotation, Vector2 scale, int maxTime, bool addLight = true, Action<Particle> extraUpdateAction = null)
        {
            Position = position;
            Velocity = velocity;
            Rotation = rotation;
            _scale = scale;
            MaxTime = maxTime;
            _colors = [color];
            _addLight = addLight;
            _action = extraUpdateAction;
        }

        public GlowLineParticle(Vector2 position, Vector2 velocity, Color[] colors, float rotation, Vector2 scale, int maxTime, bool addLight = true, Action<Particle> extraUpdateAction = null) : this(position, velocity, Color.White, rotation, scale, maxTime, addLight, extraUpdateAction) 
        {
            _colors = colors;
        }

        public override void Update()
        {
            if (_addLight)
                Lighting.AddLight(Position, Color.R / 255f, Color.G / 255f, Color.B / 255f);

            _action?.Invoke(this);
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
            
            var bloomTex = ParticleHandler.GetTexture(Type);

            spriteBatch.Draw(bloomTex, Position - Main.screenPosition, null, color * 0.2f, Rotation + MathHelper.PiOver2, bloomTex.Size() / 2, _scale.Length() * progress, SpriteEffects.None, 0);

            spriteBatch.Draw(texture, Position - Main.screenPosition, null, color, Rotation + MathHelper.PiOver2, texture.Size() / 2, _scale * progress, SpriteEffects.None, 0);
            
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color.White * 0.5f, Rotation + MathHelper.PiOver2, texture.Size() / 2, _scale * progress * 0.5f, SpriteEffects.None, 0);        
        }
    }
}
