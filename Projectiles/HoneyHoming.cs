using BombusApisBee.Buffs;

namespace BombusApisBee.Projectiles
{
    public class HoneyHoming : BeeProjectile
    {
        public override string Texture => "BombusApisBee/Projectiles/BlankProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey");
        }


        public override void SafeSetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 420;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.player[Projectile.owner].AddBuff(BuffID.Honey, 240);
            if (Projectile.ai[0] == 0f)
                Main.player[Projectile.owner].AddBuff(ModContent.BuffType<ImprovedHoney>(), 240);
        }
        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.timeLeft < 390 && target.CanBeChasedBy(Projectile, false);
        }
        public override void AI()
        {

            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 4f)
            {
                for (int i = 0; i < 3; i++)
                {
                    int num = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Honey, 0f, 0f, 100, default(Color), 1f);
                    Main.dust[num].noGravity = true;
                    Dust obj = Main.dust[num];
                    obj.velocity *= 0f;
                }
            }
            if (Projectile.timeLeft < 390)
            {
                float distanceRequired = 1500f;
                float homingVelocity = 12f;
                float N = 20f;
                Vector2 destination = Projectile.Center;
                bool locatedTarget = false;
                for (int i = 0; i < 200; i++)
                {
                    float extraDistance = (float)(Main.npc[i].width / 2 + Main.npc[i].height / 2);
                    if (Main.npc[i].CanBeChasedBy(Projectile, false) && Projectile.WithinRange(Main.npc[i].Center, distanceRequired + extraDistance))
                    {
                        destination = Main.npc[i].Center;
                        locatedTarget = true;
                        break;
                    }
                }
                if (locatedTarget)
                {
                    Vector2 homeDirection = Utils.SafeNormalize(destination - Projectile.Center, Vector2.UnitY);
                    Projectile.velocity = (Projectile.velocity * N + homeDirection * homingVelocity) / (N + 1f);
                    return;
                }
            }
        }
    }
}
