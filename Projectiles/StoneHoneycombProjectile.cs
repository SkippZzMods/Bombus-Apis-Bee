using Microsoft.Xna.Framework.Graphics;

namespace BombusApisBee.Projectiles
{
    public class StoneHoneycombProjectile : BeeProjectile
    {
        public override string Texture => BombusApisBee.BeeWeapon + "StoneHoneycomb";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Petrified Honeycomb");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(10))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Stone);

            if (Main.rand.NextBool(15))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.HoneyDust>());

            Projectile.rotation += (0.35f * (Projectile.velocity.X * 0.15f)) * Projectile.direction;
            Projectile.velocity.Y += 0.2f;
            if (Projectile.velocity.Y > 0)
            {
                if (Projectile.velocity.Y < 13f)
                    Projectile.velocity.Y *= 1.085f;
                else
                    Projectile.velocity.Y *= 1.04f;
            }
            if (Projectile.velocity.Y > 16f)
                Projectile.velocity.Y = 16f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, null, lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.75f, (i / (float)Projectile.oldPos.Length)), 0, 0);
            }
            return true;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact with { Volume = 0.85f, PitchVariance = 0.15f }, Projectile.position);
            for (int i = 1; i < 4; i++)
            {
                Gore.NewGorePerfect(Projectile.GetSource_Death(), Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), Main.rand.NextVector2Circular(5f, 5f), Mod.Find<ModGore>("StoneHoneycombGore_" + i).Type).timeLeft = 90;
            }
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool(3) ? DustID.Bone : DustID.Stone);
            }

            for (int i = 0; i < Main.rand.Next(3); i++)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center + Main.rand.NextVector2Circular(15f, 15f),
                    Main.rand.NextVector2Circular(3.5f, 3.5f), Main.player[Projectile.owner].beeType(), Main.player[Projectile.owner].beeDamage(Projectile.damage / 2), 0f, Projectile.owner);

                Gore.NewGorePerfect(Projectile.GetSource_Death(), Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), Main.rand.NextVector2Circular(5f, 5f), Mod.Find<ModGore>("DriedHoneycombGore_" + Main.rand.Next(1, 4)).Type).timeLeft = 90;
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Projectile.velocity.Y > 12f)
                crit = true;
        }
    }
}
