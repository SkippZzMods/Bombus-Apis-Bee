namespace BombusApisBee.BeeDamageClass
{
    public class HymenoptraDamageClass : DamageClass
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("hymenoptra damage");
        }

        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == Generic)
                return StatInheritanceData.Full;

            return StatInheritanceData.None;
        }
    }
}
