using BombusApisBee.BeeHelperProj;

namespace BombusApisBee.Projectiles
{
    public class SkeletalBee : BeeHelper
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("SkeletalBee");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SafeOnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Bleeding, 600, true);
        }
    }
}