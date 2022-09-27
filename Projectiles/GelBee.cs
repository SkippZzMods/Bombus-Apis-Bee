using BombusApisBee.BeeHelperProj;

namespace BombusApisBee.Projectiles
{
    public class GelBee : BeeHelper
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("GelBee");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SafeSetDefaults() { }
        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 vel = new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(8, 10));
                    Projectile.NewProjectile(Projectile.GetSource_Death(), new Vector2(Projectile.Center.X, Projectile.Center.Y - 600), vel, ModContent.ProjectileType<GelBounce>(), Projectile.damage, 1, player.whoAmI);
                }
            }
        }
        public override void SafeAI()
        {
            if (Main.rand.NextBool())
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.ManaRegeneration);
                dust.noGravity = true;
                dust.scale = 1.6f;
            }

        }

    }
}