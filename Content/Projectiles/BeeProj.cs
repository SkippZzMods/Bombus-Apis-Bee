namespace BombusApisBee.Content.Projectiles
{
    //TODO: REMOVE THIS BRO  WHAT 
    public abstract class BeeProjectile : ModProjectile
    {
        public virtual void SafeSetDefaults() { }
        public sealed override void SetDefaults()
        {
            SafeSetDefaults();
            Projectile.DamageType = GetInstance<HymenoptraDamageClass>();

            Projectile.hostile = false;
        }
    }
}