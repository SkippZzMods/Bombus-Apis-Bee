namespace BombusApisBee.Buffs
{
    public class HeartOfNectar : ModBuff
    {
        public override void SetStaticDefaults()
        {

            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Bombus().HeartOfNectar = true;
        }
    }
}
