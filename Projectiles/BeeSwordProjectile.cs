namespace BombusApisBee.Projectiles
{
    public class BeeSwordProjectile : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("BeeSwordProjectile");
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = 27;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 1050;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 0;

        }

        public override void AI()
        {
            for (int i = 0; i < 3; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<HoneyDust>());
                dust.noGravity = true;
                dust.scale = 0.6f;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int numberProjectiles = 2 + Main.rand.Next(2); // 4 or 5 shots
            for (int i = 0; i < numberProjectiles; i++)
            {
                Player player = Main.player[Projectile.owner];
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        int type = player.beeType();
                        int damage1 = player.beeDamage(Projectile.damage);
                        float knockBack = player.beeKB(Projectile.knockBack);
                        Vector2 vel = new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-5, 5));
                        Projectile.NewProjectile(Projectile.GetSource_OnHit(target), Projectile.Center, vel, type, damage1 * 1 / 3, knockBack, player.whoAmI);
                    }
                }
            }
            base.OnHitNPC(target, damage, knockback, crit);
        }
    }
}