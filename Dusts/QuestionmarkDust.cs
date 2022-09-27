﻿namespace BombusApisBee.Dusts
{
    public class QuestionmarkDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = false;
            dust.frame = new Rectangle(0, 0, 16, 24);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return new Color(255, 255, 255, 0);
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.rotation += 0.10f;
            dust.velocity *= 0.93f;
            dust.color *= 0.96f;

            dust.scale *= 0.96f;
            if (dust.scale < 0.1f)
                dust.active = false;
            return false;
        }
    }
}
