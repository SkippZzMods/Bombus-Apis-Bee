namespace BombusApisBee.Projectiles
{
    public class ChlorophyteHoneycombProjectile : BeeProjectile
    {
        public override string Texture => "BombusApisBee/Items/Weapons/BeeKeeperDamageClass/ChlorophyteHoneycomb";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chloro-comb");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.penetrate = 1;
            Projectile.friendly = true;

            Projectile.timeLeft = 30;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.Length() * 0.01f;
            Projectile.velocity *= 1.01f;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27 with { Pitch = -0.1f, PitchVariance = 0.1f, Volume = 1.25f }, Projectile.position);
            if (Main.myPlayer == Projectile.owner)
            {
                for (int i = 0; i < Main.rand.Next(7, 15); i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity.RotatedByRandom(0.6f) * Main.rand.NextFloat(0.25f, 1f), ModContent.ProjectileType<Chlorospore>(), (int)(Projectile.damage * 0.66f), 0f, Projectile.owner);
                }

                for (int i = 0; i < Main.rand.Next(3, 6); i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity.RotatedByRandom(0.25f) * Main.rand.NextFloat(0.75f, 1f), ModContent.ProjectileType<Chloroshard>(), Projectile.damage / 2, 2.5f, Projectile.owner);
                }

                for (int i = 0; i < Main.rand.Next(4, 7); i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity.RotatedByRandom(0.3f) * 0.25f, ModContent.ProjectileType<ChlorophyteBee>(), Projectile.damage / 2, 0, Projectile.owner);
                }

                for (int i = 0; i < 30; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(), 0, new Color(117, 216, 19), 1f);

                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(), 0, new Color(36, 97, 51), 1f);

                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(4f, 4f), 0, new Color(117, 216, 19), 0.75f);

                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(4f, 4f), 0, new Color(36, 97, 51), 0.75f);
                }

                for (int i = 0; i < 45; ++i)
                {
                    float angle2 = 6.2831855f * (float)i / (float)45;
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Utils.ToRotationVector2(angle2) * 2.75f, 25, new Color(117, 216, 19), 0.65f);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, null, lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.45f, (i / (float)Projectile.oldPos.Length)), 0, 0);

                Main.spriteBatch.Draw(glowTex, (Projectile.oldPos[i] + (Projectile.Size * 0.5f)) - Main.screenPosition, null, (new Color(117, 216, 19, 0) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length)),
                    Projectile.oldRot[i], glowTex.Size() / 2f, Projectile.scale * MathHelper.Lerp(0.7f, 0.05f, (i / (float)Projectile.oldPos.Length)), 0, 0);
            }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0, 0f);

            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(117, 216, 19, 0) * 0.5f, Projectile.rotation, glowTex.Size() / 2f, 0.7f, 0, 0f);

            return false;
        }
    }
    public class Chloroshard : BeeProjectile
    {
        public override bool? CanDamage() => Projectile.timeLeft < 460;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chloro-fragment");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            Main.projFrames[Type] = 3;
        }

        public override void SafeSetDefaults()
        {
            Projectile.friendly = true;

            Projectile.timeLeft = 480;
            Projectile.width = Projectile.height = 8;

            Projectile.frame = Main.rand.Next(3);

            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            bool foundTarget = false;
            Vector2 targetCenter = Vector2.Zero;
            float num = 1000f;
            for (int i = 0; i < 200; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(this, false))
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

            if (foundTarget && Projectile.timeLeft < 460)
                Projectile.velocity = (Projectile.velocity * 40f + Utils.SafeNormalize(targetCenter - Projectile.Center, Vector2.UnitX) * 16f) / 41f;
            else
            {
                Projectile.velocity *= 0.96f;

                if (Projectile.velocity.Length() < 1f)
                    Projectile.Kill();
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + (Projectile.frame == 0 ? MathHelper.ToRadians(20f) : 0f);
        }

        public override void Kill(int timeLeft)
        {
            SoundID.DD2_WitherBeastDeath.PlayWith(Projectile.Center, pitch: -0.05f, 0.1f);

            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(1.5f, 1.5f), 0, new Color(117, 216, 19), 0.35F);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, tex.Frame(verticalFrames: 3, frameY: Projectile.frame), lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, tex.Frame(verticalFrames: 3, frameY: Projectile.frame).Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.45f, (i / (float)Projectile.oldPos.Length)), 0, 0);

                Main.spriteBatch.Draw(glowTex, (Projectile.oldPos[i] + (Projectile.Size * 0.5f)) - Main.screenPosition, null, new Color(117, 216, 19, 0) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.oldRot[i], glowTex.Size() / 2f, Projectile.scale * MathHelper.Lerp(0.25f, 0.05f, (i / (float)Projectile.oldPos.Length)), 0, 0);
            }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, tex.Frame(verticalFrames: 3, frameY: Projectile.frame), lightColor, Projectile.rotation, tex.Frame(verticalFrames: 3, frameY: Projectile.frame).Size() / 2f, Projectile.scale, 0, 0f);

            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(117, 216, 19, 0), Projectile.rotation, glowTex.Size() / 2f, 0.25f, 0, 0f);
            return false;
        }
    }

    public class Chlorospore : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chloro-spore");
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.scale *= Main.rand.NextFloat(0.8f, 1.9f);
            Projectile.rotation = Main.rand.NextFloat(6.28f);

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.95f;
            Projectile.rotation += Projectile.velocity.Length() * 0.02f;

            Projectile.alpha = (int)MathHelper.Lerp(160, 255, 1f - Projectile.timeLeft / 180f);

            if (Main.rand.NextBool(10))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.Glow>(), 0, 0, 0, new Color(214, 158, 79), MathHelper.Lerp(1f, 0f, Projectile.alpha / 255f));

            Vector2 targetCenter = Projectile.Center;
            bool foundTarget = false;
            float num = 1000f;
            for (int i = 0; i < 200; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(this, false))
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
            if (foundTarget)
                Projectile.velocity = (Projectile.velocity * 20f + Utils.SafeNormalize(targetCenter - Projectile.Center, Vector2.UnitX) * 3.5f) / 21f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D bloom = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Color color = Projectile.GetAlpha(new Color(117, 216, 19, 0));
            lightColor.A = 0;

            Main.spriteBatch.Draw(bloom, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, bloom.Size() / 2f, 1f, 0f, 0f);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * MathHelper.Lerp(0.5f, 0.2f, 1f - Projectile.timeLeft / 180f), Projectile.rotation, tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(2f, 1f, 1f - Projectile.timeLeft / 180f), SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(bloom, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, bloom.Size() / 2f, 0.55f, 0f, 0f);
            return false;
        }
    }
}
