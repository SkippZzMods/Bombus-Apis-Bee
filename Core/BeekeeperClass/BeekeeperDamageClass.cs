namespace BombusApisBee.Core.BeekeeperClass
{
    public class BeekeeperDamageClass : DamageClass
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("beekeeper damage");
        }

        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == Generic)
                return StatInheritanceData.Full;

            return StatInheritanceData.None;
        }
    }
}
