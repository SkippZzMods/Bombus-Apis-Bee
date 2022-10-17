using BombusApisBee.Dusts;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BombusApisBee.Projectiles
{
    public class BeezookaRocket : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bee Rocket");
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;

            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
        }
    }
}
