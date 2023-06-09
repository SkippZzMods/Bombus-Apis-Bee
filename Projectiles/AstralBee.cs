using BombusApisBee.BeeHelperProj;

namespace BombusApisBee.Projectiles
{
    public class AstralBee : BeeHelper
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Bee");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SafeOnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                float x = Projectile.position.X + (float)Main.rand.Next(-400, 400);
                float y = Projectile.position.Y - (float)Main.rand.Next(600, 900);
                Vector2 vector7 = new Vector2(x, y);
                float num427 = Projectile.position.X + (float)(Projectile.width / 2) - vector7.X;
                float num428 = Projectile.position.Y + (float)(Projectile.height / 2) - vector7.Y;
                int num429 = 15;
                float num430 = (float)Math.Sqrt(num427 * num427 + num428 * num428);
                num430 = (float)num429 / num430;
                num427 *= num430;
                num428 *= num430;
                Projectile.NewProjectile(Projectile.GetSource_OnHit(target), x, y, num427, num428, ModContent.ProjectileType<AstralStar>(), Projectile.damage / 3, Projectile.knockBack, Projectile.owner);
            }
        }
        public override void SafeAI()
        {
            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MagicDust>(), newColor: new Color(157, 127, 207, 100));
                dust.noGravity = true;
                dust.scale = 0.8f;
            }
            if (Main.rand.NextBool(35))
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Stardust>(), Projectile.velocity * 0.05f);
            }
        }
    }
}