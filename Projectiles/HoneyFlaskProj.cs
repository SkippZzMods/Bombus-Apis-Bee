using BombusApisBee.Dusts;

namespace BombusApisBee.Projectiles
{
    public class HoneyFlaskProj : BeeProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("HoneyFlask");
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.aiStyle = 2;
            Projectile.timeLeft = 200;
            Projectile.tileCollide = true;

        }
        public override void Kill(int timeLeft)
        {
            int numberProjectiles = 5 + Main.rand.Next(4); // 4 or 5 shots
            for (int i = 0; i < numberProjectiles; i++)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 vel = new Vector2(Main.rand.NextFloat(-7, 7), Main.rand.NextFloat(-7, 7));
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, vel, ModContent.ProjectileType<HoneySmoke>(), Projectile.damage * 2 / 3, 1, Projectile.owner);
                }
            }
            SoundEngine.PlaySound(SoundID.Item107, Projectile.position);
            Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, -Projectile.oldVelocity * 0.2f, 704);
            Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, -Projectile.oldVelocity * 0.2f, 705);

        }
        public override void AI()
        {
            if (Main.rand.Next(4) == 0) // only spawn 20% of the time
            {
                int dust = ModContent.DustType<HoneyDust>();
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dust, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 150, default(Color), 0.7f);
            }

        }

    }
}