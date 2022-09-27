using BombusApisBee.BeeHelperProj;

namespace BombusApisBee.Projectiles
{
    public class SpectralBee : BeeHelper
    {
        public override int RegularBeePenetrate => 3;
        public override int GiantBeePenetrate => 4;
        public override int FrameTimer => 4;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spectral Bee");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SafeSetDefaults()
        {
            Projectile.light = 0.15f;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
        }
        public override void SafeAI()
        {
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SpectreStaff, 0f, 0f, 25, default, 0.95f);
            }
        }
    }
}