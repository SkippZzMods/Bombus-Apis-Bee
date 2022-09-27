namespace BombusApisBee.Buffs
{
    public class BrokenScope : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Broken Goggles");
            Description.SetDefault("You seem unable to mark enemies");
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}
