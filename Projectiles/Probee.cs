using BombusApisBee.BeeHelperProj;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace BombusApisBee.Projectiles
{
    public class Probee : BeeHelper
    {
        public int ShootDelay = 45;

        public override int SmallHeight => 16;

        public override int SmallWidth => 16;

        public override bool CanBeGiant => false;

        public override int RegularBeePenetrate => 3;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Probee");
            Main.projFrames[Projectile.type] = 1;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath14, Projectile.Center);

            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), default, 55,
                    Main.rand.NextBool() ? new Color(213, 95, 89) : new Color(161, 31, 85), Main.rand.NextFloat(0.5f, 0.6f)).velocity *= 1.25f;
            }
        }
        public override void SafeAI()
        {
            if (ShootDelay > 0)
                ShootDelay--;

            if (Main.rand.NextBool(Projectile.velocity.Length() < 3f ? 10 : 3))
                Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, ModContent.DustType<Dusts.GlowFastDecelerate>(), default, 55,
                    Main.rand.NextBool() ? new Color(213, 95, 89) : new Color(161, 31, 85), Main.rand.NextFloat(0.5f, 0.6f));

            NPC target = Main.npc.Where(n => n.CanBeChasedBy() && n.Distance(Projectile.Center) < 750f).OrderBy(n => Vector2.Distance(n.Center, Projectile.Center)).FirstOrDefault();

            if (target != default)
            {
                if (Collision.CanHitLine(Projectile.Center, 1, 1, target.Center, 1, 1) && ShootDelay <= 0)
                {
                    ShootDelay = 100;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.velocity * 2f, Projectile.DirectionTo(target.Center) * 8f,
                        ModContent.ProjectileType<RedLaser>(), Projectile.damage, 3f, Projectile.owner);

                    SoundID.Item33.PlayWith(Projectile.position, -0.15f, 0.1f);
                }
            }
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Main.spriteBatch.Draw(tex, Projectile.Center + Projectile.velocity - Main.screenPosition, null, new Color(161, 31, 85, 0) * 0.5f, Projectile.rotation, tex.Size() / 2f, 0.45f, 0, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center + Projectile.velocity - Main.screenPosition, null, new Color(213, 95, 89, 0) * 0.5f, Projectile.rotation, tex.Size() / 2f, 0.45f, 0, 0);
        }
    }
}