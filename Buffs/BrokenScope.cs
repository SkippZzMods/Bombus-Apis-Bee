namespace BombusApisBee.Buffs
{
    public class BrokenScope : ModBuff
    {
        public override void SetStaticDefaults()
        {

            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}
