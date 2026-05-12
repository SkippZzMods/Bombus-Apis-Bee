using BombusApisBee.Core.Systems.ParticleSystem;

namespace BombusApisBee.Content.Particles
{
    public class FallenStarParticle : Particle
    {
        internal Color[] _bloomColors;
        internal Color[] _starColors;
        public override ParticleDrawType DrawType => ParticleDrawType.Custom;

        public FallenStarParticle(Vector2 position, Vector2 velocity, Color color, float scale, int maxTime)
        {
            Position = position;
            Velocity = velocity;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Scale = scale;
            MaxTime = maxTime;
            _bloomColors = [color];
            _starColors = [color];
        }

        public FallenStarParticle(Vector2 position, Vector2 velocity, Color[] starColors, Color[] bloomColors, float scale, int maxTime) : this(position, velocity, Color.White, scale, maxTime)
        {
            _bloomColors = bloomColors;
            _starColors = starColors;
        }

        public override void Update()
        {
            Lighting.AddLight(Position, Color.R / 255f, Color.G / 255f, Color.B / 255f);
            Velocity *= 0.97f;
            Rotation += Velocity.Length() * 0.04f;
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Main.instance.LoadProjectile(9);

            var starTexture = TextureAssets.Projectile[9].Value;
            var bloomTex = ParticleHandler.GetTexture(Type);

            float progress = EaseBuilder.EaseCircularIn.Ease(1f - Progress);

            Color starColor = _starColors[0];
            Color bloomColor = _bloomColors[0];

            if (_starColors.Length == 2)
                starColor = Color.Lerp(_starColors[0], _starColors[1], progress);
            else if (_starColors.Length > 2)
                starColor = BeeUtils.MulticolorLerp(progress, _starColors);

            if (_bloomColors.Length == 2)
                bloomColor = Color.Lerp(_bloomColors[0], _bloomColors[1], progress);
            else if (_bloomColors.Length > 2)
                starColor = BeeUtils.MulticolorLerp(progress, _bloomColors);


            spriteBatch.Draw(starTexture, Position - Main.screenPosition, null, starColor, Rotation, starTexture.Size() / 2, Scale * progress, SpriteEffects.None, 0);

            spriteBatch.Draw(bloomTex, Position - Main.screenPosition, null, bloomColor * 0.2f, Rotation, bloomTex.Size() / 2, Scale * progress, SpriteEffects.None, 0);
        }
    }
}
