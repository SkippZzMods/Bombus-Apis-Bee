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
            Projectile.scale *= Main.rand.NextFloat(0.8f, 1.3f);
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

            Projectile.alpha = (int)MathHelper.Lerp(80, 255, 1f - Projectile.timeLeft / 180f);

            if (Main.rand.NextBool(7))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Honey2, 0, 0, Projectile.alpha + Main.rand.Next(25), Scale: Main.rand.NextFloat(0.8f, 1.2f)).noGravity = true;
        }
    }
}
