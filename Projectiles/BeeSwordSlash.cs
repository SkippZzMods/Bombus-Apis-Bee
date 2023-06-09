namespace BombusApisBee.Projectiles
{
    public class BeeSwordSlash : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Slash");
        }
        public override void SafeSetDefaults()
        {
            AIType = ProjectileID.Bullet;
            //projectile.aiStyle = 1;
            Projectile.width = 125;
            Projectile.height = 125;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 180;
            Projectile.scale = 1.1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 35;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Texture = TextureAssets.Projectile[Projectile.type].Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float completionRatio = (float)i / (float)Projectile.oldPos.Length;
                Color drawColor = Color.Lerp(lightColor, Color.Orange, 0.6f);
                drawColor = Color.Lerp(drawColor, new Color(255, 145, 0), completionRatio);
                drawColor = Color.Lerp(drawColor, Color.Transparent, completionRatio);
                Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                Main.spriteBatch.Draw(Texture, drawPosition, null, Projectile.GetAlpha(drawColor), Projectile.oldRot[i], Utils.Size(Texture) * 0.5f, 1.1f, 0, 0f);
            }
            return false;
        }
        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;
            Projectile.alpha = 160;
            if (Projectile.timeLeft == 180)
            {
                Projectile.velocity *= 0.1f;
            }
            else if (Projectile.timeLeft == 170)
            {
                Projectile.velocity *= 18;
            }
            if (Projectile.timeLeft < 15)
            {
                Projectile.alpha += 25;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];
            for (int i = 0; i < 3; i++)
            {
                Vector2 vel = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                while (vel.X == 0f && vel.Y == 0f)
                {
                    vel = new Vector2(Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                }
                vel.Normalize();
                vel *= (float)Main.rand.Next(70, 101) * 0.1f;
                int type = player.beeType();
                int damage2 = player.beeDamage(Projectile.damage);
                float knockback2 = player.beeKB(Projectile.knockBack);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_OnHit(target), target.Center, vel, type, damage2, knockback2, Projectile.owner);
                proj.DamageType = BeeUtils.BeeDamageClass();
            }
            const int Repeats = 45;
            for (int i = 0; i < Repeats; ++i)
            {
                float angle2 = 6.2831855f * (float)i / (float)Repeats;
                Dust dust3 = Dust.NewDustPerfect(target.Center, DustID.Honey2, null, 0, default(Color), 1.1f);
                dust3.velocity = Utils.ToRotationVector2(angle2) * 4f;
                dust3.noGravity = true;
            }
            Projectile.damage = (int)(Projectile.damage * 0.85f);
        }
    }
}
