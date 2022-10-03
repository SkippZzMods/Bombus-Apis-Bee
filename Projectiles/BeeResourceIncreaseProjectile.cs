using System;

namespace BombusApisBee.Projectiles
{
    public class BeeResourceIncreaseProjectile : ModProjectile
    {
        public override string Texture
        {
            get
            {
                return "BombusApisBee/Projectiles/BlankProj";
            }
        }
        public ref float resourceincrease => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey");
        }
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 360;
        }
        public override void AI()
        {
            Projectile.velocity.Y *= 0.98f;
            int hdusty = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Honey, 0f, 0f, 25, default, 1.5f);
            Dust dust = Main.dust[hdusty];
            dust.noGravity = true;
            dust.position.X = dust.position.X - Projectile.velocity.X * 0.2f;
            dust.position.Y = dust.position.Y + Projectile.velocity.Y * 0.2f;
            Player player = Main.player[Projectile.owner];
            bool flag = true;
            float homingSpeed = 22f;
            if (player.lifeMagnet)
            {
                homingSpeed *= 2f;
            }
            Vector2 playerVector = player.Center - Projectile.Center;
            float playerDist = playerVector.Length();
            if (playerDist < 50f && Projectile.position.X < player.position.X + (float)player.width && Projectile.position.X + (float)Projectile.width > player.position.X && Projectile.position.Y < player.position.Y + (float)player.height && Projectile.position.Y + (float)Projectile.height > player.position.Y)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    if (player.Hymenoptra().BeeResourceCurrent < player.Hymenoptra().BeeResourceMax2)
                        player.Hymenoptra().BeeResourceCurrent += (int)(resourceincrease);
                    if (player.Hymenoptra().BeeResourceCurrent > player.Hymenoptra().BeeResourceMax2)
                        player.Hymenoptra().BeeResourceCurrent = player.Hymenoptra().BeeResourceMax2;

                    CombatText.NewText(player.getRect(), BombusApisBee.honeyIncreaseColor, (int)resourceincrease, true, true);
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
            if (player.lifeMagnet && Projectile.timeLeft < 180)
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

