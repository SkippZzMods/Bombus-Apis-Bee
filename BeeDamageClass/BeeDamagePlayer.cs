namespace BombusApisBee.BeeDamageClass
{
    public class BeeDamagePlayer : ModPlayer
    {
        public float RegenRateLowerLimit = 0.09f;
        public float RegenRateStart = 0.55f;
        public float ResourceChanceAdd = 0f;
        public static BeeDamagePlayer ModPlayer(Player player)
        {
            return player.GetModPlayer<BeeDamagePlayer>();
        }
        public int BeeResourceCurrent = 100;
        public const int BeeExampleResourceMax = 100;
        public int BeeResourceMax = 100;
        public int BeeResourceMax2;
        public float BeeResourceRegenRate = 1f;
        public int BeeResourceRegenTimer = 1;
        private readonly int DefaultBeeResourceMax = 100;


        public override void Initialize()
        {
            BeeResourceMax = DefaultBeeResourceMax;
        }

        public override void ResetEffects()
        {
            ResetVariables();
        }

        public override void UpdateDead()
        {
            BeeResourceCurrent = (int)(BeeResourceMax2 * 0.75f);
            ResetVariables();
        }

        private void ResetVariables()
        {
            ResourceChanceAdd = 0f;
            RegenRateStart = 0.55f;
            RegenRateLowerLimit = 0.09f;
            BeeResourceMax2 = BeeResourceMax;
        }

        public override void PostUpdateMiscEffects()
        {
            UpdateResource();
        }
        private void UpdateResource()
        {
            BeeResourceRegenRate = RegenRateStart - BeeResourceCurrent / 85f;
            BeeResourceRegenRate = Utils.Clamp(BeeResourceRegenRate, RegenRateLowerLimit, 1f);
            BeeResourceRegenTimer++;

            if (BeeResourceRegenTimer > 30 * BeeResourceRegenRate)
            {
                BeeResourceCurrent += 1;
                BeeResourceRegenTimer = 0;
            }
            BeeResourceCurrent = Utils.Clamp(BeeResourceCurrent, 0, BeeResourceMax2);
        }
    }
}
