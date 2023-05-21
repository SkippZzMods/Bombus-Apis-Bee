using Microsoft.Xna.Framework.Graphics;

namespace BombusApisBee.Projectiles
{
    public class BigStar : BeeProjectile
    {
        public override string Texture => "BombusApisBee/Projectiles/AstralStar";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Big Star");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SafeSetDefaults()
        {
            Projectile.scale = 2f;
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.aiStyle = 5;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 150;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 0;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }

        public override void Kill(int timeLeft)
        {
            int numberProjectiles = 5 + Main.rand.Next(5); // 4 or 5 shots
            for (int i = 0; i < numberProjectiles; i++)
            {
                Player player = Main.player[Projectile.owner];
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Vector2 vel = new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-5, 5));
                        Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, vel, ModContent.ProjectileType<MiniStars>(), Projectile.damage * 1 / 2, 1, player.whoAmI);
                        var modPlayer2 = player.GetModPlayer<BombusApisBeePlayer>();
                        modPlayer2.AddShake(17);
                    }
                }
            }
        }

        public override void AI()
        {
            Projectile.tileCollide = false;
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 2)
            {
                Projectile.velocity *= 98 / 100f;
                Projectile.ai[0] = 0;
            }
        }
    }
}