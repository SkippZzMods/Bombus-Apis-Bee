using BombusApisBee.BeeDamageClass;
using Microsoft.Xna.Framework.Graphics;

namespace BombusApisBee.Projectiles
{
    public class SpectralBeeTomeHoldout : BeeProjectile
    {
        public Player Owner
        {
            get
            {
                return Main.player[Projectile.owner];
            }
        }
        public ref float Delay
        {
            get
            {
                return ref Projectile.ai[0];
            }
        }
        public override string Texture
        {
            get
            {
                return "BombusApisBee/Items/Weapons/BeeKeeperDamageClass/SpectralBeeTome";
            }
        }

        public int DelayTillNextShot;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spectral Bee Tome");
            Main.projFrames[Projectile.type] = 8;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.Bombus().HeldProj = true;
        }
        public override void AI()
        {
            Projectile.Center = Owner.Center + Vector2.UnitX * Owner.direction * 14f;
            if (++Projectile.frameCounter >= 10)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
            if (!Owner.channel || Owner.noItems || Owner.CCed)
            {
                Projectile.Kill();
                return;
            }
            if (DelayTillNextShot == 0)
                DelayTillNextShot = (int)Owner.ApplyHymenoptraSpeedTo(Owner.HeldItem.useAnimation);
            Delay += 1f;
            if (Delay > DelayTillNextShot)
            {
                ShootBees();
                ShootDusts();
                Delay = 0;
            }
            AdjustPlayerValues();
            Projectile.timeLeft = 2;
            Owner.GetModPlayer<BeeDamagePlayer>().BeeResourceRegenTimer = -60;

        }
        public void ShootBees()
        {
            if (Main.myPlayer != Projectile.owner)
            {
                return;
            }
            if (Owner.GetModPlayer<BeeDamagePlayer>().BeeResourceCurrent == 0)
            {
                Projectile.Kill();
                return;
            }
            SoundEngine.PlaySound(SoundID.NPCDeath52, Projectile.Center);
            for (int i = 0; i < 2; i++)
            {
                Vector2 vector = Projectile.Top;
                Vector2 shootVelocity = -Utils.RotatedBy(Vector2.UnitY, (Utils.NextFloat(Main.rand, -0.13f, 0.23f) * Owner.direction), default(Vector2)) * Owner.gravDir;
                shootVelocity *= Utils.NextFloat(Main.rand, 8f, 10f);
                if (Owner.velocity.Y < 0f)
                {
                    shootVelocity.Y += Owner.velocity.Y * 0.25f;
                }
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), vector, shootVelocity, ModContent.ProjectileType<SpectralBee>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            Projectile.netUpdate = true;
            Owner.UseBeeResource(2);
        }
        public void ShootDusts()
        {
            Vector2 vector = Projectile.Top;
            Vector2 dustVelocity = -Utils.RotatedBy(Vector2.UnitY, (Utils.NextFloat(Main.rand, -0.13f, 0.23f) * Owner.direction), default(Vector2)) * Owner.gravDir;
            dustVelocity *= Utils.NextFloat(Main.rand, 4f, 7f);
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDust(vector, Projectile.width, Projectile.height, DustID.DungeonSpirit, dustVelocity.X, dustVelocity.Y, 0, default, Main.rand.NextFloat(1.1f, 1.5f));
            }
        }
        public void AdjustPlayerValues()
        {
            Projectile.spriteDirection = (Projectile.direction = Owner.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = Utils.ToRotation((float)Projectile.direction * Projectile.velocity);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D bookTexture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = Utils.Frame(bookTexture, 1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 origin = Utils.Size(frame) * 0.5f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(bookTexture, drawPosition, new Rectangle?(frame), Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, Owner.direction == -1 ? SpriteEffects.FlipHorizontally : 0, 0f);
            return false;
        }

        public override bool? CanDamage()
        {
            return false;
        }
    }
}
