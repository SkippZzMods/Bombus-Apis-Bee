using Microsoft.Xna.Framework.Graphics;

namespace BombusApisBee.Projectiles
{
    public class StingerFriendly : BeeProjectile
    {
        public override string Texture => "BombusApisBee/ExtraTextures/StingerRetexture";
        public override void SafeSetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.HornetStinger;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stinger");
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 600)
                SoundID.Item17.PlayWith(Projectile.position);

            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            for (int i = 0; i < Main.rand.Next(1, 3); i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptGibs);
                dust.scale = 0.9f;
                dust.velocity *= 0.5f;
                dust.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + Projectile.Size / 2f;
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, texture.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.75f, (k / (float)Projectile.oldPos.Length)), SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
