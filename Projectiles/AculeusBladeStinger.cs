using Terraria;
using Terraria.DataStructures;

namespace BombusApisBee.Projectiles
{
    public class AculeusBladeStinger : BeeProjectile
    {
        public int pauseTimer;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stinger");
            Main.projFrames[Type] = 2;
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = Projectile.height = 15;
            Projectile.friendly = true;

            Projectile.penetrate = -1;
            Projectile.timeLeft = 360;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;

            Projectile.extraUpdates = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Main.rand.Next(2);
        }

        public override void AI()
        {
            if (pauseTimer > 0)
                pauseTimer--;

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool ShouldUpdatePosition()
        {
            return pauseTimer <= 0;
        }

        public override bool? CanDamage()
        {
            return pauseTimer <= 0;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<StingerDust>(), -Projectile.oldVelocity.RotatedByRandom(0.6f) * Main.rand.NextFloat(0.75f), Main.rand.Next(50, 255), default, 1.5f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, DustID.Poisoned, -Projectile.oldVelocity.RotatedByRandom(0.6f) * Main.rand.NextFloat(0.75f), Main.rand.Next(50, 255), default, 1.5f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Glow>(), -Projectile.oldVelocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.15f), 0, new Color(50, Main.rand.Next(150, 255), 50), Main.rand.NextFloat(0.4f, 0.6f));
            }

            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<ImpactLineDust>(), -Projectile.oldVelocity.RotatedByRandom(0.6f) * Main.rand.NextFloat(0.25f, 0.5f), 0, new Color(50, Main.rand.Next(150, 255), 50, 0), Main.rand.NextFloat(0.05f, 0.15f));
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (pauseTimer <= 0)
                pauseTimer = 9;

            new SoundStyle("BombusApisBee/Sounds/Item/Impale").PlayWith(Projectile.Center, 0.35f, 0f, 1f);

            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<StingerDust>(), -Projectile.velocity.RotatedByRandom(0.6f) * Main.rand.NextFloat(1f), Main.rand.Next(50, 255), default, 1.5f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, DustID.Poisoned, -Projectile.velocity.RotatedByRandom(0.6f) * Main.rand.NextFloat(1f), Main.rand.Next(50, 255), default, 1.5f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Glow>(), -Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.25f), 0, new Color(50, Main.rand.Next(150, 255), 50), Main.rand.NextFloat(0.4f, 0.6f));
            }

            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<ImpactLineDust>(), -Projectile.velocity.RotatedByRandom(0.6f) * Main.rand.NextFloat(0.5f, 1f), 0, new Color(50, Main.rand.Next(150, 255), 50, 0), 0.1f);
            }

            target.AddBuff(BuffID.Poisoned, 300);

            Main.player[Projectile.owner].Bombus().AddShake(2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texBlur = ModContent.Request<Texture2D>(Texture + "_Blur").Value;
            Texture2D texGlow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + Projectile.Size / 2f;
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.spriteBatch.Draw(tex, drawPos, tex.Frame(verticalFrames: 2, frameY: Projectile.frame), color, Projectile.rotation, tex.Frame(verticalFrames: 2, frameY: Projectile.frame).Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.5f, (k / (float)Projectile.oldPos.Length)), SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texBlur, drawPos, texBlur.Frame(verticalFrames: 2, frameY: Projectile.frame), color with { A = 0 } * 0.5f, Projectile.rotation, texBlur.Frame(verticalFrames: 2, frameY: Projectile.frame).Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.5f, (k / (float)Projectile.oldPos.Length)), SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texGlow, drawPos, texGlow.Frame(verticalFrames: 2, frameY: Projectile.frame), new Color(40, 255, 40, 0) * 0.05f * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length), Projectile.rotation, texGlow.Frame(verticalFrames: 2, frameY: Projectile.frame).Size() / 2f, Projectile.scale, 0f, 0f);
            }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, tex.Frame(verticalFrames: 2, frameY: Projectile.frame), lightColor, Projectile.rotation, tex.Frame(verticalFrames: 2, frameY: Projectile.frame).Size() / 2f, Projectile.scale, 0f, 0f);
            Main.spriteBatch.Draw(texBlur, Projectile.Center - Main.screenPosition, texBlur.Frame(verticalFrames: 2, frameY: Projectile.frame), lightColor with { A = 0 }, Projectile.rotation, texBlur.Frame(verticalFrames: 2, frameY: Projectile.frame).Size() / 2f, Projectile.scale, 0f, 0f);
            Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, texGlow.Frame(verticalFrames: 2, frameY: Projectile.frame), new Color(40, 255, 40, 0) * 0.15f, Projectile.rotation, texGlow.Frame(verticalFrames: 2, frameY: Projectile.frame).Size() / 2f, Projectile.scale, 0f, 0f);

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(40, 255, 40, 0) * 0.25f, 0f, bloomTex.Size() / 2f, 0.35f, 0f, 0f);

            return false;
        }
    }
}
