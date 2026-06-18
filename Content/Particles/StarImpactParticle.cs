using BombusApisBee.Core.Systems.ParticleSystem;
namespace BombusApisBee.Content.Particles
{
    public class StarImpactParticle : Particle
    {
        internal Color[] _bloomColors;
        internal Color[] _starColors;

        Vector2 _scale;
        Vector2 _endScale;

        public override ParticleDrawType DrawType => ParticleDrawType.Custom;

        public StarImpactParticle(Vector2 position, Color color, Vector2 scale, Vector2 endScale, int maxTime)
        {
            Position = position;
            _scale = scale;
            _endScale = endScale;
            MaxTime = maxTime;
            _bloomColors = [color];
            _starColors = [color];
        }

        public StarImpactParticle(Vector2 position, Color[] starColors, Color[] bloomColors, Vector2 scale, Vector2 endScale, int maxTime) : this(position, Color.White, scale, endScale, maxTime)
        {
            _bloomColors = bloomColors;
            _starColors = starColors;
        }

        public override void Update()
        {
            Lighting.AddLight(Position, _starColors[0].ToVector3() * (1f - Progress) * 0.5f);
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Main.instance.LoadProjectile(79);
            var starTexture = TextureAssets.Projectile[79].Value;
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

            Vector2 scale = Vector2.Lerp(_scale, _endScale, 1f - progress);


            spriteBatch.Draw(starTexture, Position - Main.screenPosition, null, starColor * progress, 0f, starTexture.Size() / 2, scale, SpriteEffects.None, 0);

            spriteBatch.Draw(starTexture, Position - Main.screenPosition, null, Color.White with { A = 0 } * 0.7f * progress, 0f, starTexture.Size() / 2, scale * 0.8f, SpriteEffects.None, 0);

            spriteBatch.Draw(bloomTex, Position - Main.screenPosition, null, bloomColor * 0.5f * progress, 0f, bloomTex.Size() / 2, scale.Length() * 0.35f, SpriteEffects.None, 0);

            spriteBatch.Draw(bloomTex, Position - Main.screenPosition, null, Color.White with { A = 0 } * 0.4f * progress, 0f, bloomTex.Size() / 2, scale.Length() * 0.15f, SpriteEffects.None, 0);
        }
    }
}
