using BombusApisBee.BeeDamageClass;
using BombusApisBee.Dusts;

namespace BombusApisBee.Projectiles
{
    public class TheWaspNestProjectile : BeeProjectile
    {
        internal int BeeTimer;
        public override void SetStaticDefaults()
        {
            // The following sets are only applicable to yoyo that use aiStyle 99.
            // YoyosLifeTimeMultiplier is how long in seconds the yoyo will stay out before automatically returning to the player. 
            // Vanilla values range from 3f(Wood) to 16f(Chik), and defaults to -1f. Leaving as -1 will make the time infinite.
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 12f;
            // YoyosMaximumRange is the maximum distance the yoyo sleep away from the player. 
            // Vanilla values range from 130f(Wood) to 400f(Terrarian), and defaults to 200f
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 325f;
            // YoyosTopSpeed is top speed of the yoyo projectile. 
            // Vanilla values range from 9f(Wood) to 17.5f(Terrarian), and defaults to 10f
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 15.5f;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            // aiStyle 99 is used for all yoyos, and is Extremely suggested, as yoyo are extremely difficult without them
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
                dust.scale = 1.1f;
            }
            BeeTimer++;
            if (BeeTimer > 15)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 vel = new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-5, 5));
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel, ProjectileID.Wasp, Projectile.damage, 1, Projectile.owner).DamageType = BeeUtils.BeeDamageClass();
                    BeeTimer = 0;
                    var BeeDamagePlayer = player.GetModPlayer<BeeDamagePlayer>();
                    player.UseBeeResource(1);
                }
            }
            if (player.Hymenoptra().BeeResourceCurrent <= 0)
            {
                Projectile.Kill();
            }
        }
    }
}