using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombusApisBee.Core.Common.Honeycomb
{
    public abstract class BaseHoneycombProjectile : ModProjectile
    {
        public BaseHoneycombWeapon ParentWeapon;
        public bool ComboProjectile => Projectile.ai[0] > 0;
        public int Timer
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honeycomb");

            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;

            Projectile.friendly = true;
            Projectile.DamageType = BeeUtils.BeeDamageClass();
            Projectile.ignoreWater = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
        }
    }
}
