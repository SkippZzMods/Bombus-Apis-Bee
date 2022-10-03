namespace BombusApisBee.Dusts
{
    public class StingerDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noLight = true;
            UpdateType = DustID.Poisoned;
        }
    }
}