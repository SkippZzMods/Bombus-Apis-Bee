using BombusApisBee.Core.Systems.ParticleSystem;
using ReLogic.Content;

namespace BombusApisBee.Content.Particles
{
    public class FallenStarParticle : Particle
    {
        public override ParticleDrawType DrawType => ParticleDrawType.Custom;

        public FallenStarParticle(Vector2 position, Vector2 velocity, Color color, float scale, int maxTime)
        {
            Position = position;
            Velocity = velocity;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Scale = scale;
            MaxTime = maxTime;
            Color = color;
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

            spriteBatch.Draw(starTexture, Position - Main.screenPosition, null, Color, Rotation, starTexture.Size() / 2, Scale * progress, SpriteEffects.None, 0);

            spriteBatch.Draw(bloomTex, Position - Main.screenPosition, null, Color * 0.2f, Rotation, bloomTex.Size() / 2, Scale * progress, SpriteEffects.None, 0);
        }
    }
}
