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

    public class LihzardianHornetRelicCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lihzardian Hornet Relic Cooldown");
            Description.SetDefault("The relic seems to no longer work.. for now");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}
