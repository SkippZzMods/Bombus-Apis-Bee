using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombusApisBee.Dusts
{
    public class FrostedDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noLight = false;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            if (!dust.noGravity)
                dust.velocity.Y += 0.15f;
            else
                dust.velocity *= 0.97f;

            dust.rotation += 0.1f;
            dust.scale *= 0.97f;
            dust.alpha += 4;

            if (dust.scale <= 0.2 || dust.alpha >= 255)
                dust.active = false;

            return false;
        }
    }
}
