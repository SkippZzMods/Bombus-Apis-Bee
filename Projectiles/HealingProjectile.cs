namespace BombusApisBee.Projectiles
{
    public class HealingProjectile : ModProjectile
    {
        public override string Texture
        {
            get
            {
                return "BombusApisBee/Projectiles/BlankProj";
            }
        }
        public ref float healing => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galactic Healing Orb");
        }
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 4f)
            {
                for (int i = 0; i < 3; i++)
                {
                    int num = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.LifeDrain, 0f, 0f, 0, default(Color), 1.2f);
                    Main.dust[num].noGravity = true;
                    Dust obj = Main.dust[num];
                    obj.velocity *= 0f;
                }
                for (int i = 0; i < 5; i++)
                {
                    int num = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Shadowflame, 0f, 0f, 0, default(Color), 1.4f);
                    Main.dust[num].noGravity = true;
                    Dust obj = Main.dust[num];
                    obj.velocity *= 0f;
                }
            }
            Player player = Main.player[Projectile.owner];
            bool flag = true;
            float homingSpeed = 24f;
            if (player.lifeMagnet)
            {
                homingSpeed *= 1.5f;
            }
            Vector2 playerVector = player.Center - Projectile.Center;
            float playerDist = playerVector.Length();
            if (playerDist < 50f && Projectile.position.X < player.position.X + (float)player.width && Projectile.position.X + (float)Projectile.width > player.position.X && Projectile.position.Y < player.position.Y + (float)player.height && Projectile.position.Y + (float)Projectile.height > player.position.Y)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    player.Heal((int)healing);
                    NetMessage.SendData(66, -1, -1, null, Projectile.owner, (float)healing, 0f, 0f, 0, 0, 0);
                }
                Projectile.Kill();
            }
            if (flag)
            {
                playerDist = homingSpeed / playerDist;
                playerVector.X *= playerDist;
                playerVector.Y *= playerDist;
                Projectile.velocity.X = (Projectile.velocity.X * 15f + playerVector.X) / (15f + 1f);
                Projectile.velocity.Y = (Projectile.velocity.Y * 15f + playerVector.Y) / (15f + 1f);
                return;
            }
            if (player.lifeMagnet && Projectile.timeLeft < 300)
            {
                playerDist = homingSpeed / playerDist;
                playerVector.X *= playerDist;
                playerVector.Y *= playerDist;
                Projectile.velocity.X = (Projectile.velocity.X * 15f + playerVector.X) / (15f + 1f);
                Projectile.velocity.Y = (Projectile.velocity.Y * 15f + playerVector.Y) / (15f + 1f);
            }
        }
    }
}

