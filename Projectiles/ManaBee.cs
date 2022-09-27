using BombusApisBee.BeeHelperProj;

namespace BombusApisBee.Projectiles
{
    public class ManaBee : BeeHelper
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mana Bee");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SafeSetDefaults()
        {
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {

            Player player = Main.player[Projectile.owner];
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 vel = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    while (vel.X == 0f && vel.Y == 0f)
                    {
                        vel = new Vector2(Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    }
                    vel.Normalize();
                    vel *= (float)Main.rand.Next(70, 101) * 0.1f;
                    Projectile.NewProjectile(Projectile.GetSource_OnHit(target), Projectile.Center, vel, ModContent.ProjectileType<ManaBolt>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
                }
            }
        }


        public override void SafeAI()
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonWater);
            dust.noGravity = true;
            dust.scale = 1.1f;
        }
    }
}