﻿namespace BombusApisBee.Dusts
{
    public class SmokeDustColor : ModDust
    {
        public override string Texture => "BombusApisBee/Dusts/SmokeDust";
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.scale *= Main.rand.NextFloat(0.8f, 2f);
            dust.frame = new Rectangle(0, Main.rand.Next(2) * 32, 32, 32);
            dust.rotation = Main.rand.NextFloat(6.28f);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color gray = new Color(25, 25, 25);
            Color ret;
            if (dust.alpha < 80)
            {
                ret = Color.Lerp(dust.color, dust.color * 0.75f, dust.alpha / 80f);
            }
            else if (dust.alpha < 160)
            {
                ret = Color.Lerp(dust.color * 0.75f, gray, (dust.alpha - 80) / 80f);
            }
            else
                ret = gray;
            return ret * ((255 - dust.alpha) / 255f);
        }

        public override bool Update(Dust dust)
        {
            dust.velocity *= 0.98f;
            dust.velocity.X *= 0.95f;
            dust.color *= 0.98f;

            if (dust.alpha > 100)
            {
                dust.scale *= 0.975f;
                dust.alpha += 2;
            }
            else
            {
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.1f);
                dust.scale *= 0.985f;
                dust.alpha += 4;
            }

            dust.position += dust.velocity;
            dust.rotation += 0.04f;

            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
    }

    public class SmokeDustColorGravity : SmokeDustColor
    {
        public override string Texture => "BombusApisBee/Dusts/SmokeDust";
        public override bool Update(Dust dust)
        {
            if (dust.velocity.Y > -2)
                dust.velocity.Y -= 0.1f;
            else
                dust.velocity.Y *= 0.92f;

            if (dust.velocity.Y > 0)
                dust.velocity.Y *= 0.85f;

            base.Update(dust);
            return false;
        }
    }
}
