namespace BombusApisBee.Projectiles
{
    public class HoneyTiedEighthNote : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Note");
        }
        public override void SafeSetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.TiedEighthNote);
            Projectile.aiStyle = -1;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - (float)Projectile.alpha / 255f);
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.X * 0.1f;
            Projectile.spriteDirection = -Projectile.direction;
            if (Main.rand.Next(1) == 0)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<HoneyDust>(), 0f, 0f, 60, default, 0.7f);
                Main.dust[dust].noGravity = true;
            }
        }
        public override void Kill(int timeLeft)
        {
            for (int num465 = 0; num465 < 8; num465++)
            {
                int num466 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<HoneyDust>(), 0f, 0f, 80, default, 1.5f);
                Main.dust[num466].noGravity = true;
            }
        }
    }
}
