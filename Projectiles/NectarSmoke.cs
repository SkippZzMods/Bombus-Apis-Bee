namespace BombusApisBee.Projectiles
{
    public class NectarSmoke : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nectar Cloud");
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.scale *= Main.rand.NextFloat(0.8f, 1.9f);
            Projectile.rotation = Main.rand.NextFloat(6.28f);

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 14;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.95f;
            Projectile.rotation += Projectile.velocity.Length() * 0.02f;

            Projectile.alpha = (int)MathHelper.Lerp(160, 255, 1f - Projectile.timeLeft / 180f);

            if (Main.rand.NextBool(7))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.GlowFastDecelerate>(), 0, 0, 0, new Color(214, 158, 79), MathHelper.Lerp(1f, 0f, Projectile.alpha / 255f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D bloom = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Color color = Projectile.GetAlpha(new Color(214, 158, 79, 0));

            Main.spriteBatch.Draw(bloom, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, bloom.Size() / 2f, 1.35f, 0f, 0f);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, color * MathHelper.Lerp(0.5f, 0.2f, 1f - Projectile.timeLeft / 180f), Projectile.rotation, tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(2f, 1f, 1f - Projectile.timeLeft / 180f), SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(bloom, Projectile.Center - Main.screenPosition, null, color * 0.5f, Projectile.rotation, bloom.Size() / 2f, 0.65f, 0f, 0f);
            return false;
        }
    }
}
