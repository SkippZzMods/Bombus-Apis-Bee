namespace BombusApisBee.Buffs
{
    public class HoneyManipulatorCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Manipulator Cooldown");
            Description.SetDefault("The Honey Manipulator needs to recharge");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}
