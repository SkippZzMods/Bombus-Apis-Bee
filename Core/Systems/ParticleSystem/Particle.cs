using BombusApisBee.Core.Systems.PixelationSystem;

namespace BombusApisBee.Core.Systems.ParticleSystem
{
    /// <summary>
    /// Represents a particle with a position, velocity, rotation, scale and transparency.
    /// TL;DR: Better dust.
    /// </summary>
    public class Particle
    {
        public int ID; // you don't have to use this
        public int Type; // you don't have to use this

        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Origin;

        public Color Color;

        public float Rotation;
        public float Scale;

        public uint TimeActive;
        public int MaxTime;

        public RenderLayer Layer = RenderLayer.UnderProjectiles;
        public RenderLayer LayerPixel = RenderLayer.UnderProjectiles;
        public bool drawPixellated = false;

        public Texture2D Texture => ParticleHandler.GetTexture(Type);
        public float Progress => TimeActive / (float)MaxTime;

        /// <summary>
        /// Determines whether the particle is drawn using its default drawing method, the default drawing method with additive color blending, or a custom drawing type.
        /// If set to Custom, the CustomDraw method will be called.
        /// </summary>
        public virtual ParticleDrawType DrawType => ParticleDrawType.DefaultAlphaBlend;

        /// <summary>
        /// Determines which layer the particle is drawn on, i.e. when the drawing is called relative to other vanilla entities.
        /// Defaults to before projectiles are drawn.
        /// </summary>
        public virtual RenderLayer DrawLayer => Layer;

        /// <summary>
        /// Determines which layer the pixelated drawing is drawn on, i.e. when the drawing is called relative to other vanilla entities.
        /// Defaults to before projectiles are drawn.
        /// </summary>
        public virtual RenderLayer PixelLayer => LayerPixel;

        /// <summary>
        /// Call this when you want to clear your particle and remove it from the world.
        /// </summary>
        public void Kill() => ParticleHandler.DeleteParticleAtIndex(ID);

        /// <summary>
        /// Called every tick. Update your particle in this method.
        /// Particle velocity is automatically added to the particle position for you, and TimeAlive is incremented.
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// Allows you to do custom drawing for your particle. Only called if Particle.UseCustomDrawing is true.
        /// </summary>
        public virtual void CustomDraw(SpriteBatch spriteBatch) { }

        /// <summary>
        /// Allows you to draw pixelated sprites
        /// Uses PixelLayer to determine which layer of the pixelation system it will be drawing on
        /// </summary>
        public virtual void PixelatedDraw(SpriteBatch spriteBatch) { }
    }
}
