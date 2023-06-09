namespace BombusApisBee.Projectiles
{
    public class HoneySmoke : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Cloud");
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
            Projectile.idStaticNPCHitCooldown = 12;
        }

        public override void AI()
        {
            if (Main.player[Projectile.owner].Hitbox.Intersects(Projectile.Hitbox))
                Main.player[Projectile.owner].AddBuff(BuffID.Honey, 120);

            Projectile.velocity *= 0.95f;
            Projectile.rotation += Projectile.velocity.Length() * 0.02f;

            Projectile.alpha = (int)MathHelper.Lerp(160, 255, 1f - Projectile.timeLeft / 180f);

            if (Main.rand.NextBool(7))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Honey2, 0, 0, Projectile.alpha + Main.rand.Next(25), Scale: Main.rand.NextFloat(0.8f, 1.2f)).noGravity = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D bloom = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Color color = Projectile.GetAlpha(new Color(212, 131, 11, 0));

            Main.spriteBatch.Draw(bloom, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, bloom.Size() / 2f, 1f, 0f, 0f);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * MathHelper.Lerp(0.5f, 0.2f, 1f - Projectile.timeLeft / 180f), Projectile.rotation, tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(2f, 1f, 1f - Projectile.timeLeft / 180f), SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(bloom, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, bloom.Size() / 2f, 0.55f, 0f, 0f);
            return true;
        }
    }
}
