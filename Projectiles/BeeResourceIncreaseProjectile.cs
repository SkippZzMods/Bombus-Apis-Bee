using Microsoft.Xna.Framework.Graphics;
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
            if (Projectile.localAI[0] > 4f)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(2f, 2f), ModContent.DustType<Dusts.HoneyMetaballDust>(), -Projectile.velocity.RotatedByRandom(0.2f) * 0.15f, 0, default, 0.75f).noGravity = true;
                Dust.NewDustPerfect(Projectile.Center - Projectile.velocity + Main.rand.NextVector2Circular(2f, 2f), ModContent.DustType<Dusts.HoneyMetaballDust>(), -Projectile.velocity.RotatedByRandom(0.2f) * 0.15f, 0, default, 0.75f).noGravity = true;
            }
            else
                Projectile.localAI[0] += 1f;

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
                    player.IncreaseBeeResource((int)resourceincrease);
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

        public override void Kill(int timeLeft)
        {
            SoundID.NPCDeath19.PlayWith(Projectile.Center);

            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(2f, 2f), ModContent.DustType<Dusts.HoneyMetaballDust>(), -Projectile.velocity.RotatedByRandom(1f) * Main.rand.NextFloat(.3f), 0, default, Main.rand.NextFloat(.5f, 1.5f)).noGravity = true;
            }
        }
    }
}

