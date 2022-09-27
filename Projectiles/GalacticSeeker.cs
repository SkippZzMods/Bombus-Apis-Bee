namespace BombusApisBee.Projectiles
{
    public class GalacticSeeker : BeeProjectile
    {
        public override string Texture
        {
            get
            {
                return "BombusApisBee/Projectiles/BlankProj";
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galaxy Seeker");
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
                for (int i = 0; i < 6; i++)
                {

                    int dustType = Main.rand.Next(3);
                    dustType = ((dustType == 0) ? 27 : ((dustType == 1) ? 59 : 58));
                    int num = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 0, default(Color), 1.5f);
                    Main.dust[num].noGravity = true;
                    Dust obj = Main.dust[num];
                    obj.velocity *= 0f;
                }
            }
            if (Projectile.timeLeft < 390)
            {
                float distanceRequired = 900f;
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
