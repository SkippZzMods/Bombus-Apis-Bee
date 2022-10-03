using BombusApisBee.BeeDamageClass;
using Microsoft.Xna.Framework.Graphics;

namespace BombusApisBee.Projectiles
{
    public class StingerYoyoProj : BeeProjectile
    {
        private int stingercount;
        private int StingerTimer;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 12f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 350f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 14f;
            DisplayName.SetDefault("Stinging Yoyo");

            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
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
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.Hymenoptra().BeeResourceRegenTimer = -120;

            if (player.Hymenoptra().BeeResourceCurrent <= 0)
                Projectile.Kill();

            if (Main.rand.NextBool())
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.StingerDust>(), 0f, 0f, 65);
                dust.noGravity = true;
                dust.scale = 0.9f;
                dust.velocity *= 0.5f;
            }

            if (Main.myPlayer != Projectile.owner)
                return;

            StingerTimer++;
            if (stingercount < 4)
            {
                NPC target = Projectile.FindTargetWithinRange(500f);

                if (target != null)
                {
                    if (StingerTimer > 30 && Collision.CanHitLine(Projectile.Center, 1, 1, target.Center, 1, 1))
                    {
                        player.UseBeeResource(1);
                        StingerTimer = 0;
                        stingercount++;

                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.DirectionTo(target.Center) * 14.5f, ModContent.ProjectileType<StingerFriendly>(), Projectile.damage, 2.5f, Projectile.owner);

                        for (int i = 0; i < 7; i++)
                        {
                            Vector2 velocity = Projectile.DirectionTo(target.Center) * 14.5f;
                            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.StingerDust>(), velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(0.25f), Main.rand.Next(50, 125)).noGravity = true;
                            Dust.NewDustPerfect(Projectile.Center, DustID.Poisoned, velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(0.25f), Main.rand.Next(75)).noGravity = true;
                        }
                    }
                }
            }
            else if (StingerTimer > 45)
            {
                player.UseBeeResource(3);
                SoundID.Item17.PlayWith(Projectile.position);

                for (int i = 0; i < 4; i++)
                {
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.One.RotatedBy(MathHelper.PiOver2 * i) * 10f, ModContent.ProjectileType<HomingStinger>(), Projectile.damage * 2/3, 2f, Projectile.owner);
                }

                StingerTimer = 0;

                stingercount = 0;

                for (int i = 0; i < 15; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.StingerDust>(), Main.rand.NextVector2Circular(3.5f, 3.5f), Main.rand.Next(50, 150), Scale: 1.25f).noGravity = true;

                    Dust.NewDustPerfect(Projectile.Center, DustID.Poisoned, Main.rand.NextVector2Circular(3.5f, 3.5f), Main.rand.Next(50, 150), Scale: 1.25f).noGravity = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, null, lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.55f, (i / (float)Projectile.oldPos.Length)), 0, 0);
            }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0, 0f);
            return false;
        }
    }
}