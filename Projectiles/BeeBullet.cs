using Terraria;
namespace BombusApisBee.Projectiles
{
    public class BeeBullet : BeeProjectile
    {
        public override void SafeSetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bee Bullet");
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
            Main.projFrames[Type] = 3;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (++Projectile.frameCounter % 6 == 0)
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 300);
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            if (Main.myPlayer == Projectile.owner)
            {
                int type = player.beeType();
                int damage = player.beeDamage(Projectile.damage);
                float knockBack = player.beeKB(Projectile.knockBack);
                Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity * 0.5f, type, damage, knockBack, Projectile.owner).
                    DamageType = BeeUtils.BeeDamageClass();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + Projectile.Size / 2f;
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, texture.Frame(verticalFrames: 3, frameY: Projectile.frame), color, Projectile.rotation, texture.Frame(verticalFrames: 3, frameY: Projectile.frame).Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.35f, (k / (float)Projectile.oldPos.Length)), SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(verticalFrames: 3, frameY: Projectile.frame), lightColor, Projectile.rotation, texture.Frame(verticalFrames: 3, frameY: Projectile.frame).Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            
            return false;
        }
    }
}
