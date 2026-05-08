using BombusApisBee.Core.Common.BeeProjectile;

namespace BombusApisBee.Content.Projectiles
{
    /// <summary>
    /// Slightly weaker version of a regular bee Projectile
    /// </summary>
    public class WeakBeeProjectile : CommonBeeProjectile
    {
        //public override string Texture => "BombusApisBee/Content/Projectiles/RegularBeeProjectile";
        public WeakBeeProjectile() : base(penetrate: 2, speed: 4f, giantSpeed: 5f, hitCooldown: 15) { }

        public override bool SafePreDraw(ref Color lightColor)
        {
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Bee, Main.rand.NextVector2Circular(2f, 2f), 0, default, 1.25f).noGravity = true;
            }
        }
    }
    public class RegularBeeProjectile : CommonBeeProjectile
    {
        public RegularBeeProjectile() : base() { }

        public override bool SafePreDraw(ref Color lightColor)
        {
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Bee, Main.rand.NextVector2Circular(2f, 2f), 0, default, 1.25f).noGravity = true;
            }
        }
    }
}
