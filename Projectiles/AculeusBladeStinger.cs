using Terraria;
using Terraria.DataStructures;

namespace BombusApisBee.Projectiles
{
    public class AculeusBladeStinger : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stinger");
            Main.projFrames[Type] = 2;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = Projectile.height = 15;
            Projectile.friendly = true;

            Projectile.penetrate = -1;
            Projectile.timeLeft = 360;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            Projectile.extraUpdates = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Main.rand.Next(2);
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 600)
                SoundID.Item17.PlayWith(Projectile.position);

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, tex.Frame(verticalFrames: 2, frameY: Projectile.frame), lightColor, Projectile.rotation, tex.Frame(verticalFrames: 2, frameY: Projectile.frame).Size() / 2f, Projectile.scale, 0f, 0f);
            return false;
        }
    }
}
