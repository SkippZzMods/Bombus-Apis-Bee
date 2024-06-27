namespace BombusApisBee.Projectiles
{
    public class BeeBubble : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bee Bubble");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 45;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 255;
        }
        public override void OnKill(int timeLeft)
        {
            int numberDust = 15 + Main.rand.Next(5);
            for (int i = 0; i < numberDust; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<HoneyDust>());
                dust.noGravity = true;
                dust.scale = 0.8f;
                dust.alpha = 150;
            }
            SoundEngine.PlaySound(SoundID.Item96, Projectile.Center);
            Player player = Main.player[Projectile.owner];
            var modPlayer2 = player.GetModPlayer<BombusApisBeePlayer>();
            modPlayer2.AddShake(5);
        }
        public override void AI()
        {
            if (Projectile.alpha > 0)
                Projectile.alpha -= 15;
            Projectile.velocity *= 0.96f;
            if (Projectile.timeLeft == 0 && Projectile.owner == Main.myPlayer)
            {
                Vector2 value3 = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
                if (Main.player[Projectile.owner].gravDir == -1f)
                {
                    value3.Y = (float)(Main.screenHeight - Main.mouseY) + Main.screenPosition.Y;
                }
                Vector2 vector3 = Vector2.Normalize(value3 - Projectile.Center);
                vector3 *= 18;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, vector3, ModContent.ProjectileType<BeeBullet>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

            }
        }
    }
}