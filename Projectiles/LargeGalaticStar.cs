using Terraria;
namespace BombusApisBee.Projectiles
{
    public class LargeGalaticStar : BeeProjectile
    {
        public override string Texture => "BombusApisBee/Projectiles/GalacticStar";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SafeSetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.alpha = 50;
            Projectile.penetrate = 3;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1.25f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.timeLeft = 120;
        }
        public override void AI()
        {
            if (Projectile.soundDelay == 0 && Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) > 2f)
            {
                Projectile.soundDelay = 15;
                SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
            }
            Projectile.alpha -= 15;
            if (Projectile.alpha > 150)
            {
                Projectile.alpha = 150;
            }
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * Projectile.direction;
            int dustType = Main.rand.Next(3);
            dustType = ((dustType == 0) ? 27 : ((dustType == 1) ? 59 : 58));
            Dust.NewDust(Projectile.Center, 14, 14, dustType, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 150, default, 1f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color drawColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, Projectile.alpha) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, drawColor * 0.9f, Projectile.oldRot[k], drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 2; i++)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    while (value15.X == 0f && value15.Y == 0f)
                    {
                        value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    }
                    value15.Normalize();
                    value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
                    Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center.X, target.Center.Y, value15.X, value15.Y, ModContent.ProjectileType<GalacticSeeker>(), hit.Damage * 1 / 4, hit.Knockback, Projectile.owner, 0f, 0f);
                }
            }
        }
    }
}
