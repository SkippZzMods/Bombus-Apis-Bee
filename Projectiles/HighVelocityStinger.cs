namespace BombusApisBee.Projectiles
{
    public class HighVelocityStinger : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("High Velocity Stinger");
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.friendly = true;

            Projectile.penetrate = 3;
            Projectile.timeLeft = 360;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 600)
                SoundID.Item17.PlayWith(Projectile.position);

            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.StingerDust>());
            dust.scale = 1f;
            dust.velocity *= 0.5f;
            dust.noGravity = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, null, lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.25f, (i / (float)Projectile.oldPos.Length)), Projectile.direction == -1 ? (SpriteEffects)1 : 0, 0);
            }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, Projectile.direction == -1 ? (SpriteEffects)1 : 0, 0f);
            return false;
        }
    }
}
