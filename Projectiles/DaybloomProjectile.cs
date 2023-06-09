namespace BombusApisBee.Projectiles
{
    public class DaybloomProj : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flower");
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 18000;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Projectile.spriteDirection = -1;
            Player player = Main.player[Projectile.owner];
            if (player.Bombus().LivingFlower && !player.dead)
                Projectile.timeLeft = 2;

            Projectile.rotation = Projectile.AngleTo(player.Center);
            double deg = (player.Bombus().LivingFlowerRot + Projectile.ai[0] * 120) * 5;
            double rad = deg * (Math.PI / 180);
            double dist = 64;
            Projectile.position.X = player.Center.X - (int)(Math.Cos(rad) * dist) - Projectile.width / 2;
            Projectile.position.Y = player.Center.Y - (int)(Math.Sin(rad) * dist) - Projectile.height / 2;
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Grass);
                Main.dust[dust].noGravity = false;
                Main.dust[dust].scale = 0.5f;
            }
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Grass);
                Main.dust[dust].noGravity = false;
                Main.dust[dust].scale = 0.5f;
            }
        }
    }
}
