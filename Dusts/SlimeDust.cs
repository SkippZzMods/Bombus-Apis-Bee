namespace BombusApisBee.Dusts
{
    public class SlimeDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noLight = true;
            UpdateType = DustID.PinkSlime;
        }
    }
}