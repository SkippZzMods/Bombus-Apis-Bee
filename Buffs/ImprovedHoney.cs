namespace BombusApisBee.Buffs
{

    public class ImprovedHoney : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Improved Honey");
            Description.SetDefault("Life regeneration increased substantially");
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            var modPlayer = player.GetModPlayer<BombusApisBeePlayer>();
            modPlayer.improvedhoney = true;
        }
    }
}