using BombusApisBee.BeeDamageClass;

namespace BombusApisBee.Projectiles
{
    public class StingerYoyoProj : BeeProjectile
    {
        private int stingercount;
        private int StingerTimer;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 14f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 285f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 14f;
            DisplayName.SetDefault("Stinging Yoyo");
        }

        public override void SafeSetDefaults()
        {
            Projectile.extraUpdates = 0;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = 99;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.scale = 0.85f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 13;

        }
        public override void PostAI()
        {
            Player player = Main.player[Projectile.owner];
            var BeeDamagePlayer = player.GetModPlayer<BeeDamagePlayer>();
            BeeDamagePlayer.BeeResourceRegenTimer = -120;
            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Poisoned, 0f, 0f, 65);
                dust.noGravity = true;
                dust.scale = 0.9f;
                dust.velocity *= Main.rand.NextFloat(1.2f, 1.35f);
            }
            if (player.Hymenoptra().BeeResourceCurrent <= 0)
            {
                Projectile.Kill();
            }
            StingerTimer++;
            if (stingercount < 6)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    for (int i = 0; i < 200; i++)
                    {
                        NPC target = Main.npc[i];
                        float shootToX = target.position.X + (float)target.width * 0.5f - Projectile.Center.X;
                        float shootToY = target.position.Y - Projectile.Center.Y;
                        float distance = (float)System.Math.Sqrt((double)(shootToX * shootToX + shootToY * shootToY));
                        if (distance < 350f && !target.friendly && target.active && Main.npc[i].CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.position, 1, 1, target.position, 1, 1))
                        {
                            if (StingerTimer > 25f)
                            {
                                if (Main.myPlayer == Projectile.owner)
                                {
                                    distance = 3f / distance;
                                    shootToX *= distance * 5.5f;
                                    shootToY *= distance * 5.5f;
                                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, shootToX, shootToY, ProjectileID.HornetStinger, Projectile.damage, Projectile.knockBack, Main.myPlayer);
                                    Main.projectile[proj].DamageType = BeeUtils.BeeDamageClass();
                                    player.UseBeeResource(1);
                                    stingercount += 1;
                                    StingerTimer = 0;
                                }
                            }
                        }
                    }
                }
            }
            else if (StingerTimer > 25f && stingercount >= 6)
            {
                Vector2 speed = new Vector2(20f, 20f);
                float numberProjectiles = 8;
                float rotation = MathHelper.ToRadians(360);
                for (int i = 0; i < numberProjectiles; i++)
                {
                    Vector2 perturbedSpeed = new Vector2(speed.X, speed.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f;
                    int stinger = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.position.X, Projectile.position.Y, perturbedSpeed.X * 2, perturbedSpeed.Y * 2, ProjectileID.HornetStinger, Projectile.damage, 1f, player.whoAmI);
                    Main.projectile[stinger].GetGlobalProjectile<BombusApisBeeGlobalProjectile>().ForceBee = true;
                    Main.projectile[stinger].penetrate = 2;
                    Main.projectile[stinger].usesLocalNPCImmunity = true;
                    Main.projectile[stinger].localNPCHitCooldown = 20;
                }
                stingercount = 0;
                StingerTimer = 0;
                player.UseBeeResource(3);
            }
        }
    }
}