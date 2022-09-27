using BombusApisBee.Dusts;

namespace BombusApisBee.Projectiles
{
    public class TheHiveProjectile : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[base.Projectile.type] = 7f;
            ProjectileID.Sets.YoyosMaximumRange[base.Projectile.type] = 240f;
            ProjectileID.Sets.YoyosTopSpeed[base.Projectile.type] = 11f;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 99;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 13;
        }

        public override void PostAI()
        {
            Player player = Main.player[Projectile.owner];
            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<HoneyDust>());
                dust.noGravity = true;
                dust.scale = 0.8f;
            }
            if (player.Hymenoptra().BeeResourceCurrent <= 0)
            {
                Projectile.Kill();
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {

            Player player = Main.player[Projectile.owner];
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    int type = player.beeType();
                    int damage1 = player.beeDamage(Projectile.damage);
                    float knockBack = player.beeKB(Projectile.knockBack);
                    Vector2 vel = new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-5, 5));
                    target.AddBuff(BuffID.Poisoned, 250, true);
                    Projectile.NewProjectileDirect(Projectile.GetSource_OnHit(target), Projectile.Center, vel, type, damage1 * 2 / 3, knockBack, player.whoAmI).DamageType = BeeUtils.BeeDamageClass(); ;
                }
            }
            player.UseBeeResource(1);
        }

    }
}