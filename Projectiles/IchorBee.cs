using BombusApisBee.BeeHelperProj;

namespace BombusApisBee.Projectiles
{
    public class IchorBee : BeeHelper
    {
        internal int IchorBeeTimer = 1;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("IchorBee");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SafeSetDefaults()
        {
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 540, true);
        }

        public override void Kill(int timeLeft)
        {
            int numberDust = 9 + Main.rand.Next(2); // 4 or 5 shots
            for (int i = 0; i < numberDust; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IchorTorch);
                dust.noGravity = true;
                dust.scale = 2.75f;
            }
        }
        public override void SafeAI()
        {
            IchorBeeTimer++;
            if (Main.rand.NextBool())
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Ichor);
                dust.noGravity = true;
                dust.scale = 1.75f;
            }
            if (IchorBeeTimer >= 25)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 vel = new Vector2(Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(-5, 5));
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel, ProjectileID.GoldenShowerFriendly, Projectile.damage, 1, Projectile.owner).DamageType = BeeUtils.BeeDamageClass();
                    IchorBeeTimer = 0;
                }
            }
        }

    }
}