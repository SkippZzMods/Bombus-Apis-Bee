using BombusApisBee.Buffs;
using BombusApisBee.Dusts;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombusApisBee.Projectiles
{
    public class HymenoptraFlask_Honey : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Flask");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SafeSetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = 1;

            Projectile.timeLeft = 600;

        }
        public override void Kill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                for (int i = 0; i < Main.rand.Next(8, 12); i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2Circular(5f, 5f), ModContent.ProjectileType<HoneySmoke>(), Projectile.damage * 2/3, 0f, Projectile.owner);
                }

                for (int i = 0; i < 5; i++)
                {
                    Projectile.SpawnBee(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2Circular(3.5f, 3.5f), Projectile.damage * 0.5f).DamageType = BeeUtils.BeeDamageClass();
                }
            }

            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<HoneyDust>(), Main.rand.NextVector2Circular(2f, 2f), Main.rand.Next(50, 150), default, 1.5f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, DustID.Honey2, Main.rand.NextVector2Circular(3f, 3f), Main.rand.Next(50, 100), default, 1.5f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<HoneyDust>(), Main.rand.NextVector2Circular(6f, 6f), Main.rand.Next(50, 150), default, 1.5f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, DustID.Honey2, Main.rand.NextVector2Circular(7f, 7f), Main.rand.Next(50, 100), default, 1.5f).noGravity = true;
            }
            

            SoundEngine.PlaySound(SoundID.Item107 with {PitchVariance = 0.15f}, Projectile.position);
        }
        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.Length() * 0.02f;

            if (Projectile.timeLeft < 585)
            {
                Projectile.velocity.Y += 0.95f;
                if (Projectile.velocity.Y > 16f)
                    Projectile.velocity.Y = 16f;
            }
            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12.5f, 12.5f), Main.rand.NextBool() ? DustID.Honey2 : ModContent.DustType<HoneyDust>(), null, Main.rand.Next(100, 150), default, 1.1f).noGravity = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + (Projectile.Size * 0.5f)) - Main.screenPosition, null, lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.oldRot[i], tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.45f, (i / (float)Projectile.oldPos.Length)), 0, 0);
            }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0, 0f);

            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(163, 97, 55, 0), Projectile.rotation, glowTex.Size() / 2f, 0.75f, 0, 0f);
            return false;
        }
    }

    public class HymenoptraFlask_Nectar : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nectar Flask");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SafeSetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = 1;

            Projectile.timeLeft = 600;

        }
        public override void Kill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2Circular(4f, 4f), ModContent.ProjectileType<NectarHealingBolt>(), 0, 0f, Projectile.owner, Main.rand.Next(2, 5));
                }

                for (int i = 0; i < Main.rand.Next(2, 6); i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2Circular(8f, 8f), ModContent.ProjectileType<HomingNectar>(), Projectile.damage / 2, 3f, Projectile.owner, 1);
                }

                for (int i = 0; i < Main.rand.Next(8, 12); i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2Circular(5f, 5f), ModContent.ProjectileType<NectarSmoke>(), Projectile.damage * 2 / 3, 0f, Projectile.owner);
                }
            }

            for (int i = 0; i < 25; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 0, new Color(206, 116, 59), 0.5f);

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(3f, 3f), 0, new Color(206, 116, 59, 100), 0.65f);
            }

            SoundEngine.PlaySound(SoundID.Item107 with { PitchVariance = 0.15f }, Projectile.position);
        }
        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.Length() * 0.02f;

            if (Projectile.timeLeft < 585)
            {
                Projectile.velocity.Y += 0.95f;
                if (Projectile.velocity.Y > 16f)
                    Projectile.velocity.Y = 16f;
            }

            if (Main.rand.NextBool())
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10f, 10f), ModContent.DustType<GlowFastDecelerate>(), Main.rand.NextVector2Circular(1.5f, 1.5f), 0, new Color(206, 116, 59), 0.35f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + (Projectile.Size * 0.5f)) - Main.screenPosition, null, new Color(255, 191, 73, 0) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.oldRot[i], tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.45f, (i / (float)Projectile.oldPos.Length)), 0, 0);

                Main.spriteBatch.Draw(glowTex, (Projectile.oldPos[i] + (Projectile.Size * 0.5f)) - Main.screenPosition, null, new Color(255, 191, 73, 0) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.oldRot[i], glowTex.Size() / 2f, Projectile.scale * MathHelper.Lerp(0.5f, 0.05f, (i / (float)Projectile.oldPos.Length)), 0, 0);
            }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0, 0f);

            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(255, 191, 73, 0) * 0.5f, Projectile.rotation, glowTex.Size() / 2f, 0.55f, 0, 0f);
            return false;
        }
    }

    public class HymenoptraFlask_Stinger : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stinger Flask");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SafeSetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = 1;

            Projectile.timeLeft = 600;

        }
        public override void Kill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                for (int i = 0; i < Main.rand.Next(8, 12); i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2Circular(5f, 5f), ModContent.ProjectileType<PoisonSmoke>(), Projectile.damage * 2 / 3, 0f, Projectile.owner);
                }

                for (int i = 0; i < 6; i++)
                {
                    Vector2 velo = Vector2.UnitY.RotatedByRandom(0.7f) * -Main.rand.NextFloat(10f, 15f);
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, velo, ModContent.ProjectileType<StingerGravity>(), Projectile.damage * 2 / 3, 3f, Projectile.owner);
                }
            }

            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Poisoned, Main.rand.NextVector2Circular(2f, 2f), Main.rand.Next(50, 150), default, 1.5f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, DustID.CorruptGibs, Main.rand.NextVector2Circular(3f, 3f), Main.rand.Next(50, 100), default, 1.5f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, DustID.Poisoned, Main.rand.NextVector2Circular(6f, 6f), Main.rand.Next(50, 150), default, 1.5f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, DustID.CorruptGibs, Main.rand.NextVector2Circular(7f, 7f), Main.rand.Next(50, 100), default, 1.5f).noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.Item107 with { PitchVariance = 0.15f }, Projectile.position);
        }
        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.Length() * 0.02f;

            if (Projectile.timeLeft < 585)
            {
                Projectile.velocity.Y += 0.95f;
                if (Projectile.velocity.Y > 16f)
                    Projectile.velocity.Y = 16f;
            }

            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12.5f, 12.5f), Main.rand.NextBool() ? DustID.Poisoned : DustID.CorruptGibs, null, Main.rand.Next(100, 150), default, 1.1f).noGravity = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + (Projectile.Size * 0.5f)) - Main.screenPosition, null, lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.oldRot[i], tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.45f, (i / (float)Projectile.oldPos.Length)), 0, 0);
            }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0, 0f);

            Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(228, 226, 49, 0) * 0.65f, Projectile.rotation, glowTex.Size() / 2f, 0.45f, 0, 0f);
            return false;
        }
    }

    public class StingerGravity : BeeProjectile
    {
        public override void SafeSetDefaults()
        {
            Projectile.frame = Main.rand.Next(2);
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stinger");
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            Main.projFrames[Type] = 2;
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 600)
                SoundID.Item17.PlayWith(Projectile.position);

            if (Projectile.timeLeft < 285)
                Projectile.velocity.Y += 0.55f;

            if (Projectile.velocity.Y > 16f)
                Projectile.velocity.Y = 16f;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(70f);

            for (int i = 0; i < Main.rand.Next(1, 3); i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptGibs);
                dust.scale = 1f;
                dust.velocity *= 0.5f;
                dust.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 360);
            target.AddBuff<ImprovedPoison>(360);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = texture.Frame(verticalFrames: 2, frameY: Projectile.frame);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + Projectile.Size / 2f;
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.rotation, frame.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.75f, (k / (float)Projectile.oldPos.Length)), SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
