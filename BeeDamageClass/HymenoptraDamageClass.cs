namespace BombusApisBee.BeeDamageClass
{
    public class HymenoptraDamageClass : DamageClass
    {
        public override void SetStaticDefaults()
        {
            ClassName.SetDefault("hymenoptra damage");
        }

        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == Generic)
                return StatInheritanceData.Full;

            return StatInheritanceData.None;
        }
    }
}
