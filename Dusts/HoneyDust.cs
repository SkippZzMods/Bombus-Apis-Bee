namespace BombusApisBee.Dusts
{
    public class HoneyDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noLight = true;
            UpdateType = 153;
        }
    }

    public class HoneyDustSolid : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noLight = true;
            UpdateType = 153;
        }
    }
}