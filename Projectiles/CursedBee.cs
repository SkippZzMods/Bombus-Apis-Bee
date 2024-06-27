using BombusApisBee.BeeHelperProj;

namespace BombusApisBee.Projectiles
{
    public class CursedBee : BaseBeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Bee");
            Main.projFrames[Projectile.type] = 4;
        }


        public override void SafeOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, 720);
        }
        public override void OnKill(int timeLeft)
        {
            int numberDust = 9 + Main.rand.Next(2);
            for (int i = 0; i < numberDust; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch);
                dust.noGravity = true;
                dust.scale = 2.75f;
            }
        }
        public override void SafeAI()
        {
            if (Main.rand.NextBool())
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch);
                dust.noGravity = true;
                dust.scale = 1.75f;
            }
        }

    }
}