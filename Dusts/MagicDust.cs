namespace BombusApisBee.Dusts
{
    public class MagicDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.velocity *= 0.3f;
            dust.noGravity = true;
            dust.noLight = false;
            dust.scale *= 0.5f;
        }
        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }
        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.rotation += 0.15f;
            dust.velocity *= 0.95f;
            dust.color *= 0.98f;

            dust.scale *= 0.98f;
            if (dust.scale < 0.1f || dust.color == Color.Transparent)
                dust.active = false;
            return false;
        }
    }
}