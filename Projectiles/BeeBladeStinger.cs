using Microsoft.Xna.Framework.Graphics;

namespace BombusApisBee.Projectiles
{
    public class BeeBladeStinger : BeeProjectile
    {
        public override string Texture => "BombusApisBee/ExtraTextures/StingerRetexture";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stinger");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
        }
        public override void SafeSetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.HornetStinger);
            AIType = ProjectileID.HornetStinger;
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
        public override void AI()
        {
            Projectile.localAI[1] += 1f;
            if (Projectile.localAI[1] == 3f)
            {
                for (int i = 0; i < 12; i++)
                {
                    Vector2 vector3 = Vector2.UnitX * -(float)Projectile.width / 2f;
                    vector3 += -Utils.RotatedBy(Vector2.UnitY, (double)((float)i * 3.1415927f / 6f), default(Vector2)) * new Vector2(8f, 16f);
                    vector3 = Utils.RotatedBy(vector3, (double)(Projectile.rotation - 1.5707964f), default(Vector2));
                    int num9 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Poisoned, 0f, 0f, 150, default(Color), 1f);
                    Main.dust[num9].scale = 1.1f;
                    Main.dust[num9].noGravity = true;
                    Main.dust[num9].position = Projectile.Center + vector3;
                    Main.dust[num9].velocity = Projectile.velocity * 0.1f;
                    Main.dust[num9].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[num9].position) * 1.25f;
                }
            }
        }
    }
}
