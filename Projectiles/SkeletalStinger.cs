namespace BombusApisBee.Projectiles
{
    public class SkeletalStinger : ModProjectile
    {
        public bool EnragedStinger => Projectile.ai[0] == 1f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skeletal Stinger");
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = BeeUtils.BeeDamageClass();

            Projectile.timeLeft = 360;
            Projectile.penetrate = 2;
            Projectile.height = 12;
            Projectile.width = 12;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);

            if (Projectile.ai[0] == 1f && Main.rand.NextBool(3))
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.One.RotatedByRandom(6.28f), 25, new Color(200, 45, 20), Main.rand.NextFloat(0.2f, 0.4f));
            if (!EnragedStinger && Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Blood, default, 0, default, 0.9f);
                dust.noGravity = true;
                dust.velocity *= 0.5f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            if (EnragedStinger)
            {
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + new Vector2(0f, Projectile.gfxOffY);
                    Color color = new Color(200, 45, 20) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    color.A = 0;
                    Main.EntitySpriteDraw(tex, drawPos + new Vector2(Projectile.width, Projectile.height) * 0.5f, null, color, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0, 0);
                }
                float sin = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 5f);
                for (int k = 0; k < 4; k++)
                {
                    Vector2 dir = Vector2.UnitX.RotatedBy(k / 4f * 6.28f) * (5.5f + sin * 3.2f);
                    var color = new Color(200, 45, 20) * (0.85f - sin * 0.1f) * 0.9f;
                    color.A = 0;
                    Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition + dir, null, color, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0, 0);
                }
            }
            else
            {
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + new Vector2(0f, Projectile.gfxOffY);
                    Color color = lightColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(tex, drawPos + new Vector2(Projectile.width, Projectile.height) * 0.5f, null, color, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0, 0);
                }
            }
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0, 0);
            return false;
        }
    }
}
