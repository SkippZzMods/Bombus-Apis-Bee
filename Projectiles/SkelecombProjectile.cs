using Terraria;
namespace BombusApisBee.Projectiles
{
    public class SkelecombProjectile : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Skull");
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
        }

        public override void SafeSetDefaults()
        {
            Projectile.timeLeft = 600;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;

            Projectile.width = Projectile.height = 24;

            Projectile.penetrate = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Vector2 targetCenter = Vector2.Zero;
            bool foundTarget = false;
            float num = 1500f;
            for (int i = 0; i < 200; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy())
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
            if (foundTarget && Vector2.Distance(Projectile.Center, targetCenter) > 20f)
                Projectile.velocity = (Projectile.velocity * 35f + Utils.SafeNormalize(targetCenter - Projectile.Center, Vector2.UnitX) * 12f) / 36f;

            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(2f, 2f), 0, new Color(100, 70, 107), 0.45f);

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 0, new Color(238, 164, 255), 0.55f);
            }

            SoundID.NPCHit2.PlayWith(Projectile.Center);

            target.AddBuff<SkeletalCurse>(240);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texBloom = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, null, new Color(238, 164, 255, 0) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.5f, (i / (float)Projectile.oldPos.Length)), Projectile.direction == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0);
            }

            Main.spriteBatch.Draw(texBloom, Projectile.Center - Main.screenPosition, null, new Color(238, 164, 255, 0), 0f, texBloom.Size() / 2f, 0.55f, 0f, 0f);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, Projectile.direction == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0f);

            return false;
        }
    }
}
