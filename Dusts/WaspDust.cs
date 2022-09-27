namespace BombusApisBee.Dusts
{
    public class WaspDustOrange : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noLight = true;
        }
        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.scale *= 0.98f;
            dust.velocity *= 0.95f;
            dust.alpha += 5;
            if (dust.alpha >= 255 || dust.scale <= 0.1f)
                dust.active = false;
            return false;
        }
    }
    public class WaspDustBlack : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noLight = true;
        }
        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.scale *= 0.98f;
            dust.velocity *= 0.95f;
            dust.alpha += 5;
            if (dust.alpha >= 255 || dust.scale <= 0.1f)
                dust.active = false;
            return false;
        }
    }
}
