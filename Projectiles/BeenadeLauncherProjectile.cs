namespace BombusApisBee.Projectiles
{
    public class BeenadeLauncherProjectile : ModProjectile
    {
        public bool bouncing;

        public override bool? CanDamage() => !bouncing;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pipebeeoms");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;

            Projectile.friendly = true;
            Projectile.DamageType = BeeUtils.BeeDamageClass();

            Projectile.timeLeft = 240;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (bouncing)
            {
                Projectile.velocity.Y += 0.15f;
                Projectile.velocity *= 0.97f;
                Projectile.rotation += Projectile.velocity.Length() * 0.1f;
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                if (Projectile.timeLeft < 220)
                    Projectile.velocity.Y += 0.75f;
                if (Projectile.velocity.Y > 16f)
                    Projectile.velocity.Y = 16f;
            }

            if (Projectile.timeLeft <= 45)
            {
                for (int i = 0; i < 2; i++)
                {
                    float radius = MathHelper.Lerp(20f, 0f, 1f - (Projectile.timeLeft / 45f));
                    Vector2 circle = Main.rand.NextVector2CircularEdge(radius, radius);
                    Dust.NewDustPerfect(Projectile.Center + circle, ModContent.DustType<Glow>(), circle.DirectionTo(Projectile.Center), 0, Color.Orange, Main.rand.NextFloat(0.2f, 0.3f));
                }
            }
            else
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowFastDecelerate>(), null, 0, Color.Orange, 0.35f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }

            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }

            Projectile.velocity *= 0.9f;
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Terraria.Audio.SoundEngine.PlaySound(new Terraria.Audio.SoundStyle("BombusApisBee/Sounds/Item/FireCast"), Projectile.position);
            Projectile.timeLeft = 45;
            Projectile.velocity *= -0.3f;
            Projectile.velocity.Y -= 2.5f;
            bouncing = true;
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            new Terraria.Audio.SoundStyle("BombusApisBee/Sounds/Item/FireHit").PlayWith(pos: Projectile.position, pitchVariance: 0.1f);
            for (int i = 0; i < 25; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Glow>(), (Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(10f)).RotatedByRandom(0.3f), 0, Color.Orange, Main.rand.NextFloat(0.8f, 0.95f));
            }
            for (int i = 0; i < 40; i++)
            {
                float angle = 6.2831855f * (float)i / (float)40;
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowFastDecelerate>(), Utils.ToRotationVector2(angle) * 1.45f, 0, Color.Orange, 0.5f);
            }
            if (Main.myPlayer == Projectile.owner)
                for (int i = 0; i < Main.rand.Next(5, 9); i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, (Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(6f)).RotatedByRandom(0.3f), player.beeType(), player.beeDamage(Projectile.damage / 2), player.beeKB(0.5f), player.whoAmI);
                }
            for (int i = 0; i < 7; i++)
            {
                Vector2 velo = Projectile.rotation.ToRotationVector2();
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<ExplosionDust>(), velo.RotatedByRandom(0.3f) * Main.rand.NextFloat(5f, 25f), 70 + Main.rand.Next(60), default, Main.rand.NextFloat(0.9f, 1.5f)).rotation = Main.rand.NextFloat(6.28f);

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<ExplosionDustTwo>(), velo.RotatedByRandom(0.45f) * Main.rand.NextFloat(5f, 25f), Main.rand.Next(80) + 40, default, Main.rand.NextFloat(0.9f, 1.5f)).rotation = Main.rand.NextFloat(6.28f);

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<ExplosionDust>(), velo.RotatedByRandom(0.45f) * Main.rand.NextFloat(10f, 30f), 80 + Main.rand.Next(50), default, Main.rand.NextFloat(0.9f, 1.5f)).rotation = Main.rand.NextFloat(6.28f);
            }

            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BasicExplosion>(), (int)(Projectile.damage * 1.5f), 2f, Projectile.owner, 80);

            player.Bombus().AddShake(10);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowTex = ModContent.Request<Texture2D>(Texture + "_Glowy").Value;
            Texture2D whiteTex = ModContent.Request<Texture2D>(Texture + "_White").Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0, 0);
            if (Projectile.timeLeft < 45)
            {
                float progress = 1 - (Projectile.timeLeft / 45f);
                Color overlayColor = Color.White;
                if (progress < 0.5f)
                    overlayColor = Color.Lerp(new Color(0, 0, 0, 0), Color.Orange * 0.5f, progress * 2);
                else
                    overlayColor = Color.Lerp(Color.Orange * 0.5f, Color.White, (progress - 0.5f) * 2);

                Main.spriteBatch.Draw(whiteTex, Projectile.Center - Main.screenPosition, null, overlayColor, Projectile.rotation, whiteTex.Size() / 2f, Projectile.scale, 0, 0);

                progress *= progress;
                Color glowColor = Color.White;
                if (progress < 0.5f)
                    glowColor = Color.Lerp(new Color(0, 0, 0, 0), Color.Orange, progress * 2);
                else
                    glowColor = Color.Lerp(Color.Orange, Color.DarkOrange, (progress - 0.5f) * 2);
                glowColor.A = 0;
                Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, glowColor * 0.75f, Projectile.rotation, glowTex.Size() / 2f, Projectile.scale, 0, 0);

                Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, glowColor, 0f, bloomTex.Size() / 2f, 0.55f, 0, 0);
            }
            return false;
        }
    }
}
