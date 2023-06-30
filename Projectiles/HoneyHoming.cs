using Terraria;
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
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
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
                float homingVelocity = 12f;
                float N = 20f;
                NPC locatedTarget = Main.npc.Where(n => n.CanBeChasedBy() && n.Distance(Projectile.Center) < 1500f).OrderBy(n => Projectile.Distance(n.Center)).FirstOrDefault();

                if (locatedTarget != null)
                {
                    Vector2 homeDirection = Utils.SafeNormalize(locatedTarget.Center - Projectile.Center, Vector2.UnitY);
                    Projectile.velocity = (Projectile.velocity * N + homeDirection * homingVelocity) / (N + 1f);
                    return;
                }
            }
        }
    }

    public class HoneyHomingMetaballs : BeeProjectile
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
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundID.NPCDeath19.PlayWith(Projectile.Center);

            Main.player[Projectile.owner].AddBuff(BuffID.Honey, 240);
            if (Projectile.ai[0] == 0f)
                Main.player[Projectile.owner].AddBuff(ModContent.BuffType<ImprovedHoney>(), 240);

            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(2f, 2f), ModContent.DustType<Dusts.HoneyMetaballDust>(), -Projectile.velocity.RotatedByRandom(1f) * Main.rand.NextFloat(.3f), 0, default, Main.rand.NextFloat(.5f, 1.5f)).noGravity = true;
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.timeLeft < 410 && target.CanBeChasedBy(Projectile, false);
        }
        public override void AI()
        {
            if (Projectile.localAI[0] > 4f)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(2f, 2f), ModContent.DustType<Dusts.HoneyMetaballDust>(), -Projectile.velocity.RotatedByRandom(0.2f) * 0.15f, 0, default, 1.35f).noGravity = true;
                Dust.NewDustPerfect(Projectile.Center - Projectile.velocity + Main.rand.NextVector2Circular(2f, 2f), ModContent.DustType<Dusts.HoneyMetaballDust>(), -Projectile.velocity.RotatedByRandom(0.2f) * 0.15f, 0, default, 1.35f).noGravity = true;
            }
            else
                Projectile.localAI[0] += 1f;

            if (Projectile.timeLeft < 390)
            {
                float homingVelocity = 12f;
                float N = 20f;
                NPC locatedTarget = Main.npc.Where(n => n.CanBeChasedBy() && n.Distance(Projectile.Center) < 1500f).OrderBy(n => Projectile.Distance(n.Center)).FirstOrDefault();

                if (locatedTarget != null)
                {
                    Vector2 homeDirection = Utils.SafeNormalize(locatedTarget.Center - Projectile.Center, Vector2.UnitY);
                    Projectile.velocity = (Projectile.velocity * N + homeDirection * homingVelocity) / (N + 1f);
                    return;
                }
            }
        }
    }
}
