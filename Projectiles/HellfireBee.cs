using BombusApisBee.BeeHelperProj;

namespace BombusApisBee.Projectiles
{
    public class HellfireBee : BeeHelper
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("HellfireBee");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SafeOnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire3, 300);
        }
        public override void SafeAI()
        {
            if (Main.rand.NextBool(5))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Lava, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 150, default(Color), 0.7f).noGravity = true;
        }
    }
}