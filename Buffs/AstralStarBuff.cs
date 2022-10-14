namespace BombusApisBee.Buffs
{
    public class AstralStarBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blessed by the Heavens");
            Description.SetDefault("Increased hymenoptra damage and attack speed");
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.IncreaseBeeDamage(0.1f);
            player.IncreaseBeeUseSpeed(0.1f);
        }
    }
}
