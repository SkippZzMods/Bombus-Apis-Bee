namespace BombusApisBee.Projectiles
{
    public class PoisonSmoke : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Poison Cloud");
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
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.95f;
            Projectile.rotation += Projectile.velocity.Length() * 0.02f;

            Projectile.alpha = (int)MathHelper.Lerp(160, 255, 1f - Projectile.timeLeft / 180f);

            if (Main.rand.NextBool(15))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool() ? DustID.Poisoned : DustID.CorruptGibs, 0, 0, Projectile.alpha + 50, default, 1.2f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 360);
            target.AddBuff<ImprovedPoison>(360);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D bloom = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Color color = Projectile.GetAlpha(new Color(62, 99, 48, 0));
            lightColor.A = 0;

            Main.spriteBatch.Draw(bloom, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, bloom.Size() / 2f, 1f, 0f, 0f);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * MathHelper.Lerp(0.5f, 0.2f, 1f - Projectile.timeLeft / 180f), Projectile.rotation, tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(2f, 1f, 1f - Projectile.timeLeft / 180f), SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(bloom, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, bloom.Size() / 2f, 0.55f, 0f, 0f);
            return false;
        }
    }
}
