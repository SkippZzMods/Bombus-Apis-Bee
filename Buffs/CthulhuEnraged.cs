namespace BombusApisBee.Buffs
{
    public class CthulhuEnraged : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Enraged");
            Description.SetDefault("Increased hymenoptra damage");
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage<HymenoptraDamageClass>() += 0.12f;
        }
    }
}
