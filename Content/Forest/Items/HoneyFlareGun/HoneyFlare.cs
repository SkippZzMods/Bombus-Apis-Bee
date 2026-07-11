using BombusApisBee.Core.Systems.ParticleSystem;
using System.IO;
using Terraria;

namespace BombusApisBee.Content.Forest.Items.HoneyFlareGun
{
    public class HoneyFlare : BeeProjectile
    {
        int enemyWhoAmI;
        bool stuck = false;
        Vector2 offset = Vector2.Zero;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Flare");
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;

            Projectile.penetrate = -1;
            Projectile.timeLeft = 360;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool PreAI()
        {
            NPC target = Main.npc[enemyWhoAmI];

            if (stuck)
            {
                Projectile.position = target.position + offset;

                if (!target.active)
                    Projectile.Kill();
                
                if (Main.rand.NextBool(3))
                {
                    Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * 0.5f, DustID.Honey2, -Projectile.velocity.RotatedByRandom(0.6f) * Main.rand.NextFloat(0.1f, 0.2f),
                        Main.rand.Next(60, 150), Scale: Main.rand.NextFloat(0.5f, 1f)).noGravity = true;
                }

                return false;
            }

            return base.PreAI();
        }
        public override void AI()
        {
            if (Projectile.timeLeft < 340)
                Projectile.velocity.Y += 0.15f;

            if (Projectile.velocity.Y > 16f)
                Projectile.velocity.Y = 16f;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            ParticleHandler.SpawnParticle(new SmallCompositeSmoke(Projectile.Center, Projectile.velocity * 0.3f, new Color(255, Main.rand.Next(190, 210), 20), 20, false, true, null));
            
            if (Main.rand.NextBool())
                ParticleHandler.SpawnParticle(new SmallCompositeSmoke(Projectile.Center, Projectile.velocity * 0.3f, new Color(255, Main.rand.Next(150, 170), 20), 20, false, true, null));

            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(Projectile.Center - Projectile.velocity * 0.5f, DustID.Honey2, -Projectile.velocity.RotatedByRandom(0.6f) * Main.rand.NextFloat(0.1f, 0.2f),
                    Main.rand.Next(100), Scale: Main.rand.NextFloat(0.5f, 1.5f)).noGravity = true;
            }

            if (Projectile.timeLeft > 330)
            {
                Dust.NewDustPerfect(Projectile.Center - Projectile.velocity * 0.5f, ModContent.DustType<GlowFastDecelerate>(), -Projectile.velocity.RotatedByRandom(0.6f) * Main.rand.NextFloat(0.1f, 0.2f),
                    Main.rand.Next(100), new Color(255, 105, 0), Main.rand.NextFloat(0.3f, 0.5f));
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (stuck)
            {
                Main.npc[enemyWhoAmI].GetGlobalNPC<HoneyFlareNPC>().flares.Remove(this);
            }

            /*if (!stuck)
                return;

            Main.player[Projectile.owner].Bombus().AddShake(7);
            SoundID.DD2_ExplosiveTrapExplode.PlayWith(Projectile.position, pitchVariance: 0.1f);

            for (int i = 0; i < 35; i++)
            {
                Dust.NewDustPerfect(Projectile.Center - Projectile.velocity * 0.5f, DustID.Honey2, -Projectile.velocity.RotatedByRandom(0.85f) * Main.rand.NextFloat(0.2f, 0.4f),
                    Main.rand.Next(80), Scale: Main.rand.NextFloat(0.8f, 1.2f)).noGravity = true;
            }

            for (int i = 0; i < Main.rand.Next(1, 4); i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, -Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.5f, 1f),
                    ProjectileType<HoneyHoming>(), (int)(Projectile.damage * 0.75f), 3f, Projectile.owner, 1f);
            }

            for (int i = 0; i < Main.rand.Next(3, 6); i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, -Projectile.velocity.RotatedByRandom(0.8f) * Main.rand.NextFloat(0.15f, 0.5f),
                    ProjectileType<HoneySmoke>(), (int)(Projectile.damage * 0.75f), 0f, Projectile.owner);
            }*/
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Honey2, Main.rand.NextVector2Circular(4, 4), 110, default, 1.25f).noGravity = true;
            }

            if (!stuck && target.life > 0)
            {
                stuck = true;
                Projectile.friendly = false;
                Projectile.tileCollide = false;
                enemyWhoAmI = target.whoAmI;
                offset = Projectile.position - target.position;
                offset -= Projectile.velocity;
                Projectile.timeLeft = 1800;
                Projectile.netUpdate = true;
                target.GetGlobalNPC<HoneyFlareNPC>().flares.Add(this);
            }
            else
                Projectile.Kill();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Request<Texture2D>(Texture).Value;
            
            var bloom = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            var star = Request<Texture2D>("BombusApisBee/Assets/ExtraTextures/StarAlpha").Value;

            if (!stuck)
            {
                Main.spriteBatch.Draw(bloom, Projectile.Center - Projectile.velocity - Main.screenPosition, null, new Color(255, 180, 0, 0), 0f, bloom.Size() / 2f, Projectile.scale * 0.35f, 0, 0f);

                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Main.spriteBatch.Draw(tex, Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f - Main.screenPosition, null, lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                        Projectile.rotation, tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.65f, i / (float)Projectile.oldPos.Length), Projectile.direction == -1 ? (SpriteEffects)1 : 0, 0);
                }
            }

            if (!stuck)
                Main.spriteBatch.Draw(bloom, Projectile.Center - Main.screenPosition, null, new Color(255, 150, 0, 0), 0f, bloom.Size() / 2f, Projectile.scale * 0.4f, 0, 0f);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, Projectile.direction == -1 ? (SpriteEffects)1 : 0, 0f);
            
            if (Projectile.timeLeft > 330 && !stuck)
            {
                float lerp = EaseBuilder.EaseQuinticOut.Ease((Projectile.timeLeft - 330) / 30f);

                Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(255, 150, 0, 0) * lerp, 0f, star.Size() / 2f, Projectile.scale * 0.6f, 0, 0f);
               
                Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(255, 150, 0, 0) * lerp, MathHelper.PiOver4, star.Size() / 2f, Projectile.scale * 0.4f, 0, 0f);
            }

            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(stuck);
            writer.WritePackedVector2(offset);
            writer.Write(enemyWhoAmI);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            stuck = reader.ReadBoolean();
            offset = reader.ReadPackedVector2();
            enemyWhoAmI = reader.ReadInt32();
        }
    }
}
