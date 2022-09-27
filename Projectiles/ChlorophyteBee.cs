using BombusApisBee.BeeHelperProj;
using System;

namespace BombusApisBee.Projectiles
{
    public class ChlorophyteBee : BeeHelper
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chlorophyte Bee");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SafeSetDefaults()
        {
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 3; i++)
            {
                if (base.Projectile.owner == Main.myPlayer)
                {
                    float x = Projectile.position.X + (float)Main.rand.Next(-900, 900);
                    float y = Projectile.position.Y - (float)Main.rand.Next(-900, 900);
                    Vector2 vector7 = new Vector2(x, y);
                    float num427 = Projectile.position.X + (float)(Projectile.width / 2) - vector7.X;
                    float num428 = Projectile.position.Y + (float)(Projectile.height / 2) - vector7.Y;
                    int num429 = 26;
                    float num430 = (float)Math.Sqrt(num427 * num427 + num428 * num428);
                    num430 = (float)num429 / num430;
                    num427 *= num430;
                    num428 *= num430;
                    int num431 = Projectile.damage * 3 / 4;
                    int num432 = Projectile.NewProjectile(Projectile.GetSource_Death(), x, y, num427, num428, ProjectileID.CrystalLeafShot, num431, Projectile.knockBack, Projectile.owner);
                    Main.projectile[num432].GetGlobalProjectile<BombusApisBeeGlobalProjectile>().ForceBee = true;
                }
            }
        }
        public override void SafeAI()
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.ChlorophyteWeapon);
            dust.noGravity = true;
            dust.scale = 0.8f;
        }
    }
}