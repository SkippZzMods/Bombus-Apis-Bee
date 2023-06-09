namespace BombusApisBee.Projectiles
{
    public abstract class BeeProjectile : ModProjectile
    {
        public virtual void SafeSetDefaults() { }
        public sealed override void SetDefaults()
        {
            SafeSetDefaults();
            Projectile.DamageType = ModContent.GetInstance<HymenoptraDamageClass>();

            Projectile.hostile = false;
        }
    }
}