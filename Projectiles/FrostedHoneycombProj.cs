using Terraria;
namespace BombusApisBee.Projectiles
{
    public class FrostedHoneycombProj : BeeProjectile
    {
        public override string Texture => "BombusApisBee/Items/Weapons/BeeKeeperDamageClass/FrostedHoneycomb";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frozen Honeycomb");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
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

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);

            Vector2 velo = -Vector2.UnitY.RotatedByRandom(0.3f) * Main.rand.NextFloat(5f, 8f);

            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center + Projectile.velocity, velo, ModContent.ProjectileType<FrostedProjectile_Homing>(), (int)(Projectile.damage * 0.66f), 2f, Projectile.owner);
            velo = -Vector2.UnitY.RotatedByRandom(0.3f) * Main.rand.NextFloat(8f, 12f);
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center + Projectile.velocity, velo, ModContent.ProjectileType<FrostedProjectile_Solid>(), Projectile.damage, 5f, Projectile.owner);
            velo = -Vector2.UnitY.RotatedByRandom(0.3f) * Main.rand.NextFloat(8f, 12f);
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center + Projectile.velocity, velo, ModContent.ProjectileType<FrostedProjectile_Piercing>(), (int)(Projectile.damage * 0.75f), 1.5f, Projectile.owner);

            for (int i = 0; i < 25; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10f, 10f), ModContent.DustType<Dusts.FrostedDust>(), (velo * Main.rand.NextFloat(0.25f)).RotatedByRandom(0.3f));
                dust.noGravity = true;
                dust.velocity *= 0.5f;

                dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), ModContent.DustType<Dusts.FrostedDust>());
                dust.velocity *= 0.5f;

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), ModContent.DustType<Dusts.GlowFastDecelerate>(), newColor: new Color(153, 212, 242)).scale = 0.4f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff<Glacialstruck>(180);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, null, lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.65f, (i / (float)Projectile.oldPos.Length)), 0, 0);
            }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0, 0f);

            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(153, 212, 242, 0) * 0.5f, Projectile.rotation, glowTex.Size() / 2f, 0.65f, 0, 0f);

            return false;
        }
    }

    public class FrostedProjectile_Homing : BeeProjectile
    {
        public override bool? CanDamage() => Projectile.timeLeft < 465;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Homing Icicle");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SafeSetDefaults()
        {
            Projectile.friendly = true;

            Projectile.timeLeft = 480;
            Projectile.width = Projectile.height = 8;
        }

        public override void AI()
        {
            bool foundTarget = false;
            Vector2 targetCenter = Vector2.Zero;
            float num = 800f;
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
                Projectile.velocity = (Projectile.velocity * 20f + Utils.SafeNormalize(targetCenter - Projectile.Center, Vector2.UnitX) * 10f) / 21f;
            else
            {
                Projectile.velocity.Y += 0.10f;
                if (Projectile.velocity.Y > 0)
                {
                    if (Projectile.velocity.Y < 13f)
                        Projectile.velocity.Y *= 1.075f;
                    else
                        Projectile.velocity.Y *= 1.03f;
                }
                if (Projectile.velocity.Y > 16f)
                    Projectile.velocity.Y = 16f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }

        public override void OnKill(int timeLeft)
        {
            SoundID.DD2_WitherBeastDeath.PlayWith(Projectile.Center, pitch: 0.35f);
            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.FrostedDust>());
                dust.noGravity = true;
                dust.velocity *= 0.5f;
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(7f, 7f), ModContent.DustType<Dusts.FrostedDust>()).velocity *= 0.5f;

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), newColor: new Color(153, 212, 242)).scale = 0.3f;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff<Glacialstruck>(180);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, null, lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.45f, (i / (float)Projectile.oldPos.Length)), 0, 0);
            }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0, 0f);

            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(153, 212, 242, 0) * 0.5f, Projectile.rotation, glowTex.Size() / 2f, 0.45f, 0, 0f);
            return false;
        }
    }
    public class FrostedProjectile_Solid : BeeProjectile
    {
        public override bool? CanDamage() => Projectile.timeLeft < 455;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Solid Icicle");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SafeSetDefaults()
        {
            Projectile.friendly = true;
            Projectile.timeLeft = 480;
            Projectile.width = Projectile.height = 12;
        }

        public override void AI()
        {
            Projectile.rotation += (0.35f * (Projectile.velocity.X * 0.15f)) * Projectile.direction;
            Projectile.velocity.Y += 0.25f;
            if (Projectile.velocity.Y > 0)
            {
                if (Projectile.velocity.Y < 13f)
                    Projectile.velocity.Y *= 1.075f;
                else
                    Projectile.velocity.Y *= 1.03f;
            }
            if (Projectile.velocity.Y > 16f)
                Projectile.velocity.Y = 16f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundID.DD2_WitherBeastDeath.PlayWith(Projectile.Center, pitch: 0.35f);
            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.FrostedDust>());
                dust.noGravity = true;
                dust.velocity *= 0.5f;
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(7f, 7f), ModContent.DustType<Dusts.FrostedDust>()).velocity *= 0.5f;

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), newColor: new Color(153, 212, 242)).scale = 0.3f;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff<Glacialstruck>(240);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, null, lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.oldRot[i], tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.45f, (i / (float)Projectile.oldPos.Length)), 0, 0);
            }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0, 0f);

            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(153, 212, 242, 0) * 0.5f, Projectile.rotation, glowTex.Size() / 2f, 0.65f, 0, 0f);
            return false;
        }
    }
    public class FrostedProjectile_Piercing : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Piercing Icicle");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SafeSetDefaults()
        {
            Projectile.friendly = true;
            Projectile.timeLeft = 480;
            Projectile.width = Projectile.height = 8;
            Projectile.penetrate = 5;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.velocity.Y += 0.15f;
            if (Projectile.velocity.Y > 0)
            {
                if (Projectile.velocity.Y < 13f)
                    Projectile.velocity.Y *= 1.075f;
                else
                    Projectile.velocity.Y *= 1.03f;
            }
            if (Projectile.velocity.Y > 16f)
                Projectile.velocity.Y = 16f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundID.DD2_WitherBeastDeath.PlayWith(Projectile.Center, pitch: 0.35f);
            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.FrostedDust>());
                dust.noGravity = true;
                dust.velocity *= 0.5f;
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(7f, 7f), ModContent.DustType<Dusts.FrostedDust>()).velocity *= 0.5f;

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), newColor: new Color(153, 212, 242)).scale = 0.3f;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff<Glacialstruck>(180);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, null, lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.45f, (i / (float)Projectile.oldPos.Length)), 0, 0);
            }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0, 0f);

            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(153, 212, 242, 0) * 0.5f, Projectile.rotation, glowTex.Size() / 2f, 0.55f, 0, 0f);
            return false;
        }
    }
}
