using BombusApisBee.Core.Helpers;
using BombusApisBee.Core.Systems.ParticleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombusApisBee.Content.Particles
{
    public class WhiteFlowerParticle : Particle
    {
        const int MAX_CACHE_COUNT = 8;

        int variant;
        List<Vector2> cache;

        public virtual int FrameCount => 2;
        public override ParticleDrawType DrawType => ParticleDrawType.Custom;

        public WhiteFlowerParticle(Vector2 position, Vector2 velocity, float scale, int maxTime)
        {
            Position = position;
            Velocity = velocity;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Scale = scale;
            MaxTime = maxTime;

            variant = Main.rand.Next(FrameCount);
        }

        public override void Update()
        {
            if (cache is null)
            {
                cache = [];
                for (int i = 0; i < MAX_CACHE_COUNT; i++)
                    cache.Add(Position);
            }

            cache.Add(Position);

            while (cache.Count > MAX_CACHE_COUNT)
                cache.RemoveAt(0);

            if (Progress < 0.1f)
                Velocity.Y -= 0.15f;
            else
                Velocity.Y += 0.1f * Progress;

            Velocity *= 0.99f;
            Rotation += Velocity.Y * 0.05f;
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            var tex = ParticleHandler.GetTexture(Type);
            var bloom = Request<Texture2D>("BombusApisBee/Assets/ExtraTextures/BloomNonPremult").Value;

            float progress = EaseBuilder.EaseCircularIn.Ease(1f - Progress);

            var frame = tex.Frame(1, FrameCount, 0, variant);

            float fade;

            if (Progress < 0.25f)
                fade = (Progress / 0.25f);
            else
                fade = (1f - (Progress - 0.25f) / 0.75f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(bloom, Position - Main.screenPosition, null, Color.Black * 0.25f * fade, 0f, bloom.Size() / 2, Scale * 0.4f, 0, 0);
            spriteBatch.End();
            spriteBatch.BeginDefault();

            if (cache is not null)
                for (int i = 0; i < cache.Count; i++)
                {
                    float lerp = i / (float)cache.Count;

                    spriteBatch.Draw(tex, cache[i] - Main.screenPosition, frame, Color.White * 0.25f * fade * lerp, Rotation, frame.Size() / 2, Scale, SpriteEffects.None, 0);
                }

            spriteBatch.Draw(tex, Position - Main.screenPosition, frame, Color.White * 0.5f * fade, Rotation, frame.Size() / 2, Scale, SpriteEffects.None, 0);
        }
    }

    public class PinkFlowerParticle : WhiteFlowerParticle
    {
        public PinkFlowerParticle(Vector2 position, Vector2 velocity, float scale, int maxTime) : base(position, velocity, scale, maxTime) { }

        public override int FrameCount => 3;
    }
}
