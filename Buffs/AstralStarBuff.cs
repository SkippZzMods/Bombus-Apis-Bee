namespace BombusApisBee.Buffs
{
    public class AstralStarBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.IncreaseBeeDamage(0.1f);
            player.IncreaseBeeUseSpeed(0.1f);
        }
    }
}
