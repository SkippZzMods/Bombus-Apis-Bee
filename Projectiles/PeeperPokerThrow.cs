namespace BombusApisBee.Projectiles
{
    public class PeeperPokerThrow : BeeProjectile
    {
        public override string Texture => "BombusApisBee/Projectiles/PeeperPokerHoldout";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Peeper Poker");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SafeSetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.timeLeft = 240;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, null, lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.5f, (i / (float)Projectile.oldPos.Length)), 0, 0);
            }
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0f, 0f);
            return false;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.direction == -1)
            {
                DrawOffsetX = -10;
                DrawOriginOffsetY = -5;
            }
            Projectile.rotation = Utils.ToRotation(Projectile.velocity) + 0.7853982f;
            int dust = Dust.NewDust(new Vector2(Projectile.Center.X, Projectile.Center.Y - 14), Projectile.width, Projectile.height, DustID.Blood, 0f, 0f, 45, default, 1.25f);
            Main.dust[dust].velocity *= 2.55f;
            Main.dust[dust].noGravity = true;
            Projectile.velocity.Y = Projectile.velocity.Y + gravFloat;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 5f && Main.myPlayer == Projectile.owner)
            {
                SoundEngine.PlaySound(SoundID.NPCHit1);
                for (int i = 0; i < 3; i++)
                {
                    Vector2 vel = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    while (vel.X == 0f && vel.Y == 0f)
                    {
                        vel = new Vector2(Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    }
                    vel.Normalize();
                    vel *= (float)Main.rand.Next(70, 101) * 0.1f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.oldPosition, vel, ModContent.ProjectileType<CthulhuBee>(), Projectile.damage * 1 / 3, Projectile.knockBack, Projectile.owner);
                    const int Repeats = 30;
                    for (int d = 0; d < Repeats; ++d)
                    {
                        float angle2 = 6.2831855f * (float)d / (float)Repeats;
                        Dust dust3 = Dust.NewDustPerfect(Projectile.Center, DustID.Blood, null, 0, default(Color), 1.1f);
                        dust3.velocity = Utils.ToRotationVector2(angle2) * 4f;
                        dust3.noGravity = true;
                    }
                    const int Repeats2 = 45;
                    for (int d2 = 0; d2 < Repeats2; ++d2)
                    {
                        float angle2 = 6.2831855f * (float)d2 / (float)Repeats2;
                        Dust dust3 = Dust.NewDustPerfect(Projectile.Center, DustID.Blood, null, 0, default(Color), 1.1f);
                        dust3.velocity = Utils.ToRotationVector2(angle2) * 4f;
                        dust3.noGravity = true;
                    }
                }
                Projectile.ai[0] = -25f;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            SoundEngine.PlaySound(SoundID.NPCHit18);
            if (Main.myPlayer == Projectile.owner)
            {
                int spread = 7;
                for (int i = 0; i < 3; i++)
                {
                    Vector2 perturbedspeed = Utils.RotatedBy(new Vector2(base.Projectile.velocity.X, base.Projectile.velocity.Y + (float)Main.rand.Next(-3, 4)), (double)MathHelper.ToRadians((float)spread), default(Vector2));
                    Projectile.NewProjectile(Projectile.GetSource_OnHit(target), Projectile.Center, perturbedspeed * 0.175f, ModContent.ProjectileType<CthulhuBee>(), Projectile.damage * 1 / 3, 1f, Projectile.owner);
                }
            }
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, Projectile.velocity.X, Projectile.velocity.Y);
                dust.scale = 0.95f;
                dust.velocity *= Main.rand.NextFloat(0.25f, 0.75f);
            }
        }
        public float gravFloat = 0.075f;

    }
}
