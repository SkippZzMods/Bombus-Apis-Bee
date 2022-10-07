using BombusApisBee.Buffs;

namespace BombusApisBee.Projectiles
{
    public class HoneyBeeProj : ModProjectile
    {
        public override string Texture => "BombusApisBee/Items/Accessories/BeeKeeperDamageClass/HoneyBee";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Bee");
            Main.projFrames[Projectile.type] = 4;
        }
        public sealed override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.spriteDirection = Projectile.direction;
            if (player.dead || !player.active || !player.Bombus().HoneyBee)
            {
                player.ClearBuff(ModContent.BuffType<HoneyBeeBuff>());
            }
            if (player.HasBuff(ModContent.BuffType<HoneyBeeBuff>()))
            {
                Projectile.timeLeft = 2;
            }
            //movement
            Vector2 idlePosition = player.Center;
            idlePosition.Y -= 68f;
            Vector2 vectorToIdlePosition = idlePosition - Projectile.Center;
            float distanceToIdlePosition = vectorToIdlePosition.Length();
            float speed = 8f;
            float inertia = 20f;
            if (distanceToIdlePosition > 600f)
            {
                speed = 12f;
                inertia = 60f;
            }
            else
            {
                speed = 4f;
                inertia = 80f;
            }
            if (distanceToIdlePosition > 20f)
            {
                vectorToIdlePosition.Normalize();
                vectorToIdlePosition *= speed;
                Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
            }
            else if (Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity.X = -0.15f;
                Projectile.velocity.Y = -0.05f;
            }

            if (distanceToIdlePosition > 2000f)
            {
                Projectile.Center = idlePosition;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
            //honey regeneration
            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] > (60 * 5) && player.Hymenoptra().BeeResourceCurrent < player.Hymenoptra().BeeResourceMax2)
            {
                Vector2 velocity = vectorToIdlePosition;
                velocity.Normalize();
                velocity *= 15f;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, velocity, ModContent.ProjectileType<BeeResourceIncreaseProjectile>(), 0, 0, player.whoAmI, 0, Main.rand.Next(9, 12));
                Projectile.ai[1] = 0;
            }
            //dust
            if (Main.rand.NextBool(15))
            {
                Dust.NewDust(Projectile.direction == 1 ? Projectile.BottomRight : Projectile.Bottom, 1, 1, DustID.Honey2);
            }
        }
    }
}
