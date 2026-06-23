using BombusApisBee.Core.Helpers;
using BombusApisBee.Core.Systems.ParticleSystem;
using BombusApisBee.Core.Systems.PixelationSystem;
using ReLogic.Content;

namespace BombusApisBee.Content.Particles
{
    public class BeeParticle : Particle
    {
        public BeeParticle(Vector2 position, Vector2 velocity, float rotation, float scale, int maxTime)
        {
            Position = position;
            Color = Color.White;
            Rotation = rotation;
            Scale = scale;
            MaxTime = maxTime;
            Velocity = velocity;
        }

        public override void Update()
        {
            Velocity *= 0.99f;
            Velocity = Velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.9f, 1.1f);
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            var texture = Texture;
            var bloom = Request<Texture2D>("BombusApisBee/Assets/ExtraTextures/BloomNonPremult").Value;

            float rotation = Rotation;

            float fade;

            if (Progress < 0.5f)
                fade = (Progress / 0.5f);   
            else
                fade = (1f - (Progress - 0.5f) / 0.5f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(bloom, Position - Main.screenPosition, null, Color.Black * 0.5f * fade, 0f, bloom.Size() / 2, Scale * 0.2f, 0, 0);
            spriteBatch.End();
            spriteBatch.BeginDefault();

            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * fade, rotation, texture.Size() / 2, Scale, 0, 0);
        }

        public override ParticleDrawType DrawType => ParticleDrawType.Custom;
    }

    public class LargeBeeParticle(Vector2 position, Vector2 velocity, float rotation, float scale, int maxTime) : BeeParticle(position, velocity, rotation, scale, maxTime)
    {
        internal const int FRAME_COUNT = 4;

        internal int _frame;
        internal int _frameCounter;

        public override void Update()
        {
            base.Update();

            if (++_frameCounter > 3)
            {
                _frameCounter = 0;
                _frame++;

                if (_frame >= FRAME_COUNT)
                    _frame = 0;
            }
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Main.instance.LoadProjectile(ProjectileID.Bee);

            var texture = TextureAssets.Projectile[ProjectileID.Bee].Value;
            var bloom = Request<Texture2D>("BombusApisBee/Assets/ExtraTextures/BloomNonPremult").Value;

            float rotation = Rotation;

            float fade;

            if (Progress < 0.5f)
                fade = (Progress / 0.5f);
            else
                fade = (1f - (Progress - 0.5f) / 0.5f);

            Rectangle frame = texture.Frame(1, FRAME_COUNT, frameY: _frame);

            SpriteEffects flip = Velocity.X < 0 ? SpriteEffects.FlipHorizontally : 0f;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(bloom, Position - Main.screenPosition, null, Color.Black * 0.5f * fade, 0f, bloom.Size() / 2, Scale * 0.5f, 0, 0);

            spriteBatch.End();
            spriteBatch.BeginDefault();

            spriteBatch.Draw(texture, Position - Main.screenPosition, frame, Color * fade, rotation, frame.Size() / 2, Scale, flip, 0);
        }
    }
}
