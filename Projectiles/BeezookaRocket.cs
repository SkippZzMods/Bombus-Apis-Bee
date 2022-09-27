using BombusApisBee.Dusts;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BombusApisBee.Projectiles
{
    public class BeezookaRocket : BeeProjectile
    {
        public ref float ArcTimer
        {
            get
            {
                return ref Projectile.ai[1];
            }
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bee Rocket");
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.scale = 1f;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(tex, base.Projectile.Center - Main.screenPosition, null, base.Projectile.GetAlpha(lightColor), base.Projectile.rotation, Utils.Size(tex) / 2f, base.Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
        public override void AI()
        {
            ArcTimer += 1f;
            if (ArcTimer < 10f)
            {
                Projectile.velocity.Y -= 1.3f;
            }
            else
            {
                Projectile.velocity.Y += 1.3f;
                if (Projectile.velocity.Y > 16f)
                {
                    Projectile.velocity.Y = 16f;
                }
            }
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
            {
                Projectile.tileCollide = false;
                Projectile.ai[1] = 0f;
                Projectile.alpha = 255;
                Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
                Projectile.width = 200;
                Projectile.height = 200;
                Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
                Projectile.knockBack = 10f;
            }
            if (Math.Abs(Projectile.velocity.X) >= 8f || Math.Abs(Projectile.velocity.Y) >= 8f)
            {
                for (int num246 = 0; num246 < 2; num246++)
                {
                    float num247 = 0f;
                    float num248 = 0f;
                    if (num246 == 1)
                    {
                        num247 = Projectile.velocity.X * 0.5f;
                        num248 = Projectile.velocity.Y * 0.5f;
                    }
                    int num249 = Dust.NewDust(new Vector2(Projectile.position.X + 3f + num247, Projectile.position.Y + 3f + num248) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, ModContent.DustType<HoneyDust>(), 0f, 0f, 25, default(Color), 1f);
                    Main.dust[num249].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
                    Main.dust[num249].velocity *= 0.2f;
                    Main.dust[num249].noGravity = true;
                    num249 = Dust.NewDust(new Vector2(Projectile.position.X + 3f + num247, Projectile.position.Y + 3f + num248) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, DustID.Honey, 0f, 0f, 25, default(Color), 0.5f);
                    Main.dust[num249].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
                    Main.dust[num249].velocity *= 0.05f;
                }
            }
            if (Math.Abs(Projectile.velocity.X) < 15f && Math.Abs(Projectile.velocity.Y) < 15f)
            {
                Projectile.velocity *= 1.5f;
            }
            else if (Utils.NextBool(Main.rand, 2))
            {
                int num250 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Honey, 0f, 0f, 25, default(Color), 1f);
                Main.dust[num250].scale = 0.1f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[num250].fadeIn = 1.5f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[num250].noGravity = true;
                Main.dust[num250].position = Projectile.Center + Utils.RotatedBy(new Vector2(0f, -(float)Projectile.height / 2f), (double)Projectile.rotation, default(Vector2)) * 1.1f;
                Main.rand.Next(2);
                num250 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<HoneyDust>(), 0f, 0f, 25, default(Color), 1f);
                Main.dust[num250].scale = 1f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[num250].noGravity = true;
                Main.dust[num250].position = Projectile.Center + Utils.RotatedBy(new Vector2(0f, -(float)Projectile.height / 2f - 6f), (double)Projectile.rotation, default(Vector2)) * 1.1f;
            }
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.58f;
        }
        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            Projectile.position = Projectile.Center;
            Projectile.width = 250;
            Projectile.height = 250;
            Projectile.position -= Projectile.Size * 0.5f;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 9;
            Projectile.Damage();
            if (Main.myPlayer == Projectile.owner)
            {
                for (int i = 0; i < 5 + Main.rand.Next(4); i++)
                {
                    Vector2 vel = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    while (vel.X == 0f && vel.Y == 0f)
                    {
                        vel = new Vector2(Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    }
                    vel.Normalize();
                    vel *= (float)Main.rand.Next(70, 101) * 0.1f;
                    int type = player.beeType();
                    int damage = player.beeDamage(Projectile.damage);
                    float knockBack = player.beeKB(Projectile.knockBack);
                    Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.position, vel, type, damage * 2 / 3, knockBack, Projectile.owner).DamageType = BeeUtils.BeeDamageClass();
                }
            }
            var modPlayer2 = player.GetModPlayer<BombusApisBeePlayer>();
            modPlayer2.shakeTimer = 16;
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
            SoundEngine.PlaySound(SoundID.Item74, Projectile.position);
            for (int num520 = 0; num520 < 30; num520++)
            {
                int num521 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Honey, 0f, 0f, 25, default(Color), 1.75f);
                Main.dust[num521].velocity *= 3.2f;
                if (Utils.NextBool(Main.rand, 2))
                {
                    Main.dust[num521].scale = 0.5f;
                    Main.dust[num521].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num522 = 0; num522 < 27; num522++)
            {
                int num523 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<HoneyDust>(), 0f, 0f, 25, default(Color), 2.2f);
                Main.dust[num523].noGravity = true;
                Main.dust[num523].velocity *= 5.5f;
                num523 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<HoneyDust>(), 0f, 0f, 25, default(Color), 2.2f);
                Main.dust[num523].velocity *= 2.2f;
            }
        }
    }
}
