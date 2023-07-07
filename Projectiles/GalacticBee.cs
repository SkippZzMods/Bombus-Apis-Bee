using BombusApisBee.BeeHelperProj;

namespace BombusApisBee.Projectiles
{
    public class GalacticBee : BaseBeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galactic Bee");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SafeSetDefaults()
        {
            Projectile.penetrate = 3;
            Projectile.tileCollide = false;
        }
        public override bool SafePreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            int startY = 56 / 4 * Projectile.frame;
            Rectangle trail = new Rectangle(0, startY, 14, 56 / 4);
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color drawColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, Projectile.alpha) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, trail, drawColor, Projectile.oldRot[k], drawOrigin, Projectile.scale, spriteEffects, 0f); ;
            }
            return true;
        }
        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    float x = Projectile.position.X + (float)Main.rand.Next(-400, 400);
                    float y = Projectile.position.Y - (float)Main.rand.Next(600, 900);
                    Vector2 vector7 = new Vector2(x, y);
                    float num427 = Projectile.position.X + (float)(Projectile.width / 2) - vector7.X;
                    float num428 = Projectile.position.Y + (float)(Projectile.height / 2) - vector7.Y;
                    int num429 = 31;
                    float num430 = (float)Math.Sqrt(num427 * num427 + num428 * num428);
                    num430 = (float)num429 / num430;
                    num427 *= num430;
                    num428 *= num430;
                    int num431 = Projectile.damage;
                    int num432 = Projectile.NewProjectile(Projectile.GetSource_Death(), x, y, num427, num428, ModContent.ProjectileType<LargeGalaticStar>(), num431, Projectile.knockBack, Projectile.owner);
                }
            }
        }
        public override void SafeAI()
        {
            int dustType = Main.rand.Next(3);
            dustType = ((dustType == 0) ? 27 : ((dustType == 1) ? 59 : 58));
            Dust.NewDust(Projectile.position, 14, 14, dustType, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 150, default(Color), 0.5f);
        }
    }
}
