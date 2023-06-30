using Terraria;
namespace BombusApisBee.Projectiles
{
    public class CursedEye : BeeProjectile
    {
        public override bool? CanDamage() => Projectile.timeLeft < 465;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Eye");
            Main.projFrames[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SafeSetDefaults()
        {
            Projectile.frame = Main.rand.Next(3);
            Projectile.width = Projectile.height = Projectile.frame == 2 ? 8 : 12;
            Projectile.friendly = true;

            Projectile.penetrate = 1;
            Projectile.timeLeft = 480;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.Length() * 0.025f;

            bool foundTarget = false;
            Vector2 targetCenter = Vector2.Zero;
            float num = 1500f;
            for (int i = 0; i < 200; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(this, false) && Collision.CanHitLine(Projectile.Center, 1, 1, npc.Center, 1, 1))
                {
                    float num2 = Projectile.Distance(npc.Center);
                    if (num > num2)
                    {
                        num = num2;
                        targetCenter = npc.Center;
                        foundTarget = true;
                    }
                }
            }
            if (foundTarget && Projectile.timeLeft < 465)
            {
                Projectile.velocity = (Projectile.velocity * 20f + Utils.SafeNormalize(targetCenter - Projectile.Center, Vector2.UnitX) * 14f) / 21f;
            }

            if (Main.rand.NextBool())
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), null, 0, new Color(97, 130, 30), 0.3f);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath19, Projectile.Center);

            for (int i = 0; i < 10; i++)
            {
                Vector2 velo = Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat();
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, velo.X, velo.Y, Scale: Main.rand.NextFloat(0.7f, 1.2f)).noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, tex.Frame(verticalFrames: 3, frameY: Projectile.frame), Color.White * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, tex.Frame(verticalFrames: 3, frameY: Projectile.frame).Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.5f, (i / (float)Projectile.oldPos.Length)), 0, 0);
            }
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, tex.Frame(verticalFrames: 3, frameY: Projectile.frame), Color.White, Projectile.rotation, tex.Frame(verticalFrames: 3, frameY: Projectile.frame).Size() / 2f, Projectile.scale, 0f, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, 320);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);

            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                Projectile.velocity.X = -oldVelocity.X;

            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }
    }
}
