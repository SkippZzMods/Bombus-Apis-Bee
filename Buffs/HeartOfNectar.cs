namespace BombusApisBee.Buffs
{
    public class HeartOfNectar : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heart of Nectar");
            Description.SetDefault("Majorly increased life regeneration");
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Bombus().HeartOfNectar = true;
        }
    }
}
